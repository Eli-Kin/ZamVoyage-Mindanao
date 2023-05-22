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
                dbHelper.InsertItem(new Search_Item { Title = "Great Santa Cruz Island", ImagePath = "great_santa_cruz_island_1", Description = "Also known as “Pink Beach”, is a popular tourist destination located off the coast of Zamboanga City, Philippines. " });

                dbHelper.InsertItem(new Search_Item { Title = "Once Island", ImagePath = "once_island_1", Description = "Once Islas is a nature reserve that became an ecotourism spot back in 2018 in an effort to showcase the beauty of its islands and provide livelihood for its locals." });

                dbHelper.InsertItem(new Search_Item { Title = " Lantawan Grassland", ImagePath = "lantawan_grassland_1", Description = " It is a mountain located in Lantawan, Pasonanca. The mountain is covered in lush grasslands and is home to a wide variety of flora and fauna. " });

                dbHelper.InsertItem(new Search_Item { Title = "Merloquet Falls", ImagePath = "merloquet_falls_1", Description = "It is a stunning waterfall that is located in Barangay Sibulao, Zamboanga City, Philippines. The waterfall is situated in a lush jungle setting, surrounded by towering cliffs and dense vegetation. " });

                dbHelper.InsertItem(new Search_Item { Title = "The Fort Pilar Shrine and Museum", ImagePath = "the_fort_pilar_shrine_and_museum_1", Description = " It is a popular landmark in Zamboanga City that is steeped in history and culture." });

                dbHelper.InsertItem(new Search_Item { Title = "Metropolitan Cathedral of the Immaculate Conception", ImagePath = "the_metropolitan_cathedral_of_the_immaculate_conception_1", Description = " It is one of the oldest and most beautiful churches in Zamboanga City. It is the seat of the Archdiocese of Zamboanga and is located in the heart of the city." });

                dbHelper.InsertItem(new Search_Item { Title = "Zamboanga City Hall ", ImagePath = "the_zamboanga_city_hall_1", Description = " The center government of the city. It is located in the heart of the city and features a beautiful neoclassical architecture. " });

                dbHelper.InsertItem(new Search_Item { Title = " The Pasonanca Park", ImagePath = "the_pasonanca_park_1", Description = " Constructed in 1960, it was originally built to serve as the Youth Citizenship Training Center. " });

                dbHelper.InsertItem(new Search_Item { Title = "Paseo del Mar", ImagePath = "paseo_del_mar_1", Description = " It is a waterfront park that offers a variety of activities and attractions for visitors of all ages. " });

                dbHelper.InsertItem(new Search_Item { Title = "The Hermosa Festival", ImagePath = "the_hermosa_festival_1", Description = " The festival is a week-long celebration that features a variety of cultural performances, including traditional dances, music, and street parades. " });

                dbHelper.InsertItem(new Search_Item { Title = "Regatta de Zamboanga", ImagePath = "the_regatta_de_zamboanga_1", Description = " It is typically held in March and is one of the most colorful and exciting events in the city." });

                dbHelper.InsertItem(new Search_Item { Title = "Dia De Zamboanga", ImagePath = "dia_de_zamboanga_1", Description = " The festival is a celebration of the city's founding and its rich cultural heritage." });

                dbHelper.InsertItem(new Search_Item { Title = "Flores De Mayo", ImagePath = "flores_de_mayo_1", Description = " It is a monthlong revelry that is held during May annually, and it is celebrated in honor of the Virgin Mary." });

                dbHelper.InsertItem(new Search_Item { Title = "Zamboanga City Bird Festival", ImagePath = "zamboanga_city_bird_festival_1", Description = " Considered as the country’s biggest celebration of avifaunal diversity, Zamboanga City Bird Festival is celebrated by birdwatchers, conservationists, and tourists from local and foreign origins." });

                dbHelper.InsertItem(new Search_Item { Title = "Knickerbocker", ImagePath = "knickerbocker_1", Description = " This crazy colorful concoction is a mix of jellies and fresh fruits like watermelon, apple, pineapple, and mango, drizzled with condensed milk and topped with shaved ice and strawberry ice cream." });

                dbHelper.InsertItem(new Search_Item { Title = "Satti", ImagePath = "satti_1", Description = " This popular breakfast dish consists of grilled meat on a stick, usually chicken or beef, served with rice cakes and a spicy sauce made with chili, garlic, and vinegar. " });

                dbHelper.InsertItem(new Search_Item { Title = "Lokot-Lokot", ImagePath = "lokot_lokot_1", Description = "It is called by the locals as “jaa”, “lokot-lokot”, “Zambo Rolls, or “tagktak”, depending on where you find the delicacy. " });

                dbHelper.InsertItem(new Search_Item { Title = "Chikalang", ImagePath = "chikalang_1", Description = " It is a local delicacy that is made from glutinous rice shaped like a twisted donut or pinilipit." });

                dbHelper.InsertItem(new Search_Item { Title = "Curacha", ImagePath = "curacha_1", Description = " Curacha got its name because of its spiky, hairy appearance that kinda resembles you-know-what. But if you look closely, it kinda appears like the love child of a wide crab and a long lobster." });

                dbHelper.InsertItem(new Search_Item { Title = "Holy Smokes: Korean Grill and Resto", ImagePath = "holy_smokes_1", Description = "The restaurant is known for its delicious food, friendly staff, and cozy atmosphere. " });

                dbHelper.InsertItem(new Search_Item { Title = "Bay Tal Mal Restaurant", ImagePath = "bay_tal_mak_restaurant_1", Description = " The restaurant offers a variety of Filipino dishes such as adobo, sinigang, and kare-kare. It might be worth checking out if you're in the area and looking for some Filipino food." });

                dbHelper.InsertItem(new Search_Item { Title = "Alavar Seafood Restaurant", ImagePath = "alvar_seafood_restaurant_1", Description = " The restaurant is known for its delicious seafood dishes, particularly its famous curacha dish. The restaurant has a cozy and welcoming atmosphere, making it a great place to enjoy a meal with friends and family. " });

                dbHelper.InsertItem(new Search_Item { Title = "Kape Zambo Acoustic Bar and Restaurant", ImagePath = " kape_zambo_acoustic_bar_and_restaurant_1", Description = " The restaurant is known for its acoustic music and great food. They offer a variety of dishes, including Filipino and Asian cuisine." });

                dbHelper.InsertItem(new Search_Item { Title = "Brews Almighty", ImagePath = "brews_almighty_1", Description = " The cafe has indoor and outdoor seating, and is located near Paseo del Mar, making it a great place to relax and enjoy the view. The cafe has a laid-back and friendly atmosphere and is popular among locals and tourists alike. " });

                dbHelper.InsertItem(new Search_Item { Title = "Canelar Barter Trade Center", ImagePath = "canelar_barter_trade_center_1", Description = " It is a great place to find unique and handmade items, such as woven baskets, textiles, and wood carvings. The store is known for its affordable prices and friendly vendors, who are always happy to help customers find what they're looking for. " });

                dbHelper.InsertItem(new Search_Item { Title = "Yakan Weaving Center", ImagePath = "yakan_weaving_center_1", Description = " The center is dedicated to preserving and promoting the traditional art of Yakan weaving. The Yakan people are known for their intricate and colorful textiles, which are made using traditional weaving techniques." });

                dbHelper.InsertItem(new Search_Item { Title = "KCC Mall de Zamboanga", ImagePath = "kcc_mall_de_zamboanga_1", Description = " KCC Mall de Zamboanga is a large shopping mall located in Barangay Canelar, Zamboanga City. This mall is one of the biggest malls in Zamboanga City." });

                dbHelper.InsertItem(new Search_Item { Title = "Casa Canelar Pension", ImagePath = "casa_canelar_pension_1", Description = " It is a cozy hotel located in the heart of Zamboanga City. The hotel offers comfortable rooms that are equipped with air conditioning, a flat-screen TV, and a private bathroom. " });

                dbHelper.InsertItem(new Search_Item { Title = "Garden Orchid", ImagePath = "garden_orchid_hotel_1", Description = " Zamboanga City. The hotel offers a variety of rooms, ranging from deluxe rooms to suites, with rates starting at around PHP 4,000 per night." });

                dbHelper.InsertItem(new Search_Item { Title = "Grand Astoria Hotel", ImagePath = "grand_astoria_hotel_1", Description = " Zamboanga City. The hotel offers a variety of rooms, ranging from deluxe rooms to suites, with rates starting at around PHP 3,000 per night." });

                dbHelper.InsertItem(new Search_Item { Title = "Hamilton Business Inn", ImagePath = "hamilton_business_inn_1", Description = " The hotel offers comfortable rooms that are equipped with air conditioning, a flat-screen TV, and a private bathroom. Some rooms also come with a seating area and a balcony." });

                dbHelper.InsertItem(new Search_Item { Title = "Lantaka Hotel by the Sea", ImagePath = "lantaka_hotel_by_the_sea_1", Description = "The hotel is situated right by the sea, providing guests with stunning views of the water. Other amenities include an outdoor pool, a fitness center, and a spa. The hotel also offers meeting and event facilities." });

                dbHelper.InsertItem(new Search_Item { Title = "Zamboanga Town Home Bed and Breakfast", ImagePath = "zamboanga_town_home_bed_and_breakfast_1", Description = " The hotel has everything you need for a comfortable stay. Service-minded staff will welcome and guide you at the Zamboanga Town Home Bed and Breakfast." });

                dbHelper.InsertItem(new Search_Item { Title = "Zamboanga International Airport", ImagePath = "airplane", Description = " The airport serves the Zamboanga Peninsula region and is a hub for several airlines, including Cebu Pacific, Philippine Airlines, and AirAsia. " });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Zamboanga International Seaport",
                    ImagePath = "ship",
                    Description = "The seaport is one of the busiest in the region and serves as a gateway to nearby destinations such as Basilan, Jolo, and Tawi-Tawi."
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Bus Terminals",
                    ImagePath = "bus",
                    Description = " The terminal serves several bus companies that operate routes to other cities in Mindanao, including Dapitan, Dipolog, Pagadian, and Cotabato."
                });

                dbHelper.InsertItem(new Search_Item { Title = "Zamboanga Motor Rental", ImagePath = "motor_rental", Description = " A company in Zamboanga City offer a range of motorcycles and scooters for rent, including small and large motorcycles, automatic and manual scooters, and dirt bikes." });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Taxi",
                    ImagePath = "taxi",
                    Description = " Most taxis are equipped with air conditioning, and many have a small screen displaying the fare and distance traveled."
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Tricycle",
                    ImagePath = "tricycle",
                    Description = " Tricycles can be found throughout the city, and they are often used to access areas that are not accessible by larger vehicles."
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Additional Activities",
                    ImagePath = "explore_pink_beach_3",
                    Description = "Island Hoping at Once Islas (Eleven Islands)"
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "History",
                    ImagePath = "history_1",
                    Description = "The evolution of the name “Zamboanga” provides an interesting insight into its historical background. The early Malay settlers called the region “Jambangan”, which means Land of the Flowers."
                });



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