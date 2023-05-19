using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using ZamVoyage.Content.Mountains;

namespace ZamVoyage.Favorites
{
    public class FavoritesAdapter : RecyclerView.Adapter
    {
        private const int ViewTypeNoFavorites = 0;
        private const int ViewTypeFavoriteItem = 1;

        private List<FavoriteItem> items;

        public FavoritesAdapter(List<FavoriteItem> items)
        {
            this.items = items;
        }

        public void AddFavoriteItem(FavoriteItem favoriteItem)
        {
            items.Add(favoriteItem);
            NotifyItemInserted(items.Count - 1);
        }

        public override int GetItemViewType(int position)
        {
            if (items.Count == 0)
                return ViewTypeNoFavorites;
            else
                return ViewTypeFavoriteItem;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);

            if (viewType == ViewTypeNoFavorites)
            {
                View noFavoritesView = inflater.Inflate(Resource.Layout.item_no_favorites, parent, false);
                return new NoFavoritesViewHolder(noFavoritesView);
            }
            else
            {
                View itemView = inflater.Inflate(Resource.Layout.item_favorites, parent, false);
                return new FavoritesAdapterViewHolder(itemView, parent.Context);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            if (viewHolder is FavoritesAdapterViewHolder holder)
            {
                FavoriteItem item = items[position];
                holder.TitleTextView.Text = item.Title;
                holder.DescriptionTextView.Text = item.Description;

                int imageResource = holder.ItemView.Resources.GetIdentifier(item.ImagePath, "drawable", holder.ItemView.Context.PackageName);
                holder.ImageView.SetImageResource(imageResource);
            }
            else if (viewHolder is NoFavoritesViewHolder noFavoritesViewHolder)
            {
                // No favorites to display
                TextView noFavoritesTextView = noFavoritesViewHolder.ItemView.FindViewById<TextView>(Resource.Id.noFavoritesTextView);
                noFavoritesTextView.Text = "No Favorites";
            }
        }

        public override int ItemCount
        {
            get
            {
                if (items.Count == 0)
                    return 1; // Display the "No Favorites" view
                else
                    return items.Count;
            }
        }
    }

    public class FavoritesAdapterViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        public ImageView ImageView { get; }
        public TextView TitleTextView { get; }
        public TextView DescriptionTextView { get; }
        private FavoriteItem favoriteItem;
        private FavoriteDatabaseHelper databaseHelper;
        private Context context;

        public FavoritesAdapterViewHolder(View itemView, Context context) : base(itemView)
        {
            ImageView = itemView.FindViewById<ImageView>(Resource.Id.favoriteImageView);
            TitleTextView = itemView.FindViewById<TextView>(Resource.Id.favoriteTitleTextView);
            DescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.favoriteDescriptionTextView);
            this.context = context;
            itemView.SetOnClickListener(this);
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

    public class NoFavoritesViewHolder : RecyclerView.ViewHolder
    {
        public NoFavoritesViewHolder(View itemView) : base(itemView)
        {
            // No additional initialization required
        }
    }
}
