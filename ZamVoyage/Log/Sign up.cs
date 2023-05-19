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
using System.Text.RegularExpressions;
using Firebase.Auth;
using Firebase;
using Firebase.Firestore;
using Firebase.Database;
using Java.Util;
using Google.Android.Material.Snackbar;
using Android.Net;

namespace ZamVoyage.Log
{
    [Activity(Label = "Sign_up", Theme = "@style/AppTheme.NoActionBar")]
    public class Sign_up : Activity
    {
        private FirebaseAuth firebaseAuth;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebaseApp.InitializeApp(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.log_signup);

            firebaseAuth = FirebaseAuth.Instance;

            Typeface Abeezee = Typeface.CreateFromAsset(Assets, "abeezee.ttf");
            Typeface MontserratSemiBold = Typeface.CreateFromAsset(Assets, "montserrat_semi_bold.ttf");
            Typeface Roboto = Typeface.CreateFromAsset(Assets, "roboto.ttf");
            Typeface Poppins = Typeface.CreateFromAsset(Assets, "poppins.ttf");

            TextView hasAccount = FindViewById<TextView>(Resource.Id.has_account);
            TextView loginNow = FindViewById<TextView>(Resource.Id.login_now);
            TextView helloText = FindViewById<TextView>(Resource.Id.helloText);
            TextView registerText = FindViewById<TextView>(Resource.Id.registerText);
            TextView detailsText = FindViewById<TextView>(Resource.Id.detailsText);
            EditText emailEditText = FindViewById<EditText>(Resource.Id.emailEditText);
            EditText passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            EditText confirmPasswordEditText = FindViewById<EditText>(Resource.Id.confirmpasswordEditText);
            EditText firstnameEditText = FindViewById<EditText>(Resource.Id.firstnameEditText);
            EditText lastnameEditText = FindViewById<EditText>(Resource.Id.lastnameEditText);
            EditText usernameEditText = FindViewById<EditText>(Resource.Id.usernameEditText);

            TextView passwordwarning = FindViewById<TextView>(Resource.Id.warningTextView1);
            TextView confirmPasswordwarning = FindViewById<TextView>(Resource.Id.warningTextView2);
            TextView userNamewarning = FindViewById<TextView>(Resource.Id.warningTextView3);
            TextView lastNamewarning = FindViewById<TextView>(Resource.Id.warningTextView4);
            TextView firstNamewarning = FindViewById<TextView>(Resource.Id.warningTextView5);
            TextView emailwarning = FindViewById<TextView>(Resource.Id.warningTextView);
            TextView signUpwarning = FindViewById<TextView>(Resource.Id.warning);
            Button btnSignup = FindViewById<Button>(Resource.Id.btn_signup);

            helloText.Typeface = MontserratSemiBold;
            registerText.Typeface = MontserratSemiBold;
            detailsText.Typeface = MontserratSemiBold;
            emailEditText.Typeface = Abeezee;
            passwordEditText.Typeface = Abeezee;
            confirmPasswordEditText.Typeface = Abeezee;
            firstnameEditText.Typeface = Abeezee;
            lastnameEditText.Typeface = Abeezee;
            usernameEditText.Typeface = Abeezee;
            hasAccount.Typeface = Poppins;
            loginNow.Typeface = Poppins;
            btnSignup.Typeface = Poppins;

            passwordEditText.FocusChange +=
            new EventHandler<View.FocusChangeEventArgs>((sender, e) =>
             {
              bool hasFocus = e.HasFocus;
              if (hasFocus)
              {
                passwordwarning.Visibility = ViewStates.Visible;
                passwordwarning.Text = "Password must be at least 8 characters long and contain letters and numbers.";
                passwordwarning.SetTextColor(Android.Graphics.Color.Blue);
              }
              else
              {
                passwordwarning.Visibility = ViewStates.Gone;
                passwordwarning.SetTextColor(Android.Graphics.Color.Red);
              }
             });

