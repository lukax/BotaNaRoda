using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
using BotaNaRoda.Ndroid.Models;
using Java.IO;
using Uri = Android.Net.Uri;

namespace BotaNaRoda.Ndroid.Controllers
{
	[Activity (Label = "ItemCreateActivity",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), ParentActivity = typeof(ItemsActivity))]	
	public class ItemCreateActivity : Activity, ILocationListener
	{
        private static readonly TaskScheduler UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
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
	    private ArrayAdapter _categoriesAdapter;
		private Button _saveBtn;
	    private ItemData _itemData;

	    protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ItemCreate);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

			_locMgr = GetSystemService(LocationService) as LocationManager;
            _itemData = new ItemData();

			_itemTitleView = FindViewById<EditText> (Resource.Id.itemTitle);
			_itemDescriptionView = FindViewById<EditText> (Resource.Id.itemDescription);
			_itemImageView = FindViewById<ImageView> (Resource.Id.itemImageView);
	        _itemImageView.Click += _imageButton_Click;
		    _itemCategory = FindViewById<Spinner>(Resource.Id.itemCategory);
	        _categoriesAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.item_categories, Android.Resource.Layout.SimpleSpinnerItem);
            _categoriesAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _itemCategory.Adapter = _categoriesAdapter;
			_saveBtn = FindViewById<Button> (Resource.Id.saveButton);
			_saveBtn.Click += _saveButton_Click;
		}

	    protected override void OnResume()
	    {
	        base.OnResume();
            GetLocation();
	    }

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == CapturePhoto) {
				if (resultCode == Result.Ok) {
					// display saved image
				    _imageTaken = true;
					using (Bitmap itemImage = _itemData.GetImageFile (_newItemGuid, _itemImageView.Width, _itemImageView.Height)) {
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

		    var item = new ItemCreateBindingModel
		    {
		        Name = _itemTitleView.Text,
		        Description = _itemDescriptionView.Text,
		        Category = (CategoryType) _categoriesAdapter.GetPosition(_itemCategory.SelectedItem),
		        Address = _addresses.First().FeatureName,
		        Latitude = _currentLocation.Latitude,
		        Longitude = _currentLocation.Longitude
		    };

            BackgroundWorker worker = new BackgroundWorker();
	        worker.DoWork += async (o, args) => await _itemData.Service.SaveItem(item);
	        worker.RunWorkerCompleted += (o, args) => Task.FromResult(0).ContinueWith(t => { Finish(); }, UiScheduler);
            worker.RunWorkerAsync();
		}

		void _imageButton_Click (object sender, EventArgs e)
		{
			File imageFile = new File(
				_itemData.Service.GetImageFileName(_newItemGuid));
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

		void GetLocation ()
		{
		    Criteria criteria = new Criteria
		    {
		        Accuracy = Accuracy.Fine,
		        PowerRequirement = Power.High
		    };
		    _locMgr.RequestSingleUpdate (criteria, this, null);
			_progressDialog = ProgressDialog.Show (this, "", "Obtendo localização");
		}


	}
}

