"use strict";
class CartItemsQuantityValidator {
    constructor(loadingScreenSelector, inputSelector, cartItemIdSelector, warningParagraphSelector, sweetAlert) {
        this.inputSelector = inputSelector;
        this.cartItemIdSelector = cartItemIdSelector;
        this.warningParagraphSelector = warningParagraphSelector;
        this.sweetAlert = sweetAlert;
        this.inputs = null;
        this.loadingScreen = document.querySelector(loadingScreenSelector);
    }
    gatherInputsData() {
        this.inputs = document.querySelectorAll(this.inputSelector);
        let inputsData = [];
        this.inputs.forEach(input => {
            const inputAncestor = this.gatherInputAncestorDiv(input);
            const cartItemIdMeta = inputAncestor.querySelector(this.cartItemIdSelector);
            const cartItemId = cartItemIdMeta.content;
            const inputData = {
                cartItemId: cartItemId,
                quantity: input.value,
            };
            inputsData.push(inputData);
        });
        return inputsData;
    }
    gatherInputAncestorDiv(input) {
        var _a;
        return (_a = input.parentElement) === null || _a === void 0 ? void 0 : _a.parentElement;
    }
    async validateCartQuantities() {
        let isValid = true;
        const inputsData = this.gatherInputsData();
        if (inputsData.length === 0) {
            isValid = false;
            return isValid;
        }
        this.loadingScreen.style.display = "flex";
        const formData = new FormData();
        inputsData.forEach((item, index) => {
            Object.keys(item).forEach((key) => {
                formData.append(`CollectionOfDTOs[${index}].${key}`, item[key]);
            });
        });
        try {
            const response = await fetch("/User/Cart/ValidateCartQuantity", {
                method: "PATCH",
                body: formData,
            });
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const result = await response.json();
            if (result.success) {
                this.handleUpdatedQuantities(result.itemsWithMaxQuantity);
            }
            else {
                isValid = false;
                toastr.error("Something went wrong, try again later.");
            }
        }
        catch (error) {
            isValid = false;
            toastr.error("Something went wrong, try again later.");
        }
        finally {
            this.loadingScreen.style.display = "none";
        }
        return isValid;
    }
    handleUpdatedQuantities(updatedItems) {
        let foundAny = false;
        updatedItems.forEach(updatedItem => {
            const foundPair = Array.from(this.inputs)
                .map(i => ({
                input: i,
                ancestor: this.gatherInputAncestorDiv(i)
            }))
                .find(({ ancestor }) => {
                const itemIdMeta = ancestor.querySelector(this.cartItemIdSelector);
                console.log(itemIdMeta.content);
                return itemIdMeta && itemIdMeta.content === updatedItem.cartItemId.toString();
            });
            if (foundPair) {
                const { input, ancestor } = foundPair;
                const cartItemWarning = ancestor.querySelector(this.warningParagraphSelector);
                cartItemWarning.innerText = "";
                if (Number(input.value) > Number(updatedItem.quantity)) {
                    input.value = updatedItem.quantity;
                    cartItemWarning.innerText = "Desired quantity is out of stock. The product quantity has been adjusted to the maximum available.";
                    foundAny = true;
                }
            }
        });
        if (foundAny) {
            this.loadingScreen.style.display = "none";
            this.sweetAlert.FireSweetAlert("Update", "Some of your items are out of stock at the desired quantity. Their quantities have been adjusted in your cart.", () => { });
        }
        this.loadingScreen.style.display = "none";
    }
}
//# sourceMappingURL=CartItemsQuantityValidator.js.map