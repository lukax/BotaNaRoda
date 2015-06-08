
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
using Android.Net;
using Android.Provider;
using Android.Graphics;

namespace BotaNaRoda.Android
{
	[Activity (Label = "ItemEditActivity",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]			
	public class ItemEditActivity : Activity, ILocationListener
	{
		const int CAPTURE_PHOTO = 0;
		EditText _itemDescriptionView;
		Item _item;
		LocationManager _locMgr;
		ImageButton _locationImageButton;
		EditText _itemAddressView;
		Location currentLocation; 
		ProgressDialog _progressDialog;
		ImageButton _mapImageButton;
		ImageView _itemImageView;
		ImageButton _cameraImageButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ItemEdit);
		
			_locMgr = GetSystemService(Context.LocationService) as LocationManager;

			_itemDescriptionView = FindViewById<EditText> (Resource.Id.itemDescriptionText);
			_itemAddressView = FindViewById<EditText> (Resource.Id.itemAddressText);
			_locationImageButton = FindViewById<ImageButton> (Resource.Id.locationImageButton);
			_locationImageButton.Click += _locationImageButton_Click;
			_mapImageButton = FindViewById<ImageButton> (Resource.Id.mapImageButton);
			_mapImageButton.Click += _mapImageButton_Click;
			_cameraImageButton = FindViewById<ImageButton> (Resource.Id.cameraImageButton);
			_cameraImageButton.Click += _cameraImageButton_Click;
			_itemImageView = FindViewById<ImageView> (Resource.Id.itemImageView);

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

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == CAPTURE_PHOTO) {
				if (resultCode == Result.Ok) {
					// display saved image
					Bitmap poiImage = ItemData.GetImageFile (_item.Id);
					_itemImageView.SetImageBitmap (poiImage);
					if (poiImage != null)
						poiImage.Dispose ();
				}
				else {
					// let the user know the photo was cancelled
					Toast toast = Toast.MakeText (this, "No picture captured.",
						ToastLength.Short);
					toast.Show();
				} }
			else
				base.OnActivityResult (requestCode, resultCode, data);
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
			
		void _cameraImageButton_Click (object sender, EventArgs e)
		{
			Java.IO.File imageFile = new Java.IO.File(
				ItemData.Service.GetImageFileName(_item.Id));
			global::Android.Net.Uri imageUri = global::Android.Net.Uri.FromFile (imageFile);

			Intent cameraIntent = new Intent(MediaStore.ActionImageCapture);
			cameraIntent.PutExtra (MediaStore.ExtraOutput, imageUri);
			cameraIntent.PutExtra (MediaStore.ExtraSizeLimit, 1.5 * 1024);

			PackageManager packageManager = PackageManager;
			IList<ResolveInfo> activities =
				packageManager.QueryIntentActivities(cameraIntent, 0);
			if (activities.Count == 0) {
				AlertDialog.Builder alertConfirm = new AlertDialog.Builder (this);
				alertConfirm.SetCancelable (false);
				alertConfirm.SetPositiveButton ("OK", delegate {});
				alertConfirm.SetMessage ("No camera app available.");
				alertConfirm.Show ();
			}
			else {
				StartActivityForResult (cameraIntent, CAPTURE_PHOTO);
			}
		}

		void _mapImageButton_Click (object sender, EventArgs e)
		{
			global::Android.Net.Uri geoUri;
			if (String.IsNullOrEmpty (_itemAddressView.Text)) {
				geoUri = global::Android.Net.Uri.Parse (String.Format ("geo:{0},{1}", _item.Latitude, _item.Longitude)); 
			} else {
				geoUri = global::Android.Net.Uri.Parse (String.Format ("geo:0,0?q={0}", _itemAddressView.Text));
			}
			Intent mapIntent = new Intent (Intent.ActionView, geoUri);

			PackageManager packageManager = PackageManager;
			IList<ResolveInfo> activities =
				packageManager.QueryIntentActivities(mapIntent, 0);
			if (activities.Count == 0) {
				AlertDialog.Builder alertConfirm = new AlertDialog.Builder (this);
				alertConfirm.SetCancelable (false);
				alertConfirm.SetPositiveButton ("OK", delegate {});
				alertConfirm.SetMessage ("No map app available.");
				alertConfirm.Show ();
			} else {
				StartActivity (mapIntent);
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


	}
}

