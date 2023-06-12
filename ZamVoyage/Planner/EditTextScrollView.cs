using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZamVoyage.Planner
{
    public class EditTextScrollView : ScrollView
    {
        public EditTextScrollView(Context context) : base(context)
        {
        }

        public EditTextScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public EditTextScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public EditTextScrollView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (ev.Action == MotionEventActions.Down)
            {
                // Check if the touch event is inside the EditText
                View editText = FindViewById<EditText>(Resource.Id.descriptionEditText);
                if (editText != null)
                {
                    Rect editTextRect = new Rect();
                    editText.GetGlobalVisibleRect(editTextRect);
                    if (editTextRect.Contains((int)ev.RawX, (int)ev.RawY))
                    {
                        // Return false to allow the EditText to handle the touch event and scroll
                        return false;
                    }
                }
            }

            // If the touch event is outside the EditText, let the parent ScrollView handle the touch event and scroll
            return base.OnInterceptTouchEvent(ev);
        }
    }
}