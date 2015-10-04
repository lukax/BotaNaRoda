
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
using Android.Database;

namespace BotaNaRoda.Ndroid
{
	public class ImageUtil
	{
		public static string GetPathToImage(Context context, Android.Net.Uri uri)
		{
			string path = null;
			// The projection contains the columns we want to return in our query.
			string[] projection = new[] { 
				Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data
			};
			using (ICursor cursor = Application.Context.ContentResolver.Query(uri, projection, null, null, null))
			{
				if (cursor != null && cursor.MoveToFirst())
				{
					int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
					path = cursor.GetString(columnIndex);
				}
			}
			return path;
		}
	}
}

