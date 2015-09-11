using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
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
            _itemService = new ItemRestService(Activity, _userRepository);

            _itemsListView = view.FindViewById<ListView>(Resource.Id.conversationList);
            _adapter = new ConversationListAdapter(Activity, new List<ConversationViewModel>());
            _itemsListView.Adapter = _adapter;
            _itemsListView.ItemClick += _itemsListView_ItemClick;

            return view;
        }

        private void _itemsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            
        }
    }
}