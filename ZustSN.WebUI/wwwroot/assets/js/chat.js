'use strict';
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

connection.start().then(function () {
    //GetAllUsers();
    console.log("Connected");
}).catch(function (err) {
    return console.error(err.toString());
})

let element = document.querySelector("#alert");
connection.on("Connect", function (info) {
    console.log("Connect Work");
    //GetAllUsers();
    //element.style.display = "block";
    //element.innerHTML = info;
    //setTimeout(() => {
    //    element.innerHTML = "";
    //    element.style.display = "none";
    //}, 5000);
})
connection.on("Disconnect", function (info) {
    console.log("DisConnect Work");
    //GetAllUsers();
    //element.style.display = "block";
    //element.innerHTML = info;
    //setTimeout(() => {
    //    element.innerHTML = "";
    //    element.style.display = "none";
    //}, 5000);
})

var linkElement = document.getElementById('sender');

var senderId = linkElement.getAttribute('data-sender');
document.querySelectorAll('.chat-box').forEach(item => {
    item.addEventListener('click', event => {
        let userId = event.currentTarget.getAttribute('data-user-id');
        let username = event.currentTarget.getAttribute('data-username');
        let imageUrl = event.currentTarget.getAttribute('data-image-url');
        openChatForUser(userId, username, imageUrl);
        fetchChatHistory(senderId, userId);
    });
});


function openChatForUser(userId, username, imageUrl) {
    if (username != null) {
        document.querySelector('#userName').textContent = username;
        var imageElement = document.querySelector('#userImg');
        imageElement.src = imageUrl.includes('http') ? imageUrl : '/assets/images/user/' + imageUrl;
        sessionStorage.setItem('selectedUserId', userId);
        sessionStorage.setItem('selectedUsername', username);
        sessionStorage.setItem('selectedUserImageUrl', imageUrl);
        document.querySelector('.live-chat-body').style.display = 'block';
    }
    //fetch(`https://localhost:7129/Message/LiveChat/${userId}`)
    //    .then(response => response.json())
    //    .then(data => {
    //        // Assuming 'data' is an array of chat messages
    //        // Clear existing messages
    //        var chatContent = document.querySelector('.chat-content');
    //        chatContent.innerHTML = '';

    //        // Append each message to the chat content
    //        data.forEach(message => {
    //            var messageElement = document.createElement('div');
    //            messageElement.className = 'chat-message';
    //            messageElement.innerHTML = `
    //                <p>${message.text}</p>
    //                <span class="time d-block">${message.time}</span>
    //            `;
    //            chatContent.appendChild(messageElement);
    //        });
    //    })
    //    .catch(error => {
    //        console.error('Error fetching chat history:', error);
    //    });

}

