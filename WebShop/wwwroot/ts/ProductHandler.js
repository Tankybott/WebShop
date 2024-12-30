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
    constructor(productIdSelector, discountIdSelector, nameSelector, categorySelector, descriptionSelector, editorSelector, photoUploader, saveButtonSelector, discountStartTimeInput, discountEndTimeInput, discountPercentageInput) {
        this.nameSelector = nameSelector;
        this.categorySelector = categorySelector;
        this.descriptionSelector = descriptionSelector;
        this.editorSelector = editorSelector;
        this.photoUploader = photoUploader;
        this.saveButtonSelector = saveButtonSelector;
        this.discountStartTimeInput = discountStartTimeInput;
        this.discountEndTimeInput = discountEndTimeInput;
        this.discountPercentageInput = discountPercentageInput;
        this.isDiscountChanged = false;
        this.attachSaveButtonHandler();
        this.productIdInput = document.querySelector(productIdSelector);
        this.discountIdInput = document.querySelector(discountIdSelector);
        this.discountStartTimeInput = document.getElementById("discount-start");
        this.discountEndTimeInput = document.getElementById("discount-end");
        this.discountPercentageInput = document.getElementById("discount-percentage");
        this.discountStartTimeInput.addEventListener('change', () => {
            this.isDiscountChanged = true;
            console.log(this.isDiscountChanged);
        });
        this.discountEndTimeInput.addEventListener('change', () => this.isDiscountChanged = true);
        this.discountPercentageInput.addEventListener('change', () => this.isDiscountChanged = true);
    }
    attachSaveButtonHandler() {
        const saveButton = document.querySelector("#save-button");
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
            // Add event listeners for change detection
            // Get content from Quill editor
            const editorContent = this.getEditorContent();
            // Await the asynchronous photo upload preparation
            const formData = this.photoUploader.preparePhotosToUpload();
            //for (const [key, value] of formData.entries()) {
            //    console.log(`${key}: ${value}`);
            //}
            formData.append("Id", this.productIdInput.value.trim());
            formData.append("DiscountId", this.discountIdInput.value.trim());
            formData.append("Name", nameInput.value.trim());
            formData.append("ShortDescription", descriptionInput.value.trim());
            formData.append("FullDescription", editorContent);
            formData.append("CategoryId", categorySelect.value.trim());
            formData.append("DiscountStartDate", this.discountStartTimeInput.value.trim());
            formData.append("DiscountEndDate", this.discountEndTimeInput.value.trim());
            formData.append("DiscountPercentage", this.discountPercentageInput.value.trim());
            formData.append("IsDisocuntChanged", this.isDiscountChanged.toString());
            return formData;
        });
    }
    handleProductUpsert() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                const formData = yield this.prepareFormData();
                const response = yield fetch("/Admin/Product/UpsertAjax", {
                    method: "POST",
                    body: formData,
                    headers: {
                        "X-Requested-With": "XMLHttpRequest",
                    },
                });
                if (!response.ok) {
                    throw new Error("Failed to upsert product.");
                }
                console.log("Product upserted successfully.");
            }
            catch (error) {
                console.error("Error during product upsert:", error);
            }
            finally {
                window.location.href = "/Admin/Product/Index";
            }
        });
    }
}
