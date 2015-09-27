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
    public class ConversationDetailActivity : AppCompatActivity
    {
        private UserRepository _userRepository;
        private ItemRestService _itemService;
        private ConversationDetailViewModel _conversation;
        private ViewHolder _holder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ConversationListItem);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

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

            _userRepository = new UserRepository();
            _itemService = new ItemRestService(new UserRepository());
        }

        private void Refresh()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                _conversation = _itemService.GetConversation(Intent.GetStringExtra("conversationId")).Result;
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                RunOnUiThread(UpdateUi);
            };
            worker.RunWorkerAsync();
        }


        private void UpdateUi()
        {
            _holder.ItemName.Text = _conversation.ItemName;
            _holder.ItemDistance.Text = _conversation.
            _holder.UserName.Text = _conversation.ToUserName;

            Picasso.With(this).Load(_conversation.ItemThumbImage).Fit().Tag(this).Into(_holder.ItemImage);
            Picasso.With(this).Load(_conversation.ToUserAvatar).Fit().Tag(this).Into(_holder.AuthorImage);


        }

        private class ViewHolder
        {
            internal ImageView ItemImage;
            internal TextView ItemName;
            internal TextView ItemDistance;
            internal ImageView UserImage;
            internal TextView UserName;
            internal ListView MessageList;
            internal ImageView AuthorImage;
            internal EditText ChatMessageText;
            internal ImageButton ChatSendButton;
        }
    }
}