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
                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Great Santa Cruz Island",
                    ImagePath = "great_santa_cruz_island_1",
                    Description = "Also known as 'Pink Beach', is a popular tourist destination located off the coast of Zamboanga City, Philippines.",
                    Categories = new List<string> { "Beaches", "Islands", "Tourist Spots", "Attractions", "Great Santa Cruz Island" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Once Island",
                    ImagePath = "once_island_1",
                    Description = "Once Islas is a nature reserve that became an ecotourism spot back in 2018 in an effort to showcase the beauty of its islands and provide livelihood for its locals.",
                    Categories = new List<string> { "Beaches", "Islands", "Nature Reserves", "Ecotourism", "Attractions", "Once Island" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Lantawan Grassland",
                    ImagePath = "lantawan_grassland_1",
                    Description = "It is a mountain located in Lantawan, Pasonanca. The mountain is covered in lush grasslands and is home to a wide variety of flora and fauna.",
                    Categories = new List<string> { "Mountains", "Nature", "Attractions", "Lantawan Grassland" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Merloquet Falls",
                    ImagePath = "merloquet_falls_1",
                    Description = "It is a stunning waterfall that is located in Barangay Sibulao, Zamboanga City, Philippines. The waterfall is situated in a lush jungle setting, surrounded by towering cliffs and dense vegetation.",
                    Categories = new List<string> { "Waterfalls", "Nature", "Attractions", "Merloquet Falls" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "The Fort Pilar Shrine and Museum",
                    ImagePath = "the_fort_pilar_shrine_and_museum_1",
                    Description = "It is a popular landmark in Zamboanga City that is steeped in history and culture.",
                    Categories = new List<string> { "Historical Sites", "Museums", "Fort Pilar Shrine", "Culture", "Landmarks/Historical Sites", "Attractions", "The Fort Pilar Shrine and Museum" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Metropolitan Cathedral of the Immaculate Conception",
                    ImagePath = "the_metropolitan_cathedral_of_the_immaculate_conception_1",
                    Description = "It is one of the oldest and most beautiful churches in Zamboanga City. It is the seat of the Archdiocese of Zamboanga and is located in the heart of the city.",
                    Categories = new List<string> { "Churches", "Religious Sites", "Metropolitan Cathedral", "Landmarks/Historical Sites", "Attractions", "Metropolitan Cathedral of the Immaculate Conception" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Zamboanga City Hall",
                    ImagePath = "the_zamboanga_city_hall_1",
                    Description = "The center government of the city. It is located in the heart of the city and features a beautiful neoclassical architecture.",
                    Categories = new List<string> { "Government Buildings", "City Hall", "Neoclassical Architecture", "Landmarks/Historical Sites", "Attractions", "Zamboanga City Hall" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "The Pasonanca Park",
                    ImagePath = "the_pasonanca_park_1",
                    Description = "Constructed in 1960, it was originally built to serve as the Youth Citizenship Training Center.",
                    Categories = new List<string> { "Parks", "Recreation", "Attractions", "The Pasonanca Park" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Paseo del Mar",
                    ImagePath = "paseo_del_mar_1",
                    Description = "It is a waterfront park that offers a variety of activities and attractions for visitors of all ages.",
                    Categories = new List<string> { "Waterfront Parks", "Recreation", "Attractions", "Paseo del Mar" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "The Hermosa Festival",
                    ImagePath = "the_hermosa_festival_1",
                    Description = "The festival is a week-long celebration that features a variety of cultural performances, including traditional dances, music, and street parades.",
                    Categories = new List<string> { "Festivals", "Cultural Celebrations", "Activities", "Hermosa Festival" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Regatta de Zamboanga",
                    ImagePath = "the_regatta_de_zamboanga_1",
                    Description = "It is typically held in March and is one of the most colorful and exciting events in the city.",
                    Categories = new List<string> { "Festivals", "Events", "Regatta", "Water Sports", "Activities", "Regatta de Zamboanga" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Dia De Zamboanga",
                    ImagePath = "dia_de_zamboanga_1",
                    Description = "The festival is a celebration of the city's founding and its rich cultural heritage.",
                    Categories = new List<string> { "Festivals", "Cultural Celebrations", "Activities", "Dia De Zamboanga" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Flores De Mayo",
                    ImagePath = "flores_de_mayo_1",
                    Description = "It is a monthlong revelry that is held during May annually, and it is celebrated in honor of the Virgin Mary.",
                    Categories = new List<string> { "Festivals", "Cultural Celebrations", "Activities", "Flores De Mayo" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Zamboanga City Bird Festival",
                    ImagePath = "zamboanga_city_bird_festival_1",
                    Description = "Considered as the country’s biggest celebration of avifaunal diversity, Zamboanga City Bird Festival is celebrated by birdwatchers, conservationists, and tourists from local and foreign origins.",
                    Categories = new List<string> { "Festivals", "Birdwatching", "Conservation", "Activities", "Zamboanga City Bird Festival" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Knickerbocker",
                    ImagePath = "knickerbocker_1",
                    Description = "This crazy colorful concoction is a mix of jellies and fresh fruits like watermelon, apple, pineapple, and mango, drizzled with condensed milk and topped with shaved ice and strawberry ice cream.",
                    Categories = new List<string> { "Food", "Desserts", "Amenities", "Knickerbocker", "Delicacies" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Satti",
                    ImagePath = "satti_1",
                    Description = "This popular breakfast dish consists of grilled meat on a stick, usually chicken or beef, served with rice cakes and a spicy sauce made with chili, garlic, and vinegar.",
                    Categories = new List<string> { "Food", "Grilled Meat", "Amenities", "Satti", "Delicacies" }
                });


                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Lokot-Lokot",
                    ImagePath = "lokot_lokot_1",
                    Description = "It is called by the locals as 'jaa', 'lokot-lokot', 'Zambo Rolls', or 'tagktak', depending on where you find the delicacy.",
                    Categories = new List<string> { "Food", "Delicacies", "Amenities", "Lokot-Lokot" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Chikalang",
                    ImagePath = "chikalang_1",
                    Description = "It is a local delicacy that is made from glutinous rice shaped like a twisted donut or pinilipit.",
                    Categories = new List<string> { "Food", "Delicacies", "Amenities", "Chikalang" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Curacha",
                    ImagePath = "curacha_1",
                    Description = "Curacha got its name because of its spiky, hairy appearance that kinda resembles you-know-what. But if you look closely, it kinda appears like the love child of a wide crab and a long lobster.",
                    Categories = new List<string> { "Food", "Seafood", "Amenities", "Curacha" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Holy Smokes: Korean Grill and Resto",
                    ImagePath = "holy_smokes_1",
                    Description = "The restaurant is known for its delicious food, friendly staff, and cozy atmosphere.",
                    Categories = new List<string> { "Restaurants", "Amenities", "Korean Cuisine", "Holy Smokes" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Bay Tal Mal Restaurant",
                    ImagePath = "bay_tal_mak_restaurant_1",
                    Description = "The restaurant offers a variety of Filipino dishes such as adobo, sinigang, and kare-kare. It might be worth checking out if you're in the area and looking for some Filipino food.",
                    Categories = new List<string> { "Restaurants", "Filipino Cuisine", "Amenities", "Bay Tal Mal Restaurant" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Alavar Seafood Restaurant",
                    ImagePath = "alvar_seafood_restaurant_1",
                    Description = "The restaurant is known for its delicious seafood dishes, particularly its famous curacha dish. The restaurant has a cozy and welcoming atmosphere, making it a great place to enjoy a meal with friends and family.",
                    Categories = new List<string> { "Restaurants", "Seafood", "Amenities", "Alavar Seafood Restaurant" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Kape Zambo Acoustic Bar and Restaurant",
                    ImagePath = "kape_zambo_acoustic_bar_and_restaurant_1",
                    Description = "The restaurant is known for its acoustic music and great food. They offer a variety of dishes, including Filipino and Asian cuisine.",
                    Categories = new List<string> { "Restaurants", "Music", "Amenities", "Kape Zambo Acoustic Bar and Restaurant" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Brews Almighty",
                    ImagePath = "brews_almighty_1",
                    Description = "The cafe has indoor and outdoor seating, and is located near Paseo del Mar, making it a great place to relax and enjoy the view. The cafe has a laid-back and friendly atmosphere and is popular among locals and tourists alike.",
                    Categories = new List<string> { "Restaurants", "Cafes", "Relaxation", "Amenities", "Brews Almighty" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Canelar Barter Trade Center",
                    ImagePath = "canelar_barter_trade_center_1",
                    Description = "It is a great place to find unique and handmade items, such as woven baskets, textiles, and wood carvings. The store is known for its affordable prices and friendly vendors, who are always happy to help customers find what they're looking for.",
                    Categories = new List<string> { "Shopping", "Handicrafts", "Souvenir Shops", "Amenities", "Canelar Barter Trade Center" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Yakan Weaving Center",
                    ImagePath = "yakan_weaving_center_1",
                    Description = "The center is dedicated to preserving and promoting the traditional art of Yakan weaving. The Yakan people are known for their intricate and colorful textiles, which are made using traditional weaving techniques.",
                    Categories = new List<string> { "Culture", "Art", "Souvenir Shops", "Amenities", "Yakan Weaving Center" }
                });


                dbHelper.InsertItem(new Search_Item
                {
                    Title = "KCC Mall de Zamboanga",
                    ImagePath = "kcc_mall_de_zamboanga_1",
                    Description = "KCC Mall de Zamboanga is a large shopping mall located in Barangay Canelar, Zamboanga City. This mall is one of the biggest malls in Zamboanga City.",
                    Categories = new List<string> { "Shopping", "Malls", "KCC Mall de Zamboanga" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Casa Canelar Pension",
                    ImagePath = "casa_canelar_pension_1",
                    Description = "It is a cozy hotel located in the heart of Zamboanga City. The hotel offers comfortable rooms that are equipped with air conditioning, a flat-screen TV, and a private bathroom.",
                    Categories = new List<string> { "Hotels", "Accommodations", "Casa Canelar Pension" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Garden Orchid",
                    ImagePath = "garden_orchid_hotel_1",
                    Description = "The hotel offers a variety of rooms, ranging from deluxe rooms to suites, with rates starting at around PHP 4,000 per night.",
                    Categories = new List<string> { "Hotels", "Accommodations", "Garden Orchid" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Grand Astoria Hotel",
                    ImagePath = "grand_astoria_hotel_1",
                    Description = "The hotel offers a variety of rooms, ranging from deluxe rooms to suites, with rates starting at around PHP 3,000 per night.",
                    Categories = new List<string> { "Hotels", "Accommodations", "Grand Astoria Hotel" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Hamilton Business Inn",
                    ImagePath = "hamilton_business_inn_1",
                    Description = "The hotel offers comfortable rooms that are equipped with air conditioning, a flat-screen TV, and a private bathroom. Some rooms also come with a seating area and a balcony.",
                    Categories = new List<string> { "Hotels", "Accommodations", "Hamilton Business Inn" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Lantaka Hotel by the Sea",
                    ImagePath = "lantaka_hotel_by_the_sea_1",
                    Description = "The hotel is situated right by the sea, providing guests with stunning views of the water. Other amenities include an outdoor pool, a fitness center, and a spa. The hotel also offers meeting and event facilities.",
                    Categories = new List<string> { "Hotels", "Accommodations", "Lantaka Hotel by the Sea" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Zamboanga Town Home Bed and Breakfast",
                    ImagePath = "zamboanga_town_home_bed_and_breakfast_1",
                    Description = "The hotel has everything you need for a comfortable stay. Service-minded staff will welcome and guide you at the Zamboanga Town Home Bed and Breakfast.",
                    Categories = new List<string> { "Hotels", "Accommodations", "Zamboanga Town Home Bed and Breakfast" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Zamboanga International Airport",
                    ImagePath = "airplane",
                    Description = "The airport serves the Zamboanga Peninsula region and is a hub for several airlines, including Cebu Pacific, Philippine Airlines, and AirAsia.",
                    Categories = new List<string> { "Transportation", "Airports", "Accessibility", "Zamboanga International Airport" }
                });


                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Zamboanga International Seaport",
                    ImagePath = "ship",
                    Description = "The seaport is one of the busiest in the region and serves as a gateway to nearby destinations such as Basilan, Jolo, and Tawi-Tawi.",
                    Categories = new List<string> { "Transportation", "Seaports", "Accessibility", "Zamboanga International Seaport" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Bus Terminals",
                    ImagePath = "bus",
                    Description = "The terminal serves several bus companies that operate routes to other cities in Mindanao, including Dapitan, Dipolog, Pagadian, and Cotabato.",
                    Categories = new List<string> { "Transportation", "Accessibility", "Bus Terminals" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Zamboanga Motor Rental",
                    ImagePath = "motor_rental",
                    Description = "A company in Zamboanga City offers a range of motorcycles and scooters for rent, including small and large motorcycles, automatic and manual scooters, and dirt bikes.",
                    Categories = new List<string> { "Transportation", "Motorcycle Rental", "Accessibility", "Zamboanga Motor Rental" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Taxi",
                    ImagePath = "taxi",
                    Description = "Most taxis are equipped with air conditioning, and many have a small screen displaying the fare and distance traveled.",
                    Categories = new List<string> { "Transportation", "Accessibility", "Taxis" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Tricycle",
                    ImagePath = "tricycle",
                    Description = "Tricycles can be found throughout the city, and they are often used to access areas that are not accessible by larger vehicles.",
                    Categories = new List<string> { "Transportation", "Accessibility", "Tricycles" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "Additional Activities",
                    ImagePath = "explore_pink_beach_3",
                    Description = "Island Hopping at Once Islas (Eleven Islands)",
                    Categories = new List<string> { "Activities", "Additional", "Explore" }
                });

                dbHelper.InsertItem(new Search_Item
                {
                    Title = "History",
                    ImagePath = "history_1",
                    Description = "The evolution of the name 'Zamboanga' provides an interesting insight into its historical background. The early Malay settlers called the region 'Jambangan', which means Land of the Flowers.",
                    Categories = new List<string> { "History", "Zamboanga History" }
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