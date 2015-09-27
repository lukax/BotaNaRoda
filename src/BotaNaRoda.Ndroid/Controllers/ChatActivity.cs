using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using BotaNaRoda.Ndroid.Util;
using Square.Picasso;

namespace BotaNaRoda.Ndroid
{
    [Activity(Label = "Bota na Roda",
        Theme = "@style/ItemDetailTheme", ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize),
        ParentActivity = typeof (MainActivity))]
    public class ChatActivity : AppCompatActivity
    {
        private UserRepository _userRepository;
        private ItemRestService _itemService;
        private ConversationDetailViewModel _conversation;
        private ViewHolder _holder;
        private ChatMessageAdapter _adapter;
        private BackgroundWorker _refreshWorker;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ConversationListItem);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _userRepository = new UserRepository();
            _itemService = new ItemRestService(new UserRepository());

            _holder = new ViewHolder
            {
                ItemImage = FindViewById<ImageView>(Resource.Id.chatItemImage),
                ItemName = FindViewById<TextView>(Resource.Id.chatItemName),
                ItemDistance = FindViewById<TextView>(Resource.Id.chatItemDistance),
                UserImage = FindViewById<ImageView>(Resource.Id.chatUserImage),
                UserName = FindViewById<TextView>(Resource.Id.chatUserName),
                MessageList = FindViewById<ListView>(Resource.Id.chatMessageList),
                ChatMessageText = FindViewById<EditText>(Resource.Id.chatMessageText),
                ChatSendButton = FindViewById<ImageButton>(Resource.Id.chatSendButton)
            };

            _adapter = new ChatMessageAdapter(this, new List<ConversationChatMessage>());
            _holder.MessageList.Adapter = _adapter;
            _holder.ChatSendButton.Click += ChatSendButtonOnClick;

            Refresh();
        }

        private void ChatSendButtonOnClick(object sender, EventArgs eventArgs)
        {
            var msg = _holder.ChatMessageText.Text;
            //...
            _holder.ChatMessageText.Text = "";
        }

        private void Refresh()
        {
            _refreshWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _refreshWorker.DoWork += (sender, args) =>
            {
                _conversation = _itemService.GetConversation(Intent.GetStringExtra("conversationId")).Result;
            };
            _refreshWorker.RunWorkerCompleted += (sender, args) =>
            {
                RunOnUiThread(UpdateUi);
            };
            _refreshWorker.RunWorkerAsync();
        }

        private void UpdateUi()
        {
            var authInfo = _userRepository.Get();

            _holder.ItemName.Text = _conversation.ItemName;
            _holder.ItemDistance.Text = _conversation.DistanceTo(authInfo);
            _holder.UserName.Text = _conversation.ToUserName;

            Picasso.With(this).Load(_conversation.ItemThumbImage).Fit().Tag(this).Into(_holder.ItemImage);
            Picasso.With(this).Load(_conversation.ToUserAvatar).Fit().Tag(this).Into(_holder.UserImage);

            _adapter.ChatMessages = _conversation.Messages;
            _adapter.NotifyDataSetChanged();
        }

        protected override void OnDestroy()
        {
            _refreshWorker?.CancelAsync();
            base.OnDestroy();
        }

        private class ViewHolder
        {
            internal ImageView ItemImage;
            internal TextView ItemName;
            internal TextView ItemDistance;
            internal ImageView UserImage;
            internal TextView UserName;
            internal ListView MessageList;
            internal EditText ChatMessageText;
            internal ImageButton ChatSendButton;
        }
    }
}