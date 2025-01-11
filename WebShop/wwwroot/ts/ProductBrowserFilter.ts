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

    private typedText: string = "";
    private minimalPrice: string = "";
    private maximalPrice: string = "";
    private showOnlyDiscount: string = "";
    private sortByValue: string = "";

    constructor(
        textInputId: string,
        minimalPriceInputId: string,
        maximalPirceInputId: string,
        showOnlyDiscountedCheckboxId: string,
        sortBySelectId: string,
    ) {
        this.textInput = document.querySelector(`#${textInputId}`) as HTMLInputElement;
        this.minimalPriceInput = document.querySelector(`#${minimalPriceInputId}`) as HTMLInputElement;
        this.maximalPriceInput = document.querySelector(`#${maximalPirceInputId}`) as HTMLInputElement;
        this.showOnlyDiscountCheckbox = document.querySelector(`#${showOnlyDiscountedCheckboxId}`) as HTMLInputElement;
        this.sortBySelect = document.querySelector(`#${sortBySelectId}`) as HTMLSelectElement;
    }

    private initValues(): void {
        this.typedText = this.textInput.value.trim();
        this.minimalPrice = this.minimalPriceInput.value.trim();
        this.maximalPrice = this.maximalPriceInput.value.trim();
        this.showOnlyDiscount = this.showOnlyDiscountCheckbox.checked ? "true" : "false";
        this.sortByValue = this.sortBySelect.value.trim();
    }

    public getFilters(): IFilterModel {
        this.initValues();
        const filterModel: IFilterModel = {
            typedTextFilter: this.typedText,
            minimalPriceFilter: this.minimalPrice,
            maximalPriceFilter: this.maximalPrice,
            showOnlyDiscountFilter: this.showOnlyDiscount,
            sortByValueFilter: this.sortByValue
        };
        console.log(filterModel)
        return filterModel
    }

    public cleanInputs(): void {
        this.textInput.value = "";
        this.minimalPriceInput.value = "";
        this.maximalPriceInput.value = "";
        this.showOnlyDiscountCheckbox.value = "";
        this.sortBySelect.selectedIndex = 0;

        this.initValues();
    }
}