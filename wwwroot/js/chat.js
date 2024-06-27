
"use strict";



var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").Disabled = true;

// Start the connection
connection.start().then(function () {
    console.log("connection started");

    document.getElementById("sendButton").Disabled = false;
}).catch (err => console.error(err.toString()));


function scrollToBottom() {
    var chatHistory = document.querySelector('.chat-history');
    chatHistory.scrollTop = chatHistory.scrollHeight;
}


// Event listener for receiving messages
connection.on("ReceiveMessage", (_senderId,_receiverId, message) => {
    if (message !== '')
    {
        console.log(_senderId, _receiverId, message);
        console.log("Message received");
        const userItem = document.querySelector('.user-item');

        // Retrieve the data attributes using dataset
        const senderProfilePath = userItem.dataset.senderProfilepath;
        const senderId = userItem.dataset.userId;
        const senderName = userItem.dataset.userName;
        const receiverId = userItem.dataset.receiverId;
        const receiverName = userItem.dataset.receiverName;
        const receiverProfilePath = userItem.dataset.receiverProfilepath;

        console.log(senderId);
        console.log(receiverId);
        console.log(senderName);
        console.log(receiverName);
        console.log(senderProfilePath);
        console.log(receiverProfilePath);

        const messageList = document.getElementById('messageList');
        const newMessage = document.createElement('li');
        newMessage.className = 'clearfix';
        newMessage.innerHTML = `
                    <div class="message-data ${senderId === _senderId ? 'text-end' : 'text-start'}">
                        <img  style="width:41px; height:41px;" src="/${senderId === _senderId ? senderProfilePath : receiverProfilePath}" alt="avatar">
                        <span class="message-data-time">${new Date().toLocaleTimeString() }</span>
                    </div>
                    <div class="message ${senderId === _senderId ? 'other-message float-right' : 'my-message float-left'}">${message}</div>
        `;
        messageList.appendChild(newMessage);
        scrollToBottom();
        //// Scroll to the bottom of the message list
        //messageList.scrollTop = messageList.scrollHeight;
    }
});


// Send message on button click
document.getElementById('sendButton').addEventListener('click', () => {
    const messageInput = document.getElementById('messageInput');
    const messageText = messageInput.value.trim();
    const senderId = document.getElementById('senderId').value;
    const receiverId = document.getElementById('receiverId').value;

    const userItem = document.querySelector('.user-item');

    // Retrieve the data attributes using dataset
    const senderProfilePath = userItem.dataset.senderProfilepath;
    if (messageText !== '') {
        console.log(senderId, receiverId, messageText);
        console.log("message sending.....")

        const messageList = document.getElementById('messageList');
        const newMessage = document.createElement('li');
        newMessage.className = 'clearfix';
        newMessage.innerHTML = `
                    <div class="message-data text-end">
                        <span class="message-data-time">${new Date().toLocaleTimeString()}</span>
                        <img  style="width:41px; height:41px;" src="/${senderProfilePath}" alt="avatar">
                    </div>
                    <div class="message other-message float-right">${messageText}</div>
        `;
        messageList.appendChild(newMessage);
        scrollToBottom();

        connection.invoke("SendMessage", senderId, receiverId, messageText)
            .catch(err => console.error("Error invoking SendMessage:", err.toString()));

        messageInput.value = '';
        console.log("message sended.....")

    }
});


document.getElementById('backwardArrow').addEventListener('click', () => {
    console.log("Backward arrow clicked!"); // Check if this message appears in the console
    // Hide the chat section and show the user list section
    document.querySelector('.chat').style.display = 'none';
    document.querySelector('.chat-blank').style.display = 'flex';
    
});

document.querySelectorAll('.user-item').forEach(item => {
    item.addEventListener('click', () => {
        const senderId = item.getAttribute('data-user-id');
        const senderName = item.getAttribute('data-user-name');
        const senderProfilepath = item.getAttribute('data-sender-profilepath');
        
        const receiverId = item.getAttribute('data-receiver-id');
        const receiverName = item.getAttribute('data-receiver-name');
        const receiverProfilepath = item.getAttribute('data-receiver-profilepath');

        document.getElementById('chatUserImg').src ="/" + receiverProfilepath;
        console.log(receiverProfilepath)
        document.getElementById('chatUserName').textContent = receiverName;
        document.getElementById('receiverId').value = receiverId;
        document.getElementById('senderId').value = senderId;
       
        document.querySelector('.chat-blank').style.display = 'none';
        document.querySelector('.chat').style.display = 'block'; 

        if (window.innerWidth <= 768) {
            document.querySelector('.people-list').classList.remove('show-list');
        }

        fetchMessages(senderId, receiverId, senderProfilepath, receiverProfilepath);
    });
});

function fetchMessages(senderId, receiverId, senderProfilePath, receiverProfilePath)
{
    console.log('Fetching messages for:', senderId, receiverId);
    // Make an AJAX request to fetch previousmessages from the server
    fetch(`/messages?userId=${senderId}&receiverId=${receiverId}`)
        .then(response => response.json())
        .then(messages => {
            // Display fetched messages in the chat history
            const messageList = document.getElementById('messageList');
            messageList.innerHTML = ''; // Clear existing messages

            messages.forEach(msg => {
               
                const newMessage = document.createElement('li');
                newMessage.className = 'clearfix';
                newMessage.innerHTML = `
                    <div class="message-data ${msg.senderId === senderId ? 'text-end' : 'text-start'}">
                        <span class="message-data-time">${new Date(msg.timestamp).toLocaleTimeString()}</span>
                        <img  style="width:41px; height:41px;" src="/${msg.senderId === senderId ? senderProfilePath : receiverProfilePath}" alt="avatar">
                    </div>
                    <div class="message ${msg.senderId === senderId ? 'other-message float-right text-white' : 'my-message float-left text-white'}" style="background-color:${msg.senderId === senderId ? 'purple': 'lightgray'}">${msg.content}</div>
                `;
                messageList.appendChild(newMessage);
                scrollToBottom();
            });

            //// Scroll to the bottom of the message list
            //messageList.scrollTop = messageList.scrollHeight;
        })
        .catch(err => console.error(err));
}






