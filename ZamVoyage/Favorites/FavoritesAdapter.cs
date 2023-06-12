using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using ZamVoyage.Content.Activities;
using ZamVoyage.Content.History;
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
            string title = TitleTextView.Text;

            if (title == "Great Santa Cruz Island")
            {
                var intent = new Intent(v.Context, typeof(Great_Santa_Cruz_Island));
                v.Context.StartActivity(intent);
            }
            else if (title == "Once Islas")
            {
                var intent = new Intent(v.Context, typeof(Once_Islas));
                v.Context.StartActivity(intent);
            }
            else if (title == "Lantawan Grassland")
            {
                var intent = new Intent(v.Context, typeof(Lantawan_Grassland));
                v.Context.StartActivity(intent);
            }
            else if (title == "Merloquet Falls")
            {
                var intent = new Intent(v.Context, typeof(Merloquet_Falls));
                v.Context.StartActivity(intent);
            }
            else if (title == "The Fort Pilar Shrine and Museum")
            {
                var intent = new Intent(v.Context, typeof(The_Fort_Pilar_Shrine_and_Museum));
                v.Context.StartActivity(intent);
            }
            else if (title == "Metropolitan Cathedral of the Immaculate Conception")
            {
                var intent = new Intent(v.Context, typeof(The_Metropolitan_Cathedral_of_the_Immaculate_Conception));
                v.Context.StartActivity(intent);
            }
            else if (title == "Zamboanga City Hall")
            {
                var intent = new Intent(v.Context, typeof(The_Zamboanga_City_Hall));
                v.Context.StartActivity(intent);
            }
            else if (title == "The Pasonanca Park")
            {
                var intent = new Intent(v.Context, typeof(The_Pasonanca_Park));
                v.Context.StartActivity(intent);
            }
            else if (title == "Paseo del Mar")
            {
                var intent = new Intent(v.Context, typeof(Paseo_del_Mar));
                v.Context.StartActivity(intent);
            }
            else if (title == "Hermosa Festival")
            {
                var intent = new Intent(v.Context, typeof(Hermosa_Festival));
                v.Context.StartActivity(intent);
            }
            else if (title == "Regatta de Zamboanga")
            {
                var intent = new Intent(v.Context, typeof(The_Regatta_De_Zamboanga));
                v.Context.StartActivity(intent);
            }
            else if (title == "Dia De Zamboanga")
            {
                var intent = new Intent(v.Context, typeof(Dia_De_Zamboanga));
                v.Context.StartActivity(intent);
            }
            else if (title == "Flores De Mayo")
            {
                var intent = new Intent(v.Context, typeof(Flores_De_Mayo));
                v.Context.StartActivity(intent);
            }
            else if (title == "Zamboanga City Bird Festival")
            {
                var intent = new Intent(v.Context, typeof(Zamboanga_City_Bird_Festival));
                v.Context.StartActivity(intent);
            }
            else if (title == "Knickerbocker")
            {
                var intent = new Intent(v.Context, typeof(Knickerbocker));
                v.Context.StartActivity(intent);
            }
            else if (title == "Satti")
            {
                var intent = new Intent(v.Context, typeof(Satti));
                v.Context.StartActivity(intent);
            }
            else if (title == "Lokot-Lokot")
            {
                var intent = new Intent(v.Context, typeof(Lokot_Lokot));
                v.Context.StartActivity(intent);
            }
            else if (title == "Chikalang")
            {
                var intent = new Intent(v.Context, typeof(Chikalang));
                v.Context.StartActivity(intent);
            }
            else if (title == "Curacha")
            {
                var intent = new Intent(v.Context, typeof(Curacha));
                v.Context.StartActivity(intent);
            }
            else if (title == "Holy Smokes: Korean Grill and Resto")
            {
                var intent = new Intent(v.Context, typeof(Holy_Smokes_Korean_Grill_and_Resto));
                v.Context.StartActivity(intent);
            }
            else if (title == "Bay Tal Mal Restaurant")
            {
                var intent = new Intent(v.Context, typeof(Bay_Tal_Mal_Restaurant));
                v.Context.StartActivity(intent);
            }
            else if (title == "Alavar Seafood Restaurant")
            {
                var intent = new Intent(v.Context, typeof(Alavar_Seafood_Restaurant));
                v.Context.StartActivity(intent);
            }
            else if (title == "Kape Zambo Acoustic Bar and Restaurant")
            {
                var intent = new Intent(v.Context, typeof(Kape_Zambo_Acoustic_Bar_and_Restaurant));
                v.Context.StartActivity(intent);
            }
            else if (title == "Brews Almighty")
            {
                var intent = new Intent(v.Context, typeof(Brews_Almighty));
                v.Context.StartActivity(intent);
            }
            else if (title == "Canelar Barter Trade Center")
            {
                var intent = new Intent(v.Context, typeof(Canelar_Barter_Trade_Center));
                v.Context.StartActivity(intent);
            }
            else if (title == "Yakan Weaving Center")
            {
                var intent = new Intent(v.Context, typeof(Yakan_Weaving_Center));
                v.Context.StartActivity(intent);
            }
            else if (title == "KCC Mall de Zamboanga")
            {
                var intent = new Intent(v.Context, typeof(KCC_Mall_de_Zamboanga));
                v.Context.StartActivity(intent);
            }
            else if (title == "Casa Canelar Pension")
            {
                var intent = new Intent(v.Context, typeof(Casa_Canelar_Pension));
                v.Context.StartActivity(intent);
            }
            else if (title == "Garden Orchid")
            {
                var intent = new Intent(v.Context, typeof(Garden_Orchid_Hotel));
                v.Context.StartActivity(intent);
            }
            else if (title == "Grand Astoria Hotel")
            {
                var intent = new Intent(v.Context, typeof(Grand_Astoria_Hotel));
                v.Context.StartActivity(intent);
            }
            else if (title == "Hamilton Business Inn")
            {
                var intent = new Intent(v.Context, typeof(Hamilton_Business_Inn));
                v.Context.StartActivity(intent);
            }
            else if (title == "Lantaka Hotel by the Sea")
            {
                var intent = new Intent(v.Context, typeof(Lantaka_Hotel_by_the_Sea));
                v.Context.StartActivity(intent);
            }
            else if (title == "Zamboanga Town Home Bed and Breakfast")
            {
                var intent = new Intent(v.Context, typeof(Zamboanga_Town_Home_Bed_and_Breakfast));
                v.Context.StartActivity(intent);
            }
            else if (title == "Zamboanga International Airport")
            {
                var intent = new Intent(v.Context, typeof(Zamboanga_International_Airport));
                v.Context.StartActivity(intent);
            }
            else if (title == "Zamboanga International Seaport")
            {
                var intent = new Intent(v.Context, typeof(Zamboanga_International_Seaport));
                v.Context.StartActivity(intent);
            }
            else if (title == "Bus Terminals")
            {
                var intent = new Intent(v.Context, typeof(Bus_Terminals));
                v.Context.StartActivity(intent);
            }
            else if (title == "Taxi")
            {
                var intent = new Intent(v.Context, typeof(Taxi));
                v.Context.StartActivity(intent);
            }
            else if (title == "Tricycle")
            {
                var intent = new Intent(v.Context, typeof(Tricycle));
                v.Context.StartActivity(intent);
            }
            else if (title == "Zamboanga Motor Rental")
            {
                var intent = new Intent(v.Context, typeof(Zamboanga_Motor_Rental));
                v.Context.StartActivity(intent);
            }
            else if (title == "History")
            {
                var intent = new Intent(v.Context, typeof(History));
                v.Context.StartActivity(intent);
            }
            else if (title == "Additional Activities")
            {
                var intent = new Intent(v.Context, typeof(Additional_Activities));
                v.Context.StartActivity(intent);
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
