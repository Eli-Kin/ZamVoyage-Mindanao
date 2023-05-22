using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using System.Collections.Generic;
using ZamVoyage.Favorites;
using Android.Support.V4.Content;
using System.Linq;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase;
using Java.Util;

namespace ZamVoyage.Content.History
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class History : AppCompatActivity
    {
        TextView prgph1, prgph2, prgph3, prgph4, prgph5, prgph6, title;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.layout_history);

            Typeface Abeezee = Typeface.CreateFromAsset(Assets, "abeezee.ttf");
            Typeface MontserratSemiBold = Typeface.CreateFromAsset(Assets, "montserrat_semi_bold.ttf");
            Typeface MontserratExtraBold = Typeface.CreateFromAsset(Assets, "montserrat_extra_bold.ttf");
            Typeface Roboto = Typeface.CreateFromAsset(Assets, "roboto.ttf");
            Typeface Poppins = Typeface.CreateFromAsset(Assets, "poppins.ttf");

            title = FindViewById<TextView>(Resource.Id.title);
            prgph1 = FindViewById<TextView>(Resource.Id.prgph1);
            prgph2 = FindViewById<TextView>(Resource.Id.prgph2);
            prgph3 = FindViewById<TextView>(Resource.Id.prgph3);
            prgph4 = FindViewById<TextView>(Resource.Id.prgph4);
            prgph5 = FindViewById<TextView>(Resource.Id.prgph5);
            prgph6 = FindViewById<TextView>(Resource.Id.prgph6);

            title.Typeface = MontserratExtraBold;
            prgph1.Typeface = Abeezee;
            prgph2.Typeface = Abeezee;
            prgph3.Typeface = Abeezee;
            prgph4.Typeface = Abeezee;
            prgph5.Typeface = Abeezee;
            prgph6.Typeface = Abeezee;

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);


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