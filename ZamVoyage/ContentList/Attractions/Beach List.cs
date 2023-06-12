using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using Firebase.Auth;
using ZamVoyage.Log;
using Firebase;
using ZamVoyage.Fragments;
using Android.Support.V4.App;
using Android.Preferences;
using Android.Gms.Tasks;
using Firebase.Firestore;
using static Java.Util.Jar.Attributes;
using static ZamVoyage.Profile.ProfileActivity;
using ZamVoyage.Content.Mountains;
using Refractored.Controls;

namespace ZamVoyage.ContentList.Attractions
{
    public class Beach_List : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}