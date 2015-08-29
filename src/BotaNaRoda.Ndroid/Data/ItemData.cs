using System.IO;
using Android.Graphics;
using Android.OS;
using Path = System.IO.Path;
using System;
using Android.Content;
using BotaNaRoda.Ndroid.Controllers;
using Xamarin.Auth;

namespace BotaNaRoda.Ndroid.Data
{
	public class ItemData
	{
	    private readonly string _storagePath;
	    public ItemRestService Service { get; set; }

	    public ItemData(Context context)
	    {
	        _storagePath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "BotaNaRoda");
            Service = new ItemRestService(context, _storagePath, new UserService(context));
	    }

        public string GetTempImageFilename(int imageNumber)
        {
            return Path.Combine(_storagePath, string.Format("itemImg_{0}.jpg", imageNumber));
        }

		[Obsolete]
        public Bitmap GetImageFile(int imageNumber, int width, int height)
		{
			string filename = GetTempImageFilename(imageNumber);
			var img = new Java.IO.File (filename);
			if (img.Exists()) {
				return LoadAndResizeBitmap (img.Path, width, height);
			}
		    return null;
		}

		private Bitmap LoadAndResizeBitmap(string fileName, int width, int height)
		{
			// First we get the the dimensions of the file on disk
			BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile(fileName, options);

			// Next we calculate the ratio that we need to resize the image by
			// in order to fit the requested dimensions.
			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;

			if (width > 0 && height > 0) {
				if (outHeight > height || outWidth > width)
				{
					inSampleSize = outWidth > outHeight
						? outHeight / height
						: outWidth / width;
				}
			}

			// Now we will load the image and have BitmapFactory resize it for us.
			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

			return resizedBitmap;
		}

	}
}

