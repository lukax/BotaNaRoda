using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;

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

	    #region implemented abstract members of BaseAdapter

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? _context.LayoutInflater.Inflate (Resource.Layout.ItemsListItem, null);

			var item = this [position];
			view.FindViewById<TextView> (Resource.Id.itemsDescription).Text = item.Name;

			//calculate distance
			if (CurrentLocation != null) {
			    Location itemLocation = new Location("")
			    {
			        Latitude = item.Latitude,
			        Longitude = item.Longitude
			    };
			    float distance = CurrentLocation.DistanceTo (itemLocation);
				view.FindViewById<TextView>(Resource.Id.itemsDistance).Text = string.Format("{0:0,0.00}m", distance);
			}
			else {
				view.FindViewById<TextView>(Resource.Id.itemsDistance).Text = "??";
			}

			var imgView = view.FindViewById<ImageView> (Resource.Id.itemsImageView);
			//load image
			using (Bitmap itemImage = _itemData.GetImageFile (item.Id, imgView.Width, imgView.Height)) {
				imgView.SetImageBitmap (itemImage);
			}
			// Dispose of the Java side bitmap.
			//GC.Collect();
			return view;
		}

		public override int Count
		{
		    get { return Items.Count(); }
		}

	    #endregion

		#region implemented abstract members of BaseAdapter

		public override ItemListViewModel this [int index]
		{
		    get { return Items.ElementAt(index); }
		}

	    #endregion
	}
}

