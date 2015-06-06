using System;
using Android.Widget;
using BotaNaRoda.Android.Entity;
using Android.App;
using Android.Views;
using System.Linq;

namespace BotaNaRoda.Android
{
	public class ItemsListAdapter : BaseAdapter<Item>
	{
		Activity context;

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
			view.FindViewById<TextView> (Resource.Id.itemsAuthor).Text = "asdf";
			view.FindViewById<TextView> (Resource.Id.itemsDescription).Text = "asdf";
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

