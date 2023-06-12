using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Fragment.App;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using Refractored.Controls;
using ZamVoyage.Bottom_SideBar;
using ZamVoyage.Content.Activities;
using ZamVoyage.Content.History;
using ZamVoyage.ContentList;
using ZamVoyage.Favorites;
using ZamVoyage.Fragments;
using ZamVoyage.Log;
using ZamVoyage.Planner;
using ZamVoyage.Profile;
using ZamVoyage.Search_Features;
using static ZamVoyage.Profile.ProfileActivity;
using ActionBar = Android.App.ActionBar;
using AlertDialog = Android.App.AlertDialog;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace ZamVoyage
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, BottomNavigationView.IOnNavigationItemSelectedListener, IOnSuccessListener, DrawerLayout.IDrawerListener
    {
        private FirebaseAuth firebaseAuth;
        private ActionBarDrawerToggle toggle;
        private FirebaseFirestore db;
        private DrawerLayout drawer;
        TextView Name, Email, Profile;
        Toolbar toolbar;
        CircleImageView profilePic;
        ImageButton favoritesButton, plansButton;
        string firstName, lastName, email, profPic;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Firebase Initialization
            FirebaseApp.InitializeApp(this);
            firebaseAuth = FirebaseAuth.Instance;
            firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);
            FirebaseUser currentUser = firebaseAuth.CurrentUser;

            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustNothing);

            //Toolbar Support
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            favoritesButton = FindViewById<ImageButton>(Resource.Id.favoritesButton);
            plansButton = FindViewById<ImageButton>(Resource.Id.plansButton);

            favoritesButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(Favorites_Activity));
                StartActivity(intent);
            };

            plansButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(PlanListActivity));
                StartActivity(intent);
            };

            //Navigation Drawer
            drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            drawer.AddDrawerListener(this);
            toggle.SyncState();
            toolbar.SetNavigationIcon(Resource.Drawable.ic_drawer_menu);

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            //Bottom Navigation
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            var logoutMenuItem = navigationView.Menu.FindItem(Resource.Id.nav_logout);

            // If there is no current user, update the title of the logout menu item
            if (currentUser == null)
            {
                logoutMenuItem.SetTitle("Exit");
            }

            //Navigation Header
            View headerView = navigationView.GetHeaderView(0);

            Name = headerView.FindViewById<TextView>(Resource.Id.nameShow);
            Email = headerView.FindViewById<TextView>(Resource.Id.emailShow);
            profilePic = (CircleImageView)headerView.FindViewById(Resource.Id.profilePic);
            Profile = headerView.FindViewById<TextView>(Resource.Id.seeProfile);

            //To Profile
            Profile.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(ProfileActivity));
                StartActivity(intent);
            };

            //Allow to function in Offline Mode
            FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder()
            .SetPersistenceEnabled(true)
            .Build();
            db = FirebaseFirestore.GetInstance(FirebaseApp.Instance);
            db.FirestoreSettings = settings;

            //Check if there is a current user
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            if (user != null)
            {
                //If there is a user, get the data in the firebase firestore
                db.Collection("users").Document(firebaseAuth.CurrentUser.Uid).Get().AddOnSuccessListener(this);
                Email.Visibility = ViewStates.Visible;
            }
            else
            {
                //If no user hide the email and set the default text
                Email.Visibility = ViewStates.Gone;
                Name.Text = "Hello Voyager!";
            }

            // Get an instance of the fragment manager
            AndroidX.Fragment.App.FragmentManager fragmentManager = SupportFragmentManager;

            // Begin a new fragment transaction
            AndroidX.Fragment.App.FragmentTransaction transaction = fragmentManager.BeginTransaction();

            // Add the default fragment to the container view
            Home_Fragment defaultFragment = new Home_Fragment();
            transaction.Replace(Resource.Id.fragment_container, defaultFragment);

            // Commit the transaction
            transaction.Commit();

            //Hide the UI
            HideSystemUI();
        }

        //Back Pressed Function
        public override void OnBackPressed()
        {
            // Check if the keyboard is open
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            if (imm.IsAcceptingText)
            {
                // Hide the system UI
                HideSystemUI();
            }
            //If the drawer is open, close the drawer if BackPressed
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                //Normal Backpressed
                base.OnBackPressed();
            }
        }

        //Get the data from another acitivity
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1 && resultCode == Result.Ok)
            {
                // Retrieve the updated data from the intent
                string firstName = data.GetStringExtra("firstName");
                string lastName = data.GetStringExtra("lastName");
                string userName = data.GetStringExtra("userName");

                // Update the data in the navigation drawer
                UpdateNavigationDrawer(firstName, lastName);
            }
        }

        //Update the Navigation drawer from the profile updates
        private void UpdateNavigationDrawer(string firstName, string lastName)
        {
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            View headerView = navigationView.GetHeaderView(0);
            TextView nameTextView = headerView.FindViewById<TextView>(Resource.Id.nameShow);
            string fullName = firstName + " " + lastName;
            nameTextView.Text = fullName;
            System.Diagnostics.Debug.WriteLine("fullName: " + nameTextView.Text);
        }

        //If the drawer is opened refresh the data so that it can be updated once the drawer is open
        public void OnDrawerOpened(View drawerView)
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            if (user != null)
            {
                db.Collection("users").Document(firebaseAuth.CurrentUser.Uid).Get().AddOnSuccessListener(this);
                string fullName = firstName + " " + lastName;
                Name.Text = fullName;
                System.Diagnostics.Debug.WriteLine("fullNameDrawer: " + Name.Text);
            }
        }

        //Default Function
        public void OnDrawerClosed(View drawerView)
        {
            ((DrawerLayout.IDrawerListener)toggle).OnDrawerClosed(drawerView);
        }

        //Default Function
        public void OnDrawerSlide(View drawerView, float slideOffset)
        {
            ((DrawerLayout.IDrawerListener)toggle).OnDrawerSlide(drawerView, slideOffset);
        }

        //Default Function
        public void OnDrawerStateChanged(int newState)
        {
            ((DrawerLayout.IDrawerListener)toggle).OnDrawerStateChanged(newState);
        }

        //Navigation Menu
        public bool OnNavigationItemSelected(IMenuItem item)
        {

            AndroidX.Fragment.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            //Menu ID
            int id = item.ItemId;

            //Navigation Drawer Menu
            //if (id == Resource.Id.nav_home)
            //{
            //    Intent homeIntent = new Intent(this, typeof(MainActivity));
            //    StartActivity(homeIntent);
            //    Finish();
            //}
            if (id == Resource.Id.nav_history)
            {
                Intent intent = new Intent(this, typeof(History));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_attraction)
            {
                Intent intent = new Intent(this, typeof(Attraction_List));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_activities)
            {
                Intent intent = new Intent(this, typeof(Additional_Activities));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_amenities)
            {
                Intent intent = new Intent(this, typeof(Amenity_List));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_accomodation)
            {
                Intent intent = new Intent(this, typeof(Accommodation_List));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_accessibility)
            {
                Intent intent = new Intent(this, typeof(Accessibility_List));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_about_us)
            {
                Intent intent = new Intent(this, typeof(AboutUs_Activity));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_references)
            {
                Intent intent = new Intent(this, typeof(References_Activity));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_contact_us)
            {
                Intent contactUs = new Intent(this, typeof(ContactUs_Activity));
                StartActivity(contactUs);
            }
            else if (id == Resource.Id.nav_settings)
            {
                Intent settings = new Intent(this, typeof(Settings_Activity));
                StartActivity(settings);
            }
            else if (id == Resource.Id.nav_logout)
            {
                //Add confirmation to the logout or exit menu
                if (firebaseAuth.CurrentUser != null)
                {
                    // User is logged in, show logout confirmation dialog
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Confirmation");
                    builder.SetMessage("Are you sure you want to log out?");
                    builder.SetPositiveButton("Logout", (dialog, which) =>
                    {
                        // Sign out the user from FirebaseAuth
                        firebaseAuth.SignOut();

                        // Reset the preference to indicate that the MainActivity has not been launched before
                        var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                        var editor = preferences.Edit();
                        editor.PutBoolean("MainActivityLaunchedBefore", false);
                        editor.Commit();

                        // Launch LoginActivity
                        Intent getStarted = new Intent(this, typeof(Get_Started));
                        StartActivity(getStarted);
                        Finish();
                    });
                    builder.SetNegativeButton("Cancel", (dialog, which) =>
                    {
                        // User cancelled the logout action, do nothing
                    });
                    builder.Show();
                }
                else
                {
                    // User is not logged in, show exit confirmation dialog
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Confirmation");
                    builder.SetMessage("Are you sure you want to exit the app?");
                    builder.SetPositiveButton("Exit", (dialog, which) =>
                    {
                        // Reset the preference to indicate that the MainActivity has not been launched before
                        var preferences = GetSharedPreferences("MyAppPreferences", FileCreationMode.Private);
                        var editor = preferences.Edit();
                        editor.PutBoolean("MainActivityLaunchedBefore", false);
                        editor.Commit();

                        // Launch LoginActivity
                        Intent getStarted = new Intent(this, typeof(Get_Started));
                        StartActivity(getStarted);
                        Finish();
                    });
                    builder.SetNegativeButton("Cancel", (dialog, which) =>
                    {
                        // User cancelled the exit action, do nothing
                    });
                    builder.Show();
                }
            }

            //Bottom Navigation Menu
            else if (id == Resource.Id.navigation_home)
            {
                SupportActionBar.Show();
                toggle.DrawerIndicatorEnabled = true;
                toolbar.SetNavigationIcon(Resource.Drawable.ic_drawer_menu);
                transaction.Replace(Resource.Id.fragment_container, new Home_Fragment());
                transaction.Commit();
                SupportFragmentManager.ExecutePendingTransactions();
                return true;
            }
            else if (id == Resource.Id.navigation_search)
            {
                SupportActionBar.Hide();
                toggle.DrawerIndicatorEnabled = false;
                toolbar.SetNavigationIcon(Resource.Drawable.ic_drawer_menu);
                transaction.Replace(Resource.Id.fragment_container, new Search_Fragment());
                transaction.Commit();
                SupportFragmentManager.ExecutePendingTransactions();
                return true;
            }
            else if (id == Resource.Id.navigation_plan)
            {
                SupportActionBar.Hide();
                toggle.DrawerIndicatorEnabled = false;
                toolbar.SetNavigationIcon(Resource.Drawable.ic_drawer_menu);
                transaction.Replace(Resource.Id.fragment_container, new Plan_Fragment());
                transaction.Commit();
                SupportFragmentManager.ExecutePendingTransactions();
                return true;
            }
            else if (id == Resource.Id.navigation_explore)
            {
                SupportActionBar.Hide();
                toggle.DrawerIndicatorEnabled = false;
                toolbar.SetNavigationIcon(Resource.Drawable.ic_drawer_menu);
                transaction.Replace(Resource.Id.fragment_container, new Explore_Fragment());
                transaction.Commit();
                SupportFragmentManager.ExecutePendingTransactions();
                return true;
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        //Hide the UI method
        private void HideSystemUI()
        {
            // Hide the navigation
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

        //Get the data in the firebase firestore using OnSuccessListener
        public void OnSuccess(Java.Lang.Object result)
        {
            FirebaseUser currentUser = firebaseAuth.CurrentUser;
            var snapshot = (DocumentSnapshot)result;

            firstName = snapshot.Get("firstName").ToString();
            lastName = snapshot.Get("lastName").ToString();
            email = snapshot.Get("email") != null ? snapshot.Get("email").ToString() : "";

            //Handle the profile Picture
            if (snapshot.Contains("profilePicture"))
            {
                var profilePictureObj = snapshot.Get("profilePicture");
                if (profilePictureObj != null)
                {
                    profPic = profilePictureObj.ToString();
                }
                else
                {
                    profPic = null; // Set profPic to null if the value is null in the database
                }
            }
            else
            {
                profPic = null; // Set profPic to null if the "profilePicture" field is not present in the snapshot
            }

            if (email != null)
            {
                string fullName = firstName + " " + lastName;

                if (!string.IsNullOrEmpty(profPic))
                {
                    // Set the profile picture
                    profilePic.SetImageBitmap(BitmapFactory.DecodeFile(profPic));
                }
                else
                {
                    // Set a default profile picture or a placeholder image
                    profilePic.SetImageResource(Resource.Drawable.logo_circle);
                }

                Name.Text = fullName;
                Email.Text = currentUser.Email;
            }
        }

    }
}

