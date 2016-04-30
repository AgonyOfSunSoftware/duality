﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Duality;
using Duality.IO;

namespace Duality.Editor.AssetManagement
{
	public class AssetExportEnvironment : IAssetExportEnvironment
	{
		private bool isPrepareStep = false;
		private string exportDir = null;
		private Resource input = null;
		private bool isHandled = false;
		private List<string> outputPaths = new List<string>();

		public bool IsPrepareStep
		{
			get { return this.isPrepareStep; }
			set { this.isPrepareStep = value; }
		}
		public string ExportDirectory
		{
			get { return this.exportDir; }
		}
		public Resource Input
		{
			get { return this.input; }
		}
		public bool IsHandled
		{
			get { return this.isHandled; }
		}
		public IEnumerable<string> OutputPaths
		{
			get { return this.outputPaths; }
		}
		
		public AssetExportEnvironment(string exportDir, Resource input)
		{
			this.exportDir = exportDir;
			this.input = input;
		}

		public void ResetAcquiredData()
		{
			this.outputPaths.Clear();
			this.isHandled = false;
		}
		public string AddOutputPath(string localFilePath)
		{
			if (string.IsNullOrWhiteSpace(localFilePath)) 
				throw new ArgumentException("File path can't be null or whitespace.", "localFilePath");

			string filePath = Path.Combine(this.exportDir, localFilePath);

			// If we're doing actual work, make sure the directory exists
			if (!this.isPrepareStep)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			}

			this.outputPaths.Add(filePath);
			this.isHandled = true;
			return filePath;
		}

		public bool GetParameter<T>(string parameterName, out T value)
		{
			return AssetInternalHelper.GetAssetInfoCustomValue<T>(this.input, parameterName, out value);
		}
		public void SetParameter<T>(string parameterName, T value)
		{
			// Disallow adjusting parameters in the preparation step.
			if (this.isPrepareStep) throw new InvalidOperationException(
				"Cannot adjust parameter values in the preparation step. " +
				"At this point, any Resource data is considered read-only.");

			AssetInternalHelper.SetAssetInfoCustomValue<T>(this.input, parameterName, value);
		}
	}
}
