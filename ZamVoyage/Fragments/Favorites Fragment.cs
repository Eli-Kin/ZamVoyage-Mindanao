using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Java.Nio.FileNio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZamVoyage.Favorites;
using Fragment = Android.Support.V4.App.Fragment;

namespace ZamVoyage.Fragments
{
    public class Favorites_Fragment : Fragment
    {
        private RecyclerView recyclerView;
        private List<FavoriteItem> favoritesItem;
        private FavoriteDatabaseHelper databaseHelper;
        private FavoritesAdapter favoritesAdapter;
        private FirebaseAuth firebaseAuth;
        private FirebaseFirestore firestore;
        private bool hasFavorites;
        ProgressDialog progressDialog;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the fragment's layout file
            View view = inflater.Inflate(Resource.Layout.fragment_favorites, container, false);

            FirebaseApp.InitializeApp(Activity);
            firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);
            firestore = FirebaseFirestore.GetInstance(FirebaseApp.Instance);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.favoritesRecyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Activity));

            if (firebaseAuth.CurrentUser != null)
            {
                GetFavoritesFromFirestore();
            }
            else
            {
                databaseHelper = new FavoriteDatabaseHelper(Activity);
                GetFavoritesFromSQL();
            }

            return view;
        }

        public class FavoritesSuccessListener : Java.Lang.Object, IOnSuccessListener
        {
            private Favorites_Fragment fragment;
            ProgressDialog progressDialog;

            public FavoritesSuccessListener(Favorites_Fragment fragment)
            {
                this.fragment = fragment;
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                var querySnapshot = (QuerySnapshot)result;
                fragment.hasFavorites = !querySnapshot.IsEmpty;

                if (fragment.hasFavorites)
                {
                    List<FavoriteItem> favoritesItems = new List<FavoriteItem>();

                    foreach (var documentSnapshot in querySnapshot.Documents)
                    {
                        FavoriteItem favoriteItem = new FavoriteItem
                        {
                            DocumentId = documentSnapshot.Id,
                            Title = documentSnapshot.GetString("Title"),
                            ImagePath = documentSnapshot.GetString("ImagePath"),
                            Description = documentSnapshot.GetString("Description")
                        };

                        favoritesItems.Add(favoriteItem);
                    }

                    fragment.ShowFavorites(favoritesItems);
                }
                else
                {
                    fragment.ShowNoFavorites();
                }
            }
        }


        public void ShowFavorites(List<FavoriteItem> favoritesItems)
        {
            favoritesAdapter = new FavoritesAdapter(favoritesItems);
            recyclerView.SetAdapter(favoritesAdapter);
        }

        public void ShowNoFavorites()
        {
            List<FavoriteItem> emptyList = new List<FavoriteItem>();
            favoritesAdapter = new FavoritesAdapter(emptyList);
            recyclerView.SetAdapter(favoritesAdapter);
        }


        private void GetFavoritesFromFirestore()
        {
            CollectionReference favoritesCollection = firestore.Collection("users")
                .Document(firebaseAuth.CurrentUser.Uid)
                .Collection("favorites");

            favoritesCollection.Get().AddOnSuccessListener(new FavoritesSuccessListener(this));
        }


        private void GetFavoritesFromSQL()
        {
            List<FavoriteItem> favoritesItems = databaseHelper.GetAll();
            recyclerView.SetAdapter(new FavoritesAdapter(favoritesItems));
        }

    }
}