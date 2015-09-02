using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Locations;
using BotaNaRoda.Ndroid.Models;

namespace BotaNaRoda.Ndroid.Data
{
    public class ItemsLoader
    {
        private readonly ItemRestService _itemRestService;
        private const int ItemsPerPage = 20;
        public List<ItemListViewModel> Items { get; set; }
        public bool CanLoadMoreItems { get; set; }
        public int CurrentPageValue { get; set; }
        public bool IsBusy { get; set; }

        public ItemsLoader(ItemRestService itemRestService)
        {
            _itemRestService = itemRestService;
            Items = new List<ItemListViewModel>();
        }

        public void LoadMoreItems(int itemsPerPagel)
        {
            IsBusy = true;

            Task.Run(() =>
            {
                var loaded = _itemRestService.GetAllItems(10000, CurrentPageValue, ItemsPerPage).Result;
                var itemListViewModels = loaded as ItemListViewModel[] ?? loaded.ToArray();

                CanLoadMoreItems = itemListViewModels.Length >= CurrentPageValue;

                Items.AddRange(itemListViewModels);

                CurrentPageValue = Items.Count;
                IsBusy = false;
            });
        }

    }
}