﻿using System;
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
using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.View;
using BotaNaRoda.Ndroid.Library;

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
                ViewPager = FindViewById<ViewPager>(Resource.Id.itemsDetailViewPager),
                ViewPagerIndicator = FindViewById<CirclePageIndicator>(Resource.Id.indicator),
                ItemAuthorNameView = FindViewById<TextView>(Resource.Id.itemsDetailAuthorName),
                ItemAuthorImageView = FindViewById<ImageView>(Resource.Id.itemsDetailAuthorImage),
                ItemDescriptionView = FindViewById<TextView>(Resource.Id.itemsDetailDescription),
                ItemLocationView = FindViewById<TextView>(Resource.Id.itemsDetailLocation),
                ReserveButton = FindViewById<Button>(Resource.Id.reserveButton),
                DistanceView = FindViewById<TextView>(Resource.Id.itemsDetailDistance),
				SubscribersListView = FindViewById<ListView>(Resource.Id.subscribers),
				SubscribersLayout = FindViewById<LinearLayout>(Resource.Id.subscribersLayout)
            };

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

            Title = _item.Name;
            _holder.ItemAuthorNameView.Text = _item.User.Name;
            _holder.ItemDescriptionView.Text = _item.Description;
            _holder.ItemLocationView.Text = _item.Locality;
            _holder.DistanceView.Text = _item.DistanceTo(_userRepository.Get());
            _holder.ViewPager.Adapter = new ItemImagePagerAdapter(this, _item.Images.Select(x => x.Url).ToArray(), SupportFragmentManager);
            _holder.ViewPagerIndicator.SetViewPager(_holder.ViewPager);
            _holder.ViewPagerIndicator.SetSnap(true);
            _holder.ItemAuthorImageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            _holder.ItemAuthorImageView.Post(() =>
            {
                Picasso.With(this)
                .Load(_item.User.Avatar)
                .Tag(this)
                .Into(_holder.ItemAuthorImageView);
            });
            
			if (_userRepository.IsLoggedIn) {
				if (_item.User.Username == _userRepository.Get ().Username) {
					if (_menu != null) {
						_menu.FindItem (Resource.Id.actionDelete).SetVisible (_item.User.Username == _userRepository.Get ().Username);
					}
				} 
				if (!_item.IsSubscribed) {
					_holder.ReserveButton.Visibility = ViewStates.Visible;
					_holder.ReserveButton.Text = "Reservar";
					_holder.ReserveButton.Click += Subscribe;
				} else {
					_holder.ReserveButton.Visibility = ViewStates.Visible;
					_holder.ReserveButton.Text = "Reservado";
				}
			}
			if (_item.Subscribers != null && _item.Subscribers.Count > 0) {
				_holder.SubscribersLayout.Visibility = ViewStates.Visible;
				_holder.SubscribersListView.Adapter = new ItemDetailSubscribersAdapter (this, _item.Subscribers);
				_holder.SubscribersListView.ItemClick += _holder_SubscribersListView_ItemClick;
			}
        }

		async void Subscribe(object sender, EventArgs e)
		{
			var result = await _itemService.Subscribe (_item.Id);
			RunOnUiThread(() =>
				{
					if(result){
						_holder.ReserveButton.Text = "Reservado";
						_holder.ReserveButton.Enabled = false;
					}
					else{
						Toast.MakeText(this, "Não foi possível reservar o produto.", ToastLength.Short);				
					}
				});
		}

		async void _holder_SubscribersListView_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			var conversationId = await _itemService.Promise (_item.Id, _item.Subscribers [e.Position].Id);

			Intent chatIntent = new Intent(this, typeof(ChatActivity));
			chatIntent.PutExtra("conversationId", conversationId);
			StartActivity(chatIntent);
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
            internal ViewPager ViewPager;
            internal TextView ItemAuthorNameView;
            internal TextView ItemDescriptionView;
            internal TextView ItemLocationView;
            internal Button ReserveButton;
            internal TextView DistanceView;
            internal ImageView ItemAuthorImageView;
			internal ListView SubscribersListView;
			internal LinearLayout SubscribersLayout;
            internal CirclePageIndicator ViewPagerIndicator;
        }
    }

}

