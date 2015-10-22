using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using Android.Support.V4.Widget;

namespace BotaNaRoda.Ndroid.Controllers
{
    public class InfiniteScrollListener : RecyclerView.OnScrollListener
    {
        private readonly ItemsAdapter _itemsAdapter;
        private readonly StaggeredGridLayoutManager _staggeredGridLayoutManager;
        private readonly Action _loadMoreItems;
        private readonly SwipeRefreshLayout _refreshLayout;
        private readonly object _scrollLockObject = new object();

        /// <summary>
        /// How many items away from the end of the list before we need to
        /// trigger a load of the next page of items
        /// </summary>
        private const int LoadNextItemsThreshold = 8;

        public InfiniteScrollListener(
          ItemsAdapter itemsAdapter,
          StaggeredGridLayoutManager staggeredGridLayoutManager,
          Action loadMoreItems,
          SwipeRefreshLayout refreshLayout)
        {
            _itemsAdapter = itemsAdapter;
            _staggeredGridLayoutManager = staggeredGridLayoutManager;
            _loadMoreItems = loadMoreItems;
            _refreshLayout = refreshLayout;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            var visibleItemCount = recyclerView.ChildCount;
            var totalItemCount = _itemsAdapter.ItemCount;

            // size of array must be >= the number of items you may have in view at 
            // any one time. Should be set to at least the same value as the 'span'
            // parameter in StaggerGridLayoutManager ctor: i.e. 2 or 3 for phone 
            // in portrait, 4 or 5 for phone in landscape, assume more for a tablet, etc.
            var firstPositions = new int[6] { -1, -1, -1, -1, -1, -1, };
            var firstVisibleItems = _staggeredGridLayoutManager.FindFirstCompletelyVisibleItemPositions(firstPositions);
            int firstPosition = firstVisibleItems.FirstOrDefault(item => item > -1);

            // remember you'll need to handle re-scrolling to last viewed item, 
            // if user flips between landscape/portrait.
            var lastPositions = new int[6] { -1, -1, -1, -1, -1, -1, };
            var lastVisibleItems = _staggeredGridLayoutManager.FindLastCompletelyVisibleItemPositions(lastPositions);
            int lastPosition = lastVisibleItems.LastOrDefault(item => item > -1);


			_refreshLayout.Enabled = firstPosition == 0;
            if (lastPosition == 0)
            {
                return;
            }

            if (totalItemCount - lastPosition <= LoadNextItemsThreshold)
            {
                //TODO code in here to be extracted as callback
                lock (_scrollLockObject)
                {
                    _loadMoreItems();
                }
            }
        }
			
    }
}