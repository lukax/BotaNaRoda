
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
using Android.Locations;
using Android.Content.PM;

namespace BotaNaRoda.Android
{
	[Activity (Label = "ItemEditActivity",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]			
	public class ItemEditActivity : Activity, ILocationListener
	{
		EditText _itemDescriptionView;
		Item _item;
		LocationManager _locMgr;
		ImageButton _locationImageButton;
		EditText _itemAddressView;
		Location currentLocation; 
		ProgressDialog _progressDialog;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ItemEdit);
		
			_locMgr = GetSystemService(Context.LocationService) as LocationManager;

			_itemDescriptionView = FindViewById<EditText> (Resource.Id.itemDescriptionText);
			_itemAddressView = FindViewById<EditText> (Resource.Id.itemAddressText);
			_locationImageButton = FindViewById<ImageButton> (Resource.Id.locationImageButton);
			_locationImageButton.Click += _locationImageButton_Click;

			_item = new Item ();
			if (Intent.HasExtra ("itemId")) {
				_item = ItemData.Service.GetAllItems () [Intent.GetIntExtra ("itemId", 0)];
				UpdateUI ();
			}
		}

		void _locationImageButton_Click (object sender, EventArgs e)
		{
			Criteria criteria = new Criteria ();
			criteria.Accuracy = Accuracy.Fine;
			criteria.PowerRequirement = Power.High;
			_locMgr.RequestSingleUpdate (criteria, this, null);
			_progressDialog = ProgressDialog.Show (this, "", "Obtendo localização");
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
			if (currentLocation == null) {
				Toast toast = Toast.MakeText (this, "Não é possivel salvar item sem a localização!", ToastLength.Short);
				toast.Show ();
				return;
			}

			_item.Description = _itemDescriptionView.Text;
			_item.Latitude = currentLocation.Latitude;
			_item.Longitude = currentLocation.Longitude;
			ItemData.Service.SaveItem (_item);
			Finish ();
		}

		void DeleteItem ()
		{
			AlertDialog.Builder alertConfirm = new AlertDialog.Builder (this);
			alertConfirm.SetCancelable (false);
			alertConfirm.SetPositiveButton ("OK", ConfirmDelete);
			alertConfirm.SetNegativeButton ("Cancel", delegate {});
			alertConfirm.SetMessage ("Tem certeza que quer remover o Item?");
			alertConfirm.Show ();
		}

		void ConfirmDelete(object sender, EventArgs e)
		{
			ItemData.Service.DeleteItem (_item);
			Toast toast = Toast.MakeText (this, "Item removido", ToastLength.Short);
			toast.Show ();
			Finish ();
		}

		void UpdateUI ()
		{
			_itemDescriptionView.Text = _item.Description;
		}

		void UpdateAddressFields(Address address){
			if(String.IsNullOrEmpty(_itemAddressView.Text)) {
				for (int i = 0; i < address.MaxAddressLineIndex; i++) {
					if (!String.IsNullOrEmpty(_itemAddressView.Text))
						_itemAddressView.Text += System.Environment.NewLine;
					_itemAddressView.Text += address.GetAddressLine (i);
				}
			}
		}

		public void OnLocationChanged (Location location)
		{
			currentLocation = location;

			Geocoder geocdr = new Geocoder (this);
			var addresses = geocdr.GetFromLocation (location.Latitude, location.Longitude, 1);
			if (addresses.Any ()) {
				UpdateAddressFields (addresses.First ());
			}

			_progressDialog.Cancel ();
		}

		public void OnProviderDisabled (string provider)
		{
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}
	}
}

