using System;
using Android.Widget;
using BotaNaRoda.Android.Entity;
using Android.App;
using Android.Views;
using System.Linq;
using Android.Locations;
using Android.Graphics;

namespace BotaNaRoda.Android
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
			view.FindViewById<TextView> (Resource.Id.itemsAuthor).Text = "Me";
			view.FindViewById<TextView> (Resource.Id.itemsDescription).Text = item.Description;

			//calculate distance
			if ((CurrentLocation != null) && (item.Latitude.HasValue) && (item.Longitude.HasValue)) {
				Location itemLocation = new Location ("");
				itemLocation.Latitude = item.Latitude.Value;
				itemLocation.Longitude = item.Longitude.Value;
				float distance = CurrentLocation.DistanceTo (itemLocation);
				view.FindViewById<TextView>
				(Resource.Id.itemsDistance).Text = String.Format("{0:0,0.00}m", distance);
			}
			else {
				view.FindViewById<TextView>
				(Resource.Id.itemsDistance).Text = "??";
			}

			//load image
			using (Bitmap itemImage = ItemData.GetImageFile (item.Id)) {
				view.FindViewById<ImageView> (Resource.Id.itemsImageView).SetImageBitmap (itemImage);
			}

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

