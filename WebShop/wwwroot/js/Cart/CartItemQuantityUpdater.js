"use strict";
class CartItemQuantityUpdater {
    constructor(quantityInputSelector, idMetaSelector, loadingScreenSelector) {
        this.idMetaSelector = idMetaSelector;
        this.lastStateOfInput = null;
        this.quantityInputs = document.querySelectorAll(quantityInputSelector);
        this.loadingScreenDiv = document.querySelector(loadingScreenSelector);
        this.addEventListenrsToInputs();
        document.addEventListener("keydown", (event) => {
            if (event.key === "Enter") {
                const activeElement = document.activeElement;
                if (activeElement && activeElement.tagName === "INPUT") {
                    activeElement.blur();
                    if (document.body.style.pointerEvents == "none")
                        document.body.style.pointerEvents = "auto";
                }
            }
        });
    }
    gatherInputAncestorDiv(input) {
        var _a;
        return (_a = input.parentElement) === null || _a === void 0 ? void 0 : _a.parentElement;
    }
    handleInputFocus(input) {
        this.lastStateOfInput = input.value;
        document.body.style.pointerEvents = "none";
    }
    async handleInputFocusLostAsync(input) {
        document.body.style.pointerEvents = "none";
        const inputValue = input.value;
        if (inputValue === this.lastStateOfInput) {
            document.body.style.pointerEvents = "auto";
            return;
        }
        ;
        this.postChangedInputValue(input);
        document.body.style.pointerEvents = "auto";
    }
    assingLastStateToInput(input) {
        input.value = this.lastStateOfInput;
    }
    async postChangedInputValue(input) {
        const inputParentElement = this.gatherInputAncestorDiv(input);
        const productIdMeta = inputParentElement.querySelector(this.idMetaSelector);
        const cartItemId = productIdMeta.content;
        const newQuantity = parseInt(input.value, 10);
        if (isNaN(newQuantity)) {
            console.error("Error: Invalid quantity value.");
            return;
        }
        if (newQuantity <= 0) {
            toastr.error('Item quantity cannot be less than 1');
            this.assingLastStateToInput(input);
            return;
        }
        this.loadingScreenDiv.style.display = "flex";
        try {
            const response = await fetch(`Cart/ChangeCartItemQuantity?cartItemId=${cartItemId}&newQuantity=${newQuantity}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
            });
            const result = await response.json();
            if (result.success) {
                toastr.success("Cart item quantity upadated sucessfully");
            }
            else {
                if (result.quantityLeft != null) {
                    toastr.error("Items out of stock, item quantity is automaticly assigned to maximum quantity of this item");
                    input.value = result.quantityLeft;
                    this.postChangedInputValue(input);
                }
                else {
                    toastr.error("There was en error when updating items quantity");
                    this.assingLastStateToInput(input);
                }
            }
        }
        catch (error) {
            toastr.error("There was en error when updating items quantity");
            this.assingLastStateToInput(input);
        }
        finally {
            this.loadingScreenDiv.style.display = "none";
        }
    }
    addEventListenrsToInputs() {
        this.quantityInputs.forEach(input => {
            input.addEventListener("focusin", () => this.handleInputFocus(input));
            input.addEventListener("focusout", () => this.handleInputFocusLostAsync(input));
        });
    }
}
//# sourceMappingURL=CartItemQuantityUpdater.js.map