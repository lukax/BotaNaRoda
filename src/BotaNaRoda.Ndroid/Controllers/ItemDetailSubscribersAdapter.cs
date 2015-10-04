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

namespace BotaNaRoda.Ndroid.Controllers
{

	public class ItemDetailSubscribersAdapter : BaseAdapter<UserViewModel>
	{
		private readonly Activity _context;
		public IList<UserViewModel> Users { get; set; }

		public ItemDetailSubscribersAdapter(Activity context, IList<UserViewModel> users)
		{
			_context = context;
			Users = users;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ConversationListItem, null);

			var user = this[position];

			view.FindViewById<TextView>(Resource.Id.conversationUserName).Text = user.Name;
			view.FindViewById<TextView>(Resource.Id.conversationItemName).Text = user.Locality;
			view.FindViewById<TextView>(Resource.Id.conversationLastMessageTime).Text = "";

			var imageView = view.FindViewById<ImageView>(Resource.Id.conversationProfileImage);
			Picasso.With(_context)
				.Load(user.Avatar)
				.Fit()
				.Tag(_context)
				.Into(imageView);

			return view;
		}

		public override int Count
		{
			get { return Users.Count; }
		}

		public override UserViewModel this[int index]
		{
			get { return Users[index]; }
		}

	}
}
