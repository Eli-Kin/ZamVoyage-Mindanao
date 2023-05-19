using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using Firebase.Auth;
using Firebase;
using Android.Gms.Tasks;
using Firebase.Firestore;
using Refractored.Controls;
using Android.Text;
using Android.Views.InputMethods;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Content.Res;
using System.IO;
using Com.Theartofdev.Edmodo.Cropper;
using File = Java.IO.File;
using ZamVoyage.Fragments;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace ZamVoyage.Favorites
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class Favorites_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_favorites);

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            FragmentManager fragmentManager = SupportFragmentManager;
            var transaction = fragmentManager.BeginTransaction();
            Android.Support.V4.App.Fragment defaultFragment = new Favorites_Fragment();
            transaction.Replace(Resource.Id.fragment_container, defaultFragment);
            transaction.Commit();

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                // Handle the back button press here
                OnBackPressed();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

    }
}