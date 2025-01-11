"use strict";
class ProductFormUpsert {
    constructor(productId, discountId, nameId, categoryId, priceId, shortDescriptionId, textEditorSelector, saveButtonSelector, photoUploader, discountHandler) {
        this.photoUploader = photoUploader;
        this.discountHandler = discountHandler;
        this.productIdInput = document.querySelector(`#${productId}`);
        this.discountIdInput = document.querySelector(`#${discountId}`);
        this.nameInput = document.querySelector(`#${nameId}`);
        this.categoryInput = document.querySelector(`#${categoryId}`);
        this.priceInput = document.querySelector(`#${priceId}`);
        this.shortDescriptionInput = document.querySelector(`#${shortDescriptionId}`);
        this.textEditor = document.querySelector(textEditorSelector);
        this.saveButton = document.querySelector(saveButtonSelector);
        this.attachSaveButtonHandler();
    }
    attachSaveButtonHandler() {
        this.saveButton.addEventListener("click", async (event) => {
            event.preventDefault();
            if (this.validateInputs()) {
                await this.handleProductUpsert();
                console.log('dupek');
            }
        });
    }
    validateInputs() {
        let isValid = true;
        if (!this.photoUploader.validateMainPhoto())
            isValid = false;
        if (!this.discountHandler.validateInputs())
            isValid = false;
        if (!this.validateName())
            isValid = false;
        if (!this.validateCategory())
            isValid = false;
        if (!this.validatePrice())
            isValid = false;
        if (!this.validateShortDescription())
            isValid = false;
        return isValid;
    }
    validateName() {
        var _a;
        let isValid = true;
        const nameValue = this.nameInput.value;
        const nameValidationParagraph = (_a = this.nameInput.parentElement) === null || _a === void 0 ? void 0 : _a.querySelector('p');
        nameValidationParagraph.innerText = "";
        if (!nameValue || nameValue.length < 1 || nameValue.length > 100) {
            nameValidationParagraph.innerText = "Product name must be between 1 and 100 characters.";
            isValid = false;
        }
        return isValid;
    }
    validateCategory() {
        var _a;
        let isValid = true;
        const categoryValue = this.categoryInput.value;
        const cateogryValidationParagraph = (_a = this.categoryInput.parentElement) === null || _a === void 0 ? void 0 : _a.querySelector('p');
        cateogryValidationParagraph.innerText = "";
        if (!categoryValue || categoryValue === "") {
            cateogryValidationParagraph.innerText = "Please select a valid category.";
            isValid = false;
        }
        return isValid;
    }
    validateShortDescription() {
        var _a;
        let isValid = true;
        const shortDescriptionValue = this.shortDescriptionInput.value;
        const shortDescriptionParagraph = (_a = this.shortDescriptionInput.parentElement) === null || _a === void 0 ? void 0 : _a.querySelector('p');
        shortDescriptionParagraph.innerText = "";
        if (!shortDescriptionValue || shortDescriptionValue.length < 1 || shortDescriptionValue.length > 500) {
            shortDescriptionParagraph.innerText = "Short description must be between 1 and 500 characters.";
        }
        return isValid;
    }
    validatePrice() {
        var _a;
        let isValid = true;
        const priceValue = this.priceInput.value;
        const priceValidationParagraph = (_a = this.priceInput.parentElement) === null || _a === void 0 ? void 0 : _a.querySelector('p');
        priceValidationParagraph.innerText = "";
        if (!priceValue || isNaN(Number(priceValue)) || parseFloat(priceValue) <= 0) {
            priceValidationParagraph.innerText = "Price must be a number greater than 0.";
            isValid = false;
        }
        return isValid;
    }
    getEditorContent() {
        return this.textEditor.innerHTML.trim();
    }
    async prepareFormData() {
        const editorContent = this.getEditorContent();
        const formData = this.photoUploader.preparePhotosToUpload();
        const discountFormData = this.discountHandler.prepareDiscountToUpload();
        for (const [key, value] of discountFormData.entries()) {
            formData.append(key, value);
        }
        formData.append("Id", this.productIdInput.value.trim());
        formData.append("DiscountId", this.discountIdInput.value.trim());
        formData.append("Name", this.nameInput.value.trim());
        formData.append("ShortDescription", this.shortDescriptionInput.value.trim());
        formData.append("Price", this.priceInput.value.trim());
        formData.append("FullDescription", editorContent);
        formData.append("CategoryId", this.categoryInput.value.trim());
        return formData;
    }
    async handleProductUpsert() {
        try {
            const formData = await this.prepareFormData();
            const response = await fetch("/Admin/Product/UpsertAjax", {
                method: "POST",
                body: formData,
                headers: {
                    "X-Requested-With": "XMLHttpRequest",
                },
            });
            if (!response.ok) {
                throw new Error("Failed to upsert product.");
            }
        }
        catch (error) {
            console.error("Error during product upsert:", error);
        }
        finally {
            window.location.href = "/Admin/Product/Index";
        }
    }
}
//# sourceMappingURL=ProductFormUpsert.js.map