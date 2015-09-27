using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.OS;
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
        private BackgroundWorker _refreshWorker;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Conversations, container, false);

            _userRepository = new UserRepository();
            _itemService = new ItemRestService(_userRepository);

            _itemsListView = view.FindViewById<ListView>(Resource.Id.conversationList);
            _adapter = new ConversationListAdapter(Activity, new List<ConversationListViewModel>());
            _itemsListView.Adapter = _adapter;
            _itemsListView.ItemClick += _itemsListView_ItemClick;

            Refresh();

            return view;
        }

        private void Refresh()
        {
            _refreshWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _refreshWorker.DoWork += (sender, args) =>
            {
                _conversations = _itemService.GetAllConversations().Result;
            };
            _refreshWorker.RunWorkerCompleted += (sender, args) =>
            {
                Activity.RunOnUiThread(UpdateUi);    
            };
            _refreshWorker.RunWorkerAsync();
        }

        private void UpdateUi()
        {
            _adapter.Conversations = _conversations;
            _adapter.NotifyDataSetChanged();
        }

        private void _itemsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent itemDetailIntent = new Intent(Activity, typeof(ChatActivity));
            itemDetailIntent.PutExtra("conversationIdId", e.Id);
            Activity.StartActivity(itemDetailIntent);
        }

        public override void OnDestroy()
        {
            _refreshWorker.CancelAsync();
            base.OnDetach();
        }
    }
}