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
using System.Threading;

namespace BotaNaRoda.Ndroid.Data
{
    public class ItemsLoader 
    {
        public enum Filter
        {
            AllItems,
            MyItemsOnly
        }

        private readonly ItemRestService _itemRestService;
        private readonly Filter _filter;
        private readonly Context _context;
        private readonly UserRepository _userRepository;
        public int ItemsPerPage { get; set; }
		public readonly List<ItemListViewModel> Items;
        public bool CanLoadMoreItems { get; set; }
        public int CurrentPageValue { get; set; }
        private bool IsBusy { get; set; }
        public event Action<ItemListViewModel> OnItemFetched;
        public event Action OnLoaded;

        public ItemsLoader(Context context, UserRepository userRepository, ItemRestService itemRestService, int itemsPerPage,
            Filter filter)
        {
            _itemRestService = itemRestService;
            _filter = filter;
            _context = context;
            _userRepository = userRepository;
            ItemsPerPage = itemsPerPage;
            Items = new List<ItemListViewModel>();
            CanLoadMoreItems = true;
            CurrentPageValue = 0;
        }

        public bool LoadMoreItemsAsync(CancellationToken cancellationToken)
        {
            var cantLoad = IsBusy || !CanLoadMoreItems;
            if (cantLoad) return false;

            Task.Run(() =>
            {
				cancellationToken.ThrowIfCancellationRequested();
                
				IsBusy = true;

                var userInfo = _userRepository.Get();

                IList<ItemListViewModel> loaded;
                if (_filter == Filter.AllItems)
                {
                    loaded = _itemRestService.GetAllItemsAsync(userInfo.Latitude, userInfo.Longitude, 10000, CurrentPageValue, ItemsPerPage).Result;
                }
                else
                {
                    loaded = _itemRestService.GetUserItems(userInfo.Id, userInfo.Latitude, userInfo.Longitude, 10000, CurrentPageValue, ItemsPerPage).Result;
                }
                var itemListViewModels = loaded as ItemListViewModel[] ?? loaded.ToArray();

                itemListViewModels = itemListViewModels.Where(x => Items.All(y => y.Id != x.Id)).ToArray();

                //Items.AddRange(itemListViewModels);

                CanLoadMoreItems = (itemListViewModels.Length != 0 &&
                    ItemsPerPage == itemListViewModels.Length);

				Parallel.ForEach(itemListViewModels.ToList(), (item) =>
                {
					cancellationToken.ThrowIfCancellationRequested();
                    var img = Picasso.With(_context).Load(item.ThumbImage.Url).Get();
                    if (img != null)
                    {
                        item.ThumbImage.Width = img.Width;
                        item.ThumbImage.Height = img.Height;
                        OnItemFetch(item);
                    }
                });

                if (itemListViewModels.Length == 0 && OnLoaded != null)
                {
                    OnLoaded();
                }

                IsBusy = false;
            }, cancellationToken);

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