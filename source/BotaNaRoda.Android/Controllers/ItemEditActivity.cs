
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Android.Entity;

namespace BotaNaRoda.Android
{
	[Activity (Label = "ItemEditActivity")]			
	public class ItemEditActivity : Activity
	{
		EditText _itemDescriptionView;
		Item _item;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ItemEdit);
		
			_itemDescriptionView = FindViewById<EditText> (Resource.Id.itemCreateDescription);

			_item = new Item ();
			if (Intent.HasExtra ("itemId")) {
				_item = ItemData.Service.GetAllItems () [Intent.GetIntExtra ("itemId", 0)];
				UpdateUI ();
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.ItemEditMenu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			base.OnPrepareOptionsMenu (menu);
			// disable delete for a new POI
			if (_item.Id == null) {
				IMenuItem item = menu.FindItem (Resource.Id.actionDelete);
				item.SetEnabled (false);
			}
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.actionSave:
					SaveItem ();
					return true;
				case Resource.Id.actionDelete:
					DeleteItem ();
					return true;
				default :
					return base.OnOptionsItemSelected(item);
			}
		}

		void SaveItem ()
		{
			_item.Description = _itemDescriptionView.Text;
			ItemData.Service.SaveItem (_item);
			Finish ();
		}

		void DeleteItem ()
		{
			ItemData.Service.DeleteItem (_item);
			Finish ();
		}

		void UpdateUI ()
		{
			_itemDescriptionView.Text = _item.Description;
		}
	}
}

