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
    public class Mountain_List : AppCompatActivity
    {

        private RecyclerView recyclerView;
        private List<ItemModel> itemList;
        private RecyclerView.Adapter adapter;

        Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.list_mountain);

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
                new ItemModel { Image = Resource.Drawable.image1, Title = "Title 1", Description = "Bla Bla Bla"},
                new ItemModel { Image = Resource.Drawable.image5, Title = "Title 2", Description = "Bla Bla Bla" },
                new ItemModel { Image = Resource.Drawable.image1, Title = "Title 3", Description = "Bla Bla Bla" },
                new ItemModel { Image = Resource.Drawable.image5, Title = "Title 4", Description = "Bla Bla Bla" },
                new ItemModel { Image = Resource.Drawable.image1, Title = "Title 5", Description = "Bla Bla Bla" }
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
                    if (TitleTextView.Text == "Title 1")
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
}