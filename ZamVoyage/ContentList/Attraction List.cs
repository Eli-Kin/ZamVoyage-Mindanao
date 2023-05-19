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
                new CategoryHeaderItem { CategoryTitle = "Vegetables List" },
                new ItemModel { Image = Resource.Drawable.image1, Title = "Carrot", Description = "This is Vegetables", Category = "Vegetables List" },
                new ItemModel { Image = Resource.Drawable.image1, Title = "Potato", Description = "This is Vegetables", Category = "Vegetables List" },
                new CategoryHeaderItem { CategoryTitle = "Fruits List" },
                new ItemModel { Image = Resource.Drawable.image5, Title = "Apple", Description = "This is Orange", Category = "Fruits List" },
                new ItemModel { Image = Resource.Drawable.image5, Title = "Orange", Description = "This is Orange", Category = "Fruits List" },
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
                    if (TitleTextView.Text == "Carrot")
                    {
                        var intent = new Intent(context, typeof(Mountain1_Content));
                        context.StartActivity(intent);
                    }
                    else
                    {
                        // Open a different activity or do nothing
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
