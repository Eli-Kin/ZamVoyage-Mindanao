using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using Firebase.Auth;
using ZamVoyage.Log;
using Firebase;
using ZamVoyage.Fragments;
using Android.Support.V4.App;
using Android.Preferences;
using Android.Gms.Tasks;
using Firebase.Firestore;
using static Java.Util.Jar.Attributes;
using static ZamVoyage.Profile.ProfileActivity;
using ZamVoyage.Content.Mountains;
using Refractored.Controls;

namespace ZamVoyage
{
    public class Home_Fragment : Fragment, IOnSuccessListener
    {
        private Mountain_Fragment mountainFragment;
        private Waterfalls_Fragment waterfallFragment;
        private Beach_Fragment beachFragment;
        private Fragment currentFragment;
        private Button mountainButton, waterfallButton, beachButton;
        private TextView helloText;
        private RelativeLayout fragmentContainer;
        private FirebaseAuth firebaseAuth;
        private const int PROFILE_ACTIVITY_REQUEST = 1;
        LinearLayout greatSantaCruzIsland, lantawanGrassland, thePasonancaPark, theHermosaFestival, knickerbocker, kccMallDeZamboanga;
        CircleImageView attraction, activities, amenities, accommodation, accessibility;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);     

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            FirebaseApp.InitializeApp(Activity);
            firebaseAuth = FirebaseAuth.Instance;
            firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);
            FirebaseUser currentUser = firebaseAuth.CurrentUser;

            // Inflate the fragment's layout file
            View view = inflater.Inflate(Resource.Layout.fragment_home, container, false);

            TextView viewAll = view.FindViewById<TextView>(Resource.Id.viewAll);
            helloText = view.FindViewById<TextView>(Resource.Id.helloText);

            viewAll.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(ContentList.Popular_List));
                StartActivity(intent);
            };

            attraction = view.FindViewById<CircleImageView>(Resource.Id.attraction);
            activities = view.FindViewById<CircleImageView>(Resource.Id.activities);
            amenities = view.FindViewById<CircleImageView>(Resource.Id.amenities);
            accommodation = view.FindViewById<CircleImageView>(Resource.Id.accommodation);
            accessibility = view.FindViewById<CircleImageView>(Resource.Id.accessibility);

            attraction.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(ContentList.Attraction_List));
                StartActivity(intent);
            };
            activities.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(ContentList.Activity_List));
                StartActivity(intent);
            };
            amenities.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(ContentList.Amenity_List));
                StartActivity(intent);
            };
            accommodation.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(ContentList.Accommodation_List));
                StartActivity(intent);
            };
            accessibility.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(ContentList.Accessibility_List));
                StartActivity(intent);
            };

            greatSantaCruzIsland = view.FindViewById<LinearLayout>(Resource.Id.greatSantaCruzIsland);
            lantawanGrassland = view.FindViewById<LinearLayout>(Resource.Id.lantawanGrassland);
            thePasonancaPark = view.FindViewById<LinearLayout>(Resource.Id.thePasonancaPark);
            theHermosaFestival = view.FindViewById<LinearLayout>(Resource.Id.theHermosaFestival);
            knickerbocker = view.FindViewById<LinearLayout>(Resource.Id.knickerbocker);
            kccMallDeZamboanga = view.FindViewById<LinearLayout>(Resource.Id.kccMallDeZamboanga);

            greatSantaCruzIsland.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(Great_Santa_Cruz_Island));
                StartActivity(intent);
            };
            lantawanGrassland.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(Lantawan_Grassland));
                StartActivity(intent);
            };
            thePasonancaPark.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(The_Pasonanca_Park));
                StartActivity(intent);
            };
            theHermosaFestival.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(Hermosa_Festival));
                StartActivity(intent);
            };
            knickerbocker.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(Knickerbocker));
                StartActivity(intent);
            };
            kccMallDeZamboanga.Click += delegate
            {
                Intent intent = new Intent(this.Activity, typeof(KCC_Mall_de_Zamboanga));
                StartActivity(intent);
            };
            //mountainFragment = new Mountain_Fragment();
            //waterfallFragment = new Waterfalls_Fragment();
            //beachFragment = new Beach_Fragment();

            //mountainButton = view.FindViewById<Button>(Resource.Id.mountain);
            //waterfallButton = view.FindViewById<Button>(Resource.Id.waterfalls);
            //beachButton = view.FindViewById<Button>(Resource.Id.beach);

            FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder()
            .SetPersistenceEnabled(true)
            .Build();

            FirebaseFirestore db = FirebaseFirestore.GetInstance(FirebaseApp.Instance);
            db.FirestoreSettings = settings;

            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            if (user != null)
            {
                db.Collection("users").Document(firebaseAuth.CurrentUser.Uid).Get().AddOnSuccessListener(this);
            }

            //fragmentContainer = view.FindViewById<RelativeLayout>(Resource.Id.fragment_container);

            //mountainButton.Click += Button_Click;
            //waterfallButton.Click += Button_Click;
            //beachButton.Click += Button_Click;

            //int selectedButtonId = PreferenceManager.GetDefaultSharedPreferences(Context).GetInt("selected_button", Resource.Id.mountain);
            //switch (selectedButtonId)
            //{
            //    case Resource.Id.mountain:
            //        currentFragment = mountainFragment;
            //        mountainButton.SetBackgroundResource(Resource.Drawable.rounded_button_focus);
            //        break;
            //    case Resource.Id.waterfalls:
            //        currentFragment = waterfallFragment;
            //        waterfallButton.SetBackgroundResource(Resource.Drawable.rounded_button_focus);
            //        break;
            //    case Resource.Id.beach:
            //        currentFragment = beachFragment;
            //        beachButton.SetBackgroundResource(Resource.Drawable.rounded_button_focus);
            //        break;
            //    default:
            //        currentFragment = mountainFragment; // or any other default fragment you want
            //        mountainButton.SetBackgroundResource(Resource.Drawable.rounded_button_focus);
            //        break;
            //}

            //var transaction = ChildFragmentManager.BeginTransaction();
            //transaction.Replace(Resource.Id.fragment_container, currentFragment);
            //transaction.Commit();

            return view;

        }

        //private void Button_Click(object sender, EventArgs e)
        //{
        //    var button = (Button)sender;
        //    Fragment newFragment = null;
        //    switch (button.Id)
        //    {
        //        case Resource.Id.mountain:
        //            newFragment = mountainFragment;
        //            mountainButton.SetBackgroundResource(Resource.Drawable.rounded_button_focus);
        //            waterfallButton.SetBackgroundResource(Resource.Drawable.rounded_button);
        //            beachButton.SetBackgroundResource(Resource.Drawable.rounded_button);
        //            SaveSelectedButtonPreference(Resource.Id.mountain);
        //            break;
        //        case Resource.Id.waterfalls:
        //            newFragment = waterfallFragment;
        //            waterfallButton.SetBackgroundResource(Resource.Drawable.rounded_button_focus);
        //            beachButton.SetBackgroundResource(Resource.Drawable.rounded_button);
        //            mountainButton.SetBackgroundResource(Resource.Drawable.rounded_button);
        //            SaveSelectedButtonPreference(Resource.Id.waterfalls);
        //            break;
        //        case Resource.Id.beach:
        //            newFragment = beachFragment;
        //            beachButton.SetBackgroundResource(Resource.Drawable.rounded_button_focus);
        //            waterfallButton.SetBackgroundResource(Resource.Drawable.rounded_button);
        //            mountainButton.SetBackgroundResource(Resource.Drawable.rounded_button);
        //            SaveSelectedButtonPreference(Resource.Id.beach);
        //            break;
        //    }
        //    if (newFragment != null && newFragment != currentFragment)
        //    {
        //        currentFragment = newFragment;
        //        var transaction = ChildFragmentManager.BeginTransaction();
        //        transaction.Replace(Resource.Id.fragment_container, currentFragment);
        //        transaction.CommitNow();
        //    }
        //}

        //private void SaveSelectedButtonPreference(int buttonId)
        //{
        //    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
        //    ISharedPreferencesEditor editor = prefs.Edit();
        //    editor.PutInt("selected_button", buttonId);
        //    editor.Apply();
        //}

        public void OnSuccess(Java.Lang.Object result)
        {
            var snapshot = (DocumentSnapshot)result;

            string firstName = snapshot.Get("firstName").ToString();
            string lastName = snapshot.Get("lastName").ToString();
            string userName = snapshot.Get("userName").ToString();
            string email = snapshot.Get("email") != null ? snapshot.Get("email").ToString() : "";

            if (email != null)
            {

                helloText.Text = "Hello " + userName + "!";

            }
        }

    }
}