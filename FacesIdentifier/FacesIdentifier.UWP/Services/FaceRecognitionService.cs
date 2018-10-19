using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacesIdentifier.UWP.Services
{
    public class FaceRecognitionService
    {
        public event EventHandler<string> TrainingStatusChanged;
        private readonly FaceServiceClient _faceServiceClient;
        public FaceRecognitionService()
        {
            _faceServiceClient = new FaceServiceClient("<<Cognitive Services API Key>>", "https://westcentralus.api.cognitive.microsoft.com/face/v1.0");
        }

        public async Task CreatePersonGroup(string personGroupId, string groupName)
        {
            await _faceServiceClient.CreatePersonGroupAsync(personGroupId.ToLower(), groupName);
        }

        public async Task<Tuple<string, CreatePersonResult>> AddNewPersonToGroup(string personGroupId, string personName)
        {

            CreatePersonResult person = await _faceServiceClient.CreatePersonAsync(personGroupId, personName);

            return new Tuple<string, CreatePersonResult>(personGroupId, person);
        }

        public async Task<AddPersistedFaceResult> RegisterPerson(string personGroupId, CreatePersonResult person, Stream stream)
        {

            var addPersistedFaceResult = await _faceServiceClient.AddPersonFaceAsync(
                 personGroupId, person.PersonId, stream);
            return addPersistedFaceResult;
        }

        public async Task TrainPersonGroup(string personGroupId)
        {
            await _faceServiceClient.TrainPersonGroupAsync(personGroupId);
        }

        public async Task<TrainingStatus> VerifyTrainingStatus(string personGroupId)
        {
            TrainingStatus trainingStatus = null;
            while (true)
            {
                TrainingStatusChanged?.Invoke(this, "Training in progress...");
                trainingStatus = await _faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }

                await Task.Delay(1000);
            }
            TrainingStatusChanged?.Invoke(this, "Training completed");
            return trainingStatus;
        }

        public async Task<string> VerifyFaceAgainstTraindedGroup(string personGroupId, Stream stream)
        {
            var faces = await _faceServiceClient.DetectAsync(stream);
            var faceIds = faces.Select(face => face.FaceId).ToArray();

            var results = await _faceServiceClient.IdentifyAsync(personGroupId, faceIds);
            foreach (var identifyResult in results)
            {
                if (identifyResult.Candidates.Length == 0)
                {
                    return "No one identified";
                }
                else
                {
                    // Get top 1 among all candidates returned
                    var candidateId = identifyResult.Candidates[0].PersonId;
                    var person = await _faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                    return "Identified as: " + person.Name + " with face ID: " + identifyResult.FaceId;
                }
            }
            return "No one identified";
        }
    }
}
