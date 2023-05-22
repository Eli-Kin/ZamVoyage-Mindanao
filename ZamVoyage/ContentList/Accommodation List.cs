using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.RecyclerView.Widget;
using static AndroidX.RecyclerView.Widget.RecyclerView;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using AndroidX.AppCompat.Widget;
using Android.Graphics;
using ZamVoyage.Content.Mountains;

namespace ZamVoyage.ContentList
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class Accommodation_List : AppCompatActivity
    {

        private RecyclerView recyclerView;
        private List<ItemModel> itemList;
        private RecyclerView.Adapter adapter;

        Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.list_accommodation);

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));

            // populate your list of items here
            itemList = new List<ItemModel>
            {
                new ItemModel { Image = Resource.Drawable.casa_canelar_pension_1, Title = "Casa Canelar Pension", Description = "It is a cozy hotel located in the heart of Zamboanga City. The hotel offers comfortable rooms that are equipped with air conditioning, a flat-screen TV, and a private bathroom."},
                new ItemModel { Image = Resource.Drawable.hamilton_business_inn_1, Title = "Hamilton Business Inn", Description = "It is a modern hotel located in the heart of Zamboanga City. The hotel offers comfortable rooms that are equipped with air conditioning, a flat-screen TV, and a private bathroom." },
                new ItemModel { Image = Resource.Drawable.zamboanga_town_home_bed_and_breakfast_1, Title = "Zamboanga Town Home Bed and Breakfast", Description = "It is conveniently located in the popular City Proper area. The hotel has everything you need for a comfortable stay. Service-minded staff will welcome and guide you at the Zamboanga Town Home Bed and Breakfast." },
                new ItemModel { Image = Resource.Drawable.garden_orchid_hotel_1, Title = "Garden Orchid Hotel", Description = "It is a luxurious hotel located in Governor Camins Avenue, Zamboanga City. The hotel offers a variety of rooms, ranging from deluxe rooms to suites, with rates starting at around PHP 4,000 per night." },
                new ItemModel { Image = Resource.Drawable.grand_astoria_hotel_1, Title = "Grand Astoria Hotel", Description = "It is another popular hotel located in Mayor Jaldon Street, Zamboanga City. The hotel offers a variety of rooms, ranging from deluxe rooms to suites, with rates starting at around PHP 3,000 per night." },
                new ItemModel { Image = Resource.Drawable.lantaka_hotel_by_the_sea_1, Title = "Lantaka Hotel by the Sea", Description = "It is a beautiful hotel located in Valderosa Street, Zamboanga City. The hotel offers a variety of rooms, ranging from deluxe rooms to suites, with rates starting at around PHP 3,000 per night." },
            };

            adapter = new MyAdapter(itemList);
            recyclerView.SetAdapter(adapter);

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


        private class MyAdapter : RecyclerView.Adapter
        {
            private List<ItemModel> items;

            public MyAdapter(List<ItemModel> items)
            {
                this.items = items;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_layout, parent, false);
                return new MyViewHolder(itemView, parent.Context);
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                MyViewHolder myHolder = holder as MyViewHolder;
                myHolder.ImageView.SetImageResource(items[position].Image);
                myHolder.TitleTextView.Text = items[position].Title;
                myHolder.DescriptionTextView.Text = items[position].Description;
            }

            public override int ItemCount => items.Count;

            private class MyViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
            {
                private Context context;
                public ImageView ImageView { get; }
                public TextView TitleTextView { get; }
                public TextView DescriptionTextView { get; }

                public MyViewHolder(View itemView, Context context) : base(itemView)
                {
                    ImageView = itemView.FindViewById<ImageView>(Resource.Id.image_view);
                    TitleTextView = itemView.FindViewById<TextView>(Resource.Id.title_text_view);
                    DescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.descript_text_view);

                    this.context = context;
                    itemView.SetOnClickListener(this); // Set the click listener for the item view
                }

                public void OnClick(View v)
                {
                    if (TitleTextView.Text == "Casa Canelar Pension")
                    {
                        var intent = new Intent(context, typeof(Casa_Canelar_Pension));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Hamilton Business Inn")
                    {
                        var intent = new Intent(context, typeof(Hamilton_Business_Inn));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Zamboanga Town Home Bed and Breakfast")
                    {
                        var intent = new Intent(context, typeof(Zamboanga_Town_Home_Bed_and_Breakfast));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Grand Astoria Hotel")
                    {
                        var intent = new Intent(context, typeof(Grand_Astoria_Hotel));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Lantaka Hotel by the Sea")
                    {
                        var intent = new Intent(context, typeof(Lantaka_Hotel_by_the_Sea));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Garden Orchid Hotel")
                    {
                        var intent = new Intent(context, typeof(Garden_Orchid_Hotel));
                        context.StartActivity(intent);
                    }
                }
            }

        }

    }
}