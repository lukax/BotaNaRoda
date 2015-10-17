using System.Collections.Generic;
using System.Threading.Tasks;
using BotaNaRoda.Ndroid.Models;

namespace BotaNaRoda.Ndroid.Data
{
    public interface IItemService
    {
        Task<IList<ItemListViewModel>> GetAllItemsAsync(double radius, int skip, int limit);
        Task<IList<ItemListViewModel>> GetAllItemsAsync(double lat, double lon, double radius, int skip, int limit);
        void RefreshCache();
        Task<ItemDetailViewModel> GetItem(string id);
        Task<string> PostItem(ItemCreateBindingModel item);
        Task<bool> DeleteItem(string id);
    }
}