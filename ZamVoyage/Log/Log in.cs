using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Org.Apache.Commons.Logging;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using static Android.Icu.Text.Transliterator;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace ZamVoyage.Log
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class Log_in : AppCompatActivity
    {
        private FirebaseAuth firebaseAuth;
        ProgressDialog progressDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebaseApp.InitializeApp(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Initialize Firebase Authentication
            firebaseAuth = FirebaseAuth.Instance;

            SetContentView(Resource.Layout.log_login);

            Typeface Abeezee = Typeface.CreateFromAsset(Assets, "abeezee.ttf");
            Typeface MontserratSemiBold = Typeface.CreateFromAsset(Assets, "montserrat_semi_bold.ttf");
            Typeface Roboto = Typeface.CreateFromAsset(Assets, "roboto.ttf");
            Typeface Poppins = Typeface.CreateFromAsset(Assets, "poppins.ttf");

            //TextView logFacebook = FindViewById<TextView>(Resource.Id.log_fb);
            //TextView logGoogle = FindViewById<TextView>(Resource.Id.log_google);
            TextView registerNow = FindViewById<TextView>(Resource.Id.register_now);
            TextView noAccount = FindViewById<TextView>(Resource.Id.no_account);
            TextView helloText = FindViewById<TextView>(Resource.Id.helloText);
            TextView welcomeText = FindViewById<TextView>(Resource.Id.welcomeText);
            TextView detailsText = FindViewById<TextView>(Resource.Id.detailsText);
            EditText emailText = FindViewById<EditText>(Resource.Id.emailEditText);
            EditText passwordText = FindViewById<EditText>(Resource.Id.passwordEditText);
            TextView warning = FindViewById<TextView>(Resource.Id.warning);
            TextView forgotText = FindViewById<TextView>(Resource.Id.forgotText);
            Button btnSignin = FindViewById<Button>(Resource.Id.btn_signin);

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            // Check if the keyboard is open
            InputMethodManager inputMethodManager = (InputMethodManager)GetSystemService(InputMethodService);

            helloText.Typeface = MontserratSemiBold;
            welcomeText.Typeface = MontserratSemiBold;
            detailsText.Typeface = MontserratSemiBold;
            emailText.Typeface = Abeezee;
            passwordText.Typeface = Abeezee;
            forgotText.Typeface = Abeezee;
            //logFacebook.Typeface = Roboto;
            //logGoogle.Typeface = Roboto;
            registerNow.Typeface = Poppins;
            noAccount.Typeface = Poppins;
            btnSignin.Typeface = Poppins;

            btnSignin.Click += async (sender, e) =>
            {
                string email = emailText.Text.Replace(" ", "");
                string password = passwordText.Text;

                // Hide the keyboard
                inputMethodManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);

                // Show the progress dialog
                ShowProgressDialog("Loading...");

                if (!IsOnline())
                {
                    // Dismiss the progress dialog
                    progressDialog.Dismiss();
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Unable to connect to the internet.");
                    alert.SetMessage("Please check your network connection and try again.");
                    alert.SetPositiveButton("OK", (senderAlert, args) => {
                        // do something when the "OK" button is clicked
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                    return;
                }

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    // Dismiss the progress dialog
                    progressDialog.Dismiss();
                    warning.Visibility = ViewStates.Visible;
                    warning.Text = "Please enter your email and password.";
                    return;
                }
                else
                {
                    emailText.ClearFocus();
                    passwordText.ClearFocus();
                }

                try
                {

                    // Call the signInWithEmailAndPassword method on the FirebaseAuth instance
                    var userCredential = await firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

                    var user = userCredential.User;

                    // Check if the user's email is verified
                    if (user != null && user.IsEmailVerified)
                    {
                        // Dismiss the progress dialog
                        progressDialog.Dismiss();
                        // If sign-in is successful and email is verified, direct to Logout page
                        Intent homeIntent = new Intent(this, typeof(MainActivity));
                        StartActivity(homeIntent);
                        Finish();
                    }
                    else
                    {
                        // Dismiss the progress dialog
                        progressDialog.Dismiss();

                        // If the user's email is not verified, display an error message to the user and sign them out
                        warning.Visibility = ViewStates.Visible;
                        warning.Text = "Your email is not verified. Please verify your email before logging in.";
                        var currentUser = firebaseAuth.CurrentUser;
                        if (currentUser != null && !currentUser.IsEmailVerified)
                        {
                            await currentUser.SendEmailVerificationAsync(null);
                            Toast.MakeText(this, "Verification email sent.", ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(this, "Verification email sent failed.", ToastLength.Short).Show();
                        }
                        // Sign out the user explicitly
                        FirebaseAuth.Instance.SignOut();
                        return;
                    }
                    // Dismiss the progress dialog
                    progressDialog.Dismiss();
                }
                catch (FirebaseAuthInvalidUserException)
                {
                    // Dismiss the progress dialog
                    progressDialog.Dismiss();
                    // If the email address is not registered, display an error message to the user
                    warning.Visibility = ViewStates.Visible;
                    warning.Text = "Invalid email address.";
                    return;
                }
                catch (FirebaseAuthInvalidCredentialsException)
                {
                    // Dismiss the progress dialog
                    progressDialog.Dismiss();
                    // If the password is incorrect, display an error message to the user
                    warning.Visibility = ViewStates.Visible;
                    warning.Text = "Incorrect password.";
                    return;
                }
                catch (Exception ex)
                {
                    // Dismiss the progress dialog
                    progressDialog.Dismiss();
                    // If there is an error with sign-in, display the error message to the user
                    warning.Visibility = ViewStates.Visible;
                    warning.Text = "Your email is not verified. Please verify your email before logging in.";
                    return;
                }
            };


            registerNow.Click += (sender, e) =>
            {
                // Reset the preference to indicate that the MainActivity has not been launched before
                var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                var editor = preferences.Edit();
                editor.PutBoolean("MainActivityLaunchedBefore", false);
                editor.Commit();

                StartActivity(typeof(Sign_up));
            };

            forgotText.Click += (sender, e) =>
            {
                // Reset the preference to indicate that the MainActivity has not been launched before
                var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                var editor = preferences.Edit();
                editor.PutBoolean("MainActivityLaunchedBefore", false);
                editor.Commit();

                StartActivity(typeof(Forgot_Password));
            };

            //FirebaseUser currentUser = firebaseAuth.CurrentUser;
            //if (currentUser != null)
            //{
            //    // User is already logged in, launch HomeActivity
            //    Intent homeIntent = new Intent(this, typeof(MainActivity));
            //    StartActivity(homeIntent);
            //    Finish();
            //}

            HideSystemUI();

        }

        private void ShowProgressDialog(string message)
        {
            progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage(message);
            progressDialog.SetCancelable(false);
            progressDialog.Show();
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

        public override void OnBackPressed()
        {
            var intent = new Intent(this, typeof(Get_Started));
            intent.SetFlags(ActivityFlags.ClearTop); // this will clear the back stack
            StartActivity(intent);
            Finish(); // this will finish the current activity
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

        private bool IsOnline()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
            return networkInfo != null && networkInfo.IsConnectedOrConnecting;
        }


    }
}