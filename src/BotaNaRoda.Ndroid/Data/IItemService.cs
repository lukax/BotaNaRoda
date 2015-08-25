using System.Collections.Generic;
using System.Threading.Tasks;
using BotaNaRoda.Ndroid.Models;

namespace BotaNaRoda.Ndroid.Data
{
    public interface IItemService
    {
		Task<IEnumerable<ItemListViewModel>> GetAllItems();
        void RefreshCache();
        Task<ItemDetailViewModel> GetItem(string id);
        Task<string> SaveItem(ItemCreateBindingModel item);
        Task<bool> DeleteItem(string id);
    }
}