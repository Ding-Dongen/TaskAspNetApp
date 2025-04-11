    // Global modal handler
document.addEventListener("DOMContentLoaded", function () {
    window.openModal = function(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.style.display = "flex";
        }
    };

    window.closeModal = function(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.style.display = "none";
        }
    };

    // Open any modal with [data-modal="modalId"]
    document.querySelectorAll("[data-modal]").forEach(button => {
        button.addEventListener("click", function () {
            const modalId = this.getAttribute("data-modal");
            openModal(modalId);
        });
    });

   
    document.querySelectorAll(".close-modal").forEach(button => {
        button.addEventListener("click", function () {
            const modal = this.closest(".upload-modal-overlay, .modal-overlay");
            if (modal) {
                closeModal(modal.id);
            }
        });
    });

    
    window.addEventListener("click", function (event) {
        if (event.target.classList.contains("upload-modal-overlay") || event.target.classList.contains("modal-overlay")) {
            closeModal(event.target.id);
        }
    });
});



function initializeQuillEditors() {
    document.querySelectorAll('.quill-editor:not([data-quill-initialized])').forEach(function (editorEl) {
        const targetSelector = editorEl.getAttribute('data-target');
        const hiddenInput = document.querySelector(targetSelector);
        const toolbarSelector = editorEl.getAttribute('data-toolbar');
        const toolbarOptions = toolbarSelector ? toolbarSelector : '#custom-toolbar';

        const quill = new Quill(editorEl, {
            theme: 'snow',
            modules: {
                toolbar: toolbarOptions
            }
        });

        editorEl.setAttribute('data-quill-initialized', 'true');

        if (hiddenInput && hiddenInput.value) {
            quill.root.innerHTML = hiddenInput.value;
        }

        // ✅ Attach form submit listener scoped to this editor only
        const form = editorEl.closest("form");
        if (form) {
            form.addEventListener("submit", function () {
                hiddenInput.value = quill.root.innerHTML;
            });
        }
    });
}


document.addEventListener('DOMContentLoaded', () => {
    setupDarkMode();
    initializeSignalR();
    restoreNotificationBadge();
    updateNotificationBadge();
    setupNotificationToggle();
    setupOutsideClickClose();
});

// === DARK MODE TOGGLE ===
function setupDarkMode() {
    const toggle = document.getElementById('darkModeToggle');
    if (!toggle) return;

    toggle.addEventListener('change', function () {
        document.body.classList.toggle('dark-mode');
        localStorage.setItem('darkMode', this.checked);
    });

    if (localStorage.getItem('darkMode') === 'true') {
        document.body.classList.add('dark-mode');
        toggle.checked = true;
    }
}

// === SIGNALR ===
let connection = null;
let isConnecting = false;

async function initializeSignalR() {
    if (isConnecting || typeof signalR === 'undefined') return;
    isConnecting = true;

    try {
        if (connection) await connection.stop();

        connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub", {
                withCredentials: true,
                skipNegotiation: false,
                transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        connection.onreconnecting(err => {
            console.warn("🔄 Reconnecting...", err);
            isConnecting = false;
        });

        connection.onreconnected(id => {
            console.log("✅ Reconnected with ID:", id);
            isConnecting = false;
            loadNotifications();
        });

        connection.onclose(err => {
            console.warn("❌ SignalR closed", err);
            isConnecting = false;
            setTimeout(initializeSignalR, 5000);
        });

        connection.on("ReceiveNotification", notification => {
            console.log("📩 New notification:", notification);
            addNotification(notification);
            updateNotificationBadge();
            document.getElementById('notificationPanel').style.display = 'block';
        });

        await connection.start();
        console.log("✅ SignalR connected:", connection.connectionId);
        isConnecting = false;

    } catch (err) {
        console.error("🚨 SignalR error:", err);
        isConnecting = false;
        setTimeout(initializeSignalR, 5000);
    }
}

document.addEventListener('visibilitychange', () => {
    if (document.visibilityState === 'visible' && (!connection || connection.state === signalR.HubConnectionState.Disconnected)) {
        initializeSignalR();
    }
});

