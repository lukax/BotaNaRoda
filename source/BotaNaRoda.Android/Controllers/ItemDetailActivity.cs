﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Android.Entity;
using Android.Content.PM;
using Android.Graphics;

namespace BotaNaRoda.Android
{
	[Activity (Label = "ItemDetailActivity",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]			
	public class ItemDetailActivity : Activity
	{
		ImageView _itemImageView;
		TextView _itemAuthorView;
		TextView _itemDescriptionView;
		Item _item;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.ItemDetail);

			// Create your application here

			_itemImageView = FindViewById<ImageView> (Resource.Id.itemsDetailImage);
			_itemAuthorView = FindViewById<TextView> (Resource.Id.itemsDetailAuthor);
			_itemDescriptionView = FindViewById<TextView> (Resource.Id.itemsDetailDescription);

			_item = new Item ();
			if (Intent.HasExtra ("itemId")) {
				_item = ItemData.Service.GetAllItems () [Intent.GetIntExtra ("itemId", -1)];
				using (Bitmap itemImage = ItemData.GetImageFile (_item.Id)) {
					_itemImageView.SetImageBitmap (itemImage);
				}
				UpdateUI ();
			}
		}

		void UpdateUI()
		{
			_itemAuthorView.Text = "Me";
			_itemDescriptionView.Text = _item.Description;
		}
	}
}
