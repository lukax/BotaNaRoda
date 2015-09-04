
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace BotaNaRoda.WebApi.Hubs
{
    public class UserDetail
    {
        public string id { get; set; }
        public string connectionId { get; set; }
    }

    public class ChatHub : Hub
    {
        private readonly ItemsContext _context;
        private static Dictionary<int, List<UserDetail>> groupManager = new Dictionary<int, List<UserDetail>>();

        public ChatHub(ItemsContext context)
        {
            _context = context;
        }

        //public void Connect(int conversationId, string myId)
        //{
        //try
        //{
        //    var messages = _context.Conversations.Find(x => x.)
        //    var messages = repo.FindMessages(conversationId).Select(m => new { Id = m.Id, FromUserName = m.FromUserName, Message = m.Message, IsFromMobile = m.IsFromMobile, FromUserId = m.FromUserId });
        //    string connectionId = Context.ConnectionId;

        //    Clients.Client(connectionId).onConnected(messages);
        //    //Verificar se já existe um grupo criado com o ID especificado
        //    //Atualizar o "ConnectionId" do Usuário com o seguinte Id(myId)
        //    if (groupManager.ContainsKey(conversationId))
        //    {
        //        UserDetail userInConversation = groupManager[conversationId].FirstOrDefault(x => x.id == myId);
        //        if (userInConversation == null)
        //        {
        //            groupManager[conversationId].Add(new UserDetail { connectionId = connectionId, id = myId });
        //            Groups.Add(connectionId, conversationId.ToString());
        //        }
        //        else
        //        {
        //            Groups.Remove(userInConversation.connectionId, conversationId.ToString());
        //            userInConversation.connectionId = Context.ConnectionId;
        //            Groups.Add(connectionId, conversationId.ToString());
        //        }
        //    }
        //    //Adicionar grupo
        //    else
        //    {
        //        Groups.Add(connectionId, conversationId.ToString());
        //        groupManager.Add(conversationId, new List<UserDetail> { new UserDetail { connectionId = connectionId, id = myId } });
        //    }
        //}
        //catch (Exception e)
        //{
        //    throw e;
        //}
        //ToggleUnreadMessages(conversation.GoWalkMessages.Where(x => x.FromUserId != myId));
        //}

        //    public async Task SendMessage(GoWalkConversationMessages message, bool isClient)
        //    {
        //        message = await AddMessageinDatabase(message, isClient);

        //        string id = message.GoWalkConversationsId.ToString();

        //        Clients.Group(id).messageReceived(new { Id = message.Id, Message = message.Message, FromUserName = message.FromUserName, FromUserId = message.FromUserId, IsFromMobile = message.IsFromMobile });
        //        //Clients.OthersInGroup(id).verifyMessageIsRead(message.Id);
        //    }

        //    //public void SetMessageRead(int id)
        //    //{
        //    //    DB.GoWalkMessages.FirstOrDefault(x => x.Id == id).IsRead = true;
        //    //    DB.SaveChangesAsync();            
        //    //}


        //    public async Task VerifyMessagesFromMobile(GoWalkConversationMessages message)
        //    {
        //        try
        //        {
        //            List<GoWalkConversationMessages> newMessages =
        //                repo.FindLatestsMessagesFromMobile(message.GoWalkConversationsId, message.Id);
        //            if (newMessages.Count > 0)
        //                Clients.Group(message.GoWalkConversationsId.ToString()).messagesReceived(newMessages);
        //            else
        //            {
        //                //message.Message = "não tem nova.count = 0";

        //                //Clients.Group(message.GoWalkConversationsId.ToString()).messagesReceived(new List<GoWalkConversationMessages>() { message });
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            //throw;
        //        }
        //    }

        //    public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        //    {
        //        try
        //        {
        //            string myId = repo.FindUserIdByName(Context.User.Identity.Name);
        //            var myGroups = groupManager.Where(x => x.Value.Select(k => k.connectionId == Context.ConnectionId).FirstOrDefault()).ToList();

        //            foreach (var group in myGroups)
        //            {
        //                foreach (UserDetail user in group.Value)
        //                {
        //                    if (user.connectionId == Context.ConnectionId)
        //                    {
        //                        group.Value.Remove(user);
        //                        break;
        //                    }
        //                }
        //                if (group.Value.Count == 0)
        //                    groupManager.Remove(group.Key);
        //            }
        //            //if (group.Value.Count == 0)
        //            //  groupManager.Remove(group.Key);
        //        }
        //        catch (Exception)
        //        {

        //        }

        //        return base.OnDisconnected(stopCalled);
        //    }

        //    //#endregion

        //    //#region private Messages

        //    private async Task<GoWalkConversationMessages> AddMessageinDatabase(GoWalkConversationMessages message, bool isClient)
        //    {
        //        message.IsRead = false;
        //        message.DateTime = DateTime.Now;
        //        if (await repo.AddMessageAsync(message, isClient))
        //            return message;
        //        else
        //            return message;
        //        //CurrentMessage.Add(new MessageDetail { UserName = userName, Message = message });

        //        /*if (CurrentMessage.Count > 100)
        //            CurrentMessage.RemoveAt(0);*/
        //    }
        //    #endregion
    }
}