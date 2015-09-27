using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Models;
using Square.Picasso;

namespace BotaNaRoda.Ndroid
{
    public class ConversationListAdapter : BaseAdapter<ConversationListViewModel>
    {
        private readonly Activity _context;
        public IList<ConversationListViewModel> Conversations { get; set; }

        public ConversationListAdapter(Activity context, IList<ConversationListViewModel> conversations)
        {
            _context = context;
            Conversations = conversations;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ConversationListItem, null);

            var conversation = this[position];

            view.FindViewById<TextView>(Resource.Id.conversationUserName).Text = conversation.ToUserName;
            view.FindViewById<TextView>(Resource.Id.conversationItemName).Text = conversation.ItemName;
            view.FindViewById<TextView>(Resource.Id.conversationLastMessageTime).Text = conversation.LastUpdated.ToString("dd/MM hh:mm");

            var imageView = view.FindViewById<ImageView>(Resource.Id.conversationProfileImage);
            Picasso.With(_context)
                .Load(conversation.ItemThumbImage)
                .Fit()
                .Tag(_context)
                .Into(imageView);

            return view;
        }

        public override int Count
        {
            get { return Conversations.Count; }
        }
        
        public override ConversationListViewModel this[int index]
        {
            get { return Conversations[index]; }
        }
        
    }
}