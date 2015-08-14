using System;
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Entity;

namespace BotaNaRoda.Ndroid.Controllers
{
	public class ItemsListAdapter : BaseAdapter<Item>
	{
		Activity context;
		public Location CurrentLocation { get; set; }

		public ItemsListAdapter (Activity context)
		{
			this.context = context;
		}

		#region implemented abstract members of BaseAdapter

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? context.LayoutInflater.Inflate (Resource.Layout.ItemsListItem, null);

			var item = this [position];
			view.FindViewById<TextView> (Resource.Id.itemsDescription).Text = item.Description;

			//calculate distance
			if ((CurrentLocation != null) && (item.Latitude.HasValue) && (item.Longitude.HasValue)) {
				Location itemLocation = new Location ("");
				itemLocation.Latitude = item.Latitude.Value;
				itemLocation.Longitude = item.Longitude.Value;
				float distance = CurrentLocation.DistanceTo (itemLocation);
				view.FindViewById<TextView>(Resource.Id.itemsDistance).Text = String.Format("{0:0,0.00}m", distance);
			}
			else {
				view.FindViewById<TextView>(Resource.Id.itemsDistance).Text = "??";
			}

			var imgView = view.FindViewById<ImageView> (Resource.Id.itemsImageView);
			//load image
			using (Bitmap itemImage = ItemData.GetImageFile (item.Id, imgView.Width, imgView.Height)) {
				imgView.SetImageBitmap (itemImage);
			}
			// Dispose of the Java side bitmap.
			//GC.Collect();
			return view;
		}

		public override int Count {
			get {
				return ItemData.Service.GetAllItems ().Count;
			}
		}

		#endregion

		#region implemented abstract members of BaseAdapter

		public override Item this [int index] {
			get {
				return ItemData.Service.GetAllItems () [index];
			}
		}

		#endregion
	}
}

