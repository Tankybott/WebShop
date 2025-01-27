"use strict";
class ProductBrowserFilter {
    constructor(textInputId, minimalPriceInputId, maximalPirceInputId, showOnlyDiscountedCheckboxId, sortBySelectId, productBrowserApiManager) {
        this.productBrowserApiManager = productBrowserApiManager;
        this.textInput = document.querySelector(`#${textInputId}`);
        this.minimalPriceInput = document.querySelector(`#${minimalPriceInputId}`);
        this.maximalPriceInput = document.querySelector(`#${maximalPirceInputId}`);
        this.showOnlyDiscountCheckbox = document.querySelector(`#${showOnlyDiscountedCheckboxId}`);
        this.sortBySelect = document.querySelector(`#${sortBySelectId}`);
    }
    initValues() {
        this.productBrowserApiManager.ApiCallOptions.typedTextFilter = this.textInput.value.trim();
        this.productBrowserApiManager.ApiCallOptions.minimalPriceFilter = this.minimalPriceInput.value.trim();
        this.productBrowserApiManager.ApiCallOptions.maximalPriceFilter = this.maximalPriceInput.value.trim();
        this.productBrowserApiManager.ApiCallOptions.showOnlyDiscountFilter = this.showOnlyDiscountCheckbox.checked ? "true" : "false";
        this.productBrowserApiManager.ApiCallOptions.sortByValueFilter = this.sortBySelect.value.trim();
    }
    resetFilters() {
        this.textInput.value = "";
        this.minimalPriceInput.value = "";
        this.maximalPriceInput.value = "";
        this.showOnlyDiscountCheckbox.value = "";
        this.sortBySelect.selectedIndex = 0;
        this.initValues();
    }
}
