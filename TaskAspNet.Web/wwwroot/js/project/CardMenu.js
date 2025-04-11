// card-menu.js

let currentOpenMenu = null;

function toggleMenu(projectId) {
    const menu = document.getElementById('overlay-' + projectId);
    if (!menu) return;

    if (currentOpenMenu && currentOpenMenu !== menu) {
        currentOpenMenu.style.display = 'none';
        document.removeEventListener('click', handleClickOutside);
    }

    const isVisible = menu.style.display === 'block';

    if (isVisible) {
        menu.style.display = 'none';
        document.removeEventListener('click', handleClickOutside);
        currentOpenMenu = null;
    } else {
        menu.style.display = 'block';
        currentOpenMenu = menu;

        setTimeout(() => {
            document.addEventListener('click', handleClickOutside);
        }, 0);
    }

    function handleClickOutside(event) {
        if (!menu.contains(event.target)) {
            menu.style.display = 'none';
            document.removeEventListener('click', handleClickOutside);
            currentOpenMenu = null;
        }
    }
}




// Opens the modal and loads member checkboxes
async function openAddRemoveMemberModal(projectId) {
    console.log(`Opening modal for project ID: ${projectId}`);
    document.getElementById('modalProjectId').value = projectId;
    document.getElementById('addRemoveMemberModal').style.display = 'flex';

    document.getElementById('modalSearchInput').value = '';
    document.getElementById('modalSearchResults').innerHTML = '';

    const modalMemberList = document.getElementById('modalMemberList');
    modalMemberList.innerHTML = '<p>Loading members...</p>';

    try {
        const urlAssigned = `${window.location.origin}/Member/GetMembers?projectId=${projectId}`;
        console.log(`Fetching assigned members from: ${urlAssigned}`);
        const respAssigned = await fetch(urlAssigned);
        if (!respAssigned.ok) throw new Error(`HTTP error! Status: ${respAssigned.status} URL: ${respAssigned.url}`);
        const currentMembers = await respAssigned.json();
        console.log("Current Members:", currentMembers);

        renderCheckboxList(currentMembers);
    } catch (err) {
        console.error("Error loading members:", err);
        document.getElementById('modalMemberList').innerHTML = `<p>No member assigned to the project</p>`;
    }
}

// Renders the checkboxes for members in the modal
function renderCheckboxList(members) {
    const modalMemberList = document.getElementById('modalMemberList');
    modalMemberList.innerHTML = '';

    if (!members || members.length === 0) {
        modalMemberList.innerHTML = '<p>No members assigned to this project.</p>';
        return;
    }
    const html = members.map(member => {
        const avatarUrl = member.imageData?.currentImage || '/images/membericon/default-avatar.png';
        return `
          <label class="member-label">
            <input type="checkbox" name="MemberIds" value="${member.id}" checked>
            <img src="${avatarUrl}" alt="${member.firstName} ${member.lastName}" 
                 style="width: 30px; height: 30px; border-radius: 50%; margin-right: 10px;">
            ${member.firstName} ${member.lastName}
          </label>
        `;
    }).join('');

    modalMemberList.innerHTML = html;
    console.log("Members rendered in modal.");
}

function closeAddRemoveMemberModal() {
    document.getElementById('addRemoveMemberModal').style.display = 'none';
}

document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('modalSearchInput');
    const searchResults = document.getElementById('modalSearchResults');
    if (!searchInput || !searchResults) return;

    searchInput.addEventListener('input', async () => {
        const query = searchInput.value.trim();
        if (query.length < 2) {
            searchResults.innerHTML = '';
            return;
        }
        try {
            const urlSearch = `${window.location.origin}/Member/Search?term=${encodeURIComponent(query)}`;
            console.log(`Searching members from: ${urlSearch}`);
            const resp = await fetch(urlSearch);
            if (!resp.ok) throw new Error(`HTTP error! Status: ${resp.status}`);
            const matches = await resp.json();
            searchResults.innerHTML = '';
            matches.forEach(m => {
                console.log("Member data:", m);
                console.log("Image path:", m.imageData?.currentImage);
                const li = document.createElement('li');
                const img = document.createElement('img');
                img.src = m.imageData?.currentImage || '/images/membericon/default-avatar.png';
                console.log("Final image src:", img.src);
                img.alt = `${m.firstName} ${m.lastName}`;
                img.style.width = '30px';
                img.style.height = '30px';
                img.style.borderRadius = '50%';
                img.style.marginRight = '10px';
                li.appendChild(img);
                li.appendChild(document.createTextNode(`${m.firstName}`));
                const space = document.createTextNode(' ');
                li.appendChild(space);
                li.appendChild(document.createTextNode(`${m.lastName}`));
                li.style.display = 'flex';
                li.style.alignItems = 'center';
                li.style.padding = '5px';
                li.style.cursor = 'pointer';
                li.addEventListener('click', () => addMemberFromSearch(m));
                searchResults.appendChild(li);
            });
        } catch (err) {
            console.error("Search error:", err);
        }
    });
});

// When a search result is clicked, add that member to the checkbox list
function addMemberFromSearch(member) {
    console.log("Member object:", member);
    console.log("Member image data:", member.imageData);
    console.log("Member image path:", member.imageData?.currentImage);

    const modalMemberList = document.getElementById('modalMemberList');
    let checkbox = modalMemberList.querySelector(`input[name="MemberIds"][value="${member.id}"]`);
    if (checkbox) {
        checkbox.checked = true;
        checkbox.scrollIntoView({ behavior: "smooth", block: "center" });
        console.log(`Checkbox for member ${member.id} checked.`);
    } else {
        const avatarUrl = member.imageData?.currentImage || '/images/membericon/default-avatar.png';
        console.log("Using avatar URL:", avatarUrl);
        const label = document.createElement('label');
        label.classList.add("member-label");

        const input = document.createElement('input');
        input.type = 'checkbox';
        input.name = 'MemberIds';
        input.value = member.id;
        input.checked = true;

        const img = document.createElement('img');
        img.src = avatarUrl;
        img.alt = `${member.firstName} ${member.lastName}`;
        img.style.width = '30px';
        img.style.height = '30px';
        img.style.borderRadius = '50%';
        img.style.marginRight = '10px';

        const nameSpan = document.createElement('span');
        nameSpan.textContent = `${member.firstName} ${member.lastName}`;

        label.appendChild(input);
        label.appendChild(img);
        label.appendChild(nameSpan);

        modalMemberList.appendChild(label);
        console.log(`Checkbox for member ${member.id} created and checked.`);
    }

    document.getElementById('modalSearchInput').value = '';
    document.getElementById('modalSearchResults').innerHTML = '';
}


