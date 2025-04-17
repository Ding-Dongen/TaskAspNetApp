function toggleClientMenu(clientId) {
    console.log("Toggle menu triggered for clientId:", clientId);
    const overlayId = 'overlay-' + clientId;
    const overlayMenu = document.getElementById(overlayId);
    if (!overlayMenu) {
        console.error("Overlay menu not found:", overlayId);
        return;
    }

    document.querySelectorAll('.overlay-menu').forEach(menu => {
        if (menu.id !== overlayId) {
            menu.style.display = 'none';
        }
    });

    overlayMenu.style.display = overlayMenu.style.display === 'block' ? 'none' : 'block';
    console.log("Menu display status:", overlayMenu.style.display);
}

function openEditClientModal(clientId) {
    toggleClientMenu(clientId);

    const modal = document.getElementById('editMemberModal-' + clientId);
    const container = document.getElementById('editMemberContainer-' + clientId);

    if (!modal || !container) {
        console.error('Modal elements not found for clientId:', clientId);
        return;
    }

    fetch(`/Client/Edit/${clientId}`)
        .then(response => response.text())
        .then(html => {
            container.innerHTML = html;
            modal.style.display = 'block';
        })
        .catch(error => {
            console.error('Error loading edit form:', error);
        });
}

function openClientDetailsModal(clientId) {
    toggleClientMenu(clientId);

    const modal = document.getElementById('detailsModal-' + clientId);

    if (!modal) {
        console.error('Details modal not found for clientId:', clientId);
        return;
    }

    modal.style.display = 'block';
}

// Close modals on overlay click
document.addEventListener('click', function (event) {
    if (!event.target.closest('.overlay-menu') && !event.target.closest('.dotes')) {
        document.querySelectorAll('.overlay-menu').forEach(menu => {
            menu.style.display = 'none';
        });
    }

    if (event.target.classList.contains('upload-modal-overlay')) {
        event.target.style.display = 'none';
    }
});

// Close modal on close button click
document.querySelectorAll('.close-modal').forEach(button => {
    button.addEventListener('click', function () {
        this.closest('.upload-modal-overlay').style.display = 'none';
    });
});
