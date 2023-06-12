using Android.App;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZamVoyage.Search_Features
{
    public class SearchDatabaseHelper : SQLiteOpenHelper
    {
        private const string DatabaseName = "items.db";
        private const int DatabaseVersion = 1;

        public SearchDatabaseHelper(Context context)
            : base(context, DatabaseName, null, DatabaseVersion)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            string createTable = "CREATE TABLE items (_id INTEGER PRIMARY KEY AUTOINCREMENT, title TEXT, image_path TEXT, description TEXT, categories TEXT)";
            db.ExecSQL(createTable);
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL("DROP TABLE IF EXISTS items");
            OnCreate(db);
        }

        public long InsertItem(Search_Item item)
        {
            SQLiteDatabase db = WritableDatabase;
            ContentValues values = new ContentValues();
            values.Put("title", item.Title);
            values.Put("description", item.Description);
            values.Put("image_path", item.ImagePath);
            values.Put("categories", string.Join(",", item.Categories));  // Store categories as comma-separated string
            long id = db.Insert("items", null, values);
            db.Close();
            return id;
        }

        public List<Search_Item> SearchItems(string query)
        {
            List<Search_Item> items = new List<Search_Item>();
            SQLiteDatabase db = ReadableDatabase;
            string[] columns = new string[] { "title", "image_path", "description", "categories" };
            string selection = "categories LIKE ?";
            string[] selectionArgs = new string[] { "%" + query + "%" };
            string orderBy = "title ASC";
            ICursor cursor = db.Query("items", columns, selection, selectionArgs, null, null, orderBy);
            while (cursor.MoveToNext())
            {
                Search_Item item = new Search_Item();
                item.Title = cursor.GetString(cursor.GetColumnIndex("title"));
                item.Description = cursor.GetString(cursor.GetColumnIndex("description"));
                item.ImagePath = cursor.GetString(cursor.GetColumnIndex("image_path"));
                string categoriesString = cursor.GetString(cursor.GetColumnIndex("categories"));
                item.Categories = categoriesString.Split(',').ToList();
                items.Add(item);
            }
            cursor.Close();
            db.Close();
            return items;
        }

        public void DeleteItem(string title)
        {
            SQLiteDatabase db = WritableDatabase;
            db.Delete("items", "title = ?", new string[] { title });
            db.Close();
        }


    }
}