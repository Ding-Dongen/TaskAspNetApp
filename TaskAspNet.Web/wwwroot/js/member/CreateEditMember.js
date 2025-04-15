document.addEventListener("DOMContentLoaded", function () {
    const openModalBtn = document.getElementById("btnOpenCreateModal");
    const modal = document.getElementById("createMemberModal");

    
    if (openModalBtn && modal) {
        openModalBtn.addEventListener("click", function () {
            openModal("createMemberModal");
        });
    }
 
    initializeAddressAndPhoneHandlers();
    initializeImageModalHandlers();
    showCameraOverlayIfNoImage();
});

function toggleEditIconBasedOnImage(previewElement, container) {
    if (!previewElement || !container) return;

    const src = previewElement.src;
    const defaultFilename = "default.png";

    if (src && !src.endsWith(defaultFilename)) {
        container.classList.add("has-image");
    } else {
        container.classList.remove("has-image");
    }
}


function initializeAddressAndPhoneHandlers() {
   
    document.querySelectorAll('.btn-add-address').forEach(button => {
        button.addEventListener('click', function () {
            const type = this.dataset.type;
            addAddress(type);
        });
    });

    
    document.querySelectorAll('.btn-add-phone').forEach(button => {
        button.addEventListener('click', function () {
            const type = this.dataset.type;
            addPhone(type);
        });
    });
}

function initializeAccordionHandlers() {
    const toggles = document.querySelectorAll(".accordion-toggle");

    toggles.forEach(button => {
        button.addEventListener("click", () => {
            const targetId = button.getAttribute("data-target");
            const target = document.getElementById(targetId);

            
            document.querySelectorAll(".accordion-content").forEach(panel => {
                if (panel !== target) {
                    panel.style.display = "none";
                }
            });

            
            if (target.style.display === "block") {
                target.style.display = "none";
            } else {
                target.style.display = "block";
            }
        });
    });
}

// Run it
document.addEventListener("DOMContentLoaded", function () {
    initializeAccordionHandlers();
});


function addAddress(type) {
    const container = document.getElementById("addressesContainer");
    const index = container.querySelectorAll(".address-block").length;

    const html = `
        <div class="address-block" data-type="${type}">
            <div class="address-header">
                <span class="address-type">${type}</span>
                <button type="button" class="btn-remove" onclick="removeThis(this)">
                    <i class="fa-solid fa-times"></i>
                </button>
            </div>
            <input type="hidden" name="Addresses[${index}].AddressType" value="${type}" />
            <div class="form-row">
                <input name="Addresses[${index}].Address" class="form-control" placeholder="Street Address" />
            </div>
            <div class="form-row">
                <input name="Addresses[${index}].ZipCode" class="form-control" placeholder="Zip Code" />
            </div>
            <div class="form-row">
                <input name="Addresses[${index}].City" class="form-control" placeholder="City" />
            </div>
        </div>
    `;
    container.insertAdjacentHTML("beforeend", html);
}

function addPhone(type) {
    const container = document.getElementById("phonesContainer");
    const index = container.querySelectorAll(".phone-block").length;

    const html = `
        <div class="phone-block" data-type="${type}">
            <div class="phone-header">
                <span class="phone-type">${type}</span>
                <button type="button" class="btn-remove" onclick="removeThis(this)">
                    <i class="fa-solid fa-times"></i>
                </button>
            </div>
            <input type="hidden" name="Phones[${index}].PhoneType" value="${type}" />
            <div class="form-row">
                <input name="Phones[${index}].Phone" class="form-control" placeholder="Phone Number" />
            </div>
        </div>
    `;
    container.insertAdjacentHTML("beforeend", html);
}

function removeThis(button) {
    button.closest(".address-block, .phone-block")?.remove();
}

function openModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = "flex";
    }
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = "none";
    }
}

/*Edit member modal*/
function openEditModal(memberId) {
    
    fetch(`/Member/Edit/${memberId}`, {
        method: "GET",
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not OK");
            }
            return response.text();
        })
        .then(html => {
            
            document.getElementById("editMemberContainer").innerHTML = html;

           
            openModal("editMemberModal");

            
            initializeImageModalHandlers();
            initializeAccordionHandlers();  
            initializeAddressAndPhoneHandlers();
        })
        .catch(error => {
            console.error("Failed to load edit form:", error);
        });
}

