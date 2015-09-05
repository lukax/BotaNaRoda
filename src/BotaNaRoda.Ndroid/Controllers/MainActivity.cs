
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.Widget;
using BotaNaRoda.Ndroid.Controllers;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using Square.Picasso;
using Fragment = Android.Support.V4.App.Fragment;

namespace BotaNaRoda.Ndroid
{
	[Activity (Label = "Bota na Roda", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MainTheme")]
    public class MainActivity : AppCompatActivity, AdapterView.IOnItemClickListener
	{
        private DrawerLayout _mDrawerLayout;
        private ListView _mLeftDrawer;
        private Dictionary<string, Type> _mLeftDataSet;
        private ArrayAdapter<string> _mLeftAdapter;
		private MyActionBarDrawerToggle _mDrawerToggle;
	    private TextView _userLocalityTextView;
	    private TextView _userNameTextView;
	    private ImageView _avatarImageView;
	    private Fragment _currentFragment;

	    protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			_mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			_mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled(true);

	        _mLeftDataSet = new Dictionary<string, Type>
	        {
	            {"Itens próximos a mim", typeof (ItemsFragment)},
	            {"Conversas", typeof (ConversationsFragment)}
	        };
	        _mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _mLeftDataSet.Keys.ToArray());
			_mLeftDrawer.Adapter = _mLeftAdapter;
            _mLeftDrawer.OnItemClickListener = this;

			_mDrawerToggle = new MyActionBarDrawerToggle(
				this,								//Host Activity
				_mDrawerLayout,						//DrawerLayout
				Resource.String.ApplicationName,	//Opened Message
				Resource.String.ApplicationName		//Closed Message
			);

			_mDrawerLayout.SetDrawerListener(_mDrawerToggle);
			_mDrawerToggle.SyncState();

            //Profile
            _avatarImageView = FindViewById<ImageView>(Resource.Id.avatar);
            _userNameTextView = FindViewById<TextView>(Resource.Id.userName);
            _userLocalityTextView = FindViewById<TextView>(Resource.Id.userLocality);

            //Container
            LoadFragment(_mLeftDataSet.First().Value);
		}

	    protected override void OnStart()
	    {
	        base.OnStart();
            GetUserInfo();
	    }

	    public override bool OnOptionsItemSelected (IMenuItem item)
		{		
			switch (item.ItemId)
			{
			    case Android.Resource.Id.Home:
				    //The hamburger icon was clicked which means the drawer toggle will handle the event
				    //all we need to do is ensure the right drawer is closed so the don't overlap
				    _mDrawerToggle.OnOptionsItemSelected(item);
				    return true;
			    default:
				    return base.OnOptionsItemSelected (item);
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.ItemsMenu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			_mDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			_mDrawerToggle.OnConfigurationChanged(newConfig);
		}

	    private void GetUserInfo()
	    {
	        ItemRestService restService = new ItemRestService(this, new UserRepository(this));
            BackgroundWorker worker = new BackgroundWorker();
	        worker.DoWork += (sender, args) =>
	        {
                UserViewModel profile = restService.GetUserProfileAsync().Result;
	            args.Result = profile;
	        };
	        worker.RunWorkerCompleted += (sender, args) =>
	        {
	            var profile = args.Result as UserViewModel;
	            if (profile != null)
	            {
                    RunOnUiThread(() =>
                    {
                        _userNameTextView.Text = profile.Username;
                        _userLocalityTextView.Text = profile.Locality;
                        Picasso.With(this)
                           .Load(profile.Avatar)
                           .Fit()
                           .Tag(this)
                           .Into(_avatarImageView);
                    });
                }
	        };
            worker.RunWorkerAsync();
	    }

        private void LoadFragment(Type value)
        {
            var tx = SupportFragmentManager.BeginTransaction();
            if (_currentFragment == null)
            {
                _currentFragment = (Fragment) Activator.CreateInstance(value);
                tx.Add(Resource.Id.container, _currentFragment, value.Name);
            }
            else
            {
                _currentFragment = (Fragment) Activator.CreateInstance(value);
                tx.Replace(Resource.Id.container, _currentFragment, value.Name);
            }
            tx.Commit();
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
	    {
            _mDrawerLayout.CloseDrawers();
            LoadFragment(_mLeftDataSet.ElementAt(position).Value);
	    }
	}
}

