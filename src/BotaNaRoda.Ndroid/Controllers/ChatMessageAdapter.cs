using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Models;
using Square.Picasso;
using BotaNaRoda.Ndroid.Data;
using Android.Support.V7.Widget;

namespace BotaNaRoda.Ndroid
{
    public class ChatMessageAdapter : BaseAdapter<ConversationChatMessage>
    {
        private readonly Activity _context;
        public IList<ConversationChatMessage> ChatMessages { get; set; }
		private readonly User _currentUser;

        public ChatMessageAdapter(Activity context, IList<ConversationChatMessage> chatMessages, User currentUser)
        {
            _context = context;
			_currentUser = currentUser;
            ChatMessages = chatMessages;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
			var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ChatMessage, null);

            var msg = this[position];

            view.FindViewById<TextView>(Resource.Id.chatMessageText).Text = msg.Message;
            view.FindViewById<TextView>(Resource.Id.chatMessageTime).Text = msg.SentAt.ToString("dd/MM HH:mm");
			var cardView = view.FindViewById<CardView> (Resource.Id.card_view);

			if (_currentUser.Id == msg.SentBy) {
				cardView.SetBackgroundColor (Android.Graphics.Color.PaleGreen);	
				((FrameLayout.LayoutParams)cardView.LayoutParameters).Gravity = GravityFlags.End;
				((FrameLayout.LayoutParams)cardView.LayoutParameters).LeftMargin = 150;
				((FrameLayout.LayoutParams)cardView.LayoutParameters).RightMargin = 5;
			} 
			else {
				cardView.SetBackgroundColor (Android.Graphics.Color.White);
			    ((FrameLayout.LayoutParams) cardView.LayoutParameters).Gravity = GravityFlags.Start;
				((FrameLayout.LayoutParams)cardView.LayoutParameters).RightMargin = 150;
				((FrameLayout.LayoutParams)cardView.LayoutParameters).LeftMargin = 5;
			}

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