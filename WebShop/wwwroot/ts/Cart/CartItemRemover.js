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
class CartItemRemover {
    constructor(deleteButtonSelector, loadingScreenSelector, itemIdMetaSelector, sweetAlert) {
        this.itemIdMetaSelector = itemIdMetaSelector;
        this.sweetAlert = sweetAlert;
        this.deleteButtons = document.querySelectorAll(deleteButtonSelector);
        this.loadingScreen = document.querySelector(loadingScreenSelector);
        this.addEventListenersToButtons();
    }
    deleteItem(itemsDeleteButton) {
        return __awaiter(this, void 0, void 0, function* () {
            var _a;
            const buttonParent = itemsDeleteButton.parentElement;
            const itemIdMeta = (_a = buttonParent === null || buttonParent === void 0 ? void 0 : buttonParent.parentElement) === null || _a === void 0 ? void 0 : _a.querySelector(this.itemIdMetaSelector);
            const itemId = itemIdMeta.content;
            yield this.deleteCartItemFromServer(itemId);
        });
    }
    deleteCartItemFromServer(id) {
        return __awaiter(this, void 0, void 0, function* () {
            this.loadingScreen.style.display = "flex";
            try {
                const response = yield fetch(`/User/Cart/DeleteCartItem?id=${id}`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                if (!response.ok) {
                    const errorText = yield response.text();
                    throw new Error(`Failed to remove item: ${errorText}`);
                }
                this.loadingScreen.style.display = "none";
                location.reload();
            }
            catch (error) {
                console.error("Error:", error);
                toastr.error(`Something went wrong try again later`);
                this.loadingScreen.style.display = "none";
            }
        });
    }
    addEventListenersToButtons() {
        this.deleteButtons.forEach(button => {
            button.addEventListener('click', () => this.sweetAlert.FireSweetAlert("Warning", "Are you sure you want to delete this item from your cart?", () => this.deleteItem(button), () => { }, "Confirm"));
        });
    }
}
