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
using Java.IO;
using Environment = System.Environment;
using Uri = Android.Net.Uri;

namespace BotaNaRoda.Ndroid.Controllers
{
	[Activity (Label = "ItemCreateActivity",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]			
	public class ItemCreateActivity : Activity, ILocationListener
	{
        private const int CapturePhoto = 0;
        private EditText _itemDescriptionView;
        private LocationManager _locMgr;
        private EditText _itemTitleView;
        private Location _currentLocation;
        private ProgressDialog _progressDialog;
        private ImageView _itemImageView;
        private Spinner _itemCategory;
	    private bool _imageTaken;
	    private IList<Address> _addresses;
	    private readonly string _newItemGuid = Guid.NewGuid().ToString();

	    protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ItemCreate);
		
			_locMgr = GetSystemService(LocationService) as LocationManager;

			_itemTitleView = FindViewById<EditText> (Resource.Id.itemTitle);
			_itemDescriptionView = FindViewById<EditText> (Resource.Id.itemDescription);
			_itemImageView = FindViewById<ImageView> (Resource.Id.itemImageView);
            _itemImageView.Click += _imageButton_Click;
		    _itemCategory = FindViewById<Spinner>(Resource.Id.itemCategory);
	        var categoriesAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.item_categories, Android.Resource.Layout.SimpleSpinnerItem);
            categoriesAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _itemCategory.Adapter = categoriesAdapter;
			FindViewById<Button> (Resource.Id.saveButton).Click += _saveButton_Click;
		}

	    protected override void OnResume()
	    {
	        base.OnResume();
            GetLocation();
	    }

	    public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.ItemCreateMenu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			base.OnPrepareOptionsMenu (menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId)
			{
           //     case Resource.Id.actionMap:
			        //OpenMap();
			        //return true;
                case Resource.Id.actionLocation:
                    GetLocation();
			        return true;
				default :
					return base.OnOptionsItemSelected(item);
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == CapturePhoto) {
				if (resultCode == Result.Ok) {
					// display saved image
				    _imageTaken = true;
					using (Bitmap itemImage = ItemData.GetImageFile (_newItemGuid, _itemImageView.Width, _itemImageView.Height)) {
						_itemImageView.SetImageBitmap (itemImage);
					}
				} else {
					// let the user know the photo was cancelled
					Toast toast = Toast.MakeText (this, "Nenhuma foto capturada", ToastLength.Short);
					toast.Show ();
				} 
			} else {
				base.OnActivityResult (requestCode, resultCode, data);
			}
		}

		public void OnLocationChanged (Location location)
		{
			_currentLocation = location;
            Geocoder geocdr = new Geocoder(this);
            _addresses = geocdr.GetFromLocation(location.Latitude, location.Longitude, 1);
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

		void _saveButton_Click (object sender, EventArgs e)
		{
			if (_currentLocation == null) {
				Toast toast = Toast.MakeText (this, "Não é possivel salvar item sem a localização!", ToastLength.Short);
				toast.Show ();
				return;
			}
            if (!_imageTaken)
            {
                Toast toast = Toast.MakeText(this, "Não é possivel salvar item sem pelo menos uma foto!", ToastLength.Short);
                toast.Show();
                return;
            }

		    var item = new Item
		    {
		        Name = _itemTitleView.Text,
		        Id = _newItemGuid,
		        Description = _itemDescriptionView.Text,
		        Category = _itemCategory.SelectedItem.ToString(),
		        Address = _addresses.First().FeatureName,
		        Latitude = _currentLocation.Latitude,
		        Longitude = _currentLocation.Longitude
		    };
		    ItemData.Service.SaveItem (item);
			Finish ();
		}

		void _imageButton_Click (object sender, EventArgs e)
		{
			File imageFile = new File(
				ItemData.Service.GetImageFileName(_newItemGuid));
			var imageUri = Uri.FromFile (imageFile);

			Intent cameraIntent = new Intent(MediaStore.ActionImageCapture);
			cameraIntent.PutExtra (MediaStore.ExtraOutput, imageUri);
			cameraIntent.PutExtra (MediaStore.ExtraSizeLimit, 1 * 1024);

			PackageManager packageManager = PackageManager;
			IList<ResolveInfo> activities =
				packageManager.QueryIntentActivities(cameraIntent, 0);
			if (activities.Count == 0) {
				AlertDialog.Builder alertConfirm = new AlertDialog.Builder (this);
				alertConfirm.SetCancelable (false);
				alertConfirm.SetPositiveButton ("OK", delegate {});
				alertConfirm.SetMessage ("No camera app available");
				alertConfirm.Show ();
			}
			else {
				StartActivityForResult (cameraIntent, CapturePhoto);
			}
		}

		//void OpenMap ()
		//{
		//	Uri geoUri;
		//	if (string.IsNullOrEmpty (_itemTitleView.Text)) {
		//		geoUri = Uri.Parse (string.Format ("geo:{0},{1}", _item.Latitude, _item.Longitude)); 
		//	} else {
		//		geoUri = Uri.Parse (string.Format ("geo:0,0?q={0}", _itemTitleView.Text));
		//	}
		//	Intent mapIntent = new Intent (Intent.ActionView, geoUri);

		//	PackageManager packageManager = PackageManager;
		//	IList<ResolveInfo> activities =
		//		packageManager.QueryIntentActivities(mapIntent, 0);
		//	if (activities.Count == 0) {
		//		AlertDialog.Builder alertConfirm = new AlertDialog.Builder (this);
		//		alertConfirm.SetCancelable (false);
		//		alertConfirm.SetPositiveButton ("OK", delegate {});
		//		alertConfirm.SetMessage ("No map app available.");
		//		alertConfirm.Show ();
		//	} else {
		//		StartActivity (mapIntent);
		//	}
		//}

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

