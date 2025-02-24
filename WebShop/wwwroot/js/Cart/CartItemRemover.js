"use strict";
class CartItemRemover {
    constructor(deleteButtonSelector, loadingScreenSelector, itemIdMetaSelector, sweetAlert) {
        this.itemIdMetaSelector = itemIdMetaSelector;
        this.sweetAlert = sweetAlert;
        this.deleteButtons = document.querySelectorAll(deleteButtonSelector);
        this.loadingScreen = document.querySelector(loadingScreenSelector);
        this.addEventListenersToButtons();
    }
    async deleteItem(itemsDeleteButton) {
        var _a;
        const buttonParent = itemsDeleteButton.parentElement;
        const itemIdMeta = (_a = buttonParent === null || buttonParent === void 0 ? void 0 : buttonParent.parentElement) === null || _a === void 0 ? void 0 : _a.querySelector(this.itemIdMetaSelector);
        const itemId = itemIdMeta.content;
        await this.deleteCartItemFromServer(itemId);
    }
    async deleteCartItemFromServer(id) {
        this.loadingScreen.style.display = "flex";
        try {
            const response = await fetch(`/User/Cart/DeleteCartItem?id=${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            if (!response.ok) {
                const errorText = await response.text();
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
    }
    addEventListenersToButtons() {
        this.deleteButtons.forEach(button => {
            button.addEventListener('click', () => this.sweetAlert.FireSweetAlert("Warning", "Are you sure you want to delete this item from your cart?", () => this.deleteItem(button), () => { }, "Confirm"));
        });
    }
}
//# sourceMappingURL=CartItemRemover.js.map