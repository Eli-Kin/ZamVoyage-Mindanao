using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using DocumentFormat.OpenXml.Wordprocessing;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZamVoyage.Log;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using Color = Android.Graphics.Color;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using View = Android.Views.View;

namespace ZamVoyage.Bottom_SideBar
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class ContactUs_Activity : AppCompatActivity
    {
        private EditText nameEditText, emailEditText, subjectEditText, messageEditText;
        private Button sendButton;
        ProgressDialog progressDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_contact_us);

            nameEditText = FindViewById<EditText>(Resource.Id.nameEditText);
            emailEditText = FindViewById<EditText>(Resource.Id.emailEditText);
            subjectEditText = FindViewById<EditText>(Resource.Id.subjectEditText);
            messageEditText = FindViewById<EditText>(Resource.Id.messageEditText);
            sendButton = FindViewById<Button>(Resource.Id.sendButton);

            messageEditText.SetOnTouchListener(new TouchListener());

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#FFFFFF"));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            sendButton.Click += SendButton_Click;

        }

        public class TouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View view, MotionEvent motionEvent)
            {
                if (view.Id == Resource.Id.messageEditText)
                {
                    EditText editText = (EditText)view;
                    int lines = editText.LineCount;
                    int maxLines = 5; // Maximum number of lines before scrolling is enabled

                    if (lines > maxLines)
                    {
                        view.Parent.RequestDisallowInterceptTouchEvent(true);
                        switch (motionEvent.Action & MotionEventActions.Mask)
                        {
                            case MotionEventActions.Up:
                                view.Parent.RequestDisallowInterceptTouchEvent(false);
                                break;
                        }
                    }
                }
                return false;
            }
        }

        private async void SendButton_Click(object sender, System.EventArgs e)
        {

            // Check for internet connectivity
            if (!IsConnectedToInternet())
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("No Internet Connection")
                       .SetMessage("Please check your internet connection and try again.")
                       .SetPositiveButton("OK", (dialog, which) =>
                       {
                           // Dismiss the dialog
                           AlertDialog alertDialog = dialog as AlertDialog;
                           alertDialog.Dismiss();
                       })
                       .Show();
                return;
            }

            // Validate the form fields
            if (string.IsNullOrWhiteSpace(nameEditText.Text) ||
                string.IsNullOrWhiteSpace(emailEditText.Text) ||
                string.IsNullOrWhiteSpace(subjectEditText.Text) ||
                string.IsNullOrWhiteSpace(messageEditText.Text))
            {
                Toast.MakeText(this, "Please fill in all fields.", ToastLength.Short).Show();
                return;
            }

            // Show the progress dialog
            ShowProgressDialog("Sending email...");

            // Create the email message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("", "elikingofficial@gmail.com")); // Your Gmail address
            email.To.Add(new MailboxAddress("", "elikingofficial@gmail.com")); // Your Gmail address
            email.Subject = subjectEditText.Text;
            email.Body = new TextPart("plain")
            {
                Text = $"Name: {nameEditText.Text}\n" +
                       $"Email: {emailEditText.Text}\n\n" +
                       $"{messageEditText.Text}"
            };

            // Send the email
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false); // SMTP server details for Gmail
                client.AuthenticationMechanisms.Remove("XOAUTH2"); // Disable OAuth2 authentication

                // IMPORTANT: Replace "your-email@gmail.com" and "your-password" with your actual Gmail credentials
                await client.AuthenticateAsync("elikingofficial@gmail.com", "xmjqdrzfmhldjgon");

                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }

            // Dismiss the progress dialog
            progressDialog.Dismiss();

            // Display a toast message to indicate the email has been sent
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "Email sent!", ToastLength.Short).Show();
            });

            // Clear the form fields
            nameEditText.Text = "";
            emailEditText.Text = "";
            subjectEditText.Text = "";
            messageEditText.Text = "";
        }

        private bool IsConnectedToInternet()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
            return activeNetworkInfo != null && activeNetworkInfo.IsConnected;
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
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home: // Handle the back button press
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

    }
}