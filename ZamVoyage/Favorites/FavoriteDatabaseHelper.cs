using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;
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
    public class FavoriteDatabaseHelper : SQLiteOpenHelper
    {
        private const string DatabaseName = "MyDatabase.db";
        private const int DatabaseVersion = 1;
        private const string TableName = "MyTable";
        private const string IdColumn = "Id";
        private const string ImagePathColumn = "ImagePath";
        private const string TitleColumn = "Title";
        private const string DescriptionColumn = "Description";

        public FavoriteDatabaseHelper(Context context) : base(context, DatabaseName, null, DatabaseVersion)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            string createTableQuery = $"CREATE TABLE {TableName} ({IdColumn} INTEGER PRIMARY KEY AUTOINCREMENT, {ImagePathColumn} TEXT, {TitleColumn} TEXT, {DescriptionColumn} TEXT)"; 
            db.ExecSQL(createTableQuery);
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            string dropTableQuery = $"DROP TABLE IF EXISTS {TableName}";
            db.ExecSQL(dropTableQuery);
            OnCreate(db);
        }

        public bool Insert(FavoriteItem contentModel)
        {
            SQLiteDatabase db = WritableDatabase;
            ContentValues values = new ContentValues();
            values.Put(ImagePathColumn, contentModel.ImagePath);
            values.Put(TitleColumn, contentModel.Title);
            values.Put(DescriptionColumn, contentModel.Description);
            long result = db.Insert(TableName, null, values);
            return result != -1;
        }

        public bool Delete(int id)
        {
            SQLiteDatabase db = WritableDatabase;
            ContentValues values = new ContentValues();
            values.Put(ImagePathColumn, "");
            values.Put(TitleColumn, "");
            values.Put(DescriptionColumn, "");
            int result = db.Delete(TableName, $"{IdColumn}={id}", null);
            return result != 0;
        }

        public void ReopenDatabase()
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Close();
        }

        public List<FavoriteItem> GetAll()
        {
            List<FavoriteItem> contentModels = new List<FavoriteItem>();
            SQLiteDatabase db = ReadableDatabase;
            string selectAllQuery = $"SELECT * FROM {TableName}";
            ICursor cursor = db.RawQuery(selectAllQuery, null);
            if (cursor.MoveToFirst())
            {
                do
                {
                    int id = cursor.GetInt(cursor.GetColumnIndexOrThrow(IdColumn));
                    string imagePath = cursor.GetString(cursor.GetColumnIndexOrThrow(ImagePathColumn));
                    string title = cursor.GetString(cursor.GetColumnIndexOrThrow(TitleColumn));
                    string description = cursor.GetString(cursor.GetColumnIndexOrThrow(DescriptionColumn));
                    FavoriteItem contentModel = new FavoriteItem
                    {
                        Id = id,
                        ImagePath = imagePath,
                        Title = title,
                        Description = description
                    };
                    contentModels.Add(contentModel);
                } while (cursor.MoveToNext());
            }
            cursor.Close();
            return contentModels;
        }
    }
}