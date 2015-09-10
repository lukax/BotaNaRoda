using System;
using System.ComponentModel;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using Square.Picasso;

namespace BotaNaRoda.Ndroid
{
	public class MyActionBarDrawerToggle : SupportActionBarDrawerToggle
	{
	    private readonly UserRepository _userRepository;
	    private readonly TextView _userAddressTextView;
        private readonly TextView _userNameTextView;
        private readonly ImageView _pictureImageView;
	    private readonly AppCompatActivity _host;

	    public MyActionBarDrawerToggle (AppCompatActivity host,
                DrawerLayout drawerLayout, int openedResource, int closedResource,
                UserRepository userRepository) 
			: base(host, drawerLayout, openedResource, closedResource)
        {
            _host = host;
            _userRepository = userRepository;
            //Profile
		    _pictureImageView = host.FindViewById<ImageView>(Resource.Id.avatar);
            _userNameTextView = host.FindViewById<TextView>(Resource.Id.userName);
            _userAddressTextView = host.FindViewById<TextView>(Resource.Id.userLocality);
        }

		public override void OnDrawerOpened (Android.Views.View drawerView)
		{	
			base.OnDrawerOpened (drawerView);

            GetUserInfo();
		}

		public override void OnDrawerClosed (Android.Views.View drawerView)
		{
			base.OnDrawerClosed (drawerView);			
		}

		public override void OnDrawerSlide (Android.Views.View drawerView, float slideOffset)
		{
			base.OnDrawerSlide (drawerView, slideOffset);
		}

        private void GetUserInfo()
        {
            if (_userRepository.IsLoggedIn)
            {
                var currentUsr = _userRepository.Get();

                _userNameTextView.Text = currentUsr.Name;
                _userAddressTextView.Text = currentUsr.Address;
                _pictureImageView.Post(() =>
                {
                    Picasso.With(_host)
                        .Load(currentUsr.Picture)
                        .Fit()
                        .Tag(this)
                        .Into(_pictureImageView);
                });
            }
        }
    }
}

