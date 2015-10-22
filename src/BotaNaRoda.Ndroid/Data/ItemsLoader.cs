using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Database;
using Android.Locations;
using BotaNaRoda.Ndroid.Models;
using Square.Picasso;

namespace BotaNaRoda.Ndroid.Data
{
    public class ItemsLoader 
    {
        private readonly ItemRestService _itemRestService;
        private readonly Context _context;
        public int ItemsPerPage { get; set; }
		public readonly List<ItemListViewModel> Items;
        public bool CanLoadMoreItems { get; set; }
        public int CurrentPageValue { get; set; }
        private bool IsBusy { get; set; }
        public event Action<ItemListViewModel> OnItemFetched;

        public ItemsLoader(Context context, ItemRestService itemRestService, int itemsPerPage)
        {
            _itemRestService = itemRestService;
            _context = context;
            ItemsPerPage = itemsPerPage;
            Items = new List<ItemListViewModel>();
            CanLoadMoreItems = true;
            CurrentPageValue = 0;
        }

        public async Task<bool> LoadMoreItemsAsync()
        {
            var cantLoad = IsBusy || !CanLoadMoreItems;
            if (cantLoad) return false;

            await Task.Run(() =>
            {
                IsBusy = true;

                var loaded = _itemRestService.GetAllItemsAsync(10000, CurrentPageValue, ItemsPerPage).Result;
                var itemListViewModels = loaded as ItemListViewModel[] ?? loaded.ToArray();

                itemListViewModels = itemListViewModels.Where(x => Items.All(y => y.Id != x.Id)).ToArray();

                //Items.AddRange(itemListViewModels);

                CanLoadMoreItems = (itemListViewModels.Length != 0 &&
                    ItemsPerPage == itemListViewModels.Length);

                Parallel.ForEach(itemListViewModels, (item) =>
                {
                    var img = Picasso.With(_context).Load(item.ThumbImage.Url).Get();
                    if (img != null)
                    {
                        item.ThumbImage.Width = img.Width;
                        item.ThumbImage.Height = img.Height;
                        OnItemFetch(item);
                    }
                });

                IsBusy = false;
            });

            return true;
        }

        private void OnItemFetch(ItemListViewModel item)
        {
            if (Items.All(x => x.Id != item.Id))
            {
                Items.Add(item);
                CurrentPageValue = Items.Count;
                if (OnItemFetched != null)
                {
                    OnItemFetched(item);
                }
            }
        }

    }

}