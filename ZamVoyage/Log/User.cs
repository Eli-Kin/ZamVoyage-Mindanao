using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Newtonsoft.Json;


namespace ZamVoyage.Log
{
    [JsonObject(MemberSerialization.OptIn)]
    public class User : Java.Lang.Object
    {
        [JsonProperty]
        public string FirstName { get; set; }

        [JsonProperty]
        public string LastName { get; set; }

        [JsonProperty]
        public string UserName { get; set; }

        [JsonProperty]
        public string Email { get; set; }

        public User(string firstName, string lastName, string userName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Email = email;
        }
    }
}