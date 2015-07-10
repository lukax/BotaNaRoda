using System.IO;
using Android.Graphics;

namespace BotaNaRoda.Ndroid.Data
{
	public class ItemData
	{
		public static readonly IItemDataService Service = new ItemJsonService(System.IO.Path.Combine(
			global::Android.OS.Environment.ExternalStorageDirectory.Path, "BotaNaRoda"));


		public static Bitmap GetImageFile(string poiId)
		{
			string filename = Service.GetImageFileName (poiId);
			if (File.Exists (filename)) {
				Java.IO.File imageFile = new Java.IO.File (filename);
				return BitmapFactory.DecodeFile (imageFile.Path);
			}
			else
				return null;
		}

	}
}