function showCameraOverlayIfNoImage() {
    const preview = document.getElementById("cameraPreview");
    const overlay = document.getElementById("cameraOverlay");
    const penIcon = document.querySelector(".edit-icon");

    if (!preview || !overlay) return;

    const defaultImageName = "default.png";
    const isDefault = preview.src.includes(defaultImageName);

    
    overlay.style.opacity = isDefault ? "1" : "0";

    
    preview.style.opacity = isDefault ? "0" : "1";

    
    if (penIcon) penIcon.style.display = isDefault ? "none" : "flex";
}


// Function to initialize image modal handlers
function initializeImageModalHandlers() {
    const openUploadModalBtn = document.getElementById("openUploadModal");
    const uploadModal = document.getElementById("uploadModal");
    const triggerFileInput = document.getElementById("triggerFileInput");
    const fileInput = document.getElementById("fileInput");
    const imagePreview = document.getElementById("imagePreview");
    const hiddenCurrentImage = document.getElementById("hiddenCurrentImage");
    const hiddenSelectedImage = document.getElementById("hiddenSelectedImage");
    const selectImage = document.getElementById("selectImage");
    const saveBtn = document.getElementById("saveImageSelection");

    
    if (openUploadModalBtn && uploadModal) {
        openUploadModalBtn.addEventListener("click", function () {
            openModal("uploadModal");
        });
    }

    
    if (triggerFileInput && fileInput && imagePreview) {
        triggerFileInput.addEventListener("click", function () {
            fileInput.click();
        });

        fileInput.addEventListener("change", function () {
            if (this.files && this.files[0]) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    imagePreview.src = e.target.result;
                    if (hiddenCurrentImage) {
                        hiddenCurrentImage.value = "";
                    }
                    if (hiddenSelectedImage) {
                        hiddenSelectedImage.value = "";
                    }
                };
                reader.readAsDataURL(this.files[0]);
            }
        });
    }

    // Predefined image selection handler
    if (selectImage && imagePreview) {
        selectImage.addEventListener("change", function () {
            const selected = this.value;
            if (selected) {
                imagePreview.src = "/images/predefined/" + selected;
                if (hiddenSelectedImage) {
                    hiddenSelectedImage.value = selected;
                }
                if (hiddenCurrentImage) {
                    hiddenCurrentImage.value = "";
                }
                if (fileInput) {
                    fileInput.value = "";
                }
            }
        });
    }

   

    // Save selection handler
    if (saveBtn && imagePreview) {
        saveBtn.addEventListener("click", function () {
            const chosenSrc = imagePreview.src;
            const cameraPreview = document.getElementById("cameraPreview");

            if (cameraPreview) {
                cameraPreview.src = chosenSrc;
                showCameraOverlayIfNoImage();
            }

            if (hiddenCurrentImage) {
                if (chosenSrc.startsWith("data:")) {
                    hiddenCurrentImage.value = chosenSrc;
                    if (hiddenSelectedImage) {
                        hiddenSelectedImage.value = "";
                    }
                } else if (chosenSrc.includes("/predefined/")) {
                    hiddenCurrentImage.value = chosenSrc;
                    if (hiddenSelectedImage) {
                        hiddenSelectedImage.value = chosenSrc.split("/predefined/")[1];
                    }
                }
            }

            closeModal("uploadModal");
        });
    }
}

// -- OPEN UPLOAD MODAL --
const openUploadModalBtn = document.getElementById("openUploadModal");
const uploadModal = document.getElementById("uploadModal");
const closeModalBtn = uploadModal?.querySelector(".close-modal");

if (openUploadModalBtn && uploadModal) {
    openUploadModalBtn.addEventListener("click", function () {
        uploadModal.style.display = "flex";
    });
}


if (closeModalBtn) {
    closeModalBtn.addEventListener("click", function () {
        uploadModal.style.display = "none";
    });
}


window.addEventListener("click", function (event) {
    if (event.target === uploadModal) {
        uploadModal.style.display = "none";
    }
});

// -- CHOOSE FILE & PREVIEW --
const triggerFileInput = document.getElementById("triggerFileInput");
const fileInput = document.getElementById("fileInput");
const imagePreview = document.getElementById("imagePreview");
const hiddenCurrentImage = document.getElementById("hiddenCurrentImage");
const hiddenSelectedImage = document.getElementById("hiddenSelectedImage");

