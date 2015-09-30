using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using BotaNaRoda.Ndroid.Util;
using Xamarin.Auth;
using Square.Picasso;
using AlertDialog = Android.App.AlertDialog;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Bota na Roda",
        Theme = "@style/ItemDetailTheme", ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), 
        ParentActivity = typeof(MainActivity))]
    public class ItemDetailActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private static readonly TaskScheduler UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private ItemDetailViewModel _item;

        private IMenu _menu;
        private ItemRestService _itemService;
        private UserRepository _userRepository;
        private ViewHolder _holder;
        private BackgroundWorker _refreshWorker;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ItemDetail);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _userRepository = new UserRepository();
            _itemService = new ItemRestService(new UserRepository());

            _holder = new ViewHolder
            {
                ItemImageView = FindViewById<ImageView>(Resource.Id.itemsDetailImage),
                ItemAuthorNameView = FindViewById<TextView>(Resource.Id.itemsDetailAuthorName),
                ItemAuthorImageView = FindViewById<ImageView>(Resource.Id.itemsDetailAuthorImage),
                ItemDescriptionView = FindViewById<TextView>(Resource.Id.itemsDetailDescription),
                ItemLocationView = FindViewById<TextView>(Resource.Id.itemsDetailLocation),
                ReserveButton = FindViewById<Button>(Resource.Id.reserveButton),
                DistanceView = FindViewById<TextView>(Resource.Id.itemsDetailDistance),
            };

            _holder.ReserveButton.Click += ReserveItem;

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

        void ReserveItem(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                _holder.ReserveButton.Text = "Reservado";
                _holder.ReserveButton.Enabled = false;
            });
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
            _itemService.DeleteItem(_item.Id);
            Toast toast = Toast.MakeText(this, "Item removido", ToastLength.Short);
            toast.Show();
            Finish();
        }

        void Refresh()
        {
            _refreshWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
            _refreshWorker.DoWork += (sender, args) =>
            {
                _item = _itemService.GetItem(Intent.GetStringExtra("itemId")).Result;
            };
            _refreshWorker.RunWorkerCompleted += (sender, args) =>
            {
                RunOnUiThread(UpdateUi);
            };
            _refreshWorker.RunWorkerAsync();
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

        private void UpdateUi()
        {
            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapFragment).GetMapAsync(callback: this);
            if (_userRepository.IsLoggedIn && _menu != null)
            {
                _menu.FindItem(Resource.Id.actionDelete).SetVisible(_item.User.Username == _userRepository.Get().Username);
            }

            Title = _item.Name;
            _holder.ItemAuthorNameView.Text = _item.User.Name;
            _holder.ItemDescriptionView.Text = _item.Description;
            _holder.ItemLocationView.Text = _item.Locality;
            _holder.DistanceView.Text = _item.DistanceTo(_userRepository.Get());
            Picasso.With(this)
                .Load(_item.User.Avatar)
                .Resize(_holder.ItemAuthorImageView.Width, _holder.ItemAuthorImageView.Height)
                .CenterCrop()
                .Tag(this)
                .Into(_holder.ItemAuthorImageView);
            Picasso.With(this)
               .Load(_item.Images.First().Url)
               .Resize(_holder.ItemImageView.Width, _holder.ItemImageView.Height)
               .CenterCrop()
               .Tag(this)
               .Into(_holder.ItemImageView);
        }

        protected override void OnDestroy()
        {
			if (_refreshWorker != null) {
				_refreshWorker.CancelAsync();
			}
            base.OnDestroy();
        }

        private class ViewHolder
        {
            internal ImageView ItemImageView;
            internal TextView ItemAuthorNameView;
            internal TextView ItemDescriptionView;
            internal TextView ItemLocationView;
            internal Button ReserveButton;
            internal TextView DistanceView;
            internal ImageView ItemAuthorImageView;
        }
    }
}

