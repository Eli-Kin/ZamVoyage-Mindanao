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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZamVoyage.Log;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using Color = Android.Graphics.Color;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace ZamVoyage.Bottom_SideBar
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class Settings_Activity : AppCompatActivity, IOnSuccessListener
    {
        string firstName, lastName, profPic, email;
        private FirebaseAuth firebaseAuth;
        private FirebaseFirestore firestore;
        EditText editTextNewEmail, currentPasswordEditText, newPasswordEditText, repeatNewPasswordEditText;
        TextView currentPasswordWarningTextView, newPasswordWarningTextView, repeatNewPasswordWarningTextView;
        ImageButton buttonChangeEmail;
        Button buttonChangePassword, deleteAccountButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);

            FirebaseApp.InitializeApp(this);
            firebaseAuth = FirebaseAuth.Instance;
            firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);
            FirebaseUser currentUser = firebaseAuth.CurrentUser;

            editTextNewEmail = FindViewById<EditText>(Resource.Id.editTextNewEmail);
            buttonChangeEmail = FindViewById<ImageButton>(Resource.Id.emailSendButton);
            currentPasswordEditText = FindViewById<EditText>(Resource.Id.currentPasswordEditText);
            newPasswordEditText = FindViewById<EditText>(Resource.Id.newPasswordEditText);
            repeatNewPasswordEditText = FindViewById<EditText>(Resource.Id.repeatNewPasswordEditText);
            currentPasswordWarningTextView = FindViewById<TextView>(Resource.Id.currentPasswordWarningTextView);
            newPasswordWarningTextView = FindViewById<TextView>(Resource.Id.newPasswordWarningTextView);
            repeatNewPasswordWarningTextView = FindViewById<TextView>(Resource.Id.repeatNewPasswordWarningTextView);
            buttonChangePassword = FindViewById<Button>(Resource.Id.buttonChangePassword);
            deleteAccountButton = FindViewById<Button>(Resource.Id.deleteAccountButton);

            buttonChangeEmail.Click += ButtonChangeEmail_Click;
            buttonChangePassword.Click += ButtonChangePassword_Click;
            deleteAccountButton.Click += DeleteAccountButton_Click;

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#FFFFFF"));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder()
            .SetPersistenceEnabled(true)
            .Build();

            firestore = FirebaseFirestore.GetInstance(FirebaseApp.Instance);
            firestore.FirestoreSettings = settings;

            if (currentUser != null)
            {
                DocumentReference docRef = firestore.Collection("users").Document(currentUser.Uid);
                docRef.Get().AddOnSuccessListener(this);
            }
            else
            {
                editTextNewEmail.Enabled = false;
                buttonChangeEmail.Enabled = false;
                buttonChangePassword.Enabled = false;
                currentPasswordEditText.Enabled = false;
                newPasswordEditText.Enabled = false;
                repeatNewPasswordEditText.Enabled = false;
            }
        }

        private void DeleteAccountButton_Click(object sender, System.EventArgs e)
        {

            // Check for internet connectivity
            if (!IsConnectedToInternet())
            {
                AlertDialog.Builder builder1 = new AlertDialog.Builder(this);
                builder1.SetTitle("No Internet Connection")
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

            // Create an AlertDialog.Builder
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            // Set the alert dialog title and message
            builder.SetTitle("Delete Account");
            builder.SetMessage("Are you sure you want to delete your account? This action cannot be undone.");

            // Set the positive button text and action
            builder.SetPositiveButton("Delete", (s, args) =>
            {
                // Delete the user account
                FirebaseUser user = firebaseAuth.CurrentUser;
                user.Delete().AddOnCompleteListener(this, new DeleteAccountCompleteListener());

                // Sign the user out
                FirebaseAuth.Instance.SignOut();

                // Reset the preference to indicate that the MainActivity has not been launched before
                var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                var editor = preferences.Edit();
                editor.PutBoolean("MainActivityLaunchedBefore", false);
                editor.Commit();

                // Redirect to the login screen
                Intent intent = new Intent(this, typeof(Get_Started));
                intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(intent);
                Finish();

            });

            // Set the negative button text and action
            builder.SetNegativeButton("Cancel", (s, args) =>
            {
                // Dismiss the alert dialog
                builder.Dispose();
            });

            // Create and show the alert dialog
            AlertDialog dialog = builder.Create();
            dialog.Show();
        }

        private class DeleteAccountCompleteListener : Java.Lang.Object, IOnCompleteListener
        {
            public void OnComplete(Task task)
            {
                if (task.IsSuccessful)
                {
                    // Account deleted successfully
                    Toast.MakeText(Application.Context, "Account deleted successfully", ToastLength.Short).Show();
                    // You can redirect the user to a different activity or perform any other actions here
                }
                else
                {
                    // Account deletion failed
                    Toast.MakeText(Application.Context, "Failed to delete account", ToastLength.Short).Show();
                    // You can display an error message or handle the failure case here
                }
            }
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

        private async void ButtonChangePassword_Click(object sender, EventArgs e)
        {
            string currentPassword = currentPasswordEditText.Text;
            string newPassword = newPasswordEditText.Text;
            string repeatNewPassword = repeatNewPasswordEditText.Text;

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

            // Clear previous warning messages
            currentPasswordWarningTextView.Visibility = ViewStates.Gone;
            newPasswordWarningTextView.Visibility = ViewStates.Gone;
            repeatNewPasswordWarningTextView.Visibility = ViewStates.Gone;

            // Validate password fields
            if (string.IsNullOrEmpty(currentPassword))
            {
                // Show warning
                currentPasswordWarningTextView.Text = "Please add your current password.";
                currentPasswordWarningTextView.Visibility = ViewStates.Visible;
                return;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                // Show warning
                newPasswordWarningTextView.Text = "Please enter your new password.";
                newPasswordWarningTextView.Visibility = ViewStates.Visible;
                return;
            }

            if (string.IsNullOrEmpty(repeatNewPassword))
            {
                // Show warning
                repeatNewPasswordWarningTextView.Text = "Please confirm your new password.";
                repeatNewPasswordWarningTextView.Visibility = ViewStates.Visible;
                return;
            }

            if (newPassword.Length < 8 || !Regex.IsMatch(newPassword, @"[a-zA-Z]") || !Regex.IsMatch(newPassword, @"\d"))
            {
                // Show warning if the new password does not meet the requirements
                newPasswordWarningTextView.Text = "Password must be at least 8 characters long and contain both letters and numbers.";
                newPasswordWarningTextView.Visibility = ViewStates.Visible;
                return;
            }

            if (newPassword != repeatNewPassword)
            {
                // Show warning if the new password and repeat new password do not match
                repeatNewPasswordWarningTextView.Text = "Passwords do not match.";
                repeatNewPasswordWarningTextView.Visibility = ViewStates.Visible;
                return;
            }

            // Proceed with password change
            try
            {
                FirebaseUser user = FirebaseAuth.Instance.CurrentUser;

                // Reauthenticate the user with the current password
                var credential = EmailAuthProvider.GetCredential(user.Email, currentPassword);
                await user.ReauthenticateAndRetrieveDataAsync(credential);

                if (user != null)
                {
                    await user.UpdatePasswordAsync(newPassword);

                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Password Changed");
                    builder.SetMessage("Your password has been successfully changed. Please log in again.");
                    builder.SetPositiveButton("OK", (sender, args) =>
                    {
                        // Sign the user out
                        FirebaseAuth.Instance.SignOut();

                        // Reset the preference to indicate that the MainActivity has not been launched before
                        var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                        var editor = preferences.Edit();
                        editor.PutBoolean("MainActivityLaunchedBefore", false);
                        editor.Commit();

                        // Redirect to the login screen
                        Intent intent = new Intent(this, typeof(Log_in));
                        intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
                        StartActivity(intent);
                        Finish();
                    });

                    AlertDialog dialog = builder.Create();
                    dialog.Show();

                }
                else
                {
                    // User is not logged in
                    Toast.MakeText(this, "User not logged in", ToastLength.Short).Show();
                }
            }
            catch (FirebaseAuthInvalidUserException)
            {
                // Handle invalid user exception
                Toast.MakeText(this, "Invalid user", ToastLength.Short).Show();
            }
            catch (Java.Lang.Exception ex)
            {
                if (ex.Message.Contains("ERROR_WRONG_PASSWORD"))
                {
                    Toast.MakeText(this, "Invalid current password", ToastLength.Short).Show();
                }
                else
                {
                    // Show warning
                    currentPasswordWarningTextView.Text = "Invalid current password.";
                    currentPasswordWarningTextView.Visibility = ViewStates.Visible;
                    return;
                }
            }
        }



        private async void ButtonChangeEmail_Click(object sender, EventArgs e)
        {
            string newEmail = editTextNewEmail.Text.Trim();

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

            try
            {
                FirebaseUser user = FirebaseAuth.Instance.CurrentUser;

                // Check if the new email is different from the current email
                if (newEmail != user.Email)
                {
                    // Send email verification to the new email address
                    await user.VerifyBeforeUpdateEmail(newEmail);

                    Toast.MakeText(this, "Email verification sent to new email address", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "New email address is the same as the current email", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Failed to update email address: " + ex.Message, ToastLength.Short).Show();
            }
        }

        private bool IsConnectedToInternet()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
            return activeNetworkInfo != null && activeNetworkInfo.IsConnected;
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            FirebaseUser currentUser = firebaseAuth.CurrentUser;
            var snapshot = (DocumentSnapshot)result;

            firstName = snapshot.Get("firstName").ToString();
            lastName = snapshot.Get("lastName").ToString();
            profPic = snapshot.Get("profilePicture").ToString();
            email = snapshot.Get("email") != null ? snapshot.Get("email").ToString() : "";

            editTextNewEmail.Text = currentUser.Email;

        }
    }
}