using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZamVoyage.Content.Mountains;

namespace ZamVoyage.Search_Features
{
    public class SearchAdapter : RecyclerView.Adapter
    {
        private const int ViewTypeNoFavorites = 0;
        private const int ViewTypeSearchItem = 1;

        private List<Search_Item> items;

        public SearchAdapter(List<Search_Item> items)
        {
            this.items = items;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            Context context = parent.Context;
            LayoutInflater inflater = LayoutInflater.From(context);

            if (viewType == ViewTypeNoFavorites)
            {
                View noSearchView = inflater.Inflate(Resource.Layout.item_no_search, parent, false);
                return new NoSearchViewHolder(noSearchView);
            }
            else
            {
                View itemView = inflater.Inflate(Resource.Layout.item_search_layout, parent, false);
                return new ItemViewHolder(itemView);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ItemViewHolder itemViewHolder)
            {
                Search_Item item = items[position];
                int resourceId = holder.ItemView.Context.Resources.GetIdentifier(item.ImagePath, "drawable", holder.ItemView.Context.PackageName);
                itemViewHolder.imageView.SetImageResource(resourceId);
                itemViewHolder.titleTextView.Text = item.Title;
                itemViewHolder.descriptionTextView.Text = item.Description;
            }
            else if (holder is NoSearchViewHolder noSearchViewHolder)
            {
                TextView noSearchTextView = noSearchViewHolder.ItemView.FindViewById<TextView>(Resource.Id.noSearchTextView);
                noSearchTextView.Text = "Search Here";
            }
        }

        public void UpdateData(List<Search_Item> items)
        {
            this.items = items;
            NotifyDataSetChanged();
        }

        public override int ItemCount
        {
            get { return items.Count == 0 ? 1 : items.Count; }
        }

        public override int GetItemViewType(int position)
        {
            if (items.Count == 0)
                return ViewTypeNoFavorites;
            else
                return ViewTypeSearchItem;
        }

        private class ItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            public ImageView imageView;
            public TextView titleTextView;
            public TextView descriptionTextView;

            public ItemViewHolder(View itemView) : base(itemView)
            {
                imageView = itemView.FindViewById<ImageView>(Resource.Id.image_view);
                titleTextView = itemView.FindViewById<TextView>(Resource.Id.title_text_view);
                descriptionTextView = itemView.FindViewById<TextView>(Resource.Id.descript_text_view);
                itemView.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                string title = titleTextView.Text;

                if (title == "Title 1")
                {
                    var intent = new Intent(v.Context, typeof(Mountain1_Content));
                    v.Context.StartActivity(intent);
                }
                else if (title == "Title 2")
                {
                    // Handle click for Title 2
                }
                else if (title == "Title 3")
                {
                    // Handle click for Title 3
                }
                // Add more conditions for other titles here
                else
                {
                    // Open a different activity or do nothing
                }
            }
        }

        private class NoSearchViewHolder : RecyclerView.ViewHolder
        {
            public NoSearchViewHolder(View itemView) : base(itemView)
            {
                // No additional initialization required
            }
        }
    }
}
