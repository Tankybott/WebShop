"use strict";
class BaseCartQuantityValidator {
    constructor(loadingScreenSelector, elementWithQuantity, cartItemIdMetaSelector, warningParagraphSelector, sweetAlert) {
        this.elementWithQuantity = elementWithQuantity;
        this.cartItemIdMetaSelector = cartItemIdMetaSelector;
        this.warningParagraphSelector = warningParagraphSelector;
        this.sweetAlert = sweetAlert;
        this.elements = null;
        this.isQuantityValid = true;
        this.loadingScreen = document.querySelector(loadingScreenSelector);
    }
    gatherElementsData() {
        this.elements = document.querySelectorAll(this.elementWithQuantity);
        const data = [];
        this.elements.forEach(element => {
            const ancestor = this.gatherAncestor(element);
            const cartItemIdMeta = ancestor.querySelector(this.cartItemIdMetaSelector);
            const cartItemId = cartItemIdMeta.content;
            data.push({ cartItemId, quantity: this.getElementValue(element) });
        });
        return data;
    }
    gatherAncestor(element) {
        var _a;
        return (_a = element.parentElement) === null || _a === void 0 ? void 0 : _a.parentElement;
    }
    async validateCartQuantities() {
        this.isQuantityValid = true;
        const inputsData = this.gatherElementsData();
        if (inputsData.length === 0)
            return false;
        this.loadingScreen.style.display = "flex";
        const formData = new FormData();
        inputsData.forEach((item, index) => {
            Object.entries(item).forEach(([key, value]) => {
                formData.append(`CollectionOfDTOs[${index}].${key}`, value);
            });
        });
        try {
            const response = await fetch("/User/Cart/ValidateCartQuantity", {
                method: "PATCH",
                body: formData,
            });
            if (!response.ok)
                throw new Error(`HTTP error! Status: ${response.status}`);
            const result = await response.json();
            if (result.success) {
                this.handleUpdatedQuantities(result.itemsWithMaxQuantity);
            }
            else {
                this.isQuantityValid = false;
                toastr.error("Something went wrong, try again later.");
            }
        }
        catch (e) {
            this.isQuantityValid = false;
            toastr.error("Something went wrong, try again later.");
        }
        finally {
            this.loadingScreen.style.display = "none";
            return this.isQuantityValid;
        }
    }
    handleUpdatedQuantities(updatedItems) {
        let foundAny = false;
        updatedItems.forEach(updatedItem => {
            const match = Array.from(this.elements)
                .map(el => ({ el, ancestor: this.gatherAncestor(el) }))
                .find(({ ancestor }) => {
                const meta = ancestor.querySelector(this.cartItemIdMetaSelector);
                return meta && meta.content == updatedItem.cartItemId;
            });
            if (match) {
                const { el, ancestor } = match;
                const warning = ancestor.querySelector(this.warningParagraphSelector);
                warning.innerText = "";
                if (Number(this.getElementValue(el)) > Number(updatedItem.quantity)) {
                    this.setElementValue(el, updatedItem.quantity);
                    warning.innerText = "Desired quantity is out of stock. The quantity has been adjusted.";
                    foundAny = true;
                }
            }
        });
        if (foundAny) {
            this.loadingScreen.style.display = "none";
            this.isQuantityValid = false;
            this.sweetAlert.FireSweetAlert("Update", "Some of your items were adjusted to match available stock.", () => { });
        }
    }
}
class CartInputQuantityValidator extends BaseCartQuantityValidator {
    getElementValue(element) {
        return element.value;
    }
    setElementValue(element, value) {
        element.value = value;
    }
}
class CartTextQuantityValidator extends BaseCartQuantityValidator {
    getElementValue(element) {
        return element.innerText;
    }
    setElementValue(element, value) {
        element.innerText = value;
    }
}
//# sourceMappingURL=CartQuantityValidator.js.map