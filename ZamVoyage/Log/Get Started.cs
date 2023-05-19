using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZamVoyage.Log
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class Get_Started : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.log_getstarted);

            Typeface MontserratExtraBold = Typeface.CreateFromAsset(Assets, "montserrat_extra_bold.ttf");
            Typeface Montserrat = Typeface.CreateFromAsset(Assets, "montserrat.ttf");
            Typeface Poppins = Typeface.CreateFromAsset(Assets, "poppins.ttf");

            TextView textskip = FindViewById<TextView>(Resource.Id.skip);
            TextView btnSignin = FindViewById<TextView>(Resource.Id.btn_signin);
            TextView btnSignup = FindViewById<TextView>(Resource.Id.btn_signup);
            TextView splashTitle = FindViewById<TextView>(Resource.Id.splash_title);
            TextView splashText = FindViewById<TextView>(Resource.Id.splash_text);

            textskip.Typeface = Poppins;
            btnSignin.Typeface = Poppins;
            btnSignup.Typeface = Poppins;
            splashText.Typeface = Montserrat;
            splashTitle.Typeface = MontserratExtraBold;

            Button buttonSignIn = FindViewById<Button>(Resource.Id.btn_signin);
            buttonSignIn.Click += (sender, e) =>
            {
                // Reset the preference to indicate that the MainActivity has not been launched before
                var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                var editor = preferences.Edit();
                editor.PutBoolean("MainActivityLaunchedBefore", false);
                editor.Commit();

                Intent intent = new Intent(this, typeof(Log_in));
                StartActivity(intent);
            };
            buttonSignIn.Typeface = Poppins;

            Button buttonSignUp = FindViewById<Button>(Resource.Id.btn_signup);
            buttonSignUp.Click += (sender, e) =>
            {
                // Reset the preference to indicate that the MainActivity has not been launched before
                var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                var editor = preferences.Edit();
                editor.PutBoolean("MainActivityLaunchedBefore", false);
                editor.Commit();

                Intent intent = new Intent(this, typeof(Sign_up));
                StartActivity(intent);
            };
            buttonSignUp.Typeface = Poppins;

            textskip.Click += (sender, e) =>
            {
                // Start MainActivity
                StartActivity(typeof(MainActivity));

                // Reset the preference to indicate that the MainActivity has been launched
                var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                var editor = preferences.Edit();
                editor.PutBoolean("MainActivityLaunchedBefore", true);
                editor.Commit();
            };

            HideSystemUI();

        }

        private void HideSystemUI()
        {
            // Hide the navigation and status bars
            View decorView = Window.DecorView;
            var uiOptions = (int)decorView.SystemUiVisibility;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            decorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }

        // Override the OnWindowFocusChanged method to hide the bars when they are shown
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                HideSystemUI();
            }
        }
    }
}