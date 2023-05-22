using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;
using ZamVoyage.Content.Mountains;

namespace ZamVoyage.ContentList
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class Attraction_List : AppCompatActivity
    {
        private RecyclerView recyclerView;
        private List<ItemModel> itemList;
        private RecyclerView.Adapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.list_attraction);

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));

            itemList = new List<ItemModel>
            {
                //Beach
                new CategoryHeaderItem { CategoryTitle = "Beach" },
                new ItemModel { Image = Resource.Drawable.great_santa_cruz_island_1, Title = "Great Santa Cruz Island", Description = "Also known as “Pink Beach”, is a popular tourist destination located off the coast of Zamboanga City, Philippines. ", Category = "Beach" },
                new ItemModel { Image = Resource.Drawable.once_island_1, Title = "Once Island", Description = "Once Islas is a nature reserve that became an ecotourism spot back in 2018 in an effort to showcase the beauty of its islands and provide livelihood for its locals.", Category = "Beach" },
                //Mountain
                new CategoryHeaderItem { CategoryTitle = "Mountain" },
                new ItemModel { Image = Resource.Drawable.lantawan_grassland_1, Title = "Lantawan Grassland", Description = "It is a mountain located in Lantawan, Pasonanca. The mountain is covered in lush grasslands and is home to a wide variety of flora and fauna. ", Category = "Mountain" },
                //Waterfalls
                new CategoryHeaderItem { CategoryTitle = "Waterfalls" },
                new ItemModel { Image = Resource.Drawable.merloquet_falls_1, Title = "Merloquet Falls", Description = "It is a stunning waterfall that is located in Barangay Sibulao, Zamboanga City, Philippines. The waterfall is situated in a lush jungle setting, surrounded by towering cliffs and dense vegetation. ", Category = "Waterfalls" },
                //Landmarks/Historical Sites
                new CategoryHeaderItem { CategoryTitle = "Landmarks/Historical Sites"},
                new ItemModel { Image = Resource.Drawable.the_fort_pilar_shrine_and_museum_1, Title = "The Fort Pilar Shrine and Museum", Description = " It is a popular landmark in Zamboanga City that is steeped in", Category = "Landmarks/Historical Sites" },
                new ItemModel { Image = Resource.Drawable.the_metropolitan_cathedral_of_the_immaculate_conception_1, Title = "The Metropolitan Cathedral of the Immaculate Conception", Description = "It is one of the oldest and most", Category = "Landmarks/Historical Sites" },
                new ItemModel { Image = Resource.Drawable.the_zamboanga_city_hall_1, Title = "The Zamboanga City Hall", Description = "The center government of the city. It is located in the heart of the city and features a beautiful neoclassical architecture. The building was constructed in the early 20th century and has since been renovated and expanded.", Category = "Landmarks/Historical Sites" },
                //Amusement Park
                new CategoryHeaderItem { CategoryTitle = "Amusement Park"},
                new ItemModel { Image = Resource.Drawable.the_pasonanca_park_1, Title = "The Pasonanca Park", Description = "It is a popular tourist attraction located in Zamboanga City, Philippines. There is no entrance fee to visit the tree house, but visitors are encouraged to make a donation to help maintain the park.", Category = "Amusement Park" },
                new ItemModel { Image = Resource.Drawable.paseo_del_mar_1, Title = "Paseo del Mar", Description = "It is a waterfront park that offers a variety of activities and attractions for visitors of all ages. The park is open daily from 6 AM to 10 PM. The park features a variety of attractions,", Category = "Amusement Park" },
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
                    return new ItemViewHolder(itemView);
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
                    ItemViewHolder itemViewHolder = holder as ItemViewHolder;
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

            private class ItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
            {
                private Context context;
                public ImageView ImageView { get; }
                public TextView TitleTextView { get; }
                public TextView DescriptionTextView { get; }

                public ItemViewHolder(View itemView) : base(itemView)
                {
                    ImageView = itemView.FindViewById<ImageView>(Resource.Id.image_view);
                    TitleTextView = itemView.FindViewById<TextView>(Resource.Id.title_text_view);
                    DescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.descript_text_view);

                    context = itemView.Context;
                    itemView.SetOnClickListener(this);
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
                    else if (TitleTextView.Text == "The Fort Pilar Shrine and Museum")
                    {
                        var intent = new Intent(context, typeof(The_Fort_Pilar_Shrine_and_Museum));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "The Metropolitan Cathedral of the Immaculate Conception")
                    {
                        var intent = new Intent(context, typeof(The_Metropolitan_Cathedral_of_the_Immaculate_Conception));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "The Zamboanga City Hall")
                    {
                        var intent = new Intent(context, typeof(The_Zamboanga_City_Hall));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "The Pasonanca Park")
                    {
                        var intent = new Intent(context, typeof(The_Pasonanca_Park));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Paseo del Mar")
                    {
                        var intent = new Intent(context, typeof(Paseo_del_Mar));
                        context.StartActivity(intent);
                    }
                }
            }
        }
    }

    public class CategoryHeaderItem : ItemModel
    {
        public string CategoryTitle { get; set; }
    }
}
