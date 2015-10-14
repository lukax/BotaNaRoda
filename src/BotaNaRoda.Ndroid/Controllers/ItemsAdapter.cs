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
using Square.Picasso;

namespace BotaNaRoda.Ndroid.Controllers
{
	public class ItemsAdapter : RecyclerView.Adapter
	{
	    private readonly Context _context;
	    public readonly IList<ItemListViewModel> Items = new List<ItemListViewModel>();
	    public ILatLon UserLatLon { get; set; }

		public ItemsAdapter (Context context, ILatLon userLatLon)
		{
		    _context = context;
		    UserLatLon = userLatLon;
		    HasStableIds = true;
		}

	    public override int ItemCount
        {
            get { return Items.Count; }
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
            // you could also put an 'item type' enum field or property in your 
            // data item and do a 'switch/case' on that. It's less expensive 
            // than reflection...
            var viewHolder = holder as ItemViewHolder;
            if (viewHolder != null)
            {
                var item = Items[position];

                viewHolder.ItemId = item.Id;
                viewHolder.Name.Text = item.Name;
                viewHolder.Distance.Text = item.DistanceTo(UserLatLon);
                if (item.ThumbImage.Height.HasValue && item.ThumbImage.Width.HasValue)
                {
                    var rlp = viewHolder.Image.LayoutParameters;
                    //float ratio = item.ThumbImage.Height.Value / item.ThumbImage.Width.Value;
                    //rlp.Width = viewHolder.Image.Width;
                    //rlp.Height = (int)(viewHolder.Image.Width * ratio);
                    rlp.Height = (int) item.ThumbImage.Height.Value;
                    viewHolder.Image.LayoutParameters = rlp;
                }

                viewHolder.Image.Post(() =>
                {
                    Picasso.With(_context)
                        .Load(item.ThumbImage.Url)
                        .Resize(viewHolder.Image.Width, viewHolder.Image.Height) //Not needed image already resized
                        .CenterCrop()
                        .Into(viewHolder.Image);
                });
            }
        }
       
        private class ItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            internal new string ItemId;
            internal readonly ImageView Image;
            internal readonly TextView Name;
            internal readonly TextView Distance;
            internal readonly CardView CardView;

            public ItemViewHolder(View view) : base(view)
            {
                view.SetOnClickListener(this);
                Image = view.FindViewById<ImageView>(Resource.Id.itemsImageView);
                Distance = view.FindViewById<TextView>(Resource.Id.itemsDistance);
                Name = view.FindViewById<TextView>(Resource.Id.itemsDescription);
                CardView = view.FindViewById<CardView>(Resource.Id.card_view);
            }

            public void OnClick(View v)
            {
                Intent itemDetailIntent = new Intent(v.Context, typeof(ItemDetailActivity));
                itemDetailIntent.PutExtra("itemId", ItemId);
                v.Context.StartActivity(itemDetailIntent);
            }


        }
    }
}