function fetchChatHistory(senderId, receiverId) {
    var chatContent = document.querySelector('.chat-content');
    chatContent.innerHTML = '<div class="loading">Loading chat history...</div>';

    fetch(`/Message/GetChatHistory?senderId=${senderId}&receiverId=${receiverId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            chatContent.innerHTML = ''; 
            data.forEach(message => {
                var chatElement = document.createElement('div');
                chatElement.className = message.senderId === senderId ? 'chat chat-left' : 'chat';

                var chatAvatarDiv = document.createElement('div');
                chatAvatarDiv.className = 'chat-avatar';
                var avatarLink = document.createElement('a');
                avatarLink.setAttribute('routerLink', '/profile');
                avatarLink.className = 'd-inline-block';
                var avatarImg = document.createElement('img');
                avatarImg.setAttribute('src', '~/assets/images/user/user-1.jpg'); 
                avatarImg.setAttribute('width', '50');
                avatarImg.setAttribute('height', '50');
                avatarImg.className = 'rounded-circle';
                avatarImg.setAttribute('alt', 'image');
                avatarLink.appendChild(avatarImg);
                chatAvatarDiv.appendChild(avatarLink);

                var chatBodyDiv = document.createElement('div');
                chatBodyDiv.className = 'chat-body';

                var chatMessageDiv = document.createElement('div');
                chatMessageDiv.className = 'chat-message';

                var messageText = document.createElement('p');
                messageText.textContent = message.content;
                chatMessageDiv.appendChild(messageText);

                var timeSpan = document.createElement('span');
                timeSpan.className = 'time d-block';
                timeSpan.textContent = new Date(message.dateTime).toLocaleTimeString();
                chatMessageDiv.appendChild(timeSpan);

                chatBodyDiv.appendChild(chatMessageDiv);

                chatElement.appendChild(chatAvatarDiv);
                chatElement.appendChild(chatBodyDiv);

                chatContent.appendChild(chatElement);
            });

        })
        .catch(error => {
            console.error('Error fetching chat history:', error);
            chatContent.innerHTML = '<div class="error">Failed to load chat history.</div>';
        });
}

let selectedUserId = sessionStorage.getItem('selectedUserId');
let selectedUsername = sessionStorage.getItem('selectedUsername');
let selectedUserImageUrl = sessionStorage.getItem('selectedUserImageUrl');


document.getElementById('sendBtn').addEventListener('click', function (event) {
    event.preventDefault();
    var receiverId = sessionStorage.getItem('selectedUserId');
    var senderId = document.getElementById('sender').getAttribute('data-sender');
    let contentElement = document.querySelector("#message-input");
    let messageContent = contentElement.value.trim();

    if (messageContent) {
        SendMessage(senderId, receiverId, messageContent);
        addMessageToChat({ senderId, content: messageContent, dateTime: new Date() });
        contentElement.value = "";
    } else {
        console.error('Error: Message content is empty.');
    }
});



function addMessageToChat(message) {
    const chatContent = document.querySelector('.chat-content');

    var chatElement = document.createElement('div');
    chatElement.className = message.senderId === senderId ? 'chat chat-left' : 'chat';

    var chatAvatarDiv = document.createElement('div');
    chatAvatarDiv.className = 'chat-avatar';
    var avatarLink = document.createElement('a');
    avatarLink.setAttribute('routerLink', '/profile');
    avatarLink.className = 'd-inline-block';
    var avatarImg = document.createElement('img');
    avatarImg.setAttribute('src', message.avatarImageUrl || '/assets/images/user/user-11.jpg');
    avatarImg.setAttribute('width', '50');
    avatarImg.setAttribute('height', '50');
    avatarImg.className = 'rounded-circle';
    avatarImg.setAttribute('alt', 'image');
    avatarLink.appendChild(avatarImg);
    chatAvatarDiv.appendChild(avatarLink);

    var chatBodyDiv = document.createElement('div');
    chatBodyDiv.className = 'chat-body';
    var chatMessageDiv = document.createElement('div');
    chatMessageDiv.className = 'chat-message';
    var messageText = document.createElement('p');
    messageText.textContent = message.content;
    chatMessageDiv.appendChild(messageText);

    // Create the timestamp element for the message
    var timeSpan = document.createElement('span');
    timeSpan.className = 'time d-block';
    timeSpan.textContent = new Date(message.dateTime).toLocaleTimeString();
    chatMessageDiv.appendChild(timeSpan);

    // Append the message and avatar containers to the chat body
    chatBodyDiv.appendChild(chatMessageDiv);
    chatElement.appendChild(chatAvatarDiv);
    chatElement.appendChild(chatBodyDiv);

    // Append the chat element to the chat content container
    chatContent.appendChild(chatElement);
}

function updateChatBody(messages) {
    const chatContent = document.querySelector('.chat-content');
    chatContent.innerHTML = ''; // Clear current content
    messages.forEach(message => {
        // Create and append new message elements to chatContent
        // ... (code to create message elements based on your HTML structure)
    });
}
function SendMessage(senderId, receiverId) {
    let messageContent = document.querySelector("#message-input").value;
    // Construct the message object
    let messageData = {
        senderId: senderId,
        receiverId: receiverId,
        content: messageContent
    };

    // Send the message data to the server
    fetch('/Message/AddMessage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(messageData)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json(); // Or `response.text()` if the server does not send a JSON response.
        })
        .then(data => {
            console.log('Message sent successfully:', data);
        })
        .catch(error => {
            console.error('Error sending message:', error);
        });
}

connection.on("ReceiveMessage", function (senderId, message) {
    if (senderId === sessionStorage.getItem('selectedUserId')) {
        // Append the received message to the chat
        addMessageToChat({ senderId, content: message });
    }
});