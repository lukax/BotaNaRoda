var chatHub = $.connection.chatHub;
var id = "exempleID";
registerCLientMethods();

console.log(chatHub);
$.connection.hub.start().done(function() {
    
    chatHub.server.connect(id);
    console.log(chatHub.client);
    //chatHub.server.addUser(id);
});

function registerCLientMethods() {
    console.log("registring...");
    chatHub.client.onConnected = function() {
        console.log("connected!");
    }
    chatHub.client.alert = function () {
        alert("teste");
    }
}

function test(parameters) {
    console.log("testing...");
    chatHub.server.alert();
}

//----------------------------------------------




var conversation = { id: 0, myId: '', otherId: '', myName: $("#MyName").val(), otherName: '' };
var lastMobileMsgId = { Id: -1 };
var chatHub = $.connection.chatHub;
//console.log("initi chat");
$(function () {
    registerClientMethods();
});

function connect2(c) {
    //console.log("connect");
    //console.log(c);
    $.connection.hub.start().done(function () {
        //console.log("connected");
        conversation = c;
        conversation.myName = $("#MyName").val();
        registerEvents(conversation);
        chatHub.server.connect(conversation.id, conversation.myId);
    });


    //OpenPrivateChatWindow()
}

function registerEvents(conversation) {
    //BROKEN
    $('#btnSendMsg').click(function () {
        var msg = $("#txtMessage").val();

        if (msg.trim().length > 0) {
            var conId = $('#conId').val();
            chatHub.server.sendMessage({
                'GoWalkConversationsId': conversation.id,
                'FromUserId': conversation.myId,
                'ToUserId': conversation.otherId,
                'ToUserName': conversation.otherName,
                'FromUserName': conversation.myName,
                'Message': msg,
            }, $("#isClient").val());
            $("#txtMessage").val('');
        }
    });

    //$("#txtNickName").keypress(function (e) {
    //    if (e.which == 13) {
    //        $("#btnStartChat").click();
    //    }
    //});

    //$("#txtMessage").keypress(function (e) {
    //    if (e.which == 13) {
    //        $('#btnSendMsg').click();
    //    }
    //});
}

function registerClientMethods() {
    // Calls when user successfully logged in    
    chatHub.client.onConnected = function (messages) {
        //console.log("onconnected");
        //console.log(messages);
        FillMessagesHistory(messages);
        //fillImages();
        setInterval(verifyMessagesFromMobile, 3000);
    };
    chatHub.client.messagesReceived = function (messages) {
        FillMessagesHistory(messages);
        //fillImages();
    };
    chatHub.client.messageReceived = function (a) {

        //a.mine = a.FromUserId == conversation.myId;
        AddMessage(a);
        //fillImages();
    };
    chatHub.client.verifyMessageIsRead = function (a) {
        try {
            chatHub.server.setMessageRead(a);
        } catch (e) {

        }
    };
    chatHub.client.alert = function (message) {
        alert(message);
    };
}

function verifyMessagesFromMobile() {
    chatHub.server.verifyMessagesFromMobile(lastMobileMsgId);
}

function AddMessage(message) {
    message.IsFromMobile && (lastMobileMsgId = message) && (lastMobileMsgId.GoWalkConversationsId = conversation.id);
    $("#messagesContainer").append(conversationContainer(message, message.IsMine));

    var objDiv = document.getElementById("messagesContainer");
    objDiv.scrollTop = objDiv.scrollHeight;
};

function conversationContainer(message) {
    message.mine = (message.FromUserId == conversation.myId) ? "true" : "false";
    var html = '';
    html +=
        "<div class='message'>" +
            "<div class='profile-container'>" +
                "<img " +
                    " class='profile-picture'" +
                    " mine='" + message.mine + "'" +
                    " src='" + (message.mine == "true" ? $("#myProfilePicture").val() : $("#otherProfilePicture").val()) + "'" +
                "></img>" +
                "<p class='user-name'>" + message.FromUserName + "</p>" +
            "</div>" +
            "<div class='chat-container'>" +
                "<div class='message-text'>" + message.Message + "</div>" +
            "</div>" +
        "</div>";
    return html;
}

function FillMessagesHistory(messages) {
    for (var i = 0; i < messages.length; i++) {
        AddMessage(messages[i]);
    }
}

function closeChat() {
    args.onChatClosed();
    $.close();
}
