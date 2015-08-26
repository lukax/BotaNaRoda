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
using Square.Picasso;
using Uri = Android.Net.Uri;

namespace BotaNaRoda.Ndroid.Controllers
{
	[Activity (Label = "ItemCreateActivity",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), ParentActivity = typeof(ItemsActivity))]	
	public class ItemCreateActivity : Activity, ILocationListener
	{
        private static readonly TaskScheduler UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private const int CapturePhoto1 = 0;
        private const int CapturePhoto2 = 1;
        private const int CapturePhoto3 = 2;
        private LocationManager _locMgr;
        private Location _currentLocation;
        private ProgressDialog _progressDialog;
	    private bool _imageTaken;
	    private IList<Address> _addresses;
	    private ArrayAdapter _categoriesAdapter;
	    private ItemData _itemData;
	    private ViewHolder _holder;
	    private readonly string _newItemGuid = Guid.NewGuid().ToString();
        private readonly Dictionary<int, Uri> _captureCodeImageUrlDictionary = new Dictionary<int, Uri>(3);

	    protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ItemCreate);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

			_locMgr = GetSystemService(LocationService) as LocationManager;
            _itemData = new ItemData();

            _holder = new ViewHolder
            {
                ItemTitleView = FindViewById<EditText>(Resource.Id.itemTitle),
                ItemDescriptionView = FindViewById<EditText>(Resource.Id.itemDescription),
                ItemImageView1 = FindViewById<ImageView>(Resource.Id.itemImageView1),
                ItemImageView2 = FindViewById<ImageView>(Resource.Id.itemImageView2),
                ItemImageView3 = FindViewById<ImageView>(Resource.Id.itemImageView3),
                ItemCategory = FindViewById<Spinner>(Resource.Id.itemCategory),
                SaveBtn = FindViewById<Button>(Resource.Id.saveButton)
            };

	        _categoriesAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.item_categories, Android.Resource.Layout.SimpleSpinnerItem);
            _categoriesAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _holder.ItemCategory.Adapter = _categoriesAdapter;
            _holder.ItemImageView1.Click += (s, e) => TakePicture(CapturePhoto1);
            _holder.ItemImageView2.Click += (s, e) => TakePicture(CapturePhoto2);
            _holder.ItemImageView3.Click += (s, e) => TakePicture(CapturePhoto3);
			_holder.SaveBtn.Click += _saveButton_Click;
		}

	    protected override void OnResume()
	    {
	        base.OnResume();
            GetLocation();
	    }

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == CapturePhoto1 || requestCode == CapturePhoto2 || requestCode == CapturePhoto3) {
				if (resultCode == Result.Ok) {
					// display saved image
				    _imageTaken = true;

				    ImageView holder = _holder.ItemImageView1;
				    if (requestCode == CapturePhoto2)
				    {
                        holder = _holder.ItemImageView2;
				    }
                    else if (requestCode == CapturePhoto3)
                    {
                        holder = _holder.ItemImageView3;
                    }
				    var imgUrl = _captureCodeImageUrlDictionary[requestCode];

                    Picasso.With(this)
                           .Load(imgUrl)
                           .Fit()
                           .CenterInside()
                           .Tag(this)
                           .Into(holder);
				}
                else
				{
					// let the user know the photo was cancelled
					//Toast toast = Toast.MakeText (this, "Nenhuma foto capturada", ToastLength.Short);
					//toast.Show ();
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

	        var addr = _addresses.First();
		    var item = new ItemCreateBindingModel
		    {
                Images = _captureCodeImageUrlDictionary.Values.Select(x => new ImageInfo { Url = x.ToString() }).ToArray(),
		        Name = _holder.ItemTitleView.Text,
		        Description = _holder.ItemDescriptionView.Text,
		        Category = (CategoryType) _categoriesAdapter.GetPosition(_holder.ItemCategory.SelectedItem),
		        Address = addr.FeatureName,
                PostalCode = addr.PostalCode,
                CountryCode = addr.CountryCode,
                Locality = addr.Locality,
		        Latitude = _currentLocation.Latitude,
		        Longitude = _currentLocation.Longitude
		    };

            BackgroundWorker worker = new BackgroundWorker();
	        worker.DoWork += async (o, args) =>
	        {
	            await _itemData.Service.SaveItem(item);
	        };
	        worker.RunWorkerCompleted += (o, args) => Task.FromResult(0).ContinueWith(t => { Finish(); }, UiScheduler);
            worker.RunWorkerAsync();
		}


	    void TakePicture(int capturePhotoCode)
	    {
            File imageFile = new File(_itemData.GetLocalImageFileName(_newItemGuid, capturePhotoCode));
            var imageUri = Uri.FromFile(imageFile);
            _captureCodeImageUrlDictionary.Add(capturePhotoCode, imageUri);

            Intent cameraIntent = new Intent(MediaStore.ActionImageCapture);
            cameraIntent.PutExtra(MediaStore.ExtraOutput, imageUri);
            cameraIntent.PutExtra(MediaStore.ExtraSizeLimit, 1 * 1024);

            PackageManager packageManager = PackageManager;
            IList<ResolveInfo> activities =
                packageManager.QueryIntentActivities(cameraIntent, 0);
            if (activities.Count == 0)
            {
                AlertDialog.Builder alertConfirm = new AlertDialog.Builder(this);
                alertConfirm.SetCancelable(false);
                alertConfirm.SetPositiveButton("OK", delegate { });
                alertConfirm.SetMessage("No camera app available");
                alertConfirm.Show();
            }
            else
            {
                StartActivityForResult(cameraIntent, capturePhotoCode);
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

	    private class ViewHolder
	    {
	        internal EditText ItemDescriptionView;
	        internal EditText ItemTitleView;
	        internal ImageView ItemImageView1;
	        internal ImageView ItemImageView2;
	        internal ImageView ItemImageView3;
	        internal Spinner ItemCategory;
	        internal Button SaveBtn;
	    }
	}
}

