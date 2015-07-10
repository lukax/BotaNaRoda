using System.Collections.Generic;
using BotaNaRoda.Ndroid.Entity;

namespace BotaNaRoda.Ndroid.Data
{
    public interface IItemDataService
    {
		IReadOnlyList<Item> GetAllItems();
        void RefreshCache();
        Item GetItem(string id);
        void SaveItem(Item item);
        void DeleteItem(Item item);
		string GetImageFileName(string id);
    }
}