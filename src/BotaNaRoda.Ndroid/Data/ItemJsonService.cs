using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.Ndroid.Models;
using Newtonsoft.Json;

namespace BotaNaRoda.Ndroid.Data
{
    public class ItemJsonService : IItemService
    {
        private readonly string _storagePath;
        private readonly List<ItemDetailViewModel> _items = new List<ItemDetailViewModel>();

        public ItemJsonService(string storagePath)
        {
            _storagePath = storagePath;

            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);

            RefreshCache();
        }

		public Task<IEnumerable<ItemListViewModel>> GetAllItems(double lat, double lon)
        {
            return Task.Run(() => _items.Select(x => new ItemListViewModel
            {
                CreatedAt = x.CreatedAt,
                Id = x.Id,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name,
                Status = x.Status,
                ThumbImage = x.ThumbImage
            }));
        }

        public void RefreshCache()
        {
            _items.Clear();
            var fileNames = Directory.GetFiles(_storagePath, "*.json");

            foreach (var fileName in fileNames)
            {
                string itemString = File.ReadAllText(fileName);
				try{
					ItemDetailViewModel item = JsonConvert.DeserializeObject<ItemDetailViewModel>(itemString);
					_items.Add(item);
				}
				catch{
					Console.WriteLine ("Error loading item json");
				}
            }
        }

        public Task<ItemDetailViewModel> GetItem(string id)
        {
            return Task.Run(() => _items.FirstOrDefault(x => x.Id == id));
        }

        public Task<string> SaveItem(ItemCreateBindingModel item)
        {
            var theItem = new ItemDetailViewModel
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Longitude = item.Longitude,
                Latitude = item.Latitude,
                Address = item.Address,
                Category = item.Category,
                Description = item.Description,
                Images = item.Images,
                Name = item.Name
            };

            var itemString = JsonConvert.SerializeObject(theItem);
            File.WriteAllText(GetFilename(theItem.Id), itemString);

            _items.Add(theItem);

            return Task.Run(() => theItem.Id);
        }

        public Task<bool> DeleteItem(string id)
        {
            if (File.Exists(GetFilename(id)))
            {
                File.Delete(GetFilename(id));
            }
            if (File.Exists(GetImageFileName(id)))
            {
                File.Delete(GetImageFileName(id));
            }
            _items.Remove(_items.FirstOrDefault(x => x.Id == id));

            return Task.Run(() => true);
        }

        private string GetFilename(string id)
        {
			return Path.Combine(_storagePath, string.Format("item_{0}.json", id));
        }

        public string GetImageFileName(string id)
        {
            return Path.Combine(_storagePath, string.Format("itemImg_{0}_{1}.jpg", id, 0));
        }
    }
}