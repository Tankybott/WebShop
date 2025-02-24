"use strict";
class CartSynchronizationChecker {
    constructor(quantityInputSelector, loadingScreenSelector, cartIdMetaSelector, sweetAlert) {
        this.sweetAlert = sweetAlert;
        this.lastStateOfInput = null;
        this.quantityInputs = document.querySelectorAll(quantityInputSelector);
        this.loadingScreenDiv = document.querySelector(loadingScreenSelector);
        const cartIdMeta = document.querySelector(cartIdMetaSelector);
        this.cartId = cartIdMeta.content;
    }
    async initialize() {
        await this.synchronizePricesWithServerAsync();
    }
    async synchronizePricesWithServerAsync() {
        this.loadingScreenDiv.style.display = "flex";
        try {
            const response = await fetch(`Cart/SynchronizeCartPrices?cartId=${this.cartId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
            });
            const result = await response.json();
            if (result.success) {
                console.log(result.isPriceChanged);
                if (result.isPriceChanged == true) {
                    this.sweetAlert.FireSweetAlert("Update", "Prices of some items have changed. Your cart will be updated accordingly.", () => { location.reload(); });
                }
            }
            else {
                console.error("There was an error when synchronizing cart prices");
                window.location.href = "/User/Home";
            }
        }
        catch (error) {
            console.error("There was an error when synchronizing cart prices");
            window.location.href = "/User/Home";
        }
        finally {
            this.loadingScreenDiv.style.display = "none";
        }
    }
}
//# sourceMappingURL=SynchronizationChecker.js.map