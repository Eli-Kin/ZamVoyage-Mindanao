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
using ZamVoyage.Content.Activities;

namespace ZamVoyage.ContentList
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class Activity_List : AppCompatActivity
    {

        private RecyclerView recyclerView;
        private List<ItemModel> itemList;
        private RecyclerView.Adapter adapter;

        Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.list_activity);

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            //Button additionalActivities = FindViewById<Button>(Resource.Id.additionalActivities);

            //additionalActivities.Click += delegate
            // {
            //     Intent intent = new Intent(this, typeof(Additional_Activities));
            //     StartActivity(intent);
            // };

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));

            // populate your list of items here
            itemList = new List<ItemModel>
            {
                new ItemModel { Image = Resource.Drawable.the_hermosa_festival_1, Title = "The Hermosa Festival", Description = "One of the most popular cultural events in Zamboanga City. It is celebrated every October in honor of the city's patron saint, Nuestra Señora del Pilar."},
                new ItemModel { Image = Resource.Drawable.the_regatta_de_zamboanga_1, Title = "The Regatta de Zamboanga", Description = "A popular annual boat race that takes place in Zamboanga City, Philippines. It is typically held in March and is one of the most colorful and exciting events in the city." },
                new ItemModel { Image = Resource.Drawable.dia_de_zamboanga_1, Title = "Dia De Zamboanga", Description = "It is the biggest festival in Zamboanga City and is celebrated annually on February 26. The festival is a celebration of the city's founding and its rich cultural heritage." },
                new ItemModel { Image = Resource.Drawable.flores_de_mayo_1, Title = "Flores de Mayo", Description = "“Flowers of May” is the direct translation of this festival’s Spanish name. It is a monthlong revelry that is held during May annually, and it is celebrated in honor of the Virgin Mary." },
                new ItemModel { Image = Resource.Drawable.zamboanga_city_bird_festival_1, Title = "Zamboanga City Bird Festival", Description = "Considered as the country’s biggest celebration of avifaunal diversity, Zamboanga City Bird Festival is celebrated by birdwatchers, conservationists, and tourists from local and foreign origins." }
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
                    if (TitleTextView.Text == "The Hermosa Festival")
                    {
                        var intent = new Intent(context, typeof(Hermosa_Festival));
                        context.StartActivity(intent);
                    } 
                    else if (TitleTextView.Text == "The Regatta de Zamboanga")
                    {
                        var intent = new Intent(context, typeof(The_Regatta_De_Zamboanga));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Dia De Zamboanga")
                    {
                        var intent = new Intent(context, typeof(Dia_De_Zamboanga));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Flores de Mayo")
                    {
                        var intent = new Intent(context, typeof(Flores_De_Mayo));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Zamboanga City Bird Festival")
                    {
                        var intent = new Intent(context, typeof(Zamboanga_City_Bird_Festival));
                        context.StartActivity(intent);
                    }
                }
            }

        }

    }
}