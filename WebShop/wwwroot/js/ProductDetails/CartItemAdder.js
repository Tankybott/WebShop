"use strict";
class CartItemAdder {
    constructor(productQuantityInputSelector, productIdMetaSelector, productDiscountedMetaSelector, productStockQuantityMetaSelector, postButtonSelector, cartManager) {
        this.cartManager = cartManager;
        this.productQuantityInput = document.querySelector(productQuantityInputSelector);
        this.productIdMeta = document.querySelector(productIdMetaSelector);
        this.productDiscountedMeta = document.querySelector(productDiscountedMetaSelector);
        this.productStockQuntityMeta = document.querySelector(productStockQuantityMetaSelector);
        this.postButton = document.querySelector(postButtonSelector);
        this.postButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.addToCart();
        });
    }
    async addToCart() {
        if (!this.validateQuantityInput())
            return;
        const formData = new FormData();
        formData.append("ProductId", this.productIdMeta.content);
        formData.append("ProductQuantity", this.productQuantityInput.value);
        formData.append("IsAddedWithDiscount", this.productDiscountedMeta.content === "true" ? "true" : "false");
        try {
            const response = await fetch("AddItemToCart", {
                method: "POST",
                body: formData
            });
            if (response.redirected) {
                const loginUrl = new URL(response.url);
                const returnUrl = loginUrl.searchParams.get("ReturnUrl");
                if (returnUrl && returnUrl.includes("AddItemToCart")) {
                    const productDetailsUrl = `/User/ProductDetails/Details?productId=${this.productIdMeta.content}`;
                    loginUrl.searchParams.set("ReturnUrl", productDetailsUrl);
                }
                window.location.href = loginUrl.toString();
                return;
            }
            const result = await response.json();
            if (result.success) {
                toastr.success(result.message);
                this.cartManager.refreshCartQuantity();
            }
            else {
                toastr.error(result.message);
            }
        }
        catch (error) {
            toastr.error("Something went wrong, try again later.");
            console.error("Error adding to cart:", error);
        }
        this.productQuantityInput.value = "1";
    }
    validateQuantityInput() {
        const stockQuantity = parseInt(this.productStockQuntityMeta.content, 10);
        const inputQuantity = parseInt(this.productQuantityInput.value, 10);
        var quantityErrorMessage = this.productQuantityInput.nextElementSibling;
        if (isNaN(inputQuantity) || inputQuantity <= 0) {
            quantityErrorMessage.textContent = "Quantity must be at least 1.";
            return false;
        }
        if (inputQuantity > stockQuantity) {
            quantityErrorMessage.textContent = `Only ${stockQuantity} items available in stock.`;
            return false;
        }
        quantityErrorMessage.textContent = "";
        return true;
    }
}
//# sourceMappingURL=CartItemAdder.js.map