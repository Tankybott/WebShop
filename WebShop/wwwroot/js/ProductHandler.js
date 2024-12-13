"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class ProductHandler {
    constructor(productIdSelector, nameSelector, categorySelector, descriptionSelector, editorSelector, photoUploader, extraTopicUploader, saveButtonSelector) {
        this.productIdSelector = productIdSelector;
        this.nameSelector = nameSelector;
        this.categorySelector = categorySelector;
        this.descriptionSelector = descriptionSelector;
        this.editorSelector = editorSelector;
        this.photoUploader = photoUploader;
        this.extraTopicUploader = extraTopicUploader;
        this.saveButtonSelector = saveButtonSelector;
        this.attachSaveButtonHandler();
        this.productIdInput = document.querySelector(productIdSelector);
    }
    attachSaveButtonHandler() {
        const saveButton = document.querySelector(this.saveButtonSelector);
        saveButton === null || saveButton === void 0 ? void 0 : saveButton.addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
            event.preventDefault();
            if (this.validateInputs()) {
                yield this.handleProductUpsert();
            }
        }));
    }
    /**
     * Validates the input fields (Product Name and Short Description).
     * @returns {boolean} - Returns true if valid, otherwise false.
     */
    validateInputs() {
        let isValid = true;
        const inputs = document.querySelectorAll("[data-validation]");
        inputs.forEach((input) => {
            const field = input;
            const errorParagraph = field.nextElementSibling;
            const value = field.value.trim();
            const validationType = field.getAttribute("data-validation");
            let errorMessage = "";
            if (validationType === "product-name") {
                if (!value || value.length < 1 || value.length > 100) {
                    errorMessage = "Product name must be between 1 and 100 characters.";
                }
            }
            else if (validationType === "short-description") {
                if (!value || value.length < 1 || value.length > 500) {
                    errorMessage = "Short description must be between 1 and 500 characters.";
                }
            }
            else if (validationType === "category") {
                if (!value || value === "") {
                    errorMessage = "Please select a valid category.";
                }
            }
            if (errorMessage) {
                isValid = false;
                errorParagraph.textContent = errorMessage; // Display error message in <p>
                field.classList.add("is-invalid"); // Add invalid styling
            }
            else {
                errorParagraph.textContent = ""; // Clear any previous error message
                field.classList.remove("is-invalid"); // Remove invalid styling
            }
        });
        return isValid;
    }
    /**
     * Retrieves the content from the Quill editor container.
     * @returns {string} - The content of the editor.
     */
    getEditorContent() {
        const editorElement = document.querySelector(this.editorSelector);
        return editorElement ? editorElement.innerHTML.trim() : '';
    }
    /**
     * Prepares the form data for submission.
     * @returns {FormData} - The complete form data including product details and photos.
     */
    prepareFormData() {
        return __awaiter(this, void 0, void 0, function* () {
            const nameInput = document.querySelector(this.nameSelector);
            const descriptionInput = document.querySelector(this.descriptionSelector);
            const categorySelect = document.querySelector(this.categorySelector);
            // Get content from Quill editor
            const editorContent = this.getEditorContent();
            // Await the asynchronous photo upload preparation
            const formData = this.photoUploader.preparePhotosToUpload();
            const extraTopicsFormData = this.extraTopicUploader.prepareTopicsToUpload();
            for (const [key, value] of extraTopicsFormData.entries()) {
                formData.append(key, value);
            }
            for (const [key, value] of formData.entries()) {
                console.log(`${key}: ${value}`);
            }
            formData.append("Id", this.productIdInput.value.trim());
            formData.append("Name", nameInput.value.trim());
            formData.append("ShortDescription", descriptionInput.value.trim());
            formData.append("FullDescription", editorContent);
            formData.append("CategoryId", categorySelect.value.trim());
            formData.append("Discount.StartTime", discountStartTimeInput.value.trim());
            formData.append("Discount.EndTime", discountEndTimeInput.value.trim());
            formData.append("Discount.Percentage", discountPercentageInput.value.trim());
            return formData;
        });
    }
    handleProductUpsert() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                const formData = yield this.prepareFormData(); // Await the resolved FormData
                const response = yield fetch("/Admin/Product/UpsertAjax", {
                    method: "POST",
                    body: formData, // Resolved FormData passed here
                    headers: {
                        "X-Requested-With": "XMLHttpRequest",
                    },
                });
                if (!response.ok) {
                    throw new Error("Failed to upsert product.");
                }
                const data = yield response.json();
                console.log("Product upserted successfully:", data);
                window.location.href = "/Admin/Product/Index";
            }
            catch (error) {
                console.error("Error during product upsert:", error);
                window.location.href = "/Admin/Product/Index";
            }
        });
    }
}
//# sourceMappingURL=ProductHandler.js.map