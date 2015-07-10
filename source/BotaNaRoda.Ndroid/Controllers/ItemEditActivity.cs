using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Entity;

namespace BotaNaRoda.Ndroid.Controllers
{
	[Activity (Label = "ItemEditActivity",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]			
	public class ItemEditActivity : Activity, ILocationListener
	{
		const int CAPTURE_PHOTO = 0;
		EditText _itemDescriptionView;
		Item _item;
		LocationManager _locMgr;
		EditText _itemAddressView;
		Location currentLocation; 
		ProgressDialog _progressDialog;
		ImageView _itemImageView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ItemEdit);
		
			_locMgr = GetSystemService(Context.LocationService) as LocationManager;

			_itemDescriptionView = FindViewById<EditText> (Resource.Id.itemDescriptionText);
			_itemAddressView = FindViewById<EditText> (Resource.Id.itemAddressText);
			_itemImageView = FindViewById<ImageView> (Resource.Id.itemImageView);
            _itemImageView.Click += _imageButton_Click;

			_item = new Item ();
			if (Intent.HasExtra ("itemId")) {
				_item = ItemData.Service.GetAllItems () [Intent.GetIntExtra ("itemId", 0)];
				UpdateUI ();
			}
		}

	    protected override void OnResume()
	    {
	        base.OnResume();
            GetLocation();
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
				IMenuItem delete = menu.FindItem (Resource.Id.actionDelete);
				delete.SetEnabled (false);
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
                case Resource.Id.actionMap:
			        OpenMap();
			        return true;
                case Resource.Id.actionLocation:
                    GetLocation();
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
					using (Bitmap itemImage = ItemData.GetImageFile (_item.Id)) {
						_itemImageView.SetImageBitmap (itemImage);
					}
				} else {
					// let the user know the photo was cancelled
					Toast toast = Toast.MakeText (this, "No picture captured.", ToastLength.Short);
					toast.Show ();
				} 
			} else {
				base.OnActivityResult (requestCode, resultCode, data);
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
			
		void _imageButton_Click (object sender, EventArgs e)
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

		void OpenMap ()
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

		void GetLocation ()
		{
			Criteria criteria = new Criteria ();
			criteria.Accuracy = Accuracy.Fine;
			criteria.PowerRequirement = Power.High;
			_locMgr.RequestSingleUpdate (criteria, this, null);
			_progressDialog = ProgressDialog.Show (this, "", "Obtendo localização");
		}


	}
}

