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
    public class Amenity_List : AppCompatActivity
    {
        private RecyclerView recyclerView;
        private List<ItemModel> itemList;
        private RecyclerView.Adapter adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.list_amenity);

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
                //Delicacies
                new CategoryHeaderItem { CategoryTitle = "Delicacies" },
                new ItemModel { Image = Resource.Drawable.knickerbocker_1, Title = "Knickerbocker", Description = "Think of it as the fruitier, healthier stepsister of the halo-halo. This crazy colorful concoction is a mix of jellies and fresh fruits like watermelon, apple, pineapple, and mango, drizzled with condensed milk and topped with shaved ice and strawberry ice cream. ", Category = "Delicacies" },
                new ItemModel { Image = Resource.Drawable.satti_1, Title = "Satti", Description = "This popular breakfast dish consists of grilled meat on a stick, usually chicken or beef, served with rice cakes and a spicy sauce made with chili, garlic, and vinegar. ", Category = "Delicacies" },
                new ItemModel { Image = Resource.Drawable.lokot_lokot_1, Title = "Lokot-Lokot", Description = "A local delicacy found in Zamboanga city and other parts of Mindanao. It is called by the locals as “jaa”, “lokot-lokot”, “Zambo Rolls, or “tagktak”, depending on where you find the delicacy. ", Category = "Delicacies" },
                new ItemModel { Image = Resource.Drawable.chikalang_1, Title = "Chikalang", Description = "It is a snack food in bilao or packed in plastic usually sold by street vendors in Zamboanga city. It is a local delicacy that is made from glutinous rice shaped like a twisted donut or pinilipit. It is fried and coated with flour and caramelized sugar. ", Category = "Delicacies" },
                new ItemModel { Image = Resource.Drawable.curacha_1, Title = "Curacha", Description = "Curacha got its name because of its spiky, hairy appearance that kinda resembles you-know-what. But if you look closely, it kinda appears like the love child of a wide crab and a long lobster. ", Category = "Delicacies" },
                //Restaurant
                new CategoryHeaderItem { CategoryTitle = "Restaurant" },
                new ItemModel { Image = Resource.Drawable.holy_smokes_1, Title = "Holy Smokes: Korean Grill and Resto", Description = "It is an Asian restaurant located at 166 Cabato Road, 2F Blue Gym Building, Zamboanga City, Zamboanga Sibugay Province. They offer a variety of Korean dishes such as bulgogi, bibimbap, and Korean fried chicken. ", Category = "Restaurant" },
                new ItemModel { Image = Resource.Drawable.bay_tal_mak_restaurant_1, Title = "Bay Tal Mal Restaurant", Description = "It is a Filipino restaurant located at Mayor Jaldon St, Canelar, Zamboanga City, Zamboanga Sibugay Province. The restaurant offers a variety of Filipino dishes such as adobo, sinigang, and kare-kare. ", Category = "Restaurant" },
                new ItemModel { Image = Resource.Drawable.alvar_seafood_restaurant_1, Title = "Alavar Seafood Restaurant", Description = "It is a popular seafood restaurant located in the heart of Zamboanga City. The restaurant is known for its delicious seafood dishes, particularly its famous curacha dish. ", Category = "Restaurant" },
                new ItemModel { Image = Resource.Drawable.kape_zambo_acoustic_bar_and_restaurant_1, Title = "Kape Zambo Acoustic Bar and Restaurant", Description = "The restaurant is known for its acoustic music and great food. They offer a variety of dishes, including Filipino and Asian cuisine.", Category = "Restaurant" },
                new ItemModel { Image = Resource.Drawable.brews_almighty_1, Title = "Brews Almighty", Description = "It is a cozy cafe located in Zamboanga City. The cafe serves a variety of coffee drinks, such as espresso, latte, cappuccino, and cold brew. They also have a selection of teas and other beverages. ", Category = "Restaurant" },
                //Souvenir Shops
                new CategoryHeaderItem { CategoryTitle = "Souvenir Shops"},
                new ItemModel { Image = Resource.Drawable.canelar_barter_trade_center_1, Title = "Canelar Barter Trade Center", Description = "Canelar Barter Trade Center is an Arts and Crafts Store located in Barangay Canelar, Zamboanga, Zamboanga Peninsula. It is a great place to find unique and handmade items, such as woven baskets, textiles, and wood carvings. ", Category = "Souvenir Shops" },
                new ItemModel { Image = Resource.Drawable.yakan_weaving_center_1, Title = "Yakan Weaving Center", Description = "It is a cultural center located in Barangay Upper Calarian, Zamboanga City. The center is dedicated to preserving and promoting the traditional art of Yakan weaving. ", Category = "Souvenir Shops" },
                //Malls
                new CategoryHeaderItem { CategoryTitle = "Mall"},
                new ItemModel { Image = Resource.Drawable.kcc_mall_de_zamboanga_1, Title = "KCC Mall de Zamboanga", Description = "KCC Mall de Zamboanga is a large shopping mall located in Barangay Canelar, Zamboanga City. This mall is one of the biggest malls in Zamboanga City. ", Category = "Mall" },
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
                    if (TitleTextView.Text == "Knickerbocker")
                    {
                        var intent = new Intent(context, typeof(Knickerbocker));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Satti")
                    {
                        var intent = new Intent(context, typeof(Satti));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Lokot-Lokot")
                    {
                        var intent = new Intent(context, typeof(Lokot_Lokot));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Chikalang")
                    {
                        var intent = new Intent(context, typeof(Chikalang));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Curacha")
                    {
                        var intent = new Intent(context, typeof(Curacha));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Holy Smokes: Korean Grill and Resto")
                    {
                        var intent = new Intent(context, typeof(Holy_Smokes_Korean_Grill_and_Resto));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Bay Tal Mal Restaurant")
                    {
                        var intent = new Intent(context, typeof(Bay_Tal_Mal_Restaurant));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Alavar Seafood Restaurant")
                    {
                        var intent = new Intent(context, typeof(Alavar_Seafood_Restaurant));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Kape Zambo Acoustic Bar and Restaurant")
                    {
                        var intent = new Intent(context, typeof(Kape_Zambo_Acoustic_Bar_and_Restaurant));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Canelar Barter Trade Center")
                    {
                        var intent = new Intent(context, typeof(Canelar_Barter_Trade_Center));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Yakan Weaving Center")
                    {
                        var intent = new Intent(context, typeof(Yakan_Weaving_Center));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "KCC Mall de Zamboanga")
                    {
                        var intent = new Intent(context, typeof(KCC_Mall_de_Zamboanga));
                        context.StartActivity(intent);
                    }
                    else if (TitleTextView.Text == "Brews Almighty")
                    {
                        var intent = new Intent(context, typeof(Brews_Almighty));
                        context.StartActivity(intent);
                    }
                }
            }
        }
    }

}
