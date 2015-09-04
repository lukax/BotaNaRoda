using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Database;
using Android.Locations;
using BotaNaRoda.Ndroid.Models;

namespace BotaNaRoda.Ndroid.Data
{
    public class ItemsLoader
    {
        private readonly ItemRestService _itemRestService;
        public int ItemsPerPage { get; set; }
        public List<ItemListViewModel> Items { get; set; }
        public bool CanLoadMoreItems { get; set; }
        public int CurrentPageValue { get; set; }
        public bool IsBusy { get; set; }

        public ItemsLoader(ItemRestService itemRestService, int itemsPerPage)
        {
            _itemRestService = itemRestService;
            ItemsPerPage = itemsPerPage;
            Items = new List<ItemListViewModel>();
            CanLoadMoreItems = true;
            CurrentPageValue = 0;
        }

        public async Task LoadMoreItemsAsync()
        {
            IsBusy = true;

            var loaded = await _itemRestService.GetAllItems(10000, CurrentPageValue, ItemsPerPage);
            var itemListViewModels = loaded as ItemListViewModel[] ?? loaded.ToArray();

            Items.AddRange(itemListViewModels);

            CurrentPageValue = Items.Count;
            CanLoadMoreItems = ItemsPerPage == itemListViewModels.Length;

            IsBusy = false;
        }

    }
}