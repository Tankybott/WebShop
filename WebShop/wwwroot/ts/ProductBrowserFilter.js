"use strict";
class ProductBrowserFilter {
    constructor(textInputId, minimalPriceInputId, maximalPirceInputId, showOnlyDiscountedCheckboxId, sortBySelectId) {
        this.typedText = "";
        this.minimalPrice = "";
        this.maximalPrice = "";
        this.showOnlyDiscount = "";
        this.sortByValue = "";
        this.textInput = document.querySelector(`#${textInputId}`);
        this.minimalPriceInput = document.querySelector(`#${minimalPriceInputId}`);
        this.maximalPriceInput = document.querySelector(`#${maximalPirceInputId}`);
        this.showOnlyDiscountCheckbox = document.querySelector(`#${showOnlyDiscountedCheckboxId}`);
        this.sortBySelect = document.querySelector(`#${sortBySelectId}`);
    }
    initValues() {
        this.typedText = this.textInput.value.trim();
        this.minimalPrice = this.minimalPriceInput.value.trim();
        this.maximalPrice = this.maximalPriceInput.value.trim();
        this.showOnlyDiscount = this.showOnlyDiscountCheckbox.value.trim();
        this.sortByValue = this.sortBySelect.value.trim();
    }
    getFilters() {
        const filterModel = {
            typedTextFilter: this.typedText,
            minimalPriceFilter: this.minimalPrice,
            maximalPriceFilter: this.maximalPrice,
            showOnlyDiscountFilter: this.showOnlyDiscount,
            sortByValueFilter: this.sortByValue
        };
        return filterModel;
    }
    cleanInputs() {
        this.textInput.value = "";
        this.minimalPriceInput.value = "";
        this.maximalPriceInput.value = "";
        this.showOnlyDiscountCheckbox.value = "";
        this.sortBySelect.selectedIndex = 0;
        this.initValues();
    }
}
