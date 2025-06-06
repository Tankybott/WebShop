"use strict";
class ProductBrowserFilter {
    constructor(textInputId, minimalPriceInputId, maximalPirceInputId, showOnlyDiscountedCheckboxId, sortBySelectId, clearButtonSelector, applyButtonSelector, productBrowserApiManager, pagination) {
        this.productBrowserApiManager = productBrowserApiManager;
        this.pagination = pagination;
        console.log(clearButtonSelector);
        this.textInput = document.querySelector(`#${textInputId}`);
        this.minimalPriceInput = document.querySelector(`#${minimalPriceInputId}`);
        this.maximalPriceInput = document.querySelector(`#${maximalPirceInputId}`);
        this.showOnlyDiscountCheckbox = document.querySelector(`#${showOnlyDiscountedCheckboxId}`);
        this.sortBySelect = document.querySelector(`#${sortBySelectId}`);
        this.clearButton = document.querySelector(clearButtonSelector);
        this.applyButton = document.querySelector(applyButtonSelector);
        this.addButtonsEventListeners();
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
        this.showOnlyDiscountCheckbox.checked = false;
        this.sortBySelect.selectedIndex = 0;
        this.initValues();
        this.pagination.resetToDefault();
    }
    addButtonsEventListeners() {
        this.applyButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.initValues();
            this.pagination.resetToDefault();
            this.productBrowserApiManager.getProducts();
        });
        this.clearButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.resetFilters();
            this.productBrowserApiManager.getProducts();
        });
    }
}
