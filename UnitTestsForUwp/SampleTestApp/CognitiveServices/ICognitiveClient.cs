using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTestApp.CognitiveServices
{
    public interface ICognitiveClient
    {
        Task<AnalysisResult> GetImageDescription(Stream stream);
    }
}
