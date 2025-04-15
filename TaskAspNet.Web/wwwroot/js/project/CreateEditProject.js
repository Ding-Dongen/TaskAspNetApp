document.addEventListener("DOMContentLoaded", function () {
    const openModalBtn = document.getElementById("btnOpenCreateModal");
    const modal = document.getElementById("createProjectModal");

    if (openModalBtn && modal) {
        openModalBtn.addEventListener("click", function () {
            openModal("createProjectModal");

            
            setTimeout(() => {
                initializeImageModalHandlers();
                showCameraOverlayIfNoImage();

                if (typeof initializeQuillEditors === 'function') {
                    initializeQuillEditors();
                }
            }, 50); 
        });
    }

    
    if (document.getElementById("editProjectModal")?.style.display === "block") {
        initializeImageModalHandlers();
        showCameraOverlayIfNoImage();
    }
});


/*Edit project modal*/
function openEditModal(projectId) {
    fetch(`/Project/Edit/${projectId}`, {
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
            document.getElementById("editProjectContainer").innerHTML = html;

            openModal("editProjectModal");
            initializeImageModalHandlers();

            if (typeof initializeQuillEditors === 'function') {
                initializeQuillEditors();
            }
        })
        .catch(error => {
            console.error("Failed to load edit form:", error);
        });
}

// Function to initialize image modal handlers

function showCameraOverlayIfNoImage() {
    const preview = document.getElementById("cameraPreview");
    const overlay = document.getElementById("cameraOverlay");
    const editIcon = document.querySelector(".edit-icon");

    if (!preview || !overlay) {
        console.log("Missing preview or overlay element.");
        return;
    }

    const isDefault = preview.src.includes("default.png");

    if (isDefault) {
        overlay.style.opacity = "1";            
        preview.style.opacity = "0";            
        if (editIcon) editIcon.style.display = "none";  
    } else {
        overlay.style.opacity = "0";            
        preview.style.opacity = "1";            
        if (editIcon) editIcon.style.display = "flex";  
    }
}




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

            showCameraOverlayIfNoImage();

            closeModal("uploadModal");
        });
    }
}

document.addEventListener("DOMContentLoaded", initializeImageModalHandlers);

/*Create Project modal add members search*/

document.addEventListener("DOMContentLoaded", () => {
    const memberSearchInput = document.getElementById("memberSearchInput");
    const memberSearchResults = document.getElementById("memberSearchResults");
    const selectedChipsContainer = document.getElementById("selectedChipsContainer");
    const memberInputsContainer = document.getElementById("memberInputsContainer");

    if (!memberSearchInput) return;

    memberSearchInput.addEventListener("input", async () => {
        const query = memberSearchInput.value.trim();

        if (query.length < 2) {
            memberSearchResults.innerHTML = "";
            return;
        }

        try {
            const response = await fetch("/Member/Search?term=" + encodeURIComponent(query));
            if (!response.ok) {
                console.error("Member search failed:", response.status);
                return;
            }

            const members = await response.json();
            memberSearchResults.innerHTML = "";

            members.forEach(member => {
                const li = document.createElement("li");
                li.style.display = "flex";
                li.style.alignItems = "center";
                li.style.gap = "8px";
                li.style.padding = "8px";
                li.style.cursor = "pointer";

                
                const img = document.createElement("img");
                img.src = member.imageData?.currentImage || '/images/membericon/default.png';
                img.style.width = "30px";
                img.style.height = "30px";
                img.style.borderRadius = "50%";
                img.style.objectFit = "cover";
                li.appendChild(img);

                
                const textContent = document.createElement("span");
                textContent.textContent = `${member.firstName} ${member.lastName} (${member.email})`;
                li.appendChild(textContent);

                li.classList.add("search-result");

                li.addEventListener("click", () => {
                    addMemberAsChip(member);
                });

                memberSearchResults.appendChild(li);
            });
        } catch (err) {
            console.error("Error searching members:", err);
        }
    });

    function addMemberAsChip(member) {
        const hiddenInput = document.createElement("input");
        hiddenInput.type = "hidden";
        hiddenInput.name = "SelectedMemberIds";
        hiddenInput.value = member.id.toString();
        memberInputsContainer.appendChild(hiddenInput);

        const chip = document.createElement("span");
        chip.className = "chip";
        chip.style.display = "flex";
        chip.style.alignItems = "center";
        chip.style.gap = "4px";

        const img = document.createElement("img");
        img.src = member.imageData?.currentImage || '/images/membericon/default.png';
        img.style.width = "20px";
        img.style.height = "20px";
        img.style.borderRadius = "50%";
        img.style.objectFit = "cover";
        chip.appendChild(img);

        const nameSpan = document.createElement("span");
        nameSpan.textContent = `${member.firstName} ${member.lastName}`;
        chip.appendChild(nameSpan);

        const removeX = document.createElement("span");
        removeX.className = "remove-x";
        removeX.textContent = "x";
        removeX.addEventListener("click", () => {
            memberInputsContainer.removeChild(hiddenInput);
            selectedChipsContainer.removeChild(chip);
        });

        chip.appendChild(removeX);
        selectedChipsContainer.appendChild(chip);

        memberSearchInput.value = "";
        memberSearchResults.innerHTML = "";
    }
});