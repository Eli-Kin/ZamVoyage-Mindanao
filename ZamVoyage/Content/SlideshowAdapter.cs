using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ZamVoyage.Content
{
    public class SlideshowAdapter : PagerAdapter
    {
        private readonly Context context;
        private readonly List<int> imageList;

        public SlideshowAdapter(Context context, List<int> imageList)
        {
            this.context = context;
            this.imageList = imageList;
        }

        public override int Count => imageList.Count + 2; // Add 2 extra items for cyclic scrolling

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            int imagePosition = GetImagePosition(position);
            ImageView imageView = new ImageView(context);
            imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            imageView.SetImageResource(imageList[imagePosition]);
            container.AddView(imageView);
            return imageView;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            container.RemoveView((View)objectValue);
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
        {
            return view == objectValue;
        }

        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            return PositionNone; // This disables notifyDataSetChanged() to recreate all views
        }

        private int GetImagePosition(int position)
        {
            int imageCount = imageList.Count;

            if (position == 0) // First item, return last image position
                return imageCount - 1;
            else if (position == Count - 1) // Last item, return first image position
                return 0;
            else // Regular items
                return position - 1;
        }
    }

}
