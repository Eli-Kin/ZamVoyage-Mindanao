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

        public override int Count => imageList.Count;

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            ImageView imageView = new ImageView(context);
            imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            imageView.SetImageResource(imageList[position]);
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
    }

}
