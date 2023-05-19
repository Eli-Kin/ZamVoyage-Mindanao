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
using Fragment = AndroidX.Fragment.App.Fragment;

namespace ZamVoyage.Fragments
{
    public class Mountain_Fragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the fragment's layout file
            View view = inflater.Inflate(Resource.Layout.fragment_mountain, container, false);

            TextView viewAll = view.FindViewById<TextView>(Resource.Id.viewAll);

            viewAll.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(ContentList.Mountain_List));
                StartActivity(intent);
            };

            return view;
        }
    }
}