using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using Xamarin.Auth;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "ItemDetailActivity",
        ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), ParentActivity = typeof(ItemsActivity))]
    public class ItemDetailActivity : Activity, IOnMapReadyCallback
    {
        private static readonly TaskScheduler UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private ImageView _itemImageView;
        private TextView _itemAuthorView;
        private TextView _itemDescriptionView;
        private ItemDetailViewModel _item;
        private Button _reserveButton;
        private TextView _itemTitleView;
        private TextView _itemLocationView;
        private Button _reportFraudButton;
        private IMenu _menu;
        private Account _currentUser;
        private ItemData _itemData;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ItemDetail);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            _itemData = new ItemData();
            _currentUser = new UserService(this).GetCurrentUser();
            _itemImageView = FindViewById<ImageView>(Resource.Id.itemsDetailImage);
            _itemAuthorView = FindViewById<TextView>(Resource.Id.itemsDetailAuthor);
            _itemTitleView = FindViewById<TextView>(Resource.Id.itemsDetailTitle);
            _itemDescriptionView = FindViewById<TextView>(Resource.Id.itemsDetailDescription);
            _itemLocationView = FindViewById<TextView>(Resource.Id.itemsDetailLocation);
			_reserveButton = FindViewById<Button>(Resource.Id.reserveButton);
            _reserveButton.Click += ReserveItem;
            _reportFraudButton = FindViewById<Button>(Resource.Id.reportFraudButton);
            _reportFraudButton.Click += ReportFraudButtonOnClick;

            Refresh();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ItemDetailMenu, menu);
            _menu = menu;
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.actionDelete:
                    DeleteItem();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

		void ReserveItem (object sender, EventArgs e)
		{
		    _reserveButton.Text = "Reserved";
		    _reserveButton.Enabled = false;
		}

        private void ReportFraudButtonOnClick(object sender, EventArgs eventArgs)
        {
            _reportFraudButton.Visibility = ViewStates.Invisible;
        }

        void DeleteItem()
        {
            AlertDialog.Builder alertConfirm = new AlertDialog.Builder(this);
            alertConfirm.SetCancelable(false);
            alertConfirm.SetPositiveButton("OK", ConfirmDelete);
            alertConfirm.SetNegativeButton("Cancel", delegate { });
            alertConfirm.SetMessage("Tem certeza que quer remover o Item?");
            alertConfirm.Show();
        }

        void ConfirmDelete(object sender, EventArgs e)
        {
            _itemData.Service.DeleteItem(_item.Id);
            Toast toast = Toast.MakeText(this, "Item removido", ToastLength.Short);
            toast.Show();
            Finish();
        }

        void Refresh()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += async (sender, args) =>
            {
                _item = await _itemData.Service.GetItem(Intent.GetStringExtra("itemId"));
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                RunOnUiThread(() =>
                {
                    FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapFragment).GetMapAsync(callback: this);

                    _menu.FindItem(Resource.Id.actionDelete).SetVisible(_item.User.Username == _currentUser.Username);
                    using (Bitmap itemImage = _itemData.GetImageFile(_item.Id, _itemImageView.Width, _itemImageView.Height))
                    {
                        _itemAuthorView.Text = "Lucas";
                        _itemTitleView.Text = _item.Name;
                        _itemDescriptionView.Text = _item.Description;
                        _itemLocationView.Text = _item.Address;
                        _itemImageView.SetImageBitmap(itemImage);
                    }
                });
            };
            worker.RunWorkerAsync();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            LatLng location = new LatLng(_item.Latitude, _item.Longitude);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(18);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            googleMap.AddMarker(new MarkerOptions().SetPosition(location).SetTitle("Ponto de Encontro"));
            googleMap.MoveCamera(cameraUpdate);
        }

    }
}

