// SignalR Notifications Client
(function () {
    'use strict';

    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/notificationHub')
        .withAutomaticReconnect()
        .build();

    let notificationCounter = 0;

    function getIconClass(type) {
        switch (type) {
            case 'success': return 'fas fa-check-circle';
            case 'danger': return 'fas fa-times-circle';
            case 'warning': return 'fas fa-exclamation-triangle';
            case 'info': return 'fas fa-info-circle';
            default: return 'fas fa-bell';
        }
    }

    function showToast(title, message, type) {
        const container = document.getElementById('toastContainer');
        if (!container) return;

        const toast = document.createElement('div');
        toast.className = 'toast-notification toast-' + type;
        toast.innerHTML =
            '<div class="toast-icon"><i class="' + getIconClass(type) + '"></i></div>' +
            '<div class="toast-content">' +
            '<div class="toast-title">' + escapeHtml(title) + '</div>' +
            '<div class="toast-message">' + escapeHtml(message) + '</div>' +
            '</div>' +
            '<button class="toast-close"><i class="fas fa-times"></i></button>';

        container.appendChild(toast);

        toast.querySelector('.toast-close').addEventListener('click', function () {
            toast.classList.add('toast-out');
            setTimeout(function () { toast.remove(); }, 300);
        });

        setTimeout(function () {
            if (toast.parentElement) {
                toast.classList.add('toast-out');
                setTimeout(function () { toast.remove(); }, 300);
            }
        }, 5000);
    }

    function addNotificationToDropdown(title, message, type) {
        const list = document.getElementById('notificationList');
        if (!list) return;

        const empty = list.querySelector('.notification-empty');
        if (empty) empty.remove();

        const now = new Date();
        const timeStr = now.getHours().toString().padStart(2, '0') + ':' + now.getMinutes().toString().padStart(2, '0');

        const item = document.createElement('div');
        item.className = 'notification-item';
        item.innerHTML =
            '<div class="notification-item-icon ' + type + '"><i class="' + getIconClass(type) + '"></i></div>' +
            '<div class="notification-item-content">' +
            '<div class="notification-item-title">' + escapeHtml(title) + '</div>' +
            '<div class="notification-item-message">' + escapeHtml(message) + '</div>' +
            '<div class="notification-item-time">' + timeStr + '</div>' +
            '</div>';

        list.insertBefore(item, list.firstChild);

        // Update counter
        notificationCounter++;
        const countEl = document.getElementById('notificationCount');
        if (countEl) {
            countEl.textContent = notificationCounter;
            countEl.style.display = 'flex';
        }
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Handle incoming notifications
    connection.on('ReceiveNotification', function (title, message, type) {
        showToast(title, message, type);
        addNotificationToDropdown(title, message, type);
    });

    // Start connection
    connection.start().catch(function (err) {
        console.warn('SignalR connection error:', err);
    });

    // Reconnection handling
    connection.onreconnecting(function () {
        console.log('SignalR reconnecting...');
    });

    connection.onreconnected(function () {
        console.log('SignalR reconnected.');
    });
})();
