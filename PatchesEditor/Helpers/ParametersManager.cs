using Microsoft.Win32.SafeHandles;
using PatchesEditor.Data;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace PatchesEditor.Helpers
{
    public class ParametersManager : IDisposable
    {
        private readonly string paramFilePath = Path.Combine(@"C:\Patches Editor\", "PatchesEditor.params");

        /// <summary>
        /// Reads the application parameters from the file
        /// </summary>
        /// <returns></returns>
        public AppParameters ReadParameters()
        {
            if (File.Exists(paramFilePath) == false) CreateParamFile();
            return LoadAllParameters();
        }

        /// <summary>
        /// Save all parameters to a json format
        /// </summary>
        /// <param name="appParameters">Parameters to save to file</param>
        public void SaveAllParameters(AppParameters appParameters)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(paramFilePath));

            using (StreamWriter strWriter = File.CreateText(paramFilePath))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(strWriter, appParameters);
            }
            appParameters.Reset();
        }

        #region Private Methods

        /// <summary>
        /// Create the parameters file with the default values
        /// </summary>
        private void CreateParamFile()
        {
            AppParameters appParameters = new AppParameters(true);
            SaveAllParameters(appParameters);
        }

        /// <summary>
        /// Load all parameters from the json file
        /// </summary>
        /// <returns></returns>
        private AppParameters LoadAllParameters()
        {
            AppParameters? appParameters;
            using (StreamReader strReader = new StreamReader(paramFilePath))
            {
                string json = strReader.ReadToEnd();
                appParameters = JsonConvert.DeserializeObject<AppParameters>(json);
            }

            if (appParameters == null) 
                return new AppParameters(true);

            return appParameters;
        }

        #endregion

        #region IDisposable Interface

        // To detect redundant calls
        private bool _disposedValue;

        // Instantiate a SafeHandle instance.
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _safeHandle.Dispose();
                }

                _disposedValue = true;
            }
        }

        #endregion
    }
}
