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
    public class Accessibility_List : AppCompatActivity
    {

        private RecyclerView recyclerView;
        private List<ItemModel> itemList;
        private RecyclerView.Adapter adapter;

        Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.list_accessibility);

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
                new ItemModel { Image = Resource.Drawable.airplane, Title = "Zamboanga International Airport", Description = "The airport serves the Zamboanga Peninsula region and is a hub for several airlines, including Cebu Pacific, Philippine Airlines, and AirAsia. "},
                new ItemModel { Image = Resource.Drawable.ship, Title = "Zamboanga International Seaport", Description = "The seaport is one of the busiest in the region and serves as a gateway to nearby destinations such as Basilan, Jolo, and Tawi-Tawi." },
                new ItemModel { Image = Resource.Drawable.bus, Title = "Bus Terminals", Description = "The terminal has several bus bays and ticketing counters. The terminal serves several bus companies that operate routes to other cities in Mindanao, including Dapitan, Dipolog, " },
                new ItemModel { Image = Resource.Drawable.motor_rental, Title = "Zamboanga Motor Rental", Description = "A company in Zamboanga City offer a range of motorcycles and scooters for rent, including small and large motorcycles, automatic and manual scooters, and dirt bikes." },
                new ItemModel { Image = Resource.Drawable.taxi, Title = "Taxi", Description = "Most taxis are equipped with air conditioning, and many have a small screen displaying the fare and distance traveled." },
                new ItemModel { Image = Resource.Drawable.tricycle, Title = "Tricycle", Description = "The fare for a tricycle ride is usually negotiated with the driver, and it's important to agree on the fare before starting the ride." },
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
                    if (TitleTextView.Text == "Zamboanga International Airport")
                    {
                        var intent = new Intent(context, typeof(Zamboanga_International_Airport));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Zamboanga International Seaport")
                    {
                        var intent = new Intent(context, typeof(Zamboanga_International_Seaport));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Bus Terminals")
                    {
                        var intent = new Intent(context, typeof(Bus_Terminals));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Zamboanga Motor Rental")
                    {
                        var intent = new Intent(context, typeof(Zamboanga_Motor_Rental));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Taxi")
                    {
                        var intent = new Intent(context, typeof(Taxi));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Tricycle")
                    {
                        var intent = new Intent(context, typeof(Tricycle));
                        context.StartActivity(intent);
                    }
                }
            }

        }

    }
}