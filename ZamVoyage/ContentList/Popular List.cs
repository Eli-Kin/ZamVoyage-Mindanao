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
    public class Popular_List : AppCompatActivity
    {

        private RecyclerView recyclerView;
        private List<ItemModel> itemList;
        private RecyclerView.Adapter adapter;

        Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.list_popular);

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));

            // populate your list of items here
            itemList = new List<ItemModel>
            {
                new CategoryHeaderItem { CategoryTitle = "Attractions" },
                new ItemModel { Image = Resource.Drawable.great_santa_cruz_island_1, Title = "Great Santa Cruz Island", Description = "Also known as “Pink Beach”, is a popular tourist destination located off the coast of Zamboanga City, Philippines. "},
                 new ItemModel { Image = Resource.Drawable.lantawan_grassland_1, Title = "Lantawan Grassland", Description = "It is a mountain located in Lantawan, Pasonanca. The mountain is covered in lush grasslands and is home to a wide variety of flora and fauna. "},
                new ItemModel { Image = Resource.Drawable.the_pasonanca_park_1, Title = "The Pasonanca Park", Description = "It is a popular tourist attraction located in Zamboanga City, Philippines. There is no entrance fee to visit the tree house, but visitors are encouraged to make a donation to help maintain the park."},
                new ItemModel { Image = Resource.Drawable.once_island_1, Title = "Once Island", Description = "Once Islas is a nature reserve that became an ecotourism spot back in 2018 in an effort to showcase the beauty of its islands and provide livelihood for its locals."},

                new CategoryHeaderItem { CategoryTitle = "Activities" },
                new ItemModel { Image = Resource.Drawable.the_hermosa_festival_1, Title = "The Hermosa Festival", Description = "One of the most popular cultural events in Zamboanga City. It is celebrated every October in honor of the city's patron saint, Nuestra Señora del Pilar."},
                new ItemModel { Image = Resource.Drawable.flores_de_mayo_1, Title = "Flores de Mayo", Description = "“Flowers of May” is the direct translation of this festival’s Spanish name. It is a monthlong revelry that is held during May annually, and it is celebrated in honor of the Virgin Mary." },

                new CategoryHeaderItem { CategoryTitle = "Amenities" },
                new ItemModel { Image = Resource.Drawable.knickerbocker_1, Title = "Knickerbocker", Description = "Think of it as the fruitier, healthier stepsister of the halo-halo. This crazy colorful concoction is a mix of jellies and fresh fruits like watermelon, apple, pineapple, and mango, drizzled with condensed milk and topped with shaved ice and strawberry ice cream. "},
                new ItemModel { Image = Resource.Drawable.chikalang_1, Title = "Chikalang", Description = "It is a snack food in bilao or packed in plastic usually sold by street vendors in Zamboanga city. It is a local delicacy that is made from glutinous rice shaped like a twisted donut or pinilipit. It is fried and coated with flour and caramelized sugar. "},
                new ItemModel { Image = Resource.Drawable.kcc_mall_de_zamboanga_1, Title = "KCC Mall de Zamboanga", Description = "KCC Mall de Zamboanga is a large shopping mall located in Barangay Canelar, Zamboanga City. This mall is one of the biggest malls in Zamboanga City. "},
                new ItemModel { Image = Resource.Drawable.alvar_seafood_restaurant_1, Title = "Alavar Seafood Restaurant", Description = "It is a popular seafood restaurant located in the heart of Zamboanga City. The restaurant is known for its delicious seafood dishes, particularly its famous curacha dish. "},

                new CategoryHeaderItem { CategoryTitle = "Accommodations" },
                new ItemModel { Image = Resource.Drawable.casa_canelar_pension_1, Title = "Casa Canelar Pension", Description = "It is a cozy hotel located in the heart of Zamboanga City. The hotel offers comfortable rooms that are equipped with air conditioning, a flat-screen TV, and a private bathroom."},
                new ItemModel { Image = Resource.Drawable.grand_astoria_hotel_1, Title = "Grand Astoria Hotel", Description = "It is another popular hotel located in Mayor Jaldon Street, Zamboanga City. The hotel offers a variety of rooms, ranging from deluxe rooms to suites, with rates starting at around PHP 3,000 per night." },

                new CategoryHeaderItem { CategoryTitle = "Accessibilities" },
                new ItemModel { Image = Resource.Drawable.airplane, Title = "Zamboanga International Airport", Description = "The airport serves the Zamboanga Peninsula region and is a hub for several airlines, including Cebu Pacific, Philippine Airlines, and AirAsia. "},
                new ItemModel { Image = Resource.Drawable.ship, Title = "Zamboanga International Seaport", Description = "The seaport is one of the busiest in the region and serves as a gateway to nearby destinations such as Basilan, Jolo, and Tawi-Tawi." },
            };

            adapter = new MyAdapter(itemList);
            recyclerView.SetAdapter(adapter);

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }


        private class MyAdapter : RecyclerView.Adapter
        {
            private const int ViewTypeCategoryHeader = 0;
            private const int ViewTypeItem = 1;

            private List<ItemModel> items;

            public MyAdapter(List<ItemModel> items)
            {
                this.items = items;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                if (viewType == ViewTypeCategoryHeader)
                {
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.category_header_layout, parent, false);
                    return new CategoryHeaderViewHolder(itemView);
                }
                else
                {
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_layout, parent, false);
                    return new MyViewHolder(itemView);
                }
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                int viewType = GetItemViewType(position);

                if (viewType == ViewTypeCategoryHeader)
                {
                    CategoryHeaderViewHolder headerViewHolder = holder as CategoryHeaderViewHolder;
                    headerViewHolder.CategoryTitleTextView.Text = ((CategoryHeaderItem)items[position]).CategoryTitle;
                }
                else
                {
                    MyViewHolder itemViewHolder = holder as MyViewHolder;
                    itemViewHolder.ImageView.SetImageResource(items[position].Image);
                    itemViewHolder.TitleTextView.Text = items[position].Title;
                    itemViewHolder.DescriptionTextView.Text = items[position].Description;
                }
            }

            public override int GetItemViewType(int position)
            {
                if (items[position] is CategoryHeaderItem)
                {
                    return ViewTypeCategoryHeader;
                }
                else
                {
                    return ViewTypeItem;
                }
            }

            public override int ItemCount => items.Count;

            private class CategoryHeaderViewHolder : RecyclerView.ViewHolder
            {
                public TextView CategoryTitleTextView { get; }

                public CategoryHeaderViewHolder(View itemView) : base(itemView)
                {
                    CategoryTitleTextView = itemView.FindViewById<TextView>(Resource.Id.category_title_text_view);
                }
            }

            private class MyViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
            {
                private Context context;
                public ImageView ImageView { get; }
                public TextView TitleTextView { get; }
                public TextView DescriptionTextView { get; }

                public MyViewHolder(View itemView) : base(itemView)
                {
                    ImageView = itemView.FindViewById<ImageView>(Resource.Id.image_view);
                    TitleTextView = itemView.FindViewById<TextView>(Resource.Id.title_text_view);
                    DescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.descript_text_view);

                    this.context = context;
                    itemView.SetOnClickListener(this); // Set the click listener for the item view
                }

                public void OnClick(View v)
                {
                    if (TitleTextView.Text == "Great Santa Cruz Island")
                    {
                        var intent = new Intent(context, typeof(Great_Santa_Cruz_Island));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Once Island")
                    {
                        var intent = new Intent(context, typeof(Once_Islas));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Lantawan Grassland")
                    {
                        var intent = new Intent(context, typeof(Lantawan_Grassland));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Merloquet Falls")
                    {
                        var intent = new Intent(context, typeof(Merloquet_Falls));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "The Pasonanca Park")
                    {
                        var intent = new Intent(context, typeof(The_Pasonanca_Park));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "The Hermosa Festival")
                    {
                        var intent = new Intent(context, typeof(Hermosa_Festival));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Knickerbocker")
                    {
                        var intent = new Intent(context, typeof(Knickerbocker));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "KCC Mall de Zamboanga")
                    {
                        var intent = new Intent(context, typeof(KCC_Mall_de_Zamboanga));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Alavar Seafood Restaurant")
                    {
                        var intent = new Intent(context, typeof(Alavar_Seafood_Restaurant));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Flores de Mayo")
                    {
                        var intent = new Intent(context, typeof(Flores_De_Mayo));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Casa Canelar Pension")
                    {
                        var intent = new Intent(context, typeof(Casa_Canelar_Pension));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Chikalang")
                    {
                        var intent = new Intent(context, typeof(Chikalang));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Grand Astoria Hotel")
                    {
                        var intent = new Intent(context, typeof(Grand_Astoria_Hotel));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Zamboanga International Airport")
                    {
                        var intent = new Intent(context, typeof(Zamboanga_International_Airport));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Zamboanga International Seaport")
                    {
                        var intent = new Intent(context, typeof(Zamboanga_International_Seaport));
                        context.StartActivity(intent);
                    }
                }
            }

        }

    }
}