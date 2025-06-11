interface IFilterModel {
    typedTextFilter: string;
    minimalPriceFilter: string;
    maximalPriceFilter: string;
    showOnlyDiscountFilter: string;
    sortByValueFilter: string;
}

class ProductBrowserFilter {
    private readonly textInput: HTMLInputElement;
    private readonly minimalPriceInput: HTMLInputElement;
    private readonly maximalPriceInput: HTMLInputElement;
    private readonly showOnlyDiscountCheckbox: HTMLInputElement;
    private readonly sortBySelect: HTMLSelectElement;
    private readonly clearButton: HTMLButtonElement;
    private readonly applyButton: HTMLButtonElement;

    constructor(
        textInputId: string,
        minimalPriceInputId: string,
        maximalPirceInputId: string,
        showOnlyDiscountedCheckboxId: string,
        sortBySelectId: string,
        clearButtonSelector: string,
        applyButtonSelector: string,
        private readonly productBrowserApiManager: ProductBrowserApiManager,
        private readonly pagination: ProductBrowserPagination
    ) {
        this.textInput = document.querySelector(`#${textInputId}`) as HTMLInputElement;
        this.minimalPriceInput = document.querySelector(`#${minimalPriceInputId}`) as HTMLInputElement;
        this.maximalPriceInput = document.querySelector(`#${maximalPirceInputId}`) as HTMLInputElement;
        this.showOnlyDiscountCheckbox = document.querySelector(`#${showOnlyDiscountedCheckboxId}`) as HTMLInputElement;
        this.sortBySelect = document.querySelector(`#${sortBySelectId}`) as HTMLSelectElement;
        this.clearButton = document.querySelector(clearButtonSelector) as HTMLButtonElement;
        this.applyButton = document.querySelector(applyButtonSelector) as HTMLButtonElement;

        this.addButtonsEventListeners();
    }

    private initValues(): void {
        this.productBrowserApiManager.ApiCallOptions.typedTextFilter = this.textInput.value.trim();
        this.productBrowserApiManager.ApiCallOptions.minimalPriceFilter = this.minimalPriceInput.value.trim();
        this.productBrowserApiManager.ApiCallOptions.maximalPriceFilter = this.maximalPriceInput.value.trim();
        this.productBrowserApiManager.ApiCallOptions.showOnlyDiscountFilter = this.showOnlyDiscountCheckbox.checked ? "true" : "false";
        this.productBrowserApiManager.ApiCallOptions.sortByValueFilter = this.sortBySelect.value.trim();
    }

    public resetFilters(): void {
        this.textInput.value = "";
        this.minimalPriceInput.value = "";
        this.maximalPriceInput.value = "";
        this.showOnlyDiscountCheckbox.checked = false;
        this.sortBySelect.selectedIndex = 0;

        this.initValues();
        this.pagination.resetToDefault();
    }

    private addButtonsEventListeners() {
        this.applyButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.initValues();
            this.pagination.resetToDefault();
            this.productBrowserApiManager.getProducts();
        })
        this.clearButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.resetFilters();
            this.productBrowserApiManager.getProducts();
        })
    }
}