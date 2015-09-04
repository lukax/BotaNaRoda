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

        public void LoadMoreItems(Action onLoaded = null)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                IsBusy = true;

                var loaded = _itemRestService.GetAllItems(10000, CurrentPageValue, ItemsPerPage).Result;
                var itemListViewModels = loaded as ItemListViewModel[] ?? loaded.ToArray();

                Items.AddRange(itemListViewModels);

                CurrentPageValue = Items.Count;
                CanLoadMoreItems = ItemsPerPage == itemListViewModels.Length;
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                IsBusy = false;

                if (onLoaded != null)
                {
                    onLoaded();
                }
            };
            worker.RunWorkerAsync();
        }

    }
}