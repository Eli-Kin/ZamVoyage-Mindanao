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
using System.Text;

namespace ZamVoyage.Planner
{
    public class Plan
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string DocumentId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string LocationTo { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Transportation { get; set; }
        public string Accomodation { get; set; }

    }
}