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
using ZamVoyage.ContentList;

namespace ZamVoyage.Content.Activities
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class Additional_Activities : AppCompatActivity
    {
        TextView prgph1, prgph2, prgph3, prgph4, prgph5, prgph6, prgph7, prgph8, prgph9, title;
        Button festivals;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.layout_additional_activities);

            Typeface Abeezee = Typeface.CreateFromAsset(Assets, "abeezee.ttf");
            Typeface MontserratSemiBold = Typeface.CreateFromAsset(Assets, "montserrat_semi_bold.ttf");
            Typeface MontserratExtraBold = Typeface.CreateFromAsset(Assets, "montserrat_extra_bold.ttf");
            Typeface Roboto = Typeface.CreateFromAsset(Assets, "roboto.ttf");
            Typeface Poppins = Typeface.CreateFromAsset(Assets, "poppins.ttf");

            festivals = FindViewById<Button>(Resource.Id.festivals);

            festivals.Click += delegate
            {
                Intent intent = new Intent(this, typeof(Activity_List));
                StartActivity(intent);
            };

            title = FindViewById<TextView>(Resource.Id.title);
            prgph1 = FindViewById<TextView>(Resource.Id.prgph1);
            prgph2 = FindViewById<TextView>(Resource.Id.prgph2);
            prgph3 = FindViewById<TextView>(Resource.Id.prgph3);
            prgph5 = FindViewById<TextView>(Resource.Id.prgph5);
            prgph6 = FindViewById<TextView>(Resource.Id.prgph6);
            prgph7 = FindViewById<TextView>(Resource.Id.prgph7);
            prgph9 = FindViewById<TextView>(Resource.Id.prgph9);

            title.Typeface = MontserratExtraBold;
            prgph1.Typeface = Abeezee;
            prgph2.Typeface = Abeezee;
            prgph3.Typeface = Abeezee;
            prgph5.Typeface = Abeezee;
            prgph6.Typeface = Abeezee;
            prgph7.Typeface = Abeezee;
            prgph9.Typeface = Abeezee;

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            ViewPager viewPager1 = FindViewById<ViewPager>(Resource.Id.viewPager1);
            TabLayout tabLayout1 = FindViewById<TabLayout>(Resource.Id.indicator1);
            List<int> imageList1 = new List<int>
            {
                Resource.Drawable.island_hopping_1,
                Resource.Drawable.island_hopping_2,
                Resource.Drawable.island_hopping_3,
                Resource.Drawable.island_hopping_4,
            };
            SlideshowAdapter adapter1 = new SlideshowAdapter(this, imageList1);
            viewPager1.Adapter = adapter1;

            // Set up TabLayout and ViewPager interaction
            viewPager1.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout1));
            tabLayout1.SetupWithViewPager(viewPager1, true);

            tabLayout1.GetTabAt(1).Select();

            // Add OnPageChangeListener and hide extra tabs for viewPager1
            viewPager1.AddOnPageChangeListener(new ViewPagerCyclicScrollListener(viewPager1, adapter1));
            ViewGroup tabStrip1 = (ViewGroup)tabLayout1.GetChildAt(0);
            tabStrip1.GetChildAt(0).Visibility = ViewStates.Gone;
            tabStrip1.GetChildAt(imageList1.Count + 1).Visibility = ViewStates.Gone;

            ViewPager viewPager2 = FindViewById<ViewPager>(Resource.Id.viewPager2);
            TabLayout tabLayout2 = FindViewById<TabLayout>(Resource.Id.indicator2);
            List<int> imageList2 = new List<int>
            {
                Resource.Drawable.explore_pink_beach_1,
                Resource.Drawable.explore_pink_beach_2,
                Resource.Drawable.explore_pink_beach_3,
                Resource.Drawable.explore_pink_beach_4,
            };
            SlideshowAdapter adapter2 = new SlideshowAdapter(this, imageList2);
            viewPager2.Adapter = adapter2;

            // Set up TabLayout and ViewPager interaction
            viewPager2.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout2));
            tabLayout2.SetupWithViewPager(viewPager2, true);

            tabLayout2.GetTabAt(1).Select();

            // Add OnPageChangeListener and hide extra tabs for viewPager2
            viewPager2.AddOnPageChangeListener(new ViewPagerCyclicScrollListener(viewPager2, adapter2));
            ViewGroup tabStrip2 = (ViewGroup)tabLayout2.GetChildAt(0);
            tabStrip2.GetChildAt(0).Visibility = ViewStates.Gone;
            tabStrip2.GetChildAt(imageList2.Count + 1).Visibility = ViewStates.Gone;

            ViewPager viewPager3 = FindViewById<ViewPager>(Resource.Id.viewPager3);
            TabLayout tabLayout3 = FindViewById<TabLayout>(Resource.Id.indicator3);
            List<int> imageList3 = new List<int>
            {
                Resource.Drawable.explore_merloquet_falls_1,
                Resource.Drawable.explore_merloquet_falls_2,
                Resource.Drawable.explore_merloquet_falls_3,
                Resource.Drawable.explore_merloquet_falls_4,
            };
            SlideshowAdapter adapter3 = new SlideshowAdapter(this, imageList3);
            viewPager3.Adapter = adapter3;

            // Set up TabLayout and ViewPager interaction
            viewPager3.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout3));
            tabLayout3.SetupWithViewPager(viewPager3, true);

            tabLayout3.GetTabAt(1).Select();

            // Add OnPageChangeListener and hide extra tabs for viewPager3
            viewPager3.AddOnPageChangeListener(new ViewPagerCyclicScrollListener(viewPager3, adapter3));
            ViewGroup tabStrip3 = (ViewGroup)tabLayout3.GetChildAt(0);
            tabStrip3.GetChildAt(0).Visibility = ViewStates.Gone;
            tabStrip3.GetChildAt(imageList3.Count + 1).Visibility = ViewStates.Gone;

            ViewPager viewPager5 = FindViewById<ViewPager>(Resource.Id.viewPager5);
            TabLayout tabLayout5 = FindViewById<TabLayout>(Resource.Id.indicator5);
            List<int> imageList5 = new List<int>
            {
                Resource.Drawable.visit_camp_1,
                Resource.Drawable.visit_camp_2,
                Resource.Drawable.visit_camp_3,
                Resource.Drawable.visit_camp_4,
            };
            SlideshowAdapter adapter5 = new SlideshowAdapter(this, imageList5);
            viewPager5.Adapter = adapter5;

            // Set up TabLayout and ViewPager interaction
            viewPager5.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout5));
            tabLayout5.SetupWithViewPager(viewPager5, true);

            tabLayout5.GetTabAt(1).Select();

            // Add OnPageChangeListener and hide extra tabs for viewPager5
            viewPager5.AddOnPageChangeListener(new ViewPagerCyclicScrollListener(viewPager5, adapter5));
            ViewGroup tabStrip5 = (ViewGroup)tabLayout5.GetChildAt(0);
            tabStrip5.GetChildAt(0).Visibility = ViewStates.Gone;
            tabStrip5.GetChildAt(imageList5.Count + 1).Visibility = ViewStates.Gone;

            ViewPager viewPager6 = FindViewById<ViewPager>(Resource.Id.viewPager6);
            TabLayout tabLayout6 = FindViewById<TabLayout>(Resource.Id.indicator6);
            List<int> imageList6 = new List<int>
            {
                Resource.Drawable.bird_watching_1,
                Resource.Drawable.bird_watching_2,
                Resource.Drawable.bird_watching_3,
                Resource.Drawable.bird_watching_4,
            };
            SlideshowAdapter adapter6 = new SlideshowAdapter(this, imageList6);
            viewPager6.Adapter = adapter6;

            // Set up TabLayout and ViewPager interaction
            viewPager6.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout6));
            tabLayout6.SetupWithViewPager(viewPager6, true);

            tabLayout6.GetTabAt(1).Select();

            // Add OnPageChangeListener and hide extra tabs for viewPager6
            viewPager6.AddOnPageChangeListener(new ViewPagerCyclicScrollListener(viewPager6, adapter6));
            ViewGroup tabStrip6 = (ViewGroup)tabLayout6.GetChildAt(0);
            tabStrip6.GetChildAt(0).Visibility = ViewStates.Gone;
            tabStrip6.GetChildAt(imageList6.Count + 1).Visibility = ViewStates.Gone;

            ViewPager viewPager7 = FindViewById<ViewPager>(Resource.Id.viewPager7);
            TabLayout tabLayout7 = FindViewById<TabLayout>(Resource.Id.indicator7);
            List<int> imageList7 = new List<int>
            {
                Resource.Drawable.hiking_1,
                Resource.Drawable.hiking_2,
                Resource.Drawable.hiking_3,
                Resource.Drawable.hiking_4,
            };
            SlideshowAdapter adapter7 = new SlideshowAdapter(this, imageList7);
            viewPager7.Adapter = adapter7;

            // Set up TabLayout and ViewPager interaction
            viewPager7.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout7));
            tabLayout7.SetupWithViewPager(viewPager7, true);

            tabLayout7.GetTabAt(1).Select();

            // Add OnPageChangeListener and hide extra tabs for viewPager7
            viewPager7.AddOnPageChangeListener(new ViewPagerCyclicScrollListener(viewPager7, adapter7));
            ViewGroup tabStrip7 = (ViewGroup)tabLayout7.GetChildAt(0);
            tabStrip7.GetChildAt(0).Visibility = ViewStates.Gone;
            tabStrip7.GetChildAt(imageList7.Count + 1).Visibility = ViewStates.Gone;

            ViewPager viewPager9 = FindViewById<ViewPager>(Resource.Id.viewPager9);
            TabLayout tabLayout9 = FindViewById<TabLayout>(Resource.Id.indicator9);
            List<int> imageList9 = new List<int>
            {
                Resource.Drawable.sight_seeing_1,
                Resource.Drawable.sight_seeing_2,
                Resource.Drawable.sight_seeing_3,
                Resource.Drawable.sight_seeing_4,
            };
            SlideshowAdapter adapter9 = new SlideshowAdapter(this, imageList9);
            viewPager9.Adapter = adapter9;

            // Set up TabLayout and ViewPager interaction
            viewPager9.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout9));
            tabLayout9.SetupWithViewPager(viewPager9, true);

            tabLayout9.GetTabAt(1).Select();

            // Add OnPageChangeListener and hide extra tabs for viewPager9
            viewPager9.AddOnPageChangeListener(new ViewPagerCyclicScrollListener(viewPager9, adapter9));
            ViewGroup tabStrip9 = (ViewGroup)tabLayout9.GetChildAt(0);
            tabStrip9.GetChildAt(0).Visibility = ViewStates.Gone;
            tabStrip9.GetChildAt(imageList9.Count + 1).Visibility = ViewStates.Gone;
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

    }
}