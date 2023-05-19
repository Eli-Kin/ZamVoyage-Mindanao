using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Joins;
using System.Text;

namespace ZamVoyage.Planner
{
    public class PlanDatabaseHelper
    {
        SQLiteConnection database;

        public PlanDatabaseHelper(string dbPath)
        {
            database = new SQLiteConnection(dbPath);
            database.CreateTable<Plan>();
        }

        public List<Plan> GetPlans()
        {
            return database.Table<Plan>().ToList();
        }

        public int SavePlan(Plan plan)
        {
            return database.Insert(plan);
        }

        public Plan GetPlanById(int id)
        {
            return database.Table<Plan>().FirstOrDefault(p => p.Id == id);
        }

        public int UpdatePlan(Plan plan)
        {
            return database.Update(plan);
        }

        public int DeletePlan(int planId)
        {
            return database.Delete<Plan>(planId);
        }
    }
}