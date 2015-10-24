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
using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using BotaNaRoda.Ndroid.Library;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Bota na Roda",
        Theme = "@style/MainTheme", 
        ParentActivity = typeof(MainActivity), LaunchMode = LaunchMode.SingleTop)]
    public class UserDetailActivity : AppCompatActivity
    {
        public const string UserIdExtra = "userId";
        private string _userId;
        private IMenu _menu;
        private ItemRestService _itemService;
        private UserRepository _userRepository;
        private ViewHolder _holder;
        private UserViewModel _user;
        private ItemsAdapter _userItemsAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.UserDetail);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _userId = bundle != null ? bundle.GetString(UserIdExtra) : Intent.GetStringExtra(UserIdExtra);

            _userRepository = new UserRepository();
            _itemService = new ItemRestService(new UserRepository());

            _holder = new ViewHolder
            {
                UserDetailImage = FindViewById<ImageView>(Resource.Id.userDetailImage),
                UserDetailName = FindViewById<TextView>(Resource.Id.userDetailName),
                UserDetaiLocation = FindViewById<TextView>(Resource.Id.userDetailLocation),
                UserDetailDonations = FindViewById<TextView>(Resource.Id.userDetailDonations),
                UserDetailItemsGrid = FindViewById<RecyclerView>(Resource.Id.userDetailItemsGrid),
            };

            _holder.UserDetailItemsGrid.HasFixedSize = false;
            var sglm = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
            _holder.UserDetailItemsGrid.SetLayoutManager(sglm);

            _userItemsAdapter = new ItemsAdapter(this, _userRepository.Get());
            _holder.UserDetailItemsGrid.SetAdapter(_userItemsAdapter);

            Refresh();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(UserIdExtra, _userId);
            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.ItemDetailMenu, menu);
            //_menu = menu;
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        
		private async void Refresh()
		{
			_user = await _itemService.GetUserProfileAsync(_userId);

		    var authInfo = _userRepository.Get();
		    _userItemsAdapter.Items.Clear();
            _userItemsAdapter.Items.AddRange(await _itemService.GetUserItems(_userId, authInfo.Latitude, authInfo.Longitude, 0, 0, 20));

			UpdateUi();
		}

        private void UpdateUi()
        {
            _holder.UserDetailName.Text = _user.Name;
            _holder.UserDetaiLocation.Text = _user.Locality ?? "Brasil";
            _holder.UserDetailDonations.Text =  _user.DonationsCount.ToString() + " doações";

            Picasso.With(this).Load(_user.Avatar).Into(_holder.UserDetailImage);

            _userItemsAdapter.NotifyDataSetChanged();
        }
        
        private class ViewHolder
        {
            internal ImageView UserDetailImage;
            internal TextView UserDetailName;
            internal TextView UserDetaiLocation;
            internal TextView UserDetailDonations;
            internal RecyclerView UserDetailItemsGrid;
        }
    }
}

