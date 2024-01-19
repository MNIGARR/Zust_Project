

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
document.getElementById('photo-input').addEventListener('change', function (e) {
    const selectedFiles = Array.from(e.target.files);
    const imagePreviewContainer = document.getElementById('image-preview-container');

    if (selectedFiles && selectedFiles.length > 0) {
        selectedFiles.forEach(file => {
            const reader = new FileReader();
            reader.onload = function (event) {
                const imagePreview = document.createElement('img');
                imagePreview.src = event.target.result;
                imagePreviewContainer.appendChild(imagePreview);
            };
            reader.readAsDataURL(file);
        });
    }
    this.value = '';
});

document.getElementById('video-input').addEventListener('change', function (e) {
    const file = e.target.files[0];
    if (!file) {
        return;
    }

    const videoPreviewContainer = document.getElementById('video-preview-container');
    videoPreviewContainer.innerHTML = '';

    const videoElement = document.createElement('video');
    videoElement.src = URL.createObjectURL(file);
    videoElement.controls = true;
    videoElement.width = 320;
    videoElement.height = 240; 
    videoPreviewContainer.appendChild(videoElement);
});

<form id="uploadForm" onsubmit="submitForm('post'); return false;">
    <button type="submit">Post</button>
</form>
function submitPost() {
    const postContent = document.getElementById('post-content').value;

    const formData = new FormData();
    formData.append('content', postContent);

    sendPostToServer(formData);
}

function submitPost() {
    const postContent = document.getElementById('post-content').value;

    // Create an object for the post
    const postData = {
        content: postContent
    };

    // Send the data to the server
    fetch('/posts', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(postData)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(post => {
            console.log('Post added successfully:', post);

            // Now update the UI to reflect the new post
            addPostToUI(post);
        })
        .catch(error => {
            console.error('Error adding post:', error);
        });
}

function addPostToUI(post) {
    const postsContainer = document.querySelector('.news-feed');
    const newPostElement = document.createElement('div');
    newPostElement.className = 'news-feed-post';

    newPostElement.innerHTML = `
        <div class="post-content">${post.content}</div>
        // ... other post details ...
    `;

    // Add the new post to the top of the posts list
    postsContainer.prepend(newPostElement);

    // Clear the textarea
    document.getElementById('post-content').value = '';
}


function addPostToUI(postData) {
    const postsContainer = document.querySelector('.news-feed'); // Container where you want to add the new post
    const newPost = document.createElement('div');
    newPost.className = 'news-feed-post';

    // Assuming postData contains fields like .author, .content, .image, etc.
    // Create the inner HTML of the post using postData
    newPost.innerHTML = `
        <div class="post-header d-flex justify-content-between align-items-center">
            // ... header content using postData ...
        </div>
        <div class="post-body">
            <p>${postData.content}</p>
            <div class="post-image">
                <img src="${postData.image}" alt="image">
            </div>
            // ... other post elements ...
        </div>
    `;

    // Add the new post to the top of the posts container
    postsContainer.prepend(newPost);
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



