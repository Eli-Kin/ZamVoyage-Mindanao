using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using SearchView = AndroidX.AppCompat.Widget.SearchView;
using AndroidX.RecyclerView.Widget;
using static Android.Content.ClipData;

namespace ZamVoyage.Search_Features
{
    public class Search_Fragment : Fragment, SearchView.IOnQueryTextListener
    {
        private SearchView searchView;
        private RecyclerView recyclerView;
        private SearchAdapter itemAdapter;
        private List<Search_Item> items;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the fragment's layout file
            View view = inflater.Inflate(Resource.Layout.fragment_search, container, false);

            var prefs = Application.Context.GetSharedPreferences("MyPrefs", FileCreationMode.Private);
            if (!prefs.GetBoolean("DataInserted", false))
            {
                // Data has not been inserted, so insert it
                SearchDatabaseHelper dbHelper = new SearchDatabaseHelper(Context);
                dbHelper.InsertItem(new Search_Item { Title = "Squash", ImagePath = "image1", Description = "..." });
                dbHelper.InsertItem(new Search_Item { Title = "Carrot", ImagePath = "image2", Description = "..." });
                dbHelper.InsertItem(new Search_Item { Title = "Potato", ImagePath = "image3", Description = "..." });
                dbHelper.InsertItem(new Search_Item { Title = "Cabbage", ImagePath = "image4", Description = "..." });
                dbHelper.InsertItem(new Search_Item { Title = "Eggplant", ImagePath = "image5", Description = "..." });
                dbHelper.InsertItem(new Search_Item { Title = "Apple", ImagePath = "image1", Description = "..." });
                dbHelper.InsertItem(new Search_Item { Title = "Banana", ImagePath = "image2", Description = "..." });
                dbHelper.InsertItem(new Search_Item { Title = "Cherry", ImagePath = "image3", Description = "..." });
                dbHelper.InsertItem(new Search_Item { Title = "Durian", ImagePath = "image4", Description = "..." });
                dbHelper.InsertItem(new Search_Item { Title = "Orange", ImagePath = "image5", Description = "..." });

                // Update the flag to indicate that data has been inserted
                var editor = prefs.Edit();
                editor.PutBoolean("DataInserted", true);
                editor.Apply();
            }

            searchView = view.FindViewById<SearchView>(Resource.Id.searchView);
            searchView.SetOnQueryTextListener(this);
            searchView.SetIconifiedByDefault(false);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            items = new List<Search_Item>();
            itemAdapter = new SearchAdapter(items);
            recyclerView.SetAdapter(itemAdapter);

            return view;
        }

        public bool OnQueryTextChange(string newText)
        {
            if (newText.Length >= 1)
            {
                SearchDatabaseHelper dbHelper = new SearchDatabaseHelper(Context);
                items = dbHelper.SearchItems(newText);
                dbHelper.Close();
                itemAdapter.UpdateData(items);
            }
            else
            {
                // If the search text is empty, clear the list and update the adapter
                items.Clear();
                itemAdapter.UpdateData(items);
            }
            return false;
        }

        public bool OnQueryTextSubmit(string query)
        {
            return false;
        }
    }
}