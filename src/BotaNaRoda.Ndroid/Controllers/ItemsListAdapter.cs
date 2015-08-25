using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using BotaNaRoda.Ndroid.Util;
using Square.Picasso;

namespace BotaNaRoda.Ndroid.Controllers
{
	public class ItemsListAdapter : BaseAdapter<ItemListViewModel>
	{
	    private readonly Activity _context;
	    private readonly ItemData _itemData;

	    public IEnumerable<ItemListViewModel> Items { get; set; }
	    public Location CurrentLocation { get; set; }

		public ItemsListAdapter (Activity context, ItemData itemData)
		{
		    _context = context;
		    _itemData = itemData;
		    Items = new List<ItemListViewModel> ();
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? _context.LayoutInflater.Inflate (Resource.Layout.ItemsListItem, parent, false);
		    var holder = view.Tag as ViewHolder ?? new ViewHolder
		    {
                Image = view.FindViewById<ImageView>(Resource.Id.itemsImageView),
                Distance = view.FindViewById<TextView>(Resource.Id.itemsDistance),
                Name = view.FindViewById<TextView>(Resource.Id.itemsDescription)
            };

			var item = this [position];

			holder.Name.Text = item.Name;
		    holder.Distance.Text = GeoUtil.GetDistance(CurrentLocation, item);

            Picasso.With(_context)
                   .Load(item.ThumbImage.Url)
                   .Placeholder(Resource.Drawable.placeholder)
                   .Error(Resource.Drawable.error)
                   .ResizeDimen(Resource.Dimension.list_detail_image_size, Resource.Dimension.list_detail_image_size)
                   .CenterInside()
                   .Tag(_context)
                   .Into(holder.Image);

            return view;
		}

		public override int Count
		{
		    get { return Items.Count(); }
		}

		public override ItemListViewModel this [int index]
		{
		    get { return Items.ElementAt(index); }
		}

        private class ViewHolder : Java.Lang.Object
        {
            internal ImageView Image;
            internal TextView Name;
            internal TextView Distance;
        }
    }
}

