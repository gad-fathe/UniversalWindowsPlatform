using AzureMapsApp.UWP.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureMapsApp.UWP.Services
{
    public class MapService
    {
        private RestClient _restClient;

        public MapService()
        {
            _restClient = new RestClient("http://localhost:63369/api");
        }

        public async Task<DirectionsResponse> GetDirections(DirectionsRequest directionsRequest)
        {
            var request = new RestRequest("map", Method.POST);
            request.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(directionsRequest), ParameterType.RequestBody);
            var response = await _restClient.ExecuteTaskAsync<string>(request, default(CancellationToken));
            var directions = JsonConvert.DeserializeObject<DirectionsResponse>(response.Data);
            return directions;
        }
    }
}
