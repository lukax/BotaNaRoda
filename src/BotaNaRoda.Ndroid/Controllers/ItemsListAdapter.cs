using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using BotaNaRoda.Ndroid.Util;
//using Square.Picasso;
using Square.Picasso;

namespace BotaNaRoda.Ndroid.Controllers
{
	public class ItemsListAdapter : RecyclerView.Adapter
	{
	    private readonly Context _context;

	    public IEnumerable<ItemListViewModel> Items { get; set; }
	    public Location CurrentLocation { get; set; }

		public ItemsListAdapter (Context context)
		{
		    _context = context;
		    Items = new List<ItemListViewModel> ();
		}

        public override int ItemCount
        {
            get { return Items.Count(); }
        }

        public override long GetItemId (int position)
		{
			return position;
		}

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layoutInflater = LayoutInflater.From(parent.Context);
            var view = layoutInflater.Inflate(Resource.Layout.ItemsListItem, parent, false);
            var viewHolder = new ItemViewHolder(view);
            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = Items.ElementAt(position);

            // you could also put an 'item type' enum field or property in your 
            // data item and do a 'switch/case' on that. It's less expensive 
            // than reflection...
            if (holder.GetType() == typeof(ItemViewHolder))
            {
                var viewHolder = holder as ItemViewHolder;
                if (viewHolder != null)
                {
                    viewHolder.Name.Text = item.Name;
                    viewHolder.Distance.Text = GeoUtil.GetDistance(CurrentLocation, item);
                    Picasso.With(_context)
                            .Load(item.ThumbImage.Url)
                            .Placeholder(Resource.Drawable.placeholder)
                            .Error(Resource.Drawable.error)
							.Fit()
							.CenterCrop()
                            .Tag(_context)
                            .Into(viewHolder.Image);
                }
            }
        }

        public override int GetItemViewType(int position)
        {
            return 0;
        }
        
        private class ItemViewHolder : RecyclerView.ViewHolder
        {
            internal readonly ImageView Image;
            internal readonly TextView Name;
            internal readonly TextView Distance;

            public ItemViewHolder(View view) : base(view)
            {
                Image = view.FindViewById<ImageView>(Resource.Id.itemsImageView);
                Distance = view.FindViewById<TextView>(Resource.Id.itemsDistance);
                Name = view.FindViewById<TextView>(Resource.Id.itemsDescription);
            }
        }
    }
}

