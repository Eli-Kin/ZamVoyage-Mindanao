﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ZamVoyage.Log
{
    [Activity(Label = "Forgot_Password", Theme = "@style/AppTheme.NoActionBar")]
    public class Forgot_Password : Activity
    {
        private FirebaseAuth auth;
        private TextView resendText;
        private Button resendButton;
        private Timer timer;
        private int countdown = 60;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebaseApp.InitializeApp(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.log_forgot_password);

            Typeface Abeezee = Typeface.CreateFromAsset(Assets, "abeezee.ttf");
            Typeface MontserratSemiBold = Typeface.CreateFromAsset(Assets, "montserrat_semi_bold.ttf");
            Typeface Roboto = Typeface.CreateFromAsset(Assets, "roboto.ttf");
            Typeface Poppins = Typeface.CreateFromAsset(Assets, "poppins.ttf");

            TextView helloText = FindViewById<TextView>(Resource.Id.helloText);
            TextView emailText = FindViewById<TextView>(Resource.Id.emailText);
            TextView forgotText = FindViewById<TextView>(Resource.Id.forgotText);
            EditText emailEditText = FindViewById<EditText>(Resource.Id.emailEditText);
            resendText = FindViewById<TextView>(Resource.Id.resendText);
            resendButton = FindViewById<Button>(Resource.Id.forgotPasswordButton);

            helloText.Typeface = MontserratSemiBold;
            emailText.Typeface = MontserratSemiBold;
            forgotText.Typeface = MontserratSemiBold;
            emailEditText.Typeface = Poppins;
            resendText.Typeface = Abeezee;
            resendButton.Typeface = Abeezee;

            // Restore the remaining time from SharedPreferences
            if (savedInstanceState == null)
            {
                var preferences = GetSharedPreferences("Forgot_Password", FileCreationMode.Private);
                countdown = preferences.GetInt("countdown", 60);
            }
            else
            {
                countdown = savedInstanceState.GetInt("countdown", 60);
            }

            // If the countdown timer is running, disable the button and show the countdown text
            if (countdown < 60)
            {
                resendButton.Enabled = false;
                timer = new Timer();
                timer.Interval = 1000; // 1 second
                timer.Elapsed += OnTimerElapsed;
                timer.Start();
                resendText.Text = $"You can resend the verification in {countdown} seconds";
                resendText.Visibility = Android.Views.ViewStates.Visible;
            }

            resendButton.Click += OnButtonClicked;

            auth = FirebaseAuth.GetInstance(FirebaseApp.Instance);
            resendButton.Click += OnForgotPasswordButtonClick;
        }

        private async void OnForgotPasswordButtonClick(object sender, EventArgs e)
        {
            EditText emailEditText = FindViewById<EditText>(Resource.Id.emailEditText);
            string email = emailEditText.Text.Replace(" ", "");
            if (string.IsNullOrEmpty(email))
            {
                resendText.Text = "Please enter your email.";
                return;
            }
            else
            {
                resendText.Text = " ";
                return;
            }
            try
            {
                await auth.SendPasswordResetEmailAsync(email);
                emailEditText.ClearFocus();
                Toast.MakeText(this, "Password reset email sent", ToastLength.Short).Show();
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

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("countdown", countdown);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Save the remaining time to SharedPreferences
            var preferences = GetSharedPreferences("Forgot_Password", FileCreationMode.Private);
            var editor = preferences.Edit();
            editor.PutInt("countdown", countdown);
            editor.Commit();
        }

        private void OnButtonClicked(object sender, System.EventArgs e)
        {
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

            // Disable the button
            resendButton.Enabled = false;

            // Start the countdown timer
            timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Elapsed += OnTimerElapsed;
            timer.Start();

            // Show the countdown text
            resendText.Text = $"You can resend the link in {countdown} seconds";
            resendText.Visibility = Android.Views.ViewStates.Visible;
        }

        private bool IsOnline()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
            return networkInfo != null && networkInfo.IsConnectedOrConnecting;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                countdown--;
                resendText.Text = $"You can resend the link in {countdown} seconds";

                if (countdown == 0)
                {
                    // Stop the timer
                    timer.Stop();
                    timer.Dispose();
                    timer = null;

                    // Enable the button
                    resendButton.Enabled = true;

                    // Hide the countdown text
                    resendText.Visibility = Android.Views.ViewStates.Gone;

                    // Reset the countdown value
                    countdown = 60;
                }
            });
        }
    }
}
