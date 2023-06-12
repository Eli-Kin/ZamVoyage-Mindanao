using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
//using Com.Gigamole.Infinitecycleviewpager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZamVoyage.Content
{
    [Activity(Label = "Content_Test")]
    public class Content_Test : Activity
    {
        List<int> listImage = new List<int>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.layout_test);

            //InitData();
            //HorizontalInfiniteCycleViewPager viewPager = FindViewById<HorizontalInfiniteCycleViewPager>(Resource.Id.hicvp);
            //MyAdapter adapter = new MyAdapter(listImage, BaseContext);
            //viewPager.Adapter = adapter;

        }

        private void InitData()
        {
            listImage.Add(Resource.Drawable.image1);
            listImage.Add(Resource.Drawable.image2);
            listImage.Add(Resource.Drawable.image3);
            listImage.Add(Resource.Drawable.image4);
            listImage.Add(Resource.Drawable.image5);
        }

    }
}