using System;
using BotaNaRoda.Android.Data;
using System.IO;

namespace BotaNaRoda.Android
{
	public class ItemData
	{
		public static readonly IItemDataService Service = new ItemJsonService(Path.Combine(
			global::Android.OS.Environment.ExternalStorageDirectory.Path, "BotaNaRoda"));
	}
}

