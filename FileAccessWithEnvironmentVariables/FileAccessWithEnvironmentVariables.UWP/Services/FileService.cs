using FileAccessWithEnvironmentVariables.UWP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FileAccessWithEnvironmentVariables.UWP.Services
{
    public class FileService : IFileService
    {
        public async Task<string> LoadFileContentUsingPathFromEnvironemntVariable(string environmentVariableName)
        {
            if (string.IsNullOrEmpty(environmentVariableName))
            {
                throw new ArgumentNullException("Please provide the name of environment variable");
            }

            var filePathFromEnvironmentVariable = Environment.GetEnvironmentVariable(environmentVariableName);

            if (string.IsNullOrEmpty(filePathFromEnvironmentVariable))
            {
                throw new ArgumentNullException("Please provide the value of environment variable");
            }

            var textFileFromExternalPath = await StorageFile.GetFileFromPathAsync(filePathFromEnvironmentVariable);

            var fileContent = await FileIO.ReadTextAsync(textFileFromExternalPath);

            return fileContent;
        }
    }
}
