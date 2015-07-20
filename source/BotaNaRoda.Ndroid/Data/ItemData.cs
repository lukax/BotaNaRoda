using System.IO;
using Android.Graphics;
using Android.OS;
using Path = System.IO.Path;

namespace BotaNaRoda.Ndroid.Data
{
	public class ItemData
	{
		public static readonly IItemDataService Service = new ItemJsonService(Path.Combine(
			Environment.ExternalStorageDirectory.Path, "BotaNaRoda"));


		public static Bitmap GetImageFile(string itemId)
		{
			string filename = Service.GetImageFileName (itemId);
			if (File.Exists (filename)) {
				Java.IO.File imageFile = new Java.IO.File (filename);
				return BitmapFactory.DecodeFile (imageFile.Path);
			}
		    return null;
		}

	}
}

