using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using System.Collections.Generic;
using ZamVoyage.Favorites;
using Android.Support.V4.Content;
using System.Linq;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase;
using Java.Util;

namespace ZamVoyage.Content.Mountains
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class Lantawan_Grassland : AppCompatActivity, Android.Gms.Tasks.IOnSuccessListener
    {
        private bool isToggleOn = false;
        Drawable addedDrawable, addDrawable;
        TextView prgph1, prgph2;
        private FavoriteDatabaseHelper databaseHelper;
        private ImageButton toggleButton;
        private ImageView imageView;
        private TextView title;
        private FavoriteItem favoritesItem;
        private FavoritesAdapter favoritesAdapter;
        int imageResource;

        // Firebase Firestore variables
        private FirebaseAuth firebaseAuth;
        private FirebaseFirestore firestore;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.layout_lantawan_grassland);

            FirebaseApp.InitializeApp(this);
            firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);
            firestore = FirebaseFirestore.GetInstance(FirebaseApp.Instance);

            databaseHelper = new FavoriteDatabaseHelper(this);

            Typeface Abeezee = Typeface.CreateFromAsset(Assets, "abeezee.ttf");
            Typeface MontserratSemiBold = Typeface.CreateFromAsset(Assets, "montserrat_semi_bold.ttf");
            Typeface MontserratExtraBold = Typeface.CreateFromAsset(Assets, "montserrat_extra_bold.ttf");
            Typeface Roboto = Typeface.CreateFromAsset(Assets, "roboto.ttf");
            Typeface Poppins = Typeface.CreateFromAsset(Assets, "poppins.ttf");

            title = FindViewById<TextView>(Resource.Id.title);
            prgph1 = FindViewById<TextView>(Resource.Id.prgph1);

            imageView = FindViewById<ImageView>(Resource.Id.itemImage);

            title.Typeface = MontserratExtraBold;

            prgph1.Typeface = Abeezee;

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            TabLayout tabLayout = FindViewById<TabLayout>(Resource.Id.indicator);
            List<int> imageList = new List<int>
            {
                Resource.Drawable.lantawan_grassland_1,
                Resource.Drawable.lantawan_grassland_2,
                Resource.Drawable.lantawan_grassland_3,
                Resource.Drawable.lantawan_grassland_4,
            };
            SlideshowAdapter adapter = new SlideshowAdapter(this, imageList);
            viewPager.Adapter = adapter;

            // Set up TabLayout and ViewPager interaction
            viewPager.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout));
            tabLayout.SetupWithViewPager(viewPager, true);

            tabLayout.GetTabAt(1).Select();

            toggleButton = FindViewById<ImageButton>(Resource.Id.toggleButton);

            int id = GetContentIdFromDatabase();
            if (id != -1)
            {
                favoritesItem = databaseHelper.GetAll().Find(cm => cm.Id == id);
            }
            else
            {
                favoritesItem = new FavoriteItem
                {
                    ImagePath = "lantawan_grassland_1",
                    Title = "Lantawan Grassland",
                    Description = "It is a mountain located in Lantawan, Pasonanca. The mountain is covered in lush grasslands and is home to a wide variety of flora and fauna."
                };
            }

            imageResource = Resources.GetIdentifier(favoritesItem.ImagePath, "drawable", PackageName);
            imageView.SetImageResource(imageResource);
            title.Text = favoritesItem.Title;
            //prgph1.Text = favoritesItem.Description;

            toggleButton.Click += OnToggleButtonClicked;
        }

        private int GetContentIdFromDatabase()
        {
            List<FavoriteItem> favoritesItems = databaseHelper.GetAll();
            FavoriteItem favoriteContent = favoritesItems.FirstOrDefault(cm => cm.ImagePath == "lantawan_grassland_1" && cm.Title == "Lantawan Grassland" && cm.Description == "It is a mountain located in Lantawan, Pasonanca. The mountain is covered in lush grasslands and is home to a wide variety of flora and fauna.");
            return favoriteContent?.Id ?? -1;
        }

        private void OnToggleButtonClicked(object sender, EventArgs e)
        {
            addedDrawable = Resources.GetDrawable(Resource.Drawable.ic_added);
            addedDrawable.SetTint(Color.ParseColor("#0a6ac2"));

            addDrawable = Resources.GetDrawable(Resource.Drawable.ic_add);
            addDrawable.SetTint(Color.ParseColor("#0D8BFF"));

            var toggleButton = (ImageButton)sender;

            if (firebaseAuth.CurrentUser != null)
            {
                // User is logged in
                if (isToggleOn)
                {
                    RemoveFromFavoritesFirestore();
                    toggleButton.SetImageDrawable(addDrawable);
                    isToggleOn = false;
                }
                else
                {
                    AddToFavoritesFirestore();
                    toggleButton.SetImageDrawable(addedDrawable);
                    isToggleOn = true;
                }
            }
            else
            {
                // No user is logged in
                if (isToggleOn)
                {
                    RemoveFromFavoritesSQL();
                    toggleButton.SetImageDrawable(addDrawable);
                    isToggleOn = false;
                }
                else
                {
                    AddToFavoritesSQL();
                    toggleButton.SetImageDrawable(addedDrawable);
                    isToggleOn = true;
                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (firebaseAuth.CurrentUser != null)
            {
                // User is logged in, store the toggle state for logged-in user
                firestore.Collection("users").Document(firebaseAuth.CurrentUser.Uid).Collection("favorites").Document("LG").Get().AddOnSuccessListener(this);
            }
            else
            {
                // No user is logged in, store the toggle state for non-logged-in user
                ISharedPreferences prefs = GetSharedPreferences("LGNonLoggedInToggleState", FileCreationMode.Private);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutBoolean("toggleState", isToggleOn);
                editor.Commit();
            }
        }

        private void OnBackPressed()
        {
            // Set the result based on the toggle state
            Intent intent = new Intent();
            intent.PutExtra("ToggleState", isToggleOn);
            SetResult(Result.Ok, intent);

            base.OnBackPressed();
        }


        protected override void OnResume()
        {
            base.OnResume();

            if (firebaseAuth.CurrentUser != null)
            {
                // User is logged in, retrieve the toggle state for logged-in user
                firestore.Collection("users").Document(firebaseAuth.CurrentUser.Uid).Collection("favorites").Document("LG").Get().AddOnSuccessListener(this);
            }
            else
            {
                // No user is logged in, retrieve the toggle state for non-logged-in user
                ISharedPreferences prefs = GetSharedPreferences("LGNonLoggedInToggleState", FileCreationMode.Private);
                isToggleOn = prefs.GetBoolean("toggleState", false);
            }

            addedDrawable = Resources.GetDrawable(Resource.Drawable.ic_added);
            addedDrawable.SetTint(Color.ParseColor("#0a6ac2"));

            addDrawable = Resources.GetDrawable(Resource.Drawable.ic_add);
            addDrawable.SetTint(Color.ParseColor("#0D8BFF"));

            var toggleButton = FindViewById<ImageButton>(Resource.Id.toggleButton);
            toggleButton.SetImageDrawable(isToggleOn ? addedDrawable : addDrawable);
        }


        private async void AddToFavoritesFirestore()
        {
            try
            {
                string uniqueId = Guid.NewGuid().ToString();

                DocumentReference document = firestore.Collection("users").Document(firebaseAuth.CurrentUser.Uid).Collection("favorites").Document("LG");

                favoritesItem.DocumentId = uniqueId;

                HashMap data = new HashMap();
                data.Put("DocumentId", favoritesItem.DocumentId);
                data.Put("ImagePath", favoritesItem.ImagePath);
                data.Put("Title", favoritesItem.Title);
                data.Put("Description", favoritesItem.Description);

                document.Set(data);

                Toast.MakeText(this, "Added to favorites (Firestore)", ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Failed to add content to favorites (Firestore)", ToastLength.Short).Show();
                Console.WriteLine("Firestore Error: " + e.Message);
            }
        }


        private void RemoveFromFavoritesFirestore()
        {
            try
            {
                DocumentReference document = firestore.Collection("users").Document(firebaseAuth.CurrentUser.Uid).Collection("favorites").Document("LG");
                document.Delete();

                Toast.MakeText(this, "Removed from favorites (Firestore)", ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Failed to remove content from favorites (Firestore)", ToastLength.Short).Show();
                Console.WriteLine("Firestore Error: " + e.Message);
            }
        }


        private void AddToFavoritesSQL()
        {
            bool result = databaseHelper.Insert(favoritesItem);
            Recreate();

            if (result)
            {
                Toast.MakeText(this, "Added to favorites (SQL Database)", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Failed to add content to favorites (SQL Database)", ToastLength.Short).Show();
            }
        }

        private void RemoveFromFavoritesSQL()
        {
            bool result = databaseHelper.Delete(favoritesItem.Id);
            Recreate();

            if (result)
            {
                Toast.MakeText(this, "Removed from favorites (SQL Database)", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Failed to remove content from favorites (SQL Database)", ToastLength.Short).Show();
            }
        }


        //protected override void OnPause()
        //{
        //    // Store the toggle state on pause
        //    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
        //    ISharedPreferencesEditor editor = prefs.Edit();
        //    editor.PutBoolean("toggleState", isToggleOn);
        //    editor.Commit();

        //    base.OnPause();
        //}

        //protected override void OnResume()
        //{
        //    // Retrieve the toggle state on resume
        //    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
        //    isToggleOn = prefs.GetBoolean("toggleState", false);

        //    addedDrawable = Resources.GetDrawable(Resource.Drawable.ic_added);
        //    addedDrawable.SetTint(Color.ParseColor("#0a6ac2"));

        //    addDrawable = Resources.GetDrawable(Resource.Drawable.ic_add);
        //    addDrawable.SetTint(Color.ParseColor("#0D8BFF"));

        //    var toggleButton = FindViewById<ImageButton>(Resource.Id.toggleButton);

        //    toggleButton.SetImageDrawable(isToggleOn ? addedDrawable : addDrawable);

        //    base.OnResume();
        //}

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

        public void OnSuccess(Java.Lang.Object result)
        {
            var snapshot = (DocumentSnapshot)result;

            if (snapshot.Exists())
            {
                isToggleOn = true;
            }
            else
            {
                isToggleOn = false;
            }
            addedDrawable = Resources.GetDrawable(Resource.Drawable.ic_added);
            addedDrawable.SetTint(Color.ParseColor("#0a6ac2"));

            addDrawable = Resources.GetDrawable(Resource.Drawable.ic_add);
            addDrawable.SetTint(Color.ParseColor("#0D8BFF"));

            var toggleButton = FindViewById<ImageButton>(Resource.Id.toggleButton);
            toggleButton.SetImageDrawable(isToggleOn ? addedDrawable : addDrawable);
        }
    }
}
