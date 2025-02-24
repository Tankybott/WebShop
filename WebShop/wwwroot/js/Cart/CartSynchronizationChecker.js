"use strict";
class CartSynchronizationChecker {
    constructor(inputsSelector, loadingScreenSelector, cartIdMetaSelector, cartItemIdMetaSelector, warningParagraphSelector, sweetAlert) {
        this.cartItemIdMetaSelector = cartItemIdMetaSelector;
        this.warningParagraphSelector = warningParagraphSelector;
        this.sweetAlert = sweetAlert;
        this.loadingScreenDiv = document.querySelector(loadingScreenSelector);
        this.inputs = document.querySelectorAll(inputsSelector);
        const cartIdMeta = document.querySelector(cartIdMetaSelector);
        this.cartId = cartIdMeta.content;
        this.checkForModifiedItemsInSession();
    }
    async initialize() {
        await this.synchronizePricesWithServerAsync();
    }
    async synchronizePricesWithServerAsync() {
        this.loadingScreenDiv.style.display = "flex";
        try {
            const response = await fetch(`Cart/SynchronizeCartPrices?cartId=${this.cartId}`, {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json"
                },
            });
            const result = await response.json();
            if (result.success) {
                if (result.modifiedCartItemsIds.length > 0) {
                    sessionStorage.setItem("modifiedCartItemsIds", JSON.stringify(result.modifiedCartItemsIds));
                    this.sweetAlert.FireSweetAlert("Update", "Prices of some items have changed. Your cart will be updated accordingly.", () => {
                        location.reload();
                    });
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
    checkForModifiedItemsInSession() {
        const storedModifiedIds = sessionStorage.getItem("modifiedCartItemsIds");
        if (!storedModifiedIds)
            return;
        const modifiedIds = JSON.parse(storedModifiedIds);
        if (modifiedIds.length === 0)
            return;
        this.inputs.forEach(input => {
            const cartItemId = this.getCartItemId(input);
            console.log(cartItemId);
            if (modifiedIds.map(String).includes(cartItemId)) {
                const warningParagraph = this.getInputWarningParagraph(input);
                console.log(warningParagraph);
                if (warningParagraph) {
                    console.log(warningParagraph);
                    warningParagraph.textContent = "This item's price was adjusted due to price updates.";
                }
            }
        });
        sessionStorage.removeItem("modifiedCartItemsIds");
    }
    getInputWarningParagraph(input) {
        const inputAncestor = this.getInputAncestor(input);
        return inputAncestor.querySelector(this.warningParagraphSelector);
    }
    getCartItemId(input) {
        const inputAncestor = this.getInputAncestor(input);
        const cartItemIdMeta = inputAncestor.querySelector(this.cartItemIdMetaSelector);
        return cartItemIdMeta.content;
    }
    getInputAncestor(input) {
        var _a;
        return (_a = input.parentElement) === null || _a === void 0 ? void 0 : _a.parentElement;
    }
}
//# sourceMappingURL=CartSynchronizationChecker.js.map