// Dashboard JS - Sidebar toggle, notification dropdown, general UI
(function () {
    'use strict';

    // Sidebar Toggle
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarClose = document.getElementById('sidebarClose');
    const mainWrapper = document.getElementById('mainWrapper');

    function createOverlay() {
        let overlay = document.querySelector('.sidebar-overlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.className = 'sidebar-overlay';
            document.body.appendChild(overlay);
            overlay.addEventListener('click', closeSidebar);
        }
        return overlay;
    }

    function openSidebar() {
        if (!sidebar) return;
        sidebar.classList.add('show');
        const overlay = createOverlay();
        overlay.classList.add('show');
    }

    function closeSidebar() {
        if (!sidebar) return;
        sidebar.classList.remove('show');
        const overlay = document.querySelector('.sidebar-overlay');
        if (overlay) overlay.classList.remove('show');
    }

    if (sidebarToggle) sidebarToggle.addEventListener('click', openSidebar);
    if (sidebarClose) sidebarClose.addEventListener('click', closeSidebar);

    // Notification Dropdown
    const notificationBtn = document.getElementById('notificationBtn');
    const notificationDropdown = document.getElementById('notificationDropdown');

    if (notificationBtn && notificationDropdown) {
        notificationBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            notificationDropdown.classList.toggle('show');
        });

        document.addEventListener('click', function (e) {
            if (!notificationDropdown.contains(e.target) && e.target !== notificationBtn) {
                notificationDropdown.classList.remove('show');
            }
        });
    }

    // Clear Notifications
    const clearBtn = document.getElementById('clearNotifications');
    if (clearBtn) {
        clearBtn.addEventListener('click', function () {
            const list = document.getElementById('notificationList');
            if (list) {
                list.innerHTML = '<div class="notification-empty"><i class="fas fa-bell-slash"></i><p>لا توجد إشعارات</p></div>';
            }
            const count = document.getElementById('notificationCount');
            if (count) {
                count.style.display = 'none';
                count.textContent = '0';
            }
        });
    }

    // Auto-dismiss alerts after 5 seconds
    document.querySelectorAll('.alert-dismissible').forEach(function (alert) {
        setTimeout(function () {
            const closeBtn = alert.querySelector('.btn-close');
            if (closeBtn) closeBtn.click();
        }, 5000);
    });

    // Staggered animation for cards
    document.querySelectorAll('.col-xl-3, .col-xl-4, .col-lg-4, .col-lg-6').forEach(function (el, i) {
        el.style.animationDelay = (i * 0.08) + 's';
    });

})();
