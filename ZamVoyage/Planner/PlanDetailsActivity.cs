using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Java.Text;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Util.EventLogTags;
using AlertDialog = Android.App.AlertDialog;

namespace ZamVoyage.Planner
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class PlanDetailsActivity : AppCompatActivity, IOnSuccessListener
    {
        TextView titleTextView, locationTextView, locationToTextView, descriptionTextView, dateTextView, timeTextView, transportationTextView, accomodationTextView;
        EditText titleEditText, locationEditText, locationToEditText, descriptionEditText;
        Button editSaveButton, selectDateButton, chooseTimeButton, cancelButton;
        Spinner transportationSpinner, accomodationSpinner;
        RelativeLayout transportationLayout, accomodationLayout;
        DatePickerDialog datePickerDialog;
        DateTime selectedDate;
        TimePickerDialog timePickerDialog;
        SimpleDateFormat timeFormat;
        DateTime selectedTime;
        private PlanDatabaseHelper databaseHelper;
        private FirebaseAuth firebaseAuth;
        private FirebaseFirestore firestore;
        int planId;
        string planDocId;
        Plan plan;
        bool isEditMode = false;
        bool hasDataChanged = false;
        string originalTitleText, originalLocationText, originalLocationToText, originalDescriptionText, originalDateText, originalTimeText, originalTransportationText, originalAccomodationText;
        string selectedTransportation, selectedAccomodation;
        ArrayAdapter<string> accomodationAdapter, travelAdapter;
        ProgressDialog progressDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_plan_details);

            // Initialize Firebase
            FirebaseApp.InitializeApp(this);
            firestore = FirebaseFirestore.GetInstance(FirebaseApp.Instance);
            firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);

            FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder()
            .SetPersistenceEnabled(true)
            .Build();

            // Initialize database
            databaseHelper = new PlanDatabaseHelper(
                System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    "plans.db"));

            // Find UI elements
            titleTextView = FindViewById<TextView>(Resource.Id.titleTextView);
            locationTextView = FindViewById<TextView>(Resource.Id.locationTextView);
            locationToTextView = FindViewById<TextView>(Resource.Id.locationToTextView);
            descriptionTextView = FindViewById<TextView>(Resource.Id.descriptionTextView);
            transportationTextView = FindViewById<TextView>(Resource.Id.transportationTextView);
            accomodationTextView = FindViewById<TextView>(Resource.Id.accomodationTextView);
            titleEditText = FindViewById<EditText>(Resource.Id.titleEditText);
            locationEditText = FindViewById<EditText>(Resource.Id.locationEditText);
            locationToEditText = FindViewById<EditText>(Resource.Id.locationToEditText);
            descriptionEditText = FindViewById<EditText>(Resource.Id.descriptionEditText);
            selectDateButton = FindViewById<Button>(Resource.Id.selectDateButton);
            chooseTimeButton = FindViewById<Button>(Resource.Id.chooseTimeButton);
            transportationSpinner = FindViewById<Spinner>(Resource.Id.transportationSpinner);
            accomodationSpinner = FindViewById<Spinner>(Resource.Id.accomodationSpinner);
            dateTextView = FindViewById<TextView>(Resource.Id.dateTextView);
            timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            transportationLayout = FindViewById<RelativeLayout>(Resource.Id.transportationLayout);
            accomodationLayout = FindViewById<RelativeLayout>(Resource.Id.accomodationLayout);
            editSaveButton = FindViewById<Button>(Resource.Id.editSaveButton);
            cancelButton = FindViewById<Button>(Resource.Id.cancelButton);

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

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
                "Boat",
                "Yurt",
                "Aparthotel",
                "Pension",
                "Riad",
                "Homestay",
                "Other"
            };

            travelAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, travelModes);
            travelAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            transportationSpinner.Adapter = travelAdapter;
            //selectedTransportation = transportationSpinner.SelectedItem.ToString();
            transportationSpinner.ItemSelected += Spinner_ItemSelected;

            accomodationAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, accomodationModes);
            accomodationAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            accomodationSpinner.Adapter = accomodationAdapter;
            //selectedAccomodation = accomodationSpinner.SelectedItem.ToString();
            accomodationSpinner.ItemSelected += Spinner_ItemSelected;

            // Add TextChanged event handlers to the EditText fields
            titleEditText.TextChanged += DataChangedEventHandler;
            locationEditText.TextChanged += DataChangedEventHandler;
            locationToEditText.TextChanged += DataChangedEventHandler;
            descriptionEditText.TextChanged += DataChangedEventHandler;
            selectDateButton.TextChanged += DataChangedEventHandler;
            chooseTimeButton.TextChanged += DataChangedEventHandler;

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

            // Create a SimpleDateFormat with the desired time format
            timeFormat = new SimpleDateFormat("hh:mm a", Java.Util.Locale.Default);

            // Set the selected date to the user chose date
            selectedDate = DateTime.Now;

            selectedTime = DateTime.Now;

            // Retrieve the plan ID from the intent extras
            planId = Intent.GetIntExtra("PlanId", -1);
            planDocId = Intent.GetStringExtra("PlanDocId");

            // Load and display the plan details
            if (planId != -1)
            {
                if (firebaseAuth.CurrentUser != null)
                {
                    try
                    {
                        // Show the progress dialog
                        ShowProgressDialog("Loading...");

                        // Get the user's UID
                        string userId = firebaseAuth.CurrentUser.Uid;

                        // Create a reference to the plan document in Firestore
                        DocumentReference planDocRef = firestore.Collection("users").Document(userId)
                            .Collection("plans").Document(planDocId);

                        System.Diagnostics.Debug.WriteLine("RefId: " + planDocRef);

                        planDocRef.Get().AddOnSuccessListener(this);

                        System.Diagnostics.Debug.WriteLine(titleTextView.Text);
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception here
                        System.Diagnostics.Debug.WriteLine("Failed to show plan: " + ex.Message);
                        Toast.MakeText(this, "Failed to show plan: " + ex.Message, ToastLength.Short).Show();
                    }
                }
                else
                {
                    // User is not authenticated with Firebase, retrieve plan from SQL Database
                    plan = databaseHelper.GetPlanById(planId);
                    if (plan != null)
                    {
                        titleTextView.Text = plan.Title;
                        locationTextView.Text = plan.Location;
                        locationToTextView.Text = plan.LocationTo;
                        descriptionTextView.Text = plan.Description;
                        dateTextView.Text = plan.Date;
                        timeTextView.Text = plan.Time;
                        transportationTextView.Text = plan.Transportation;
                        accomodationTextView.Text = plan.Accomodation;
                    }
                }
            }

            // Set up click event for the Edit/Save button
            editSaveButton.Click += EditSaveButton_Click;
            cancelButton.Click += CancelButton_Click;

            // Set initial button text
            SetButtonState();

            // Set the editable state of the fields
            SetFieldEditableState(isEditMode);

            selectDateButton.Click += SelectDateButton_Click;

            chooseTimeButton.Click += ChooseTimeButton_Click;

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

        private void ShowProgressDialog(string message)
        {
            progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage(message);
            progressDialog.SetCancelable(false);
            progressDialog.Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                // Handle the back button press here
                OnBackPressed();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string selectedValue = spinner.GetItemAtPosition(e.Position).ToString();

            // Assign the selected value to separate variables
            if (spinner == accomodationSpinner)
            {
                selectedAccomodation = selectedValue;
            }
            else if (spinner == transportationSpinner)
            {
                selectedTransportation = selectedValue;
            }

            SetButtonState();
        }


        private void ChooseTimeButton_Click(object sender, System.EventArgs e)
        {
            // Get the current selected time or the current time if none selected
            int hour = selectedTime != null ? selectedTime.Hour : DateTime.Now.Hour;
            int minute = selectedTime != null ? selectedTime.Minute : DateTime.Now.Minute;

            // Create a TimePickerDialog and show it
            timePickerDialog = new TimePickerDialog(this, TimePickerCallback, hour, minute, false);
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
            datePickerDialog = new DatePickerDialog(this, DatePickerCallback, year, month, day);
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

        private void EditSaveButton_Click(object sender, System.EventArgs e)
        {
            isEditMode = !isEditMode;

            // Update the button text based on the edit mode
            SetButtonState();

            // Set the editable state of the fields
            SetFieldEditableState(isEditMode);

            if (isEditMode)
            {
                // Enter edit mode
                titleEditText.Text = titleTextView.Text;
                locationEditText.Text = locationTextView.Text;
                locationToEditText.Text = locationToTextView.Text;
                descriptionEditText.Text = descriptionTextView.Text;
                selectDateButton.Text = dateTextView.Text;
                chooseTimeButton.Text = timeTextView.Text;

                // Set the selection of the accomodationSpinner based on the text
                string accomodationText = accomodationTextView.Text;
                int selectedIndex = -1;
                for (int i = 0; i < accomodationAdapter.Count; i++)
                {
                    if (accomodationAdapter.GetItem(i) == accomodationText)
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                if (selectedIndex >= 0)
                {
                    accomodationSpinner.SetSelection(selectedIndex);
                }

                // Set the selection of the transportationSpinner based on the text
                string transportationText = transportationTextView.Text;
                int selectedIndex1 = -1;
                for (int i = 0; i < travelAdapter.Count; i++)
                {
                    if (travelAdapter.GetItem(i) == transportationText)
                    {
                        selectedIndex1 = i;
                        break;
                    }
                }

                if (selectedIndex1 >= 0)
                {
                    transportationSpinner.SetSelection(selectedIndex1);
                }

                // Save the original text values
                originalTitleText = titleTextView.Text;
                originalLocationText = locationTextView.Text;
                originalLocationToText = locationToTextView.Text;
                originalDescriptionText = descriptionTextView.Text;
                originalDateText = dateTextView.Text;
                originalTimeText = timeTextView.Text;
                originalAccomodationText = accomodationTextView.Text;
                originalTransportationText = transportationTextView.Text;

                // Hide text views and show edit texts
                titleTextView.Visibility = Android.Views.ViewStates.Gone;
                locationTextView.Visibility = Android.Views.ViewStates.Gone;
                locationToTextView.Visibility = Android.Views.ViewStates.Gone;
                descriptionTextView.Visibility = Android.Views.ViewStates.Gone;
                dateTextView.Visibility = Android.Views.ViewStates.Gone;
                timeTextView.Visibility = Android.Views.ViewStates.Gone;
                accomodationTextView.Visibility = Android.Views.ViewStates.Gone;
                transportationTextView.Visibility = Android.Views.ViewStates.Gone;

                titleEditText.Visibility = Android.Views.ViewStates.Visible;
                locationEditText.Visibility = Android.Views.ViewStates.Visible;
                locationToEditText.Visibility = Android.Views.ViewStates.Visible;
                descriptionEditText.Visibility = Android.Views.ViewStates.Visible;
                selectDateButton.Visibility = Android.Views.ViewStates.Visible;
                chooseTimeButton.Visibility = Android.Views.ViewStates.Visible;
                accomodationLayout.Visibility = Android.Views.ViewStates.Visible;
                transportationLayout.Visibility = Android.Views.ViewStates.Visible;

                // Disable the save button if no data has changed
                editSaveButton.Enabled = false;
                editSaveButton.SetTextColor(Color.Gray);
                editSaveButton.SetBackgroundResource(Resource.Drawable.inactive_button);

            }
            else
            {
                // Exit edit mode and save changes


                // Show text views and hide edit texts
                titleTextView.Visibility = Android.Views.ViewStates.Visible;
                locationTextView.Visibility = Android.Views.ViewStates.Visible;
                locationToTextView.Visibility = Android.Views.ViewStates.Visible;
                descriptionTextView.Visibility = Android.Views.ViewStates.Visible;
                dateTextView.Visibility = Android.Views.ViewStates.Visible;
                timeTextView.Visibility = Android.Views.ViewStates.Visible;
                accomodationTextView.Visibility = Android.Views.ViewStates.Visible;
                transportationTextView.Visibility = Android.Views.ViewStates.Visible;

                titleEditText.Visibility = Android.Views.ViewStates.Gone;
                locationEditText.Visibility = Android.Views.ViewStates.Gone;
                locationToEditText.Visibility = Android.Views.ViewStates.Gone;
                descriptionEditText.Visibility = Android.Views.ViewStates.Gone;
                selectDateButton.Visibility = Android.Views.ViewStates.Gone;
                chooseTimeButton.Visibility = Android.Views.ViewStates.Gone;
                accomodationLayout.Visibility = Android.Views.ViewStates.Gone;
                transportationLayout.Visibility = Android.Views.ViewStates.Gone;

                if (firebaseAuth.CurrentUser != null)
                {
                    try
                    {
                        // Get the user's UID
                        string userId = firebaseAuth.CurrentUser.Uid;

                        // Create a reference to the plan document in Firestore
                        DocumentReference planDocRef = firestore.Collection("users").Document(userId)
                            .Collection("plans").Document(planDocId);

                        Dictionary<string, Java.Lang.Object> updatedData = new Dictionary<string, Java.Lang.Object>();

                        Plan plan = new Plan()
                        {
                            Title = titleEditText.Text,
                            Location = locationEditText.Text,
                            LocationTo = locationToEditText.Text,
                            Description = descriptionEditText.Text,
                            Date = selectDateButton.Text,
                            Time = chooseTimeButton.Text,
                            Accomodation = accomodationSpinner.SelectedItem.ToString(),
                            Transportation = transportationSpinner.SelectedItem.ToString()
                        };

                        System.Diagnostics.Debug.WriteLine("Title: " + plan.Title);

                        // Step 3: Update the properties in the Map object
                        updatedData["Title"] = plan.Title;
                        updatedData["Location"] = plan.Location;
                        updatedData["LocationTo"] = plan.LocationTo;
                        updatedData["Description"] = plan.Description;
                        updatedData["Date"] = plan.Date;
                        updatedData["Time"] = plan.Time;
                        updatedData["Accomodation"] = plan.Accomodation;
                        updatedData["Transportation"] = plan.Transportation;

                        // Step 4: Update the Firestore document
                        planDocRef.Update(updatedData);

                        // Update the text views with the edited values
                        titleTextView.Text = plan.Title;
                        locationTextView.Text = plan.Location;
                        locationToTextView.Text = plan.LocationTo;
                        descriptionTextView.Text = plan.Description;
                        dateTextView.Text = plan.Date;
                        timeTextView.Text = plan.Time;
                        accomodationTextView.Text = plan.Accomodation;
                        transportationTextView.Text = plan.Transportation;

                        // Show success message or perform any other actions
                        Toast.MakeText(this, "Plan updated successfully", ToastLength.Short).Show();
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception here
                        System.Diagnostics.Debug.WriteLine("Failed to update plan: " + ex.Message);
                        Toast.MakeText(this, "Failed to update plan: " + ex.Message, ToastLength.Short).Show();
                    }
                }
                else
                {
                    // Update the plan object with the edited values
                    plan.Title = titleEditText.Text;
                    plan.Location = locationEditText.Text;
                    plan.LocationTo = locationToEditText.Text;
                    plan.Description = descriptionEditText.Text;
                    plan.Date = selectDateButton.Text;
                    plan.Time = chooseTimeButton.Text;
                    plan.Accomodation = accomodationSpinner.SelectedItem.ToString();
                    plan.Transportation = transportationSpinner.SelectedItem.ToString();

                    // Update the plan in the database
                    databaseHelper.UpdatePlan(plan);

                    // Update the text views with the edited values
                    titleTextView.Text = plan.Title;
                    locationTextView.Text = plan.Location;
                    locationToTextView.Text = plan.LocationTo;
                    descriptionTextView.Text = plan.Description;
                    dateTextView.Text = plan.Date;
                    timeTextView.Text = plan.Time;
                    accomodationTextView.Text = plan.Accomodation;
                    transportationTextView.Text = plan.Transportation;
                    Toast.MakeText(this, "Plan saved successfully", ToastLength.Short).Show();
                }
            }
        }

        private void DataChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            SetButtonState();
        }

        private void SetButtonState()
        {
            if (isEditMode)
            {
                editSaveButton.Text = "Save";
                cancelButton.Visibility = Android.Views.ViewStates.Visible;

                if (titleEditText.Text != originalTitleText || locationEditText.Text != originalLocationText || locationToEditText.Text != originalLocationToText || descriptionEditText.Text != originalDescriptionText || selectDateButton.Text != originalDateText || chooseTimeButton.Text != originalTimeText || selectedAccomodation != originalAccomodationText || selectedTransportation != originalTransportationText)
                {
                    System.Diagnostics.Debug.WriteLine("selectedAcco: " + selectedAccomodation);
                    System.Diagnostics.Debug.WriteLine("originalAcco: " + originalAccomodationText);
                    // Enable the save button if data has changed
                    editSaveButton.Enabled = true;
                    editSaveButton.SetTextColor(Color.White);
                    editSaveButton.SetBackgroundResource(Resource.Drawable.button_gradient);
                }
                else
                {
                    // Disable the save button if no data has changed
                    editSaveButton.Enabled = false;
                    editSaveButton.SetTextColor(Color.Gray);
                    editSaveButton.SetBackgroundResource(Resource.Drawable.inactive_button);
                }
            }
            else
            {
                editSaveButton.Text = "Edit";
                cancelButton.Visibility = Android.Views.ViewStates.Gone;
                // Enable the save button if data has changed
                editSaveButton.Enabled = true;
                editSaveButton.SetTextColor(Color.White);
                editSaveButton.SetBackgroundResource(Resource.Drawable.button_gradient);
            }
        }

        private void SetFieldEditableState(bool isEditable)
        {
            titleEditText.Enabled = isEditable;
            locationEditText.Enabled = isEditable;
            locationToEditText.Enabled = isEditable;
            descriptionEditText.Enabled = isEditable;
            selectDateButton.Enabled = isEditable;
            chooseTimeButton.Enabled = isEditable;
            accomodationSpinner.Enabled = isEditable;
            transportationSpinner.Enabled = isEditable;
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            // Hide the keyboard
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(cancelButton.WindowToken, 0);

            if (isEditMode)
            {
                if (titleEditText.Text == originalTitleText &&
                    locationEditText.Text == originalLocationText &&
                    descriptionEditText.Text == originalDescriptionText &&
                    selectDateButton.Text == originalDateText &&
                    chooseTimeButton.Text == originalTimeText && locationToEditText.Text == originalLocationToText && selectedAccomodation == originalAccomodationText && selectedTransportation == originalTransportationText)
                {
                    // No changes were made, exit edit mode directly

                    // Show text views and hide edit texts
                    titleTextView.Visibility = Android.Views.ViewStates.Visible;
                    locationTextView.Visibility = Android.Views.ViewStates.Visible;
                    locationToTextView.Visibility = Android.Views.ViewStates.Visible;
                    descriptionTextView.Visibility = Android.Views.ViewStates.Visible;
                    dateTextView.Visibility = Android.Views.ViewStates.Visible;
                    timeTextView.Visibility = Android.Views.ViewStates.Visible;
                    accomodationTextView.Visibility = Android.Views.ViewStates.Visible;
                    transportationTextView.Visibility = Android.Views.ViewStates.Visible;

                    titleEditText.Visibility = Android.Views.ViewStates.Gone;
                    locationEditText.Visibility = Android.Views.ViewStates.Gone;
                    locationToEditText.Visibility = Android.Views.ViewStates.Gone;
                    descriptionEditText.Visibility = Android.Views.ViewStates.Gone;
                    selectDateButton.Visibility = Android.Views.ViewStates.Gone;
                    chooseTimeButton.Visibility = Android.Views.ViewStates.Gone;
                    accomodationLayout.Visibility = Android.Views.ViewStates.Gone;
                    transportationLayout.Visibility = Android.Views.ViewStates.Gone;

                    // Reset text fields to original values
                    titleEditText.Text = originalTitleText;
                    locationEditText.Text = originalLocationText;
                    locationToEditText.Text = originalLocationToText;
                    descriptionEditText.Text = originalDescriptionText;
                    selectDateButton.Text = originalDateText;
                    chooseTimeButton.Text = originalTimeText;
                    selectedAccomodation = originalAccomodationText;
                    selectedTransportation = originalTransportationText;

                    // Exit edit mode
                    isEditMode = false;
                    cancelButton.Visibility = Android.Views.ViewStates.Gone;
                    editSaveButton.Text = "Edit";
                    // Enable the save button if data has changed
                    editSaveButton.Enabled = true;
                    editSaveButton.SetTextColor(Color.White);
                    editSaveButton.SetBackgroundResource(Resource.Drawable.button_gradient);
                }
                else
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Cancel Edit");
                    alert.SetMessage("Are you sure you want to cancel your changes?");
                    alert.SetPositiveButton("Yes", (dialog, which) =>
                    {
                        // User confirmed, exit edit mode without saving changes

                        // Show text views and hide edit texts
                        titleTextView.Visibility = Android.Views.ViewStates.Visible;
                        locationTextView.Visibility = Android.Views.ViewStates.Visible;
                        locationToTextView.Visibility = Android.Views.ViewStates.Visible;
                        descriptionTextView.Visibility = Android.Views.ViewStates.Visible;
                        dateTextView.Visibility = Android.Views.ViewStates.Visible;
                        timeTextView.Visibility = Android.Views.ViewStates.Visible;
                        accomodationTextView.Visibility = Android.Views.ViewStates.Visible;
                        transportationTextView.Visibility = Android.Views.ViewStates.Visible;

                        titleEditText.Visibility = Android.Views.ViewStates.Gone;
                        locationEditText.Visibility = Android.Views.ViewStates.Gone;
                        locationToEditText.Visibility = Android.Views.ViewStates.Gone;
                        descriptionEditText.Visibility = Android.Views.ViewStates.Gone;
                        selectDateButton.Visibility = Android.Views.ViewStates.Gone;
                        chooseTimeButton.Visibility = Android.Views.ViewStates.Gone;
                        accomodationLayout.Visibility = Android.Views.ViewStates.Gone;
                        transportationLayout.Visibility = Android.Views.ViewStates.Gone;

                        // Reset text fields to original values
                        titleEditText.Text = originalTitleText;
                        locationEditText.Text = originalLocationText;
                        locationToEditText.Text = originalLocationToText;
                        descriptionEditText.Text = originalDescriptionText;
                        selectDateButton.Text = originalDateText;
                        chooseTimeButton.Text = originalTimeText;
                        selectedAccomodation = originalAccomodationText;
                        selectedTransportation = originalTransportationText;

                        // Exit edit mode
                        isEditMode = false;
                        cancelButton.Visibility = Android.Views.ViewStates.Gone;
                        editSaveButton.Text = "Edit";
                        // Enable the save button if data has changed
                        editSaveButton.Enabled = true;
                        editSaveButton.SetTextColor(Color.White);
                        editSaveButton.SetBackgroundResource(Resource.Drawable.button_gradient);
                    });
                    alert.SetNegativeButton("No", (dialog, which) =>
                    {
                        // User canceled, do nothing
                    });
                    alert.Show();
                }
            }
        }


        public override void OnBackPressed()
        {
            if (isEditMode)
            {
                if (titleEditText.Text == originalTitleText &&
                    locationEditText.Text == originalLocationText &&
                    descriptionEditText.Text == originalDescriptionText &&
                    selectDateButton.Text == originalDateText &&
                    chooseTimeButton.Text == originalTimeText && locationToEditText.Text == originalLocationToText && selectedAccomodation == originalAccomodationText && selectedTransportation == originalTransportationText)
                {
                    // No changes were made, directly finish the activity
                    Finish();
                }
                else
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Discard Changes");
                    builder.SetMessage("Are you sure you want to discard the changes?");
                    builder.SetPositiveButton("Discard", (sender, args) =>
                    {
                        // Prepare the result intent
                        Intent resultIntent = new Intent();
                        resultIntent.PutExtra("PlanTitle", originalTitleText);
                        resultIntent.PutExtra("PlanLocation", originalLocationText);
                        resultIntent.PutExtra("PlanLocationTo", originalLocationToText);
                        resultIntent.PutExtra("PlanDescription", originalDescriptionText);
                        resultIntent.PutExtra("PlanDate", originalDateText);
                        resultIntent.PutExtra("PlanTime", originalTimeText);
                        resultIntent.PutExtra("PlanAccomodation", originalAccomodationText);
                        resultIntent.PutExtra("PlanTransportation", originalTransportationText);

                        // Set the result and finish the activity
                        SetResult(Result.Ok, resultIntent);
                        Finish();
                    });
                    builder.SetNegativeButton("Cancel", (sender, args) =>
                    {
                        // Do nothing, stay in the current activity
                    });
                    AlertDialog dialog = builder.Create();
                    dialog.Show();
                }
            }
            else
            {
                Finish();
            }
        }


        public void OnSuccess(Java.Lang.Object result)
        {
            var snapshot = (DocumentSnapshot)result;

            if (snapshot.Exists())
            {
                Plan plan = new Plan
                {
                    DocumentId = snapshot.GetString("Id"),
                    Title = snapshot.GetString("Title"),
                    Location = snapshot.GetString("Location"),
                    LocationTo = snapshot.GetString("LocationTo"),
                    Description = snapshot.GetString("Description"),
                    Date = snapshot.GetString("Date"),
                    Time = snapshot.GetString("Time"),
                    Accomodation = snapshot.GetString("Accomodation"),
                    Transportation = snapshot.GetString("Transportation")
                };

                RunOnUiThread(() =>
                {
                    titleTextView.Text = plan.Title;
                    locationTextView.Text = plan.Location;
                    locationToTextView.Text = plan.LocationTo;
                    descriptionTextView.Text = plan.Description;
                    dateTextView.Text = plan.Date;
                    timeTextView.Text = plan.Time;
                    accomodationTextView.Text = plan.Accomodation;
                    transportationTextView.Text = plan.Transportation;
                });

                // Dismiss the progress dialog
                progressDialog.Dismiss();
            }
        }

    }
}