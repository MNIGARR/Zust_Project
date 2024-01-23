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

    var timeSpan = document.createElement('span');
    timeSpan.className = 'time d-block';
    timeSpan.textContent = new Date(message.dateTime).toLocaleTimeString();
    chatMessageDiv.appendChild(timeSpan);

    chatBodyDiv.appendChild(chatMessageDiv);
    chatElement.appendChild(chatAvatarDiv);
    chatElement.appendChild(chatBodyDiv);

    chatContent.appendChild(chatElement);
}

function updateChatBody(messages) {
    const chatContent = document.querySelector('.chat-content');
    chatContent.innerHTML = '';
    messages.forEach(message => {
        
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
        addMessageToChat({ senderId, content: message });
    }
});
function handlePhotoUpload() {
    const input = document.getElementById('photo-input');
    input.value = '';
    input.click();
}
function handleVideoUpload() {
    const input = document.getElementById('video-input');
    input.value = '';
    input.click();
}

document.getElementById('uploadForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const formData = new FormData(this);
    fetch('@Url.Action("Upload", "Home")', {
        method: 'POST',
        body: formData
    }).then(response => {
        console.log('Upload successful');
        window.location.href = '@Url.Action("Index", "Home")';
    }).catch(error => {
        console.error('Error:', error);
    });
});

function submitPost() {
    const formElement = document.getElementById('uploadForm');
    const formData = new FormData(formElement);

    fetch('/Home/Upload', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            console.log('Server response:', data); // Check the structure of 'data'

            // Initialize postData with content
            const postData = {
                content: formData.get('message')
            };

            // Add imageUrls to postData if it exists and is not empty
            if (data.imageUrls.length > 0) {
                postData.imageUrls = data.imageUrls;
            }

            // Add videoUrls to postData if it exists and is not empty
            if (data.videoUrls && data.videoUrls.length > 0 && data.videoUrls[0].URL) {
                postData.videoUrls = data.videoUrls;
            }

            addPostToUI(postData);
        })
        .catch(error => {
            console.error('Error:', error);
        });

}

function addPostToUI(postData) {
    var currentUser = document.querySelector('#currentUser').value;
    const imageUrlMatch = currentUser.match(/ImageUrl\s*=\s*([^,]+)/);
    let imageUrl = '';

    if (imageUrlMatch && imageUrlMatch[1]) {
        imageUrl = imageUrlMatch[1].trim();
    }
    const usernameMatch = currentUser.match(/Username\s*=\s*([^,]+)/);
    let username = '';
    if (usernameMatch && usernameMatch[1]) {
        username = usernameMatch[1].trim(); // Remove any unwanted characters and get the clean username
    }
    const postsContainer = document.querySelector('.news-feed-post');
    const newPost = document.createElement('div');
    newPost.className = 'news-feed news-feed-post';
    newPost.innerHTML = `
        <div class="post-header d-flex justify-content-between align-items-center">
            <div class="image">
                <a href="${postData.profileLink || '#'}"><img src="/assets/images/user/${imageUrl}" class="rounded-circle" alt="image"></a>
            </div>
            <div class="info ms-3">
                <span class="name"><a href="${postData.profileLink || '#'}">${username || 'Anonymous'}</a></span>
                <span class="small-text"><a href="#">${postData.timeAgo || 'Just now'}</a></span>
            </div>
            <div class="dropdown">
                <button class="dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="flaticon-menu"></i></button>
                <ul class="dropdown-menu">
                    <li><a class="dropdown-item d-flex align-items-center" href="#"><i class="flaticon-edit"></i> Edit Post</a></li>
                    <li><a class="dropdown-item d-flex align-items-center" href="#"><i class="flaticon-private"></i> Hide Post</a></li>
                    <li><a class="dropdown-item d-flex align-items-center" href="#"><i class="flaticon-trash"></i> Delete Post</a></li>
                </ul>
            </div>
        </div>

        <div class="post-body">
            <p>${postData.content || 'Content not available'}</p>
            <div class="post-image">
                <img src="${postData.imageUrls[0].url}" alt="image">
            </div>
        </div>

        <div class="post-comment-list">
        </div>

        <form class="post-footer">
          
            <div class="form-group">
                <textarea name="message" class="form-control" placeholder="Write a comment..."></textarea>
                <label><a href="#"><i class="flaticon-photo-camera"></i></a></label>
            </div>
        </form>
    `;

    // Add the new post to the top of the posts container
    postsContainer.prepend(newPost);
}


document.getElementById('photo-input').addEventListener('change', function (e) {
    const selectedFiles = Array.from(e.target.files);
    const imagePreviewContainer = document.getElementById('image-preview-container');
    imagePreviewContainer.innerHTML = ''; // Clear previous previews

    selectedFiles.forEach(file => {
        const reader = new FileReader();
        reader.onload = function (event) {
            const imagePreview = document.createElement('img');
            imagePreview.src = event.target.result;
            imagePreviewContainer.appendChild(imagePreview);
        };
        reader.readAsDataURL(file);
    });
});

document.getElementById('video-input').addEventListener('change', function (e) {
    const selectedFiles = Array.from(e.target.files);
    const videoPreviewContainer = document.getElementById('video-preview-container');
    videoPreviewContainer.innerHTML = ''; // Clear previous previews

    selectedFiles.forEach(file => {
        const videoPreview = document.createElement('video');
        videoPreview.src = URL.createObjectURL(file);
        videoPreview.controls = true;
        videoPreviewContainer.appendChild(videoPreview);
    });
});

(document).on('click', '.friendRequestButton', function () {
    const receiverId = $(this).data('receiverId');
    sendFriendRequest(receiverId);
});

function sendFriendRequest(receiverId, event) {
    const buttonElement = event.target;

    fetch(`/Profile/SendFriendRequest?id=${receiverId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (response.ok) {
                console.log('Friend request sent successfully');
                buttonElement.textContent = 'Pending';
                buttonElement.disabled = true;
            } else {
                console.error('Failed to send friend request');
                buttonElement.textContent = 'Add Friend';
                buttonElement.disabled = false;
            }
        })
        .catch(error => {
            console.error('Error:', error);
            buttonElement.textContent = 'Add Friend';
            buttonElement.disabled = false;
        });
}
function AcceptRequest(id, id2, requestId) {
    $.ajax({
        url: `/Profile/AcceptRequest?userId=${id}&senderId=${id2}&requestId=${requestId}`,
        method: "GET",
        success: function (data) {
            let element = document.querySelector("#alert");
            element.style.display = "block";
            element.innerHTML = "You accept request successfully";
            GetAllUsers();
            SendFollowCall(id);
            SendFollowCall(id2);
            GetMyRequests();
            GetAllUsers();

            setTimeout(() => {
                element.innerHTML = "";
                element.style.display = "none";
            }, 5000);
        }
    })
}

function hideButtons() {
    document.getElementById("acceptButton").style.display = "none";
    document.getElementById("declineButton").style.display = "none";
}