using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTestApp.CognitiveServices
{
    public class CognitiveClient : ICognitiveClient
    {
        IVisionServiceClient _visionClient;

        public CognitiveClient()
        {
            _visionClient = new VisionServiceClient("<<YOUR KEY HERE>>");
        }

        public async Task<AnalysisResult> GetImageDescription(Stream stream)
        {
            VisualFeature[] features = { VisualFeature.Tags, VisualFeature.Categories, VisualFeature.Description };
            return await _visionClient.AnalyzeImageAsync(stream, features.ToList(), null);
        }
    }
}
