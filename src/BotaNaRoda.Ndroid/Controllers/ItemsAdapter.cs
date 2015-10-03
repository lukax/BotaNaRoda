﻿using System;
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
using Square.Picasso;

namespace BotaNaRoda.Ndroid.Controllers
{
	public class ItemsAdapter : RecyclerView.Adapter
	{
	    private readonly Context _context;
	    private readonly IList<ItemListViewModel> _items;
	    public ILatLon UserLatLon { get; set; }

		public ItemsAdapter (Context context, IList<ItemListViewModel> items, ILatLon userLatLon)
		{
		    _context = context;
		    _items = items;
		    UserLatLon = userLatLon;
		}

	    public override int ItemCount
        {
            get { return _items.Count; }
        }

	    public override long GetItemId(int position)
	    {
	        return position;
	    }

	    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemsListItem, parent, false);
            return new ItemViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _items[position];

            // you could also put an 'item type' enum field or property in your 
            // data item and do a 'switch/case' on that. It's less expensive 
            // than reflection...
            if (holder.GetType() == typeof(ItemViewHolder))
            {
                var viewHolder = holder as ItemViewHolder;
                if (viewHolder != null)
                {
                    viewHolder.ItemId = item.Id;
                    viewHolder.Name.Text = item.Name;
                    viewHolder.Distance.Text = item.DistanceTo(UserLatLon);
                    viewHolder.Image.Post(() =>
                    {
                        Picasso.With(_context)
                            .Load(item.ThumbImage.Url)
                            .Resize(viewHolder.Image.Width, 0)
                            .Tag(_context)
                            .Into(viewHolder.Image);
                    });

                }
            }
        }
        
        private class ItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            internal new string ItemId;
            internal readonly ImageView Image;
            internal readonly TextView Name;
            internal readonly TextView Distance;

            public ItemViewHolder(View view) : base(view)
            {
                view.SetOnClickListener(this);
                Image = view.FindViewById<ImageView>(Resource.Id.itemsImageView);
                Distance = view.FindViewById<TextView>(Resource.Id.itemsDistance);
                Name = view.FindViewById<TextView>(Resource.Id.itemsDescription);
            }

            public void OnClick(View v)
            {
                Intent itemDetailIntent = new Intent(v.Context, typeof(ItemDetailActivity));
                itemDetailIntent.PutExtra("itemId", ItemId);
                v.Context.StartActivity(itemDetailIntent);
            }

			public string Key { get; set; }
        }
    }
}

