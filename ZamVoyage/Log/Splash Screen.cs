using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZamVoyage.Log
{
    [Activity(Label = "ZamVoyage", MainLauncher = true, NoHistory = true, Theme = "@style/AppTheme.Splash")]
    public class Splash_Screen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.splash_screen);
            HideSystemUI();

            Typeface MontserratExtraBold = Typeface.CreateFromAsset(Assets, "montserrat_extra_bold.ttf");
            Typeface Montserrat = Typeface.CreateFromAsset(Assets, "montserrat.ttf");

            TextView splashTitle = FindViewById<TextView>(Resource.Id.splash_title);
            TextView splashText = FindViewById<TextView>(Resource.Id.splash_text);

            splashText.Typeface = Montserrat;
            splashTitle.Typeface = MontserratExtraBold;

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

        protected override async void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { _ = SimulateStartup(); });
            startupWork.Start();
        }

        async Task SimulateStartup()
        {
            await Task.Delay(3000); // Simulate a bit of startup work.

            Intent intent = new Intent(this, typeof(Get_Started));
            StartActivity(intent);

            // Retrieve the shared preferences variable.
            var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);

            // Check if the MainActivity has been launched before.
            bool mainActivityLaunchedBefore = preferences.GetBoolean("MainActivityLaunchedBefore", false);

            // Check if the user is logged in.
            var firebaseAuth = FirebaseAuth.GetInstance(Firebase.FirebaseApp.Instance);
            var user = firebaseAuth.CurrentUser;

            if (mainActivityLaunchedBefore || user != null)
            {
                // If the MainActivity has been launched before and the user is logged in, start the MainActivity.
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
            else
            {

                StartActivity(new Intent(Application.Context, typeof(Get_Started)));
            }

            // Finish the splash screen activity to prevent the user from returning to it.
            Finish();
        }
    }
}