using System;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Entity;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "ItemDetailActivity",
        ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]
    public class ItemDetailActivity : Activity
    {
        ImageView _itemImageView;
        TextView _itemAuthorView;
        TextView _itemDescriptionView;
        Item _item = new Item();
        private GoogleMap mMap;
        private SwipeRefreshLayout _refresher;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ItemDetail);

            // Create your application here

            _itemImageView = FindViewById<ImageView>(Resource.Id.itemsDetailImage);
            _itemAuthorView = FindViewById<TextView>(Resource.Id.itemsDetailAuthor);
            _itemDescriptionView = FindViewById<TextView>(Resource.Id.itemsDetailDescription);
            _refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            _refresher.Refresh += delegate
            {
                Refresh();
            };

            Refresh();
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetUpMapIfNeeded();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ItemDetailMenu, menu);
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
            ItemData.Service.DeleteItem(_item);
            Toast toast = Toast.MakeText(this, "Item removido", ToastLength.Short);
            toast.Show();
            Finish();
        }

        void Refresh()
        {
            _refresher.Refreshing = true;
            _item = ItemData.Service.GetAllItems()[Intent.GetIntExtra("itemId", -1)];
			using (Bitmap itemImage = ItemData.GetImageFile(_item.Id, _itemImageView.Width, _itemImageView.Height))
            {
                _itemImageView.SetImageBitmap(itemImage);
            }
            UpdateUi();
            _refresher.Refreshing = false;
        }

        void UpdateUi()
        {
            _itemAuthorView.Text = "Me";
            _itemDescriptionView.Text = _item.Description;
            SetUpMapIfNeeded();
        }

        /**
        * Sets up the map if it is possible to do so (i.e., the Google Play services APK is correctly
        * installed) and the map has not already been instantiated.. This will ensure that we only ever
        * call {@link #setUpMap()} once when {@link #mMap} is not null.
        * <p>
        * If it isn't installed {@link SupportMapFragment} (and
        * {@link com.google.android.gms.maps.MapView
        * MapView}) will show a prompt for the user to install/update the Google Play services APK on
        * their device.
        * <p>
        * A user can return to this Activity after following the prompt and correctly
        * installing/updating/enabling the Google Play services. Since the Activity may not have been
        * completely destroyed during this process (it is likely that it would only be stopped or
        * paused), {@link #onCreate(Bundle)} may not be called again so we should call this method in
        * {@link #onResume()} to guarantee that it will be called.
        */
        private void SetUpMapIfNeeded()
        {
            // Do a null check to confirm that we have not already instantiated the map.
            if (mMap == null)
            {
                // Try to obtain the map from the SupportMapFragment.
                mMap = (FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map)).Map;
                // Check if we were successful in obtaining the map.
                if (mMap != null)
                {
                    SetUpMap();
                }
            }
        }

        /**
        * This is where we can add markers or lines, add listeners or move the camera. In this case, we
        * just add a marker near Africa.
        * <p>
        * This should only be called once and when we are sure that {@link #mMap} is not null.
        */
        private void SetUpMap()
        {
            LatLng location = new LatLng(_item.Latitude.GetValueOrDefault(), _item.Longitude.GetValueOrDefault());
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(18);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            mMap.AddMarker(new MarkerOptions().SetPosition(location).SetTitle("Ponto de Encontro"));
            mMap.MoveCamera(cameraUpdate);
        }

    }
}

