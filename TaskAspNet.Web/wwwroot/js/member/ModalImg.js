document.addEventListener("DOMContentLoaded", function () {
    const triggerFileInput = document.getElementById("triggerFileInput");
    const fileInput = document.getElementById("fileInput");
    const imagePreview = document.getElementById("imagePreview");
    const hiddenCurrentImage = document.getElementById("hiddenCurrentImage");
    const hiddenSelectedImage = document.getElementById("hiddenSelectedImage");
    const selectImage = document.getElementById("selectImage");
    const saveBtn = document.getElementById("saveImageSelection");

    if (triggerFileInput && fileInput) {
        triggerFileInput.addEventListener("click", function () {
            fileInput.click();
        });
    }

    if (fileInput && imagePreview) {
        fileInput.addEventListener("change", function () {
            const file = this.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    imagePreview.src = e.target.result;
                    imagePreview.style.display = "block";
                };
                reader.readAsDataURL(file);
            }
        });
    }

    if (selectImage && imagePreview) {
        selectImage.addEventListener("change", function () {
            const selectedValue = this.value;
            if (selectedValue) {
                imagePreview.src = "/images/membericon/" + selectedValue;
                imagePreview.style.display = "block";
            }
        });
    }

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

            const uploadModal = document.getElementById("uploadModal");
            if (uploadModal) {
                uploadModal.style.display = "none";
            }
        });
    }
}); 