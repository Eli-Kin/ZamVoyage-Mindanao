using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZamVoyage.Favorites
{
    public class FavoriteItem
    {
        public int Id { get; set; }
        public string DocumentId { get; set; }
        public string ImagePath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsToggle { get; set; }
    }
}