// === NOTIFICATION FUNCTIONS ===
async function loadNotifications() {
    try {
        const response = await fetch('/api/Notification');
        if (!response.ok) throw new Error(`Status: ${response.status}`);

        const notifications = await response.json();
        const list = document.getElementById('notificationList');
        list.innerHTML = '';

        notifications.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
            .forEach(addNotification);

        updateNotificationBadge();

    } catch (err) {
        console.error("❌ Error loading notifications:", err);
    }
}

function addNotification(notification) {
    const list = document.getElementById('notificationList');
    if (document.querySelector(`.notification-item[data-id="${notification.id}"]`)) return;

    const el = document.createElement('div');
    el.className = `notification-item ${notification.isRead ? '' : 'unread'}`;
    el.setAttribute('data-id', notification.id);
    el.onclick = () => markAsRead(notification.id);
    el.innerHTML = `
        <div class="notification-item-message">${notification.message}</div>
        <div class="notification-item-time">${new Date(notification.createdAt).toLocaleString()}</div>
        <button class="delete-notification-btn" onclick="deleteNotification(${notification.id}); event.stopPropagation();">x</button>
    `;

    const noData = list.querySelector('.notification-item:not([data-id])');
    if (noData) noData.remove();

    list.prepend(el);
}

async function markAsRead(id) {
    try {
        const res = await fetch(`/api/Notification/${id}/read`, { method: 'POST' });
        if (!res.ok) throw new Error();
        document.querySelector(`.notification-item[data-id="${id}"]`)?.classList.remove('unread');
        updateNotificationBadge();
    } catch (err) {
        console.error("❌ Failed to mark as read", err);
    }
}

async function markAllAsRead() {
    try {
        const res = await fetch('/api/Notification/mark-all-read', { method: 'POST' });
        if (!res.ok) throw new Error();
        document.querySelectorAll('.notification-item.unread').forEach(el => el.classList.remove('unread'));
        updateNotificationBadge();
    } catch (err) {
        console.error("❌ Failed to mark all as read", err);
    }
}

async function clearAllNotifications() {
    try {
        const res = await fetch('/api/Notification/dismiss-all', { method: 'DELETE' });
        if (!res.ok) throw new Error();
        document.getElementById('notificationList').innerHTML = '';
        updateNotificationBadge();
    } catch (err) {
        console.error("❌ Failed to clear notifications", err);
    }
}

async function deleteNotification(id) {
    try {
        const res = await fetch(`/api/Notification/${id}`, { method: 'DELETE' });
        if (!res.ok) throw new Error();
        document.querySelector(`.notification-item[data-id="${id}"]`)?.remove();
        updateNotificationBadge();
    } catch (err) {
        console.error("❌ Failed to delete notification", err);
    }
}

function updateNotificationBadge() {
    const badge = document.getElementById('notificationBadge');
    const unread = document.querySelectorAll('.notification-item.unread');
    const count = unread.length;

    if (count > 0) {
        badge.textContent = count;
        badge.style.display = 'block';
        localStorage.setItem("notificationBadgeCount", count);
    } else {
        badge.style.display = 'none';
        localStorage.removeItem("notificationBadgeCount");
    }
}

function restoreNotificationBadge() {
    const badge = document.getElementById('notificationBadge');
    const saved = localStorage.getItem("notificationBadgeCount");
    if (saved && parseInt(saved) > 0) {
        badge.textContent = saved;
        badge.style.display = 'block';
    }
}

// === TOGGLE PANEL ===
function setupNotificationToggle() {
    const icon = document.querySelector('.notification-container .icon-link');
    if (!icon) return;

    icon.addEventListener('click', function (e) {
        e.preventDefault();
        const panel = document.getElementById('notificationPanel');
        panel.style.display = panel.style.display === 'block' ? 'none' : 'block';
    });
}

function setupOutsideClickClose() {
    document.addEventListener('click', (event) => {
        const panel = document.getElementById('notificationPanel');
        const container = document.querySelector('.notification-container');
        if (!container.contains(event.target)) {
            panel.style.display = 'none';
        }
    });
}
