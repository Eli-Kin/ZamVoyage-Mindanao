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
using ZamVoyage.Planner;

namespace ZamVoyage.Profile
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class ProfileActivity : AppCompatActivity, IOnSuccessListener
    {
        Toolbar toolbar;
        private FirebaseAuth firebaseAuth;
        private FirebaseFirestore firestore;
        private Android.Net.Uri selectedImageUri;
        TextView user_name, user_email, cancel, viewAll;
        EditText userNameEditText, firstNameEditText, lastNameEditText;
        Button editProfile, viewPlans;
        CircleImageView profilePic;
        Bitmap bitmap;
        ProgressDialog progressDialog;
        bool isEditMode = false;
        string originalUserNameText, originalFirstNameText, originalLastNameText, originalImagePath;
        string firstName, lastName, userName, email, profPic;
        string imagePath;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_profile);

            FirebaseApp.InitializeApp(this);
            firebaseAuth = FirebaseAuth.Instance;
            firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);

            user_name = FindViewById<TextView>(Resource.Id.nameShow);
            user_email = FindViewById<TextView>(Resource.Id.emailShow);
            editProfile = FindViewById<Button>(Resource.Id.editProfile);
            userNameEditText = FindViewById<EditText>(Resource.Id.userNameEditText);
            firstNameEditText = FindViewById<EditText>(Resource.Id.firstNameEditText);
            lastNameEditText = FindViewById<EditText>(Resource.Id.lastNameEditText);
            cancel = FindViewById<TextView>(Resource.Id.cancel);
            profilePic = FindViewById<CircleImageView>(Resource.Id.profilePic);
            viewAll = FindViewById<TextView>(Resource.Id.viewAll);
            viewPlans = FindViewById<Button>(Resource.Id.viewPlans);

            viewPlans.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(PlanListActivity));
                StartActivity(intent);
            };

            viewAll.Click += delegate
            {
                Intent intent = new Intent(this, typeof(Favorites.Favorites_Activity));
                StartActivity(intent);
            };

            // Get an instance of the fragment manager from the activity's SupportFragmentManager property
            FragmentManager fragmentManager = SupportFragmentManager;

            // Start a new fragment transaction
            var transaction = fragmentManager.BeginTransaction();

            // Add a new Favorites_Fragment object to the transaction, casting it to Android.Support.V4.App.Fragment
            Android.Support.V4.App.Fragment defaultFragment = new Favorites_Fragment();

            transaction.Replace(Resource.Id.fragment_container, defaultFragment);

            // Commit the transaction to the device immediately
            transaction.Commit();

            FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder()
            .SetPersistenceEnabled(true)
            .Build();

            firestore = FirebaseFirestore.GetInstance(FirebaseApp.Instance);
            firestore.FirestoreSettings = settings;

            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            if (user != null)
            {
                user_email.Visibility = ViewStates.Visible;
                editProfile.Visibility = ViewStates.Visible;

                ShowProgressDialog("Loading data...");

                firestore.Collection("users").Document(firebaseAuth.CurrentUser.Uid).Get().AddOnSuccessListener(this);
            }
            else
            {
                editProfile.Visibility = ViewStates.Gone;
                user_email.Visibility = ViewStates.Gone;
                profilePic.Clickable = false;
            }

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.White.ToArgb());
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            // Add TextChanged event handlers to the EditText fields
            userNameEditText.TextChanged += DataChangedEventHandler;
            firstNameEditText.TextChanged += DataChangedEventHandler;
            lastNameEditText.TextChanged += DataChangedEventHandler;

            // Set initial button text
            SetButtonState();

            // Set the editable state of the fields
            SetFieldEditableState(isEditMode);

            // Set up click event for the Edit/Save button
            editProfile.Click += EditProfileButton_Click;
            profilePic.Click += ProfilePic_Click;
            cancel.Click += CancelButton_Click;


        }

        private void ShowProgressDialog(string message)
        {
            progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage(message);
            progressDialog.SetCancelable(false);
            progressDialog.Show();
        }

        private void ProfilePic_Click(object sender, EventArgs e)
        {
            // Request the external storage permission
            RequestExternalStoragePermission();
            SetButtonState();
        }

        private void RequestExternalStoragePermission()
        {
            // Check if the permission is already granted
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == Permission.Granted)
            {
                // Permission already granted, proceed with selecting and saving the profile picture
                SelectAndSaveProfilePicture();
            }
            else
            {
                // Permission not granted, request the permission
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 1);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == 1)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    // Permission granted, proceed with selecting and saving the profile picture
                    SelectAndSaveProfilePicture();
                }
                else
                {
                    // Permission denied, handle accordingly (e.g., show a message, disable the feature, etc.)
                    Toast.MakeText(this, "Permission denied to access external storage", ToastLength.Short).Show();
                }
            }
        }

        private void SelectAndSaveProfilePicture()
        {
            SetButtonState();
            CropImage.Builder()
                .SetGuidelines(CropImageView.Guidelines.On)
                .SetAspectRatio(1, 1) // Set the desired aspect ratio for the cropped image
                .SetCropShape(CropImageView.CropShape.Oval) // Set the shape of the crop area (e.g., oval)
                .SetOutputCompressFormat(Bitmap.CompressFormat.Jpeg) // Set the output image format
                .SetOutputCompressQuality(80) // Set the output image quality (0-100)
                .Start(this);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            SetButtonState();
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == CropImage.CropImageActivityRequestCode)
            {
                CropImage.ActivityResult result = CropImage.GetActivityResult(data);
                if (resultCode == Result.Ok)
                {
                    // Get the cropped image URI
                    selectedImageUri = result.Uri;
                    // Update the profile picture
                    try
                    {
                        // Get the path of the cropped image
                        imagePath = GetPathFromUri(selectedImageUri);

                        //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                        //ISharedPreferencesEditor editor = prefs.Edit();
                        //editor.PutString("ImagePath", imagePath);
                        //editor.Commit();

                        originalImagePath = imagePath;

                        SetButtonState();

                        // Update the profile picture in the UI
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            Bitmap bitmap = BitmapFactory.DecodeFile(imagePath);
                            profilePic.SetImageBitmap(bitmap);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception here
                        System.Diagnostics.Debug.WriteLine("Failed to set profile picture: " + ex.Message);
                        Toast.MakeText(this, "Failed to set profile picture: " + ex.Message, ToastLength.Short).Show();
                    }
                }
                else if (resultCode.Equals(CropImage.CropImageActivityResultErrorCode))
                {
                    // Handle the error case
                    Exception error = result.Error;
                    System.Diagnostics.Debug.WriteLine("Failed to crop image: " + error.Message);
                    Toast.MakeText(this, "Failed to crop image: " + error.Message, ToastLength.Short).Show();
                }
            }
            else if (requestCode == 1 && resultCode == Result.Ok && data != null)
            {
                // Get the toggle state from Activity B
                bool toggleState = data.GetBooleanExtra("ToggleState", false);

                // Check the toggle state and recreate Activity A if necessary
                if (!toggleState)
                {
                    Recreate();
                    return;
                }

                // Get the selected image URI
                selectedImageUri = data.Data;

                // Check if the image format is supported
                if (IsImageFormatSupported(selectedImageUri))
                {
                    // Start the image cropping activity
                    CropImage.Builder()
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetAspectRatio(1, 1) // Set the desired aspect ratio for the cropped image
                        .SetCropShape(CropImageView.CropShape.Oval) // Set the shape of the crop area (e.g., oval)
                        .SetOutputCompressFormat(Bitmap.CompressFormat.Jpeg) // Set the output image format
                        .SetOutputCompressQuality(80) // Set the output image quality (0-100)
                        .SetOutputUri(selectedImageUri) // Set the output image URI
                        .Start(this);
                }
                else
                {
                    // Show a warning for unsupported image format
                    Toast.MakeText(this, "Unsupported image format", ToastLength.Short).Show();
                }
            }
        }

        private bool IsImageFormatSupported(Android.Net.Uri imageUri)
        {
            ContentResolver contentResolver = ContentResolver;
            string mimeType = contentResolver.GetType(imageUri);
            return mimeType != null && mimeType.StartsWith("image/");
        }

        private string GetPathFromUri(Android.Net.Uri uri)
        {
            SetButtonState();
            string filePath = null;

            try
            {
                // Create a temporary file
                File tempFile = File.CreateTempFile("temp", null, CacheDir);

                // Open an InputStream from the URI
                using (Stream inputStream = ContentResolver.OpenInputStream(uri))
                {
                    // Copy the file data to the temporary file
                    using (FileStream outputStream = new FileStream(tempFile.AbsolutePath, FileMode.Create))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }

                // Get the absolute path of the temporary file
                filePath = tempFile.AbsolutePath;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Failed to get path from URI: " + ex.Message, ToastLength.Short).Show();
            }

            return filePath;
        }

        private void SaveProfilePicture(Android.Net.Uri imageUri)
        {
            SetButtonState();
            try
            {
                // Get the path of the selected image
                string imagePath = GetPathFromUri(imageUri);

                //Update the user's profile picture in Firebase Firestore
                FirebaseUser currentUser = FirebaseAuth.Instance.CurrentUser;
                if (currentUser != null)
                {
                    DocumentReference userRef = FirebaseFirestore.Instance.Collection("users").Document(currentUser.Uid);
                    userRef.Update("profilePicture", imagePath);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here
                System.Diagnostics.Debug.WriteLine("Failed to save profile picture: " + ex.Message);
                Toast.MakeText(this, "Failed to save profile picture: " + ex.Message, ToastLength.Short).Show();
            }
        }
        
        private void EditProfileButton_Click(object sender, System.EventArgs e)
        {
            isEditMode = !isEditMode;

            // Update the button text based on the edit mode
            SetButtonState();

            // Set the editable state of the fields
            SetFieldEditableState(isEditMode);

            if (isEditMode)
            {
                // Enter edit mode
                firstNameEditText.Text = firstName;
                lastNameEditText.Text = lastName;
                userNameEditText.Text = userName;

                originalUserNameText = userNameEditText.Text;
                originalFirstNameText = firstNameEditText.Text;
                originalLastNameText = lastNameEditText.Text;

                System.Diagnostics.Debug.WriteLine("original: " + originalUserNameText);
                System.Diagnostics.Debug.WriteLine("originalEText: " + userNameEditText.Text);

                //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                //imagePath = prefs.GetString("ImagePath", null);
                //originalImagePath = imagePath;

                //System.Diagnostics.Debug.WriteLine("Original Image Path in EditMode: " + originalImagePath);

                // Hide text views and show edit texts
                user_name.Visibility = Android.Views.ViewStates.Gone;
                user_email.Visibility = Android.Views.ViewStates.Gone;

                firstNameEditText.Visibility = Android.Views.ViewStates.Visible;
                lastNameEditText.Visibility = Android.Views.ViewStates.Visible;
                userNameEditText.Visibility = Android.Views.ViewStates.Visible;

                // Disable the save button initially
                editProfile.Enabled = false;
                editProfile.SetTextColor(Color.Gray);
                editProfile.SetBackgroundResource(Resource.Drawable.inactive_button);
            }
            else
            {
                // Exit edit mode and save changes

                // Hide text views and show edit texts
                user_name.Visibility = Android.Views.ViewStates.Visible;
                user_email.Visibility = Android.Views.ViewStates.Visible;

                firstNameEditText.Visibility = Android.Views.ViewStates.Gone;
                lastNameEditText.Visibility = Android.Views.ViewStates.Gone;
                userNameEditText.Visibility = Android.Views.ViewStates.Gone;

                try
                {
                    // Get the user's UID
                    string userId = firebaseAuth.CurrentUser.Uid;

                    // Create a reference to the plan document in Firestore
                    DocumentReference planDocRef = firestore.Collection("users").Document(userId);

                    Dictionary<string, Java.Lang.Object> updatedData = new Dictionary<string, Java.Lang.Object>();

                    System.Diagnostics.Debug.WriteLine("Title: " + userName);

                    // Update the properties in the Map object
                    updatedData["userName"] = userNameEditText.Text;
                    updatedData["firstName"] = firstNameEditText.Text;
                    updatedData["lastName"] = lastNameEditText.Text;

                    // Update the Firestore document
                    planDocRef.Update(updatedData);

                    // Update the text views with the edited values
                    user_name.Text = firstName + " " + lastName;

                    // Save the profile picture
                    if (selectedImageUri != null)
                    {
                        SaveProfilePicture(selectedImageUri);
                    }

                    // Show success message or perform any other actions
                    Toast.MakeText(this, "Profile saved successfully", ToastLength.Short).Show();

                    // Refresh the activity
                    Recreate();
                }
                catch (Exception ex)
                {
                    // Handle the exception here
                    System.Diagnostics.Debug.WriteLine("Failed to update profile: " + ex.Message);
                    Toast.MakeText(this, "Failed to update profile: " + ex.Message, ToastLength.Short).Show();
                }
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                // Handle the back button press
                OnBackPressed();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void OnSuccess(Java.Lang.Object result)
        {

            // Dismiss the progress dialog
            progressDialog.Dismiss();

            FirebaseUser currentUser = firebaseAuth.CurrentUser;
            var snapshot = (DocumentSnapshot)result;

            firstName = snapshot.Get("firstName").ToString();
            lastName = snapshot.Get("lastName").ToString();
            userName = snapshot.Get("userName").ToString();
            profPic = snapshot.Get("profilePicture").ToString();
            email = snapshot.Get("email") != null ? snapshot.Get("email").ToString() : "";

            if (email != null)
            {
                string fullName = firstName + " " + lastName;

                //Store the original image path for comparison
               originalImagePath = profPic;
               System.Diagnostics.Debug.WriteLine("Original Image Path In Success: " + originalImagePath);

                profilePic.SetImageBitmap(BitmapFactory.DecodeFile(profPic));
                user_name.Text = fullName;
                user_email.Text = currentUser.Email;

            }
        }

        private void DataChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            SetButtonState();
        }

        private void SetFieldEditableState(bool isEditable)
        {
            userNameEditText.Enabled = isEditable;
            firstNameEditText.Enabled = isEditable;
            lastNameEditText.Enabled = isEditable;
            profilePic.Enabled = isEditable;
        }

        private void SetButtonState()
        {
            if (isEditMode)
            {
                var dataChange = userNameEditText.Text != originalUserNameText || firstNameEditText.Text != originalFirstNameText || lastNameEditText.Text != originalLastNameText;

                profilePic.Clickable = true;
                cancel.Visibility = ViewStates.Visible;
                editProfile.Text = "Save";

                if (userNameEditText.Text != originalUserNameText || firstNameEditText.Text != originalFirstNameText || lastNameEditText.Text != originalLastNameText || profPic != originalImagePath)
                {
                    System.Diagnostics.Debug.WriteLine("Original Image Path: " + originalImagePath);
                    System.Diagnostics.Debug.WriteLine("Image Path: " + imagePath);
                    // Enable the save button in edit mode
                    editProfile.Enabled = true;
                    editProfile.SetTextColor(Color.White);
                    editProfile.SetBackgroundResource(Resource.Drawable.button_gradient);
                }
                else
                {
                    // Disable the save button initially
                    editProfile.Enabled = false;
                    editProfile.SetTextColor(Color.Gray);
                    editProfile.SetBackgroundResource(Resource.Drawable.inactive_button);
                }
            }
            else
            {
                editProfile.Text = "Edit Profile";
                cancel.Visibility = Android.Views.ViewStates.Gone;
                // Enable the save button in edit mode
                editProfile.Enabled = true;
                editProfile.SetTextColor(Color.White);
                editProfile.SetBackgroundResource(Resource.Drawable.button_gradient);
            }
        }

        public override void OnBackPressed()
        {
            if (isEditMode)
            {
                if (userNameEditText.Text != originalUserNameText ||
                    firstNameEditText.Text != originalFirstNameText ||
                    lastNameEditText.Text != originalLastNameText)
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Discard Changes");
                    builder.SetMessage("Are you sure you want to discard the changes?");
                    builder.SetPositiveButton("Discard", (sender, args) =>
                    {
                        // Hide text views and show edit texts
                        user_name.Visibility = Android.Views.ViewStates.Visible;
                        user_email.Visibility = Android.Views.ViewStates.Visible;

                        firstNameEditText.Visibility = Android.Views.ViewStates.Gone;
                        lastNameEditText.Visibility = Android.Views.ViewStates.Gone;
                        userNameEditText.Visibility = Android.Views.ViewStates.Gone;

                        profilePic.Enabled = false;

                        // Reset text fields to original values
                        firstNameEditText.Text = firstName;
                        lastNameEditText.Text = lastName;
                        userNameEditText.Text = userName;
                        profilePic.SetImageBitmap(BitmapFactory.DecodeFile(profPic));

                        // Exit edit mode
                        isEditMode = false;
                        cancel.Visibility = Android.Views.ViewStates.Gone;
                        editProfile.Text = "Edit Profile";
                        // Enable the save button in edit mode
                        editProfile.Enabled = true;
                        editProfile.SetTextColor(Color.White);
                        editProfile.SetBackgroundResource(Resource.Drawable.button_gradient);

                        // Refresh the activity
                        Recreate();
                    });
                    builder.SetNegativeButton("Cancel", (sender, args) =>
                    {
                        // Do nothing, stay in the current activity
                    });
                    AlertDialog dialog = builder.Create();
                    dialog.Show();
                }
                else
                {
                    // Hide text views and show edit texts
                    user_name.Visibility = Android.Views.ViewStates.Visible;
                    user_email.Visibility = Android.Views.ViewStates.Visible;

                    firstNameEditText.Visibility = Android.Views.ViewStates.Gone;
                    lastNameEditText.Visibility = Android.Views.ViewStates.Gone;
                    userNameEditText.Visibility = Android.Views.ViewStates.Gone;

                    profilePic.Clickable = false;

                    // Reset text fields to original values
                    firstNameEditText.Text = firstName;
                    lastNameEditText.Text = lastName;
                    userNameEditText.Text = userName;
                    profilePic.SetImageBitmap(BitmapFactory.DecodeFile(profPic));

                    // Exit edit mode
                    isEditMode = false;
                    cancel.Visibility = Android.Views.ViewStates.Gone;
                    editProfile.Text = "Edit Profile";
                    // Enable the save button in edit mode
                    editProfile.Enabled = true;
                    editProfile.SetTextColor(Color.White);
                    editProfile.SetBackgroundResource(Resource.Drawable.button_gradient);

                    // Refresh the activity
                    Recreate();
                }
            }
            else
            {
                // Prepare the result intent
                Intent resultIntent = new Intent();
                resultIntent.PutExtra("userName", originalUserNameText);
                resultIntent.PutExtra("firstName", originalFirstNameText);
                resultIntent.PutExtra("lastName", originalLastNameText);

                // Set the result and finish the activity
                SetResult(Result.Ok, resultIntent);
                Finish();

                base.OnBackPressed();
            }
        }


        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            // Hide the keyboard
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(cancel.WindowToken, 0);

            if (isEditMode)
            {
                if (userNameEditText.Text != originalUserNameText ||
                    firstNameEditText.Text != originalFirstNameText ||
                    lastNameEditText.Text != originalLastNameText)
                {
                    // Cancel button clicked in edit mode, show confirmation alert
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Cancel Edit");
                    alert.SetMessage("Are you sure you want to cancel your changes?");
                    alert.SetPositiveButton("Yes", (dialog, which) =>
                    {
                        // User confirmed, exit edit mode without saving changes

                        // Hide text views and show edit texts
                        user_name.Visibility = Android.Views.ViewStates.Visible;
                        user_email.Visibility = Android.Views.ViewStates.Visible;

                        firstNameEditText.Visibility = Android.Views.ViewStates.Gone;
                        lastNameEditText.Visibility = Android.Views.ViewStates.Gone;
                        userNameEditText.Visibility = Android.Views.ViewStates.Gone;

                        profilePic.Clickable = false;

                        // Reset text fields to original values
                        firstNameEditText.Text = firstName;
                        lastNameEditText.Text = lastName;
                        userNameEditText.Text = userName;
                        profilePic.SetImageBitmap(BitmapFactory.DecodeFile(profPic));

                        // Exit edit mode
                        isEditMode = false;
                        cancel.Visibility = Android.Views.ViewStates.Gone;
                        editProfile.Text = "Edit Profile";
                        // Enable the save button in edit mode
                        editProfile.Enabled = true;
                        editProfile.SetTextColor(Color.White);
                        editProfile.SetBackgroundResource(Resource.Drawable.button_gradient);

                        // Refresh the activity
                        Recreate();
                    });
                    alert.SetNegativeButton("No", (dialog, which) =>
                    {
                        // User canceled, do nothing
                    });
                    alert.Show();
                }
                else
                {
                    // Hide text views and show edit texts
                    user_name.Visibility = Android.Views.ViewStates.Visible;
                    user_email.Visibility = Android.Views.ViewStates.Visible;

                    firstNameEditText.Visibility = Android.Views.ViewStates.Gone;
                    lastNameEditText.Visibility = Android.Views.ViewStates.Gone;
                    userNameEditText.Visibility = Android.Views.ViewStates.Gone;

                    profilePic.Clickable = false;

                    // Reset text fields to original values
                    firstNameEditText.Text = firstName;
                    lastNameEditText.Text = lastName;
                    userNameEditText.Text = userName;
                    profilePic.SetImageBitmap(BitmapFactory.DecodeFile(profPic));

                    // Exit edit mode
                    isEditMode = false;
                    cancel.Visibility = Android.Views.ViewStates.Gone;
                    editProfile.Text = "Edit Profile";
                    // Enable the save button in edit mode
                    editProfile.Enabled = true;
                    editProfile.SetTextColor(Color.White);
                    editProfile.SetBackgroundResource(Resource.Drawable.button_gradient);

                    // Refresh the activity
                    Recreate();
                }
            }      
        }

    }
}