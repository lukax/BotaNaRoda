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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Conversations, container, false);

            _userRepository = new UserRepository();
            _itemService = new ItemRestService(_userRepository);

            _itemsListView = view.FindViewById<ListView>(Resource.Id.conversationList);
            _adapter = new ConversationListAdapter(Activity, new List<ConversationListViewModel>());
            _itemsListView.Adapter = _adapter;
            _itemsListView.ItemClick += _itemsListView_ItemClick;

            LoadMessages();

            return view;
        }

        private void LoadMessages()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                args.Result = _itemService.GetAllConversations().Result;
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    _adapter.Conversations = ((IList<ConversationListViewModel>) args.Result);
                    _adapter.NotifyDataSetChanged();
                });    
            };
            worker.RunWorkerAsync();
        }

        private void _itemsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent itemDetailIntent = new Intent(Activity, typeof(ConversationDetailActivity));
            itemDetailIntent.PutExtra("conversationIdId", e.Id);
            Activity.StartActivity(itemDetailIntent);
        }
    }
}