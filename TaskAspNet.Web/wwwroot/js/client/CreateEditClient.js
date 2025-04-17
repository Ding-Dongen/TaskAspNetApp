document.addEventListener('DOMContentLoaded', function () {
    initializeAddressAndPhoneHandlers();
    initializeAccordionHandlers();
});

function initializeAddressAndPhoneHandlers() {
    // Address handlers
    document.querySelectorAll('.btn-add-address').forEach(button => {
        button.addEventListener('click', function () {
            const type = this.dataset.type;
            addAddress(type);
        });
    });

    // Phone handlers
    document.querySelectorAll('.btn-add-phone').forEach(button => {
        button.addEventListener('click', function () {
            const type = this.dataset.type;
            addPhone(type);
        });
    });
}

function initializeAccordionHandlers() {
    document.querySelectorAll('.accordion-toggle').forEach(button => {
        button.addEventListener('click', function () {
            const targetId = this.dataset.target;
            const content = document.getElementById(targetId);
            
            // Toggle the content
            content.style.display = content.style.display === 'none' ? 'block' : 'none';
            
            // Toggle the button icon
            const icon = this.querySelector('i');
            if (content.style.display === 'none') {
                icon.classList.remove('fa-chevron-down');
                icon.classList.add('fa-chevron-right');
            } else {
                icon.classList.remove('fa-chevron-right');
                icon.classList.add('fa-chevron-down');
            }
        });
    });
}

function addAddress(type) {
    const addressesList = document.getElementById('addressesList');
    const index = addressesList.children.length;

    const addressBlock = document.createElement('div');
    addressBlock.className = 'address-block';
    addressBlock.dataset.type = type;

    addressBlock.innerHTML = `
        <div class="address-header">
            <span class="address-type">${type}</span>
            <button type="button" class="btn-remove" onclick="removeThis(this)">
                <i class="fa-solid fa-times"></i>
            </button>
        </div>
        <input type="hidden" name="Addresses[${index}].Id" value="0" />
        <input type="hidden" name="Addresses[${index}].AddressType" value="${type}" />
        <div class="form-row">
            <input name="Addresses[${index}].Address" class="form-control" placeholder="Street Address" required />
        </div>
        <div class="form-row">
            <input name="Addresses[${index}].ZipCode" class="form-control" placeholder="Zip Code" required />
        </div>
        <div class="form-row">
            <input name="Addresses[${index}].City" class="form-control" placeholder="City" required />
        </div>
    `;

    addressesList.appendChild(addressBlock);
}

function addPhone(type) {
    const phonesList = document.getElementById('phonesList');
    const index = phonesList.children.length;

    const phoneBlock = document.createElement('div');
    phoneBlock.className = 'phone-block';
    phoneBlock.dataset.type = type;

    phoneBlock.innerHTML = `
        <div class="phone-header">
            <span class="phone-type">${type}</span>
            <button type="button" class="btn-remove" onclick="removeThis(this)">
                <i class="fa-solid fa-times"></i>
            </button>
        </div>
        <input type="hidden" name="Phones[${index}].Id" value="0" />
        <input type="hidden" name="Phones[${index}].PhoneType" value="${type}" />
        <div class="form-row">
            <input name="Phones[${index}].Phone" class="form-control" placeholder="Phone Number" required />
        </div>
    `;

    phonesList.appendChild(phoneBlock);
}

function removeThis(button) {
    const block = button.closest('.address-block, .phone-block');
    block.remove();
} 