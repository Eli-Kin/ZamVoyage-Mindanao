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
using Java.Text;
using ZamVoyage.Planner;
using Java.Util;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Java.Nio.FileNio;
using Google.Type;
using DateTime = System.DateTime;
using Android.Gms.Extensions;
using ZamVoyage.Log;
using Android.Support.V4.Widget;

namespace ZamVoyage
{
    public class Plan_Fragment : Fragment
    {
        EditText titleEditText, locationEditText, descriptionEditText, locationToEditText;
        Button saveButton, planList, selectDateButton;
        DatePickerDialog datePickerDialog;
        DateTime selectedDate;
        Button chooseTimeButton;
        TimePickerDialog timePickerDialog;
        SimpleDateFormat timeFormat;
        DateTime selectedTime;
        private PlanDatabaseHelper databaseHelper;
        private FirebaseAuth firebaseAuth;
        private FirebaseFirestore firestore;
        Spinner transportationSpinner, accomodationSpinner;
        ScrollView planScrollView;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the fragment's layout file
            View view = inflater.Inflate(Resource.Layout.fragment_plan, container, false);

            FirebaseApp.InitializeApp(Activity);
            firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);

            // Initialize database
            databaseHelper = new PlanDatabaseHelper(
                System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    "plans.db"));

            // Find UI elements
            titleEditText = view.FindViewById<EditText>(Resource.Id.titleEditText);
            locationEditText = view.FindViewById<EditText>(Resource.Id.locationEditText);
            locationToEditText = view.FindViewById<EditText>(Resource.Id.locationToEditText);
            descriptionEditText = view.FindViewById<EditText>(Resource.Id.descriptionEditText);
            selectDateButton = view.FindViewById<Button>(Resource.Id.selectDateButton);
            chooseTimeButton = view.FindViewById<Button>(Resource.Id.chooseTimeButton);
            saveButton = view.FindViewById<Button>(Resource.Id.saveButton);
            transportationSpinner = view.FindViewById<Spinner>(Resource.Id.transportationSpinner);
            accomodationSpinner = view.FindViewById<Spinner>(Resource.Id.accomodationSpinner);
            planScrollView = view.FindViewById<ScrollView>(Resource.Id.planScrollView);

            titleEditText.FocusChange +=
            new EventHandler<View.FocusChangeEventArgs>((sender, e) =>
            {
                bool hasFocus = e.HasFocus;
                if (hasFocus)
                { 
                    if (titleEditText.Text == "Your Title")
                    {
                        titleEditText.Text = string.Empty;
                    }
                }
                else if (titleEditText.Text == string.Empty)
                {
                    titleEditText.Text = "Your Title";
                }

            });

            descriptionEditText.SetOnTouchListener(new TouchListener());

            List<string> travelModes = new List<string>()
            {
                "Car",
                "Bus",
                "Taxi",
                "Uber",
                "Jeepney",
                "Tricycle",
                "Bicycle",
                "Walking",
                "Public Transit",
                "Motorcycle",
                "Train",
                "Boat",
                "Ferry",
                "Ship",
                "Yacht",
                "Airplane",
                "Helicopter",
                "Other"
            };

            List<string> accomodationModes = new List<string>()
            {
                "Hotel",
                "Hostel",
                "Airbnb",
                "Camping",
                "RV",
                "Cabin",
                "Resort",
                "Motel",
                "Guest House",
                "Villa",
                "Apartment",
                "House",
                "Bungalow",
                "Lodge",
                "Inn",
                "Farm Stay",
                "Chalet",
                "Cottage",
                "Tent",
                "Tree House",
                "Campground",
                "Capsule Hotel",
                "Yurt",
                "Aparthotel",
                "Pension",
                "Riad",
                "Homestay",
                "Other"
            };

            ArrayAdapter<string> travelAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleSpinnerItem, travelModes);
            travelAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            transportationSpinner.Adapter = travelAdapter;
            transportationSpinner.ItemSelected += Spinner_ItemSelected;

            ArrayAdapter<string> accomodationAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleSpinnerItem, accomodationModes);
            accomodationAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            accomodationSpinner.Adapter = accomodationAdapter;
            accomodationSpinner.ItemSelected += Spinner_ItemSelected;

            // Create a SimpleDateFormat with the desired time format
            timeFormat = new SimpleDateFormat("hh:mm a", Java.Util.Locale.Default);

            // Set the selected date to the current date
            selectedDate = DateTime.Now;

            // Handle save button click
            saveButton.Click += SaveButton_Click;

            selectDateButton.Click += SelectDateButton_Click;

            chooseTimeButton.Click += ChooseTimeButton_Click;

            return view;
        }

        public class TouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View view, MotionEvent motionEvent)
            {
                if (view.Id == Resource.Id.descriptionEditText)
                {
                    EditText editText = (EditText)view;
                    int lines = editText.LineCount;
                    int maxLines = 5; // Maximum number of lines before scrolling is enabled

                    if (lines > maxLines)
                    {
                        view.Parent.RequestDisallowInterceptTouchEvent(true);
                        switch (motionEvent.Action & MotionEventActions.Mask)
                        {
                            case MotionEventActions.Up:
                                view.Parent.RequestDisallowInterceptTouchEvent(false);
                                break;
                        }
                    }
                }
                return false;
            }
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string selectedValue = spinner.GetItemAtPosition(e.Position).ToString();

        }

        private void ChooseTimeButton_Click(object sender, System.EventArgs e)
        {
            // Get the current selected time or the current time if none selected
            int hour = selectedTime != null ? selectedTime.Hour : DateTime.Now.Hour;
            int minute = selectedTime != null ? selectedTime.Minute : DateTime.Now.Minute;

            // Create a TimePickerDialog and show it
            timePickerDialog = new TimePickerDialog(Activity, TimePickerCallback, hour, minute, false);
            timePickerDialog.Show();
        }

        private void TimePickerCallback(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            // Get the selected time from the TimePickerDialog
            selectedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, e.HourOfDay, e.Minute, 0);

            // Format the time as a string
            string formattedTime = FormatTime(selectedTime);

            // Set the selected time as the text of the button and the TextView
            chooseTimeButton.Text = formattedTime;
        }

        private string FormatTime(DateTime time)
        {
            // Create a Calendar instance and set the time
            Calendar calendar = Calendar.Instance;
            calendar.Set(CalendarField.HourOfDay, time.Hour);
            calendar.Set(CalendarField.Minute, time.Minute);

            // Format the time as "hh:mm a"
            return timeFormat.Format(calendar.Time);
        }

        private void SelectDateButton_Click(object sender, System.EventArgs e)
        {
            // Get the current date
            int year = selectedDate.Year;
            int month = selectedDate.Month - 1; // Calendar month is zero-based
            int day = selectedDate.Day;

            // Create a DatePickerDialog and show it
            datePickerDialog = new DatePickerDialog(Activity, DatePickerCallback, year, month, day);
            datePickerDialog.Show();
        }

        private void DatePickerCallback(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            // Get the selected date from the DatePickerDialog
            DateTime newDate = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day);

            // Update the selected date
            selectedDate = newDate;

            string formattedDate = FormatDate(selectedDate);

            // Set the selected date as the text of the button
            selectDateButton.Text = formattedDate;
        }

        private string FormatDate(DateTime date)
        {
            // Create a Calendar instance and set the year, month, and day
            Calendar calendar = Calendar.Instance;
            calendar.Set(CalendarField.Year, date.Year);
            calendar.Set(CalendarField.Month, date.Month - 1); // Calendar month is zero-based
            calendar.Set(CalendarField.DayOfMonth, date.Day);

            // Format the date as "Month day, year"
            SimpleDateFormat dateFormat = new SimpleDateFormat("MMMM d, yyyy", Java.Util.Locale.Default);
            return dateFormat.Format(calendar.Time);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Get the title from the input field
            string title = titleEditText.Text.Trim();

            // Validate the title field
            if (string.IsNullOrEmpty(title))
            {
                // Display an error message
                Toast.MakeText(Activity, "Please enter a title", ToastLength.Short).Show();
                return;
            }

            // Create a new plan object
            Plan plan = new Plan
            {
                Title = title,
                Location = locationEditText.Text,
                LocationTo = locationToEditText.Text,
                Description = descriptionEditText.Text,
                Date = selectDateButton.Text,
                Time = chooseTimeButton.Text,
                Transportation = transportationSpinner.SelectedItem.ToString(),
                Accomodation = accomodationSpinner.SelectedItem.ToString()
            };

            // Check if any fields are empty and set default values
            if (string.IsNullOrEmpty(plan.Location))
            {
                plan.Location = "No Location";
            }

            if (string.IsNullOrEmpty(plan.LocationTo))
            {
                plan.LocationTo = "No Location";
            }

            if (string.IsNullOrEmpty(plan.Description))
            {
                plan.Description = "No Description";
            }

            if (string.IsNullOrEmpty(plan.Date))
            {
                plan.Date = "Select Date";
            }

            if (string.IsNullOrEmpty(plan.Time))
            {
                plan.Time = "Select Time";
            }

            if (string.IsNullOrEmpty(plan.Transportation))
            {
                plan.Transportation = "Nothing Selected Yet";
            }

            if (string.IsNullOrEmpty(plan.Accomodation))
            {
                plan.Accomodation = "Nothing Selected Yet";
            }

            // Check if there is a current user
            FirebaseUser currentUser = firebaseAuth.CurrentUser;
            if (currentUser != null)
            {
                // If there is a current user, save the plan to Firebase Firestore
                SavePlanToFirestore(plan);
            }
            else
            {
                // If there is no current user, save the plan to the SQL database
                SavePlanToSQLDatabase(plan);
            }
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            builder.SetTitle("Plan Saved Successfully!");
            builder.SetMessage("Do you want to see your Plan List?");
            builder.SetPositiveButton("See Plan", (dialog, which) =>
            {
                // Launch PlanListActivity
                Intent getStarted = new Intent(Activity, typeof(PlanListActivity));
                StartActivity(getStarted);
            });
            builder.SetNegativeButton("Done", (dialog, which) =>
            {
                // User cancelled the action, do nothing
            });
            builder.Show();
        }

        private async void SavePlanToFirestore(Plan plan)
        {
            try
            {
                firestore = FirebaseFirestore.GetInstance(FirebaseApp.Instance);
                firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);

                // Get the user's UID
                string userId = firebaseAuth.CurrentUser.Uid;

                // Create a new document reference in the "users" collection with the user's UID
                DocumentReference userDocRef = firestore.Collection("users").Document(userId);

                // Create a new subcollection reference named "plans" inside the user document
                CollectionReference plansCollectionRef = userDocRef.Collection("plans");

                string uniqueId = Guid.NewGuid().ToString();

                // Create a new document reference with the unique ID as the document name
                DocumentReference newPlanDocRef = plansCollectionRef.Document(uniqueId);

                plan.DocumentId = uniqueId;

                HashMap planData = new HashMap();
                planData.Put("Id", plan.DocumentId);
                planData.Put("Title", plan.Title);
                planData.Put("Location", plan.Location);
                planData.Put("LocationTo", plan.LocationTo);
                planData.Put("Description", plan.Description);
                planData.Put("Date", plan.Date);
                planData.Put("Time", plan.Time);
                planData.Put("Transportation", plan.Transportation);
                planData.Put("Accomodation", plan.Accomodation);

                // Save the plan data to the new document
                newPlanDocRef.Set(planData);

                Toast.MakeText(Activity, "Plan saved successfully", ToastLength.Short).Show();
                ClearInputFields();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, "Failed to save plan: " + ex.Message, ToastLength.Short).Show();
            }
        }


        private void SavePlanToSQLDatabase(Plan plan)
        {
            // Save the plan to the SQL database
            int result = databaseHelper.SavePlan(plan);

            if (result > 0)
            {
                Toast.MakeText(Activity, "Plan saved successfully", ToastLength.Short).Show();
                ClearInputFields();
            }
            else
            {
                Toast.MakeText(Activity, "Failed to save plan", ToastLength.Short).Show();
            }
        }

        private void ClearInputFields()
        {
            titleEditText.Text = "Your Title";
            locationEditText.Text = string.Empty;
            locationToEditText.Text = string.Empty;
            descriptionEditText.Text = string.Empty;
        }

    }
}