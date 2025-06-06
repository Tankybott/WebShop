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
class CartPricesSynchronizationChecker {
    constructor(inputsSelector, loadingScreenSelector, cartIdMetaSelector, cartItemIdMetaSelector, warningParagraphSelector, sweetAlert) {
        this.cartItemIdMetaSelector = cartItemIdMetaSelector;
        this.warningParagraphSelector = warningParagraphSelector;
        this.sweetAlert = sweetAlert;
        this.isSynchronized = true;
        this.loadingScreenDiv = document.querySelector(loadingScreenSelector);
        this.inputs = document.querySelectorAll(inputsSelector);
        const cartIdMeta = document.querySelector(cartIdMetaSelector);
        this.cartId = cartIdMeta.content;
        this.checkForModifiedItemsInSession();
    }
    initialize() {
        return __awaiter(this, void 0, void 0, function* () {
            yield this.synchronizePricesWithServerAsync();
        });
    }
    synchronize() {
        return __awaiter(this, void 0, void 0, function* () {
            yield this.synchronizePricesWithServerAsync();
            return this.isSynchronized;
        });
    }
    synchronizePricesWithServerAsync() {
        return __awaiter(this, void 0, void 0, function* () {
            this.isSynchronized = true;
            this.loadingScreenDiv.style.display = "flex";
            try {
                const response = yield fetch(`Cart/SynchronizeCartPrices?cartId=${this.cartId}`, {
                    method: "PATCH",
                    headers: {
                        "Content-Type": "application/json"
                    },
                });
                const result = yield response.json();
                if (result.success) {
                    if (result.modifiedCartItemsIds.length > 0) {
                        sessionStorage.setItem("modifiedCartItemsIds", JSON.stringify(result.modifiedCartItemsIds));
                        this.sweetAlert.FireSweetAlert("Update", "Prices of some items have changed. Your cart will be updated accordingly.", () => {
                            location.reload();
                        });
                        this.isSynchronized = false;
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
        });
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
