using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTablesDemoApp.Model
{
    public class User
    {
        [Newtonsoft.Json.JsonProperty("Id")]
        public string Id { get; set; }

        [Newtonsoft.Json.JsonProperty("First_name")]
        public string First_name { get; set; }

        [Newtonsoft.Json.JsonProperty("Last_name")]
        public string Last_name { get; set; }

        [Newtonsoft.Json.JsonProperty("Email")]
        public string Email { get; set; }
    }
}
