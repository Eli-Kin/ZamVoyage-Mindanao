using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BruTile.Predefined;
using Mapsui;
using Mapsui.UI;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Android;
using Mapsui.Utilities;
using Mapsui.Widgets;
using System.Collections.Generic;
using Fragment = AndroidX.Fragment.App.Fragment;
using System.Globalization;
using static Android.Views.View;
using Xamarin.Essentials;
using Map = Mapsui.Map;
using System;
using Android.Gms.Tasks;
using Android.Locations;
using Location = Xamarin.Essentials.Location;

namespace ZamVoyage.Fragments
{
    public class Explore_Fragment : Fragment
    {
        private bool isLongTouch = false;
        private const int LongTouchDuration = 1000;
        private Handler handler;
        private bool isLocationTracking = false;
        private CancellationTokenSource cancellationTokenSource;
        private MapControl mapControl;
        private LocationListener locationListener;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the fragment's layout file
            View view = inflater.Inflate(Resource.Layout.fragment_explore, container, false);

            // Initialize the handler
            handler = new Handler();

            mapControl = view.FindViewById<MapControl>(Resource.Id.mapcontrol);
            var homeButton = view.FindViewById<ImageButton>(Resource.Id.homeButton);
            var locationButton = view.FindViewById<ImageButton>(Resource.Id.userLocationButton);

            var map = new Map();
            var tileLayer = new TileLayer(KnownTileSources.Create(KnownTileSource.OpenStreetMap));
            map.Layers.Add(tileLayer);
            var zamboangaCity = new Point(122.0696, 6.9214);
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(zamboangaCity.X, zamboangaCity.Y);
            map.Home = n => n.NavigateTo(sphericalMercatorCoordinate, map.Resolutions[10]);

            // Set the limiter to keep the map within the specified boundaries
            map.Limiter = new ViewportLimiterKeepWithin
            {
                PanLimits = new BoundingBox(
                    SphericalMercator.FromLonLat(114.5, 3.0), // Southwest corner coordinates (longitude, latitude)
                    SphericalMercator.FromLonLat(127.0, 23.5) // Northeast corner coordinates (longitude, latitude)
                )
            };

            map.RotationLock = true;

            // Set the home button click event
            homeButton.Click += (sender, e) =>
            {
                // Navigate back to the home position
                mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(122.0696, 6.9214), map.Resolutions[10]);
            };

            // Set the location button click event
            locationButton.Click += LocationButton_Click;

            mapControl.Map = map;

            return view;
        }

        private async void LocationButton_Click(object sender, EventArgs e)
        {
            if (!isLocationTracking)
            {
                // Check if location permission is granted
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    // Request location permission
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        // Permission denied, show error message or handle permission denial
                        ShowErrorMessage("Location permission denied.");
                        return;
                    }
                }

                // Check if location is enabled on the device
                var locationManager = (LocationManager)Activity.GetSystemService(Context.LocationService);
                var isLocationEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
                if (!isLocationEnabled)
                {
                    // Location is disabled, prompt the user to enable it
                    AlertDialog.Builder dialog = new AlertDialog.Builder(Activity);
                    dialog.SetTitle("Enable Location");
                    dialog.SetMessage("Location is required to continue. Do you want to enable it?");
                    dialog.SetPositiveButton("Yes", (d, i) =>
                    {
                        // Open device settings to enable location
                        Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                        StartActivity(intent);
                    });
                    dialog.SetNegativeButton("No", (d, i) => { });
                    dialog.Show();
                    return;
                }

                // Start tracking the user's location
                isLocationTracking = true;
                cancellationTokenSource = new CancellationTokenSource();

                // Create a new instance of LocationListener
                locationListener = new LocationListener(mapControl);

                try
                {
                    // Subscribe to the location changed event
                    locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, locationListener);
                }
                catch (Exception ex)
                {
                    // Show error message or handle location service failure
                    ShowErrorMessage("Failed to start location tracking: " + ex.Message);
                    isLocationTracking = false;
                }
            }
            else
            {
                // Stop tracking the user's location
                isLocationTracking = false;

                // Stop the location tracking
                cancellationTokenSource.Cancel();

                try
                {
                    // Unsubscribe from the location changed event
                    LocationManager locationManager = (LocationManager)Activity.GetSystemService(Context.LocationService);
                    locationManager.RemoveUpdates(locationListener);
                }
                catch (Exception ex)
                {
                    // Show error message or handle location service failure
                    ShowErrorMessage("Failed to stop location tracking: " + ex.Message);
                }
            }
        }

        public class LocationListener : Java.Lang.Object, ILocationListener
        {
            private MapControl mapControl;

            public LocationListener(MapControl mapControl)
            {
                this.mapControl = mapControl;
            }

            public void OnLocationChanged(Android.Locations.Location location)
            {
                if (location != null)
                {
                    // Update the map with the user's location
                    var userLocation = new Point(location.Longitude, location.Latitude);
                    var mapLocation = SphericalMercator.FromLonLat(userLocation.X, userLocation.Y);

                    // Create a pin and add it to the map with the specified color
                    var pin = new Mapsui.Geometries.Point(mapLocation.X, mapLocation.Y);
                    var pinLayer = new MemoryLayer { DataSource = new MemoryProvider(pin) };
                    pinLayer.Style = new SymbolStyle
                    {
                        Fill = new Brush(new Color(0x0D, 0x8B, 0xFF)), // Specify the pin color as #0D8BFF
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = 0.5,
                        Outline = null
                    };
                    mapControl.Map.Layers.Add(pinLayer);

                    mapControl.Navigator.NavigateTo(mapLocation, mapControl.Map.Resolutions[16]);
                }
            }

            public void OnProviderDisabled(string provider)
            {
                // Provider disabled logic
            }

            public void OnProviderEnabled(string provider)
            {
                // Provider enabled logic
            }

            public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
            {
                // Status changed logic
            }
        }

        private void ShowErrorMessage(string message)
        {
            // Display the error message to the user using your desired method (e.g., toast, dialog)
            Toast.MakeText(Activity, message, ToastLength.Short).Show();
        }
    }
}