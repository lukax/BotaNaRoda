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
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using Xamarin.Auth;
using Square.Picasso;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "ItemDetailActivity",
        ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), ParentActivity = typeof(ItemsFragment))]
    public class ItemDetailActivity : Activity, IOnMapReadyCallback
    {
        private static readonly TaskScheduler UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private ItemDetailViewModel _item;

        private IMenu _menu;
        private Account _currentUser;
        private ItemRestService _itemService;
        private ViewHolder _holder;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ItemDetail);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            _currentUser = new UserRepository(this).Get();
            _itemService = new ItemRestService(this, new UserRepository(this));

            _holder = new ViewHolder
            {
                ItemImageView = FindViewById<ImageView>(Resource.Id.itemsDetailImage),
                ItemAuthorView = FindViewById<TextView>(Resource.Id.itemsDetailAuthor),
                ItemTitleView = FindViewById<TextView>(Resource.Id.itemsDetailTitle),
                ItemDescriptionView = FindViewById<TextView>(Resource.Id.itemsDetailDescription),
                ItemLocationView = FindViewById<TextView>(Resource.Id.itemsDetailLocation),
                ReserveButton = FindViewById<Button>(Resource.Id.reserveButton),
                ReportFraudButton = FindViewById<Button>(Resource.Id.reportFraudButton)
            };

            _holder.ReserveButton.Click += ReserveItem;
            _holder.ReportFraudButton.Click += ReportFraudButtonOnClick;

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
            _holder.ReserveButton.Text = "Reserved";
            _holder.ReserveButton.Enabled = false;
        }

        private void ReportFraudButtonOnClick(object sender, EventArgs eventArgs)
        {
            _holder.ReportFraudButton.Visibility = ViewStates.Invisible;
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
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += async (sender, args) =>
            {
                _item = await _itemService.GetItem(Intent.GetStringExtra("itemId"));
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                RunOnUiThread(UpdateUi);
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

        private void UpdateUi()
        {
            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapFragment).GetMapAsync(callback: this);
            _menu.FindItem(Resource.Id.actionDelete).SetVisible(_item.User.Username == _currentUser.Username);

            _holder.ItemAuthorView.Text = _item.User.Username;
            _holder.ItemTitleView.Text = _item.Name;
            _holder.ItemDescriptionView.Text = _item.Description;
            _holder.ItemLocationView.Text = _item.Address;

            Picasso.With(this)
               .Load(_item.Images.First().Url)
               .Fit()
               .Tag(this)
               .Into(_holder.ItemImageView);
        }

        private class ViewHolder
        {
            internal ImageView ItemImageView;
            internal TextView ItemAuthorView;
            internal TextView ItemDescriptionView;
            internal TextView ItemTitleView;
            internal TextView ItemLocationView;
            internal Button ReserveButton;
            internal Button ReportFraudButton;
        }
    }
}

