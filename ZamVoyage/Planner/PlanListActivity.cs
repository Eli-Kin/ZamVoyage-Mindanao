using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Org.Apache.Commons.Logging;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using static Android.Icu.Text.Transliterator;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace ZamVoyage.Planner
{
    [Activity(Label = " ", Theme = "@style/AppTheme.NoActionBar")]
    public class PlanListActivity : AppCompatActivity
    {
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        PlanAdapter planAdapter;
        private PlanDatabaseHelper databaseHelper;
        private FirebaseAuth firebaseAuth;
        private FirebaseFirestore firestore;
        private SQLiteConnection databaseConnection;
        ProgressDialog progressDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_plan_list);

            var backArrowDrawable = Resources.GetDrawable(Resource.Drawable.ic_back);
            backArrowDrawable.SetTint(Color.ParseColor("#0D8BFF"));
            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(backArrowDrawable);

            // Initialize Firebase
            FirebaseApp.InitializeApp(this);
            firebaseAuth = FirebaseAuth.Instance;
            firestore = FirebaseFirestore.Instance;

            FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder()
            .SetPersistenceEnabled(true)
            .Build();

            // Initialize database
            databaseHelper = new PlanDatabaseHelper(
                System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    "plans.db"));

            // Find RecyclerView
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            List<Plan> plans = new List<Plan>();
            planAdapter = new PlanAdapter(plans);
            planAdapter.ItemClick += PlanAdapter_ItemClick;
            recyclerView.SetAdapter(planAdapter);

            planAdapter.DeleteClick += PlanAdapter_DeleteClick;

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

        private void PlanAdapter_DeleteClick(object sender, int position)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Delete Plan");
            builder.SetMessage("Are you sure you want to delete this plan?");
            builder.SetPositiveButton("Delete", (dialog, which) =>
            {
                // Retrieve the selected plan from the adapter
                Plan selectedPlan = planAdapter.GetItem(position);

                // Check if there is a current user
                if (firebaseAuth.CurrentUser != null)
                {
                    // Show the progress dialog
                    ShowProgressDialog("Loading...");

                    // Delete the plan from Firestore
                    DeletePlanFromFirestore(selectedPlan.DocumentId, position);
                }
                else
                {
                    // Delete the plan from the SQL database
                    int deletedRows = databaseHelper.DeletePlan(selectedPlan.Id);

                    if (deletedRows > 0)
                    {
                        // Remove the item from the adapter
                        planAdapter.RemoveItem(position);
                    }
                }
            });
            builder.SetNegativeButton("Cancel", (dialog, which) =>
            {

            });
            builder.Show();
        }

        private async void DeletePlanFromFirestore(string planId, int position)
        {
            try
            {
                firestore = FirebaseFirestore.GetInstance(FirebaseApp.Instance);
                firebaseAuth = FirebaseAuth.GetInstance(FirebaseApp.Instance);

                // Get the user's UID
                string userId = firebaseAuth.CurrentUser.Uid;

                // Create a reference to the plan document in Firestore
                DocumentReference planDocRef = firestore.Collection("users").Document(userId)
                    .Collection("plans").Document(planId);

                System.Diagnostics.Debug.WriteLine("IdRef: " + planDocRef);

                // Delete the plan document
                await planDocRef.Delete();

                // Remove the item from the adapter
                planAdapter.RemoveItem(position);

                // Dismiss the progress dialog
                progressDialog.Dismiss();

                Toast.MakeText(this, "Plan deleted successfully", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Failed to delete plan: " + ex.Message, ToastLength.Short).Show();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Refresh the plan list when the activity resumes
            RefreshPlanList();
        }

        private void ShowProgressDialog(string message)
        {
            progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage(message);
            progressDialog.SetCancelable(false);
            progressDialog.Show();
        }

        private void PlanAdapter_ItemClick(object sender, int position)
        {
            // Retrieve the selected plan from the adapter
            Plan selectedPlan = planAdapter.GetItem(position);

            // Open the plan details activity and pass the plan ID as extra data
            Intent intent = new Intent(this, typeof(PlanDetailsActivity));
            intent.PutExtra("PlanId", selectedPlan.Id);
            intent.PutExtra("PlanDocId", selectedPlan.DocumentId);
            StartActivityForResult(intent, 1);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1 && resultCode == Result.Ok)
            {
                int planId = data.GetIntExtra("PlanId", -1);
                string planDocumentId = data.GetStringExtra("PlanDocumentId");
                string planTitle = data.GetStringExtra("PlanTitle");
                string planLocation = data.GetStringExtra("PlanLocation");
                string planLocationTo = data.GetStringExtra("PlanLocationTo");
                string planDescription = data.GetStringExtra("PlanDescription");
                string planDate = data.GetStringExtra("PlanDate");
                string planTime = data.GetStringExtra("PlanTime");
                string planTransportation = data.GetStringExtra("PlanTransportation");
                string planAccomodation = data.GetStringExtra("PlanAccomodation");

                // Update the corresponding item in the list with the updated plan details
                int position = planAdapter.GetPosition(planId);
                if (position >= 0)
                {
                    Plan updatedPlan = planAdapter.GetItem(position);
                    updatedPlan.DocumentId = planDocumentId;
                    updatedPlan.Title = planTitle;
                    updatedPlan.Location = planLocation;
                    updatedPlan.LocationTo = planLocationTo;
                    updatedPlan.Description = planDescription;
                    updatedPlan.Date = planDate;
                    updatedPlan.Time = planTime;
                    updatedPlan.Transportation = planTransportation;
                    updatedPlan.Accomodation = planAccomodation;
                    planAdapter.NotifyItemChanged(position);
                }
            }
        }

        private void LoadPlansFromFirestore()
        {
            // Get the user's UID
            string userId = firebaseAuth.CurrentUser.Uid;

            // Show the progress dialog
            ShowProgressDialog("Loading...");

            // Create a reference to the user's document in the "users" collection
            DocumentReference userDocRef = firestore.Collection("users").Document(userId);

            // Get the plans subcollection reference inside the user document
            CollectionReference plansCollectionRef = userDocRef.Collection("plans");

            plansCollectionRef.Get().AddOnCompleteListener(new OnPlansCompleteListener(this, progressDialog));
        }

        private class OnPlansCompleteListener : Java.Lang.Object, IOnCompleteListener
        {
            private readonly PlanListActivity planListActivity;
            private readonly ProgressDialog progressDialog;

            public OnPlansCompleteListener(PlanListActivity activity, ProgressDialog dialog)
            {
                planListActivity = activity;
                progressDialog = dialog;
            }

            public void OnComplete(Task task)
            {
                if (task.IsSuccessful)
                {
                    // Dismiss the progress dialog
                    planListActivity.RunOnUiThread(() =>
                    {
                        progressDialog.Dismiss();
                    });

                    List<Plan> plans = new List<Plan>();
                    var querySnapshot = (QuerySnapshot)task.Result;
                    foreach (var documentSnapshot in querySnapshot.Documents)
                    {
                        IDictionary<string, Java.Lang.Object> documentData = documentSnapshot.Data;
                        Plan plan = new Plan
                        {
                            DocumentId = documentData["Id"].ToString(),
                            Title = documentData["Title"].ToString(),
                            Location = documentData["Location"].ToString(),
                            Description = documentData["Description"].ToString(),
                            Date = documentData["Date"].ToString(),
                            Time = documentData["Time"].ToString()
                        };
                        plans.Add(plan);
                    }

                    planListActivity.RunOnUiThread(() =>
                    {
                        planListActivity.planAdapter.UpdateData(plans);
                    });
                }
                else
                {
                    planListActivity.RunOnUiThread(() =>
                    {
                        Toast.MakeText(Application.Context, "Failed to load plans from Firestore", ToastLength.Short).Show();
                    });
                }
            }
        }

        private void LoadPlansFromDatabase()
        {
            // Load plans from SQL database
            List<Plan> plans = databaseHelper.GetPlans();
            planAdapter.UpdateData(plans);
        }

        private void RefreshPlanList()
        {
            if (firebaseAuth.CurrentUser != null)
            {
                // User is signed in with Firebase authentication

                // Load plans from Firebase Firestore
                LoadPlansFromFirestore();
            }
            else
            {
                // No current user, use SQL database

                // Load plans from SQL database
                LoadPlansFromDatabase();
            }
        }
    }
}