using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZamVoyage.Content
{
    public class ViewPagerCyclicScrollListener : ViewPager.SimpleOnPageChangeListener
    {
        private readonly ViewPager viewPager;
        private readonly SlideshowAdapter adapter;

        public ViewPagerCyclicScrollListener(ViewPager viewPager, SlideshowAdapter adapter)
        {
            this.viewPager = viewPager;
            this.adapter = adapter;
        }

        public override void OnPageSelected(int position)
        {
            int itemCount = adapter.Count;
            if (position == itemCount - 1)
            {
                // User swiped to the end, move back to the first image without animation
                viewPager.SetCurrentItem(1, false);
            }
            else if (position == 0)
            {
                // User swiped to the beginning, move to the last image without animation
                viewPager.SetCurrentItem(itemCount - 2, false);
            }
        }
    }
}