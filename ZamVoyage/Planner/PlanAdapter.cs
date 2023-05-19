using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZamVoyage.Planner
{
    public class PlanAdapter : RecyclerView.Adapter
    {
        private List<Plan> plans;
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> DeleteClick;
        private const int EmptyViewType = 0;
        private const int PlanViewType = 1;

        public PlanAdapter(List<Plan> plans)
        {
            this.plans = plans;
        }

        public override int ItemCount => plans.Count == 0 ? 1 : plans.Count;

        public override int GetItemViewType(int position)
        {
            return plans.Count == 0 ? EmptyViewType : PlanViewType;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is PlanViewHolder planViewHolder)
            {
                planViewHolder.TitleTextView.Text = plans[position].Title;
                planViewHolder.DescriptionTextView.Text = plans[position].Description;
            }
            else if (holder is EmptyViewHolder emptyViewHolder)
            {
                emptyViewHolder.MessageTextView.Text = "No Plan";
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == EmptyViewType)
            {
                View emptyView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_no_item, parent, false);
                return new EmptyViewHolder(emptyView);
            }
            else
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_plan, parent, false);
                return new PlanViewHolder(itemView, OnItemClick, OnDeleteClick);
            }
        }

        private void OnItemClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        private void OnDeleteClick(int position)
        {
            DeleteClick?.Invoke(this, position);
        }

        public Plan GetItem(int position)
        {
            return plans[position];
        }

        public void RemoveItem(int position)
        {
            plans.RemoveAt(position);
            NotifyItemRemoved(position);
            NotifyItemRangeChanged(position, ItemCount);
        }

        public int GetPosition(int planId)
        {
            for (int i = 0; i < plans.Count; i++)
            {
                if (plans[i].Id == planId)
                {
                    return i;
                }
            }
            return -1;
        }

        public void UpdateData(List<Plan> updatedPlans)
        {
            plans = updatedPlans;
            NotifyDataSetChanged();
        }
    }

    public class PlanViewHolder : RecyclerView.ViewHolder
    {
        public TextView TitleTextView { get; private set; }
        public TextView DescriptionTextView { get; private set; }
        public ImageView DeleteImageView { get; private set; }

        public PlanViewHolder(View itemView, Action<int> itemClick, Action<int> deleteClick) : base(itemView)
        {
            TitleTextView = itemView.FindViewById<TextView>(Resource.Id.titleTextView);
            DescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.descriptionTextView);
            DeleteImageView = itemView.FindViewById<ImageView>(Resource.Id.deleteImageView);

            itemView.Click += (sender, e) => itemClick(AdapterPosition);
            DeleteImageView.Click += (sender, e) => deleteClick(AdapterPosition);
        }
    }

    public class EmptyViewHolder : RecyclerView.ViewHolder
    {
        public TextView MessageTextView { get; private set; }

        public EmptyViewHolder(View itemView) : base(itemView)
        {
            MessageTextView = itemView.FindViewById<TextView>(Resource.Id.noItemTextView);
        }
    }
}
