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
class ProductFormUpsert {
    constructor(productId, discountId, nameId, categoryId, priceId, shippingPriceFactorInputId, stockQuantity, textEditorSelector, saveButtonSelector, photoUploader, discountHandler) {
        this.photoUploader = photoUploader;
        this.discountHandler = discountHandler;
        this.productIdInput = document.querySelector(`#${productId}`);
        this.discountIdInput = document.querySelector(`#${discountId}`);
        this.nameInput = document.querySelector(`#${nameId}`);
        this.categoryInput = document.querySelector(`#${categoryId}`);
        this.priceInput = document.querySelector(`#${priceId}`);
        this.shippingPriceFactorInput = document.querySelector(`#${shippingPriceFactorInputId}`);
        this.stockQuantityInput = document.querySelector(`#${stockQuantity}`);
        this.textEditor = document.querySelector(textEditorSelector);
        this.saveButton = document.querySelector(saveButtonSelector);
        this.attachSaveButtonHandler();
        console.log('ok');
    }
    attachSaveButtonHandler() {
        this.saveButton.addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
            event.preventDefault();
            if (this.validateInputs()) {
                yield this.handleProductUpsert();
            }
        }));
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
        if (!this.validateShippingPriceFactor())
            isValid = false;
        if (!this.validateStockQuantity())
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
    validateShippingPriceFactor() {
        var _a;
        let isValid = true;
        const priceValue = this.shippingPriceFactorInput.value;
        const priceValidationParagraph = (_a = this.shippingPriceFactorInput.parentElement) === null || _a === void 0 ? void 0 : _a.querySelector('p');
        priceValidationParagraph.innerText = "";
        if (!priceValue || isNaN(Number(priceValue)) || parseFloat(priceValue) <= 0) {
            priceValidationParagraph.innerText = "Shipping price factor must be a number greater than 0.";
            isValid = false;
        }
        return isValid;
    }
    validateStockQuantity() {
        var _a;
        let isValid = true;
        const stockQuantityInputValue = this.stockQuantityInput.value;
        const stockQuantityParagraph = (_a = this.stockQuantityInput.parentElement) === null || _a === void 0 ? void 0 : _a.querySelector('p');
        stockQuantityParagraph.innerText = "";
        if (!stockQuantityInputValue || isNaN(Number(stockQuantityInputValue)) || parseFloat(stockQuantityInputValue) < 0) {
            stockQuantityParagraph.innerText = "Stock quantity cannot be lower than 0.";
            isValid = false;
        }
        return isValid;
    }
    getEditorContent() {
        // Clone the editor to prevent modifying the original DOM
        const editorClone = this.textEditor.cloneNode(true);
        // Remove tooltips
        const tooltips = editorClone.querySelectorAll(".ql-tooltip");
        tooltips.forEach(tooltip => tooltip.remove());
        // Remove contenteditable attributes from all elements
        const editableElements = editorClone.querySelectorAll("[contenteditable]");
        editableElements.forEach(el => el.removeAttribute("contenteditable"));
        // Optionally sanitize other attributes if needed (e.g., remove styles)
        const elementsWithStyles = editorClone.querySelectorAll("[style]");
        elementsWithStyles.forEach(el => el.removeAttribute("style"));
        // Return the cleaned HTML
        return editorClone.innerHTML.trim();
    }
    prepareFormData() {
        return __awaiter(this, void 0, void 0, function* () {
            const editorContent = this.getEditorContent();
            const formData = this.photoUploader.preparePhotosToUpload();
            const discountFormData = this.discountHandler.prepareDiscountToUpload();
            for (const [key, value] of discountFormData.entries()) {
                formData.append(key, value);
            }
            formData.append("Id", this.productIdInput.value.trim());
            formData.append("DiscountId", this.discountIdInput.value.trim());
            formData.append("Name", this.nameInput.value.trim());
            formData.append("Price", this.priceInput.value.trim());
            formData.append("FullDescription", editorContent);
            formData.append("CategoryId", this.categoryInput.value.trim());
            formData.append("StockQuantity", this.stockQuantityInput.value.trim());
            formData.append("ShippingPriceFactor", this.shippingPriceFactorInput.value.trim());
            return formData;
        });
    }
    handleProductUpsert() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                const formData = yield this.prepareFormData();
                console.log(formData);
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
