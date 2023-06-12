using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Service.Notification;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ZamVoyage.Log
{
    [Activity(Label = "Verification")]
    public class Verification : Activity
    {
        private FirebaseAuth auth;
        private TextView resendText;
        private Timer timer;
        private int countdown = 60;
        private Button ButtonVerify;
        ProgressDialog progressDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.log_verification);

            Typeface Abeezee = Typeface.CreateFromAsset(Assets, "abeezee.ttf");
            Typeface MontserratSemiBold = Typeface.CreateFromAsset(Assets, "montserrat_semi_bold.ttf");
            Typeface Roboto = Typeface.CreateFromAsset(Assets, "roboto.ttf");
            Typeface Poppins = Typeface.CreateFromAsset(Assets, "poppins.ttf");

            TextView verificationText = FindViewById<TextView>(Resource.Id.verificationText);
            TextView emailText = FindViewById<TextView>(Resource.Id.emailText);
            TextView verifyText = FindViewById<TextView>(Resource.Id.verifyText);
            EditText emailEditText = FindViewById<EditText>(Resource.Id.emailEditText);
            resendText = FindViewById<TextView>(Resource.Id.resendText);
            ButtonVerify = FindViewById<Button>(Resource.Id.button_verify);

            verificationText.Typeface = MontserratSemiBold;
            emailText.Typeface = MontserratSemiBold;
            verifyText.Typeface = MontserratSemiBold;
            emailEditText.Typeface = Poppins;
            resendText.Typeface = Abeezee;
            ButtonVerify.Typeface = Abeezee;

            ButtonVerify.Click += OnButtonClicked;

            ButtonVerify.Click += ButtonVerify_Click;

        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ShowProgressDialog(string message)
        {
            progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage(message);
            progressDialog.SetCancelable(false);
            progressDialog.Show();
        }

        private async void ButtonVerify_Click(object sender, System.EventArgs e)
        {
            var currentUser = auth.CurrentUser;
            // Show the progress dialog
            ShowProgressDialog("Loading...");

            try
            {
                if (currentUser != null && !currentUser.IsEmailVerified)
                {
                    await currentUser.SendEmailVerificationAsync(null);
                    Toast.MakeText(this, "Verification link sent", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "This email is already verified", ToastLength.Short).Show();
                }
                
            }
            catch (FirebaseAuthInvalidUserException)
            {
                Toast.MakeText(this, "User with this email does not exist", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error sending password reset email: " + ex.Message, ToastLength.Short).Show();
            }
        }

    }
}