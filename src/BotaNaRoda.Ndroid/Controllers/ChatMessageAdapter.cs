using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Models;
using Square.Picasso;

namespace BotaNaRoda.Ndroid
{
    public class ChatMessageAdapter : BaseAdapter<ConversationChatMessage>
    {
        private readonly Activity _context;
        public IList<ConversationChatMessage> ChatMessages { get; set; }

        public ChatMessageAdapter(Activity context, IList<ConversationChatMessage> chatMessages)
        {
            _context = context;
            ChatMessages = chatMessages;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ConversationListItem, null);

            var msg = this[position];

            view.FindViewById<TextView>(Resource.Id.chatMessageText).Text = msg.Message;
            view.FindViewById<TextView>(Resource.Id.chatMessageTime).Text = msg.Sent.ToString("MM-dd HH:mm:ss");

            return view;
        }

        public override int Count
        {
            get { return ChatMessages.Count; }
        }
        
        public override ConversationChatMessage this[int index]
        {
            get { return ChatMessages[index]; }
        }
        
    }
}