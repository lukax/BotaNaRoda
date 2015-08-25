using System.IO;
using Android.Graphics;
using Android.OS;
using Path = System.IO.Path;

namespace BotaNaRoda.Ndroid.Data
{
	public class ItemData
	{
	    private readonly string _storagePath;
	    public ItemJsonService Service { get; set; }

	    public ItemData()
	    {
	        _storagePath = Path.Combine(Environment.ExternalStorageDirectory.Path, "BotaNaRoda");
            Service = new ItemJsonService(_storagePath);
	    }

        public string GetLocalImageFileName(string id, int imageNumber)
        {
            return Path.Combine(_storagePath, string.Format("itemImg_{0}_{1}.jpg", id, imageNumber));
        }

        public Bitmap GetImageFile(string itemId, int imageNumber, int width, int height)
		{
			string filename = GetLocalImageFileName(itemId, imageNumber);
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

