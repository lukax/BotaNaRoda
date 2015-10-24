using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace BotaNaRoda.Ndroid
{
    public class ConversationsFragment : Fragment
    {
        private UserRepository _userRepository;
        private ItemRestService _itemService;
        private ListView _itemsListView;
        private ConversationListAdapter _adapter;
        private IList<ConversationListViewModel> _conversations;
        private SwipeRefreshLayout _conversationsRefreshLayout;
        private TextView _conversationsEmptyText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Conversations, container, false);

            _userRepository = new UserRepository();
            _itemService = new ItemRestService(_userRepository);

            _conversationsEmptyText = view.FindViewById<TextView>(Resource.Id.conversationsEmptyText);
            _conversationsRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.conversationsRefreshLayout);
            _conversationsRefreshLayout.Refresh += (sender, args) => Refresh();

            _itemsListView = view.FindViewById<ListView>(Resource.Id.conversationList);

            _adapter = new ConversationListAdapter(Activity, new List<ConversationListViewModel>());
            _itemsListView.Adapter = _adapter;
            _itemsListView.ItemClick += _itemsListView_ItemClick;


			Refresh();

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        private async void Refresh()
        {
            _conversationsRefreshLayout.Refreshing = true;
            _conversations = await _itemService.GetAllConversations();
            _adapter.Conversations = _conversations;
            _adapter.NotifyDataSetChanged();
            _conversationsEmptyText.Visibility = _adapter.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
            _conversationsRefreshLayout.Refreshing = false;
        }

        private void _itemsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent itemDetailIntent = new Intent(Activity, typeof(ChatActivity));
			itemDetailIntent.PutExtra(ChatActivity.ConversationIdExtra, _conversations[e.Position].Id);
            Activity.StartActivity(itemDetailIntent);
        }


    }
}