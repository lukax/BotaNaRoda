using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Android.Entity;
using Newtonsoft.Json;

namespace BotaNaRoda.Android.Data
{
	public class ItemJsonService : IItemDataService
    {
        private readonly string _storagePath;
        private readonly List<Item> _items = new List<Item>();
 
        public ItemJsonService(string storagePath)
        {
            _storagePath = storagePath;

            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);

            RefreshCache();
        }

		public IReadOnlyList<Item> GetAllItems()
        {
            return _items;
        }

        public void RefreshCache()
        {
            _items.Clear();
            var fileNames = Directory.GetFiles(_storagePath, "*.json");

            foreach (var fileName in fileNames)
            {
                string itemString = File.ReadAllText(fileName);
                Item item = JsonConvert.DeserializeObject<Item>(itemString);
                _items.Add(item);
            }
        }

        public Item GetItem(string id)
        {
			return _items.FirstOrDefault(x => x.Id == id);
        }

        public void SaveItem(Item item)
        {
            bool newItem = item.Id == null;
            if (newItem)
            {
                item.Id = Guid.NewGuid().ToString();
            }
            item.PostDate = DateTime.UtcNow;

            var itemString = JsonConvert.SerializeObject(item);
            File.WriteAllText(GetFilename(item.Id), itemString);
        
            if(newItem)
                _items.Add(item);
        }

        public void DeleteItem(Item item)
        {
			if(File.Exists(GetFilename(item.Id))){
				File.Delete(GetFilename(item.Id));
			}
			if (File.Exists (GetImageFileName (item.Id))) {
				File.Delete (GetImageFileName (item.Id));
			}

			_items.Remove(item);
        }

        private string GetFilename(string id)
        {
            return Path.Combine(_storagePath, string.Format("item{0}.json", id));
        }

		public string GetImageFileName (string id)
		{
			return Path.Combine (_storagePath, string.Format("itemImage{0}.jpg", id));
		}
    }
}