if (triggerFileInput && fileInput && imagePreview) {
    triggerFileInput.addEventListener("click", function () {
        fileInput.click();
    });

    fileInput.addEventListener("change", function () {
        if (this.files && this.files[0]) {
            const reader = new FileReader();
            reader.onload = function (e) {
                imagePreview.src = e.target.result;
                if (hiddenCurrentImage) {
                    hiddenCurrentImage.value = ""; 
                }
                if (hiddenSelectedImage) {
                    hiddenSelectedImage.value = ""; 
                }
            };
            reader.readAsDataURL(this.files[0]);
        }
    });
}

// -- SELECT PREDEFINED IMAGE --
const selectImage = document.getElementById("selectImage");
if (selectImage && imagePreview) {
    selectImage.addEventListener("change", function () {
        const selected = this.value;
        if (selected) {
            imagePreview.src = "/images/membericon/" + selected;
            if (hiddenSelectedImage) {
                hiddenSelectedImage.value = selected;
            }
            if (hiddenCurrentImage) {
                hiddenCurrentImage.value = ""; 
            }
            
            if (fileInput) {
                fileInput.value = "";
            }
        }
    });
}

// -- SAVE SELECTION (updates the main preview) --
const saveBtn = document.getElementById("saveImageSelection");
if (saveBtn && imagePreview) {
    saveBtn.addEventListener("click", function () {
        const chosenSrc = imagePreview.src;
        const cameraPreview = document.getElementById("cameraPreview");

        if (cameraPreview) {
            cameraPreview.src = chosenSrc;
        }

        
        if (hiddenCurrentImage) {
            if (chosenSrc.startsWith("data:")) {
               
                hiddenCurrentImage.value = chosenSrc;
                if (hiddenSelectedImage) {
                    hiddenSelectedImage.value = "";
                }
            } else if (chosenSrc.includes("/membericon/")) {
                
                hiddenCurrentImage.value = chosenSrc;
                if (hiddenSelectedImage) {
                    hiddenSelectedImage.value = chosenSrc.split("/membericon/")[1];
                }
            }
        }

        
        if (uploadModal) {
            uploadModal.style.display = "none";
        }
    });
}
document.addEventListener("DOMContentLoaded", function () {
    const memberForm = document.getElementById("memberForm");
    if (memberForm) {
        memberForm.addEventListener("submit", function (e) {
            const userId = document.getElementById("UserId")?.value;
            const id = document.getElementById("Id")?.value;
            console.log("📦 Submitting Member Form:");
            console.log("➡️ Id:", id);
            console.log("➡️ UserId:", userId);
        });
    }
});


//document.addEventListener("DOMContentLoaded", function () {
//    const form = document.getElementById("memberForm");

//    const fields = [
//        {
//            el: form.querySelector("[name='FirstName']"),
//            validate: val => val.trim().length > 0,
//            message: "First Name is required."
//        },
//        {
//            el: form.querySelector("[name='LastName']"),
//            validate: val => val.trim().length > 0,
//            message: "Last Name is required."
//        },
//        {
//            el: form.querySelector("[name='Email']"),
//            validate: val => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val),
//            message: "Enter a valid email address."
//        },
//        {
//            el: form.querySelector("[name='JobTitleId']"),
//            validate: val => val.trim().length > 0,
//            message: "Job Title is required."
//        }
//    ];

//    function showError(el, message) {
//        el.classList.add("input-error");

//        const icon = el.parentNode.querySelector(".error-icon");
//        if (icon) icon.style.display = "block";

//        let msg = el.parentNode.querySelector(".error-message");
//        if (!msg) {
//            msg = document.createElement("div");
//            msg.className = "error-message";
//            el.parentNode.appendChild(msg);
//        }

//        msg.textContent = message;
//    }

//    function clearError(el) {
//        el.classList.remove("input-error");

//        const icon = el.parentNode.querySelector(".error-icon");
//        if (icon) icon.style.display = "none";

//        const msg = el.parentNode.querySelector(".error-message");
//        if (msg) msg.remove();
//    }

//    fields.forEach(field => {
//        field.el.addEventListener("input", () => {
//            if (field.validate(field.el.value)) {
//                clearError(field.el);
//            } else {
//                showError(field.el, field.message);
//            }
//        });
//    });

//    form.addEventListener("submit", function (e) {
//        let hasError = false;

//        fields.forEach(field => {
//            if (!field.validate(field.el.value)) {
//                showError(field.el, field.message);
//                hasError = true;
//            } else {
//                clearError(field.el);
//            }
//        });

//        if (hasError) {
//            e.preventDefault();
//        }
//    });
//});
