using System.Collections.Generic;
using System.Threading.Tasks;
using BotaNaRoda.Ndroid.Models;

namespace BotaNaRoda.Ndroid.Data
{
    public interface IItemService
    {
        Task<IEnumerable<ItemListViewModel>> GetAllItems(double radius, int skip, int limit);
        Task<IEnumerable<ItemListViewModel>> GetAllItems(double lat, double lon, double radius, int skip, int limit);
        void RefreshCache();
        Task<ItemDetailViewModel> GetItem(string id);
        Task<string> SaveItem(ItemCreateBindingModel item);
        Task<bool> DeleteItem(string id);
    }
}