using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Square.Picasso;

namespace BotaNaRoda.Ndroid
{
    public class ConversationListAdapter : BaseAdapter<ConversationViewModel>
    {
        private readonly Activity _context;
        private readonly IList<ConversationViewModel> _conversations;

        public ConversationListAdapter(Activity context, IList<ConversationViewModel> conversations)
        {
            _context = context;
            _conversations = conversations;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ConversationListItem, null);

            var conversation = this[position];

            view.FindViewById<TextView>(Resource.Id.conversationUserName).Text = conversation.ToUser.Name;
            view.FindViewById<TextView>(Resource.Id.conversationItemName).Text = conversation.Item.Name;
            view.FindViewById<TextView>(Resource.Id.conversationLastMessageTime).Text = conversation.UpdatedAt.ToString("dd/MM hh:mm");

            var imageView = view.FindViewById<ImageView>(Resource.Id.conversationProfileImage);
            Picasso.With(_context)
                .Load(conversation.ToUser.Avatar)
                .Fit()
                .Tag(_context)
                .Into(imageView);

            return view;
        }

        public override int Count
        {
            get { return _conversations.Count; }
        }
        
        public override ConversationViewModel this[int index]
        {
            get { return _conversations[index]; }
        }
        
    }
}