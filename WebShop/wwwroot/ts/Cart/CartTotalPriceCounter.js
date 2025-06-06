"use strict";
class CartTotalPriceCounter {
    constructor(inputSelector, totalPriceSelector, itemCurrentPriceSelector) {
        this.itemCurrentPriceSelector = itemCurrentPriceSelector;
        this.inputs = document.querySelectorAll(inputSelector);
        this.totalPrice = document.querySelector(totalPriceSelector);
        this.countPrice();
    }
    getInputAncestor(input) {
        var _a;
        return (_a = input.parentElement) === null || _a === void 0 ? void 0 : _a.parentElement;
    }
    countPrice() {
        let summaryPrice = 0;
        this.inputs.forEach(input => {
            summaryPrice += this.countItemSummaryPrice(input);
        });
        this.setSummaryPrice(summaryPrice);
    }
    countItemSummaryPrice(input) {
        const inputAncestor = this.getInputAncestor(input);
        const itemCurrentPriceMeta = inputAncestor.querySelector(this.itemCurrentPriceSelector);
        const itemCurrentPrice = itemCurrentPriceMeta.content;
        return Number(input.value) * Number(itemCurrentPrice);
    }
    setSummaryPrice(price) {
        this.totalPrice.innerText = price.toString();
    }
}