            btnSignup.Click += async (sender, e) =>
            {
                string firstName = firstnameEditText.Text;
                string lastName = lastnameEditText.Text;
                string userName = usernameEditText.Text;
                string email = emailEditText.Text.Replace(" ", "");
                string password = passwordEditText.Text;
                string confirmPassword = confirmPasswordEditText.Text;

                if (!IsOnline())
                {
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

                if (firstnameEditText.Text.Trim().Length == 0)
                {
                    firstNamewarning.Visibility = ViewStates.Visible;
                    firstNamewarning.Text = "Please enter your First Name.";
                    return;
                }
                else
                {
                    firstNamewarning.Visibility = ViewStates.Gone;
                }
                if (lastnameEditText.Text.Trim().Length == 0)
                {
                    lastNamewarning.Visibility = ViewStates.Visible;
                    lastNamewarning.Text = "Please enter your Last Name.";
                    return;
                }
                else
                {
                    lastNamewarning.Visibility = ViewStates.Gone;
                }
                if (usernameEditText.Text.Trim().Length == 0)
                {
                    userNamewarning.Visibility = ViewStates.Visible;
                    userNamewarning.Text = "Please enter your Username.";
                    return;
                }
                else
                {
                    userNamewarning.Visibility = ViewStates.Gone;
                }
                if (emailEditText.Text.Trim().Length == 0)
                {
                    emailwarning.Visibility = ViewStates.Visible;
                    emailwarning.Text = "Please enter your Email";
                    return;
                }
                else
                {
                    emailwarning.Visibility = ViewStates.Gone;
                }
                if (passwordEditText.Text.Trim().Length == 0)
                {
                    passwordwarning.Visibility = ViewStates.Visible;
                    passwordwarning.Text = "Please enter your Password.";
                    return;
                }
                else
                {
                    passwordwarning.Visibility = ViewStates.Gone;
                }

                if (confirmPasswordEditText.Text.Trim().Length == 0)
                {
                    confirmPasswordwarning.Visibility = ViewStates.Visible;
                    confirmPasswordwarning.Text = "Please confirm your Password";
                    return;
                }
                else
                {
                    confirmPasswordwarning.Visibility = ViewStates.Gone;
                }

                if (!Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
                {
                    // Show an error message to the user
                    emailwarning.Visibility = ViewStates.Visible;
                    emailwarning.Text = "Please enter a valid email address.";
                    return;
                }
                else if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
                {
                    passwordwarning.Visibility = ViewStates.Visible;
                    passwordwarning.Text = "Password must be at least 8 characters long and contain letters and numbers.";
                    return;
                }
                else if (password != confirmPassword)
                {

                    passwordwarning.Visibility = ViewStates.Visible;
                    passwordwarning.Text = "Password does not match!";
                    confirmPasswordwarning.Visibility = ViewStates.Visible;
                    confirmPasswordwarning.Text = "Password does not match!";
                    return;

                }
                else if (confirmPassword.Length < 8 || !Regex.IsMatch(confirmPassword, @"[a-zA-Z]") || !Regex.IsMatch(confirmPassword, @"\d"))
                {
                    confirmPasswordwarning.Visibility = ViewStates.Visible;
                    confirmPasswordwarning.Text = "Password must be at least 8 characters long and contain letters and numbers.";
                    return;
                } else
                {
                    firstnameEditText.ClearFocus();
                    lastnameEditText.ClearFocus();
                    usernameEditText.ClearFocus();
                    emailEditText.ClearFocus();
                    passwordEditText.ClearFocus();
                    confirmPasswordEditText.ClearFocus();
                }
                try
                {
                    // Create a new user with the email and password
                    await firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);

                    FirebaseFirestore db = FirebaseFirestore.GetInstance(FirebaseApp.Instance);
                    DocumentReference docRef = db.Collection("users").Document(firebaseAuth.CurrentUser.Uid);
                    HashMap user = new HashMap();
                    user.Put("firstName", firstnameEditText.Text);
                    user.Put("lastName", lastnameEditText.Text);
                    user.Put("userName", usernameEditText.Text);
                    user.Put("email", emailEditText.Text);

                    docRef.Set(user);

                    var currentUser = firebaseAuth.CurrentUser;
                    if (currentUser != null && !currentUser.IsEmailVerified)
                    {
                        await currentUser.SendEmailVerificationAsync(null);
                    }

                    firstnameEditText.Text = "";
                    lastnameEditText.Text = "";
                    usernameEditText.Text = "";
                    emailEditText.Text = "";
                    passwordEditText.Text = "";
                    confirmPasswordEditText.Text = "";

                    // Display a success message to the user
                    signUpwarning.Visibility = ViewStates.Visible;
                    signUpwarning.Text = "Account created successfully! \n A verification email has been sent to your email address. Please verify to continue.";
                    signUpwarning.SetTextColor(Android.Graphics.Color.Green);
                }
                catch (FirebaseAuthUserCollisionException)
                {
                    // Display an error message to the user if the email is already in use
                    signUpwarning.Visibility = ViewStates.Visible;
                    signUpwarning.Text = "Email is already in use.";
                }
                catch (Exception ex)
                {
                    // Display an error message to the user
                    signUpwarning.Visibility = ViewStates.Visible;
                    signUpwarning.Text = "Error: " + ex.Message;
                }


            };

            loginNow.Click += (sender, e) =>
            {
                // Reset the preference to indicate that the MainActivity has not been launched before
                var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                var editor = preferences.Edit();
                editor.PutBoolean("MainActivityLaunchedBefore", false);
                editor.Commit();

                StartActivity(typeof(Log_in));
            };

            HideSystemUI();

        }

        private bool IsOnline()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
            return networkInfo != null && networkInfo.IsConnectedOrConnecting;
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
    }
}