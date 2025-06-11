"use strict";
class ProductBrowserApiManager {
    setItemsAmountCallback(callback) {
        this.setItemsAmount = (totalItems) => {
            callback.call(this, totalItems);
        };
    }
    setInitPaginationCallback(callback) {
        this.initPaginationCallback = callback;
    }
    constructor(itemsDisplayDivSelector, informationPSelector, spinnerDivSelector, cardGenerator) {
        this.cardGenerator = cardGenerator;
        this.ApiCallOptions = {
            categoryIDFilter: "",
            typedTextFilter: "",
            minimalPriceFilter: "",
            maximalPriceFilter: "",
            showOnlyDiscountFilter: "",
            sortByValueFilter: "",
            PageNumber: "",
            PageSize: ""
        };
        this.itemDisplayDiv = document.querySelector(itemsDisplayDivSelector);
        this.informationP = document.querySelector(informationPSelector);
        this.spinnerDiv = document.querySelector(spinnerDivSelector);
        this.handleBackNavigation();
        this.getProducts();
    }
    saveApiCallOptions() {
        sessionStorage.setItem("ApiCallOptions", JSON.stringify(this.ApiCallOptions));
    }
    restoreApiCallOptions() {
        const savedOptions = sessionStorage.getItem("ApiCallOptions");
        if (savedOptions) {
            this.ApiCallOptions = JSON.parse(savedOptions);
        }
    }
    handleBackNavigation() {
        const navigationEntry = performance.getEntriesByType("navigation")[0];
        if ((navigationEntry === null || navigationEntry === void 0 ? void 0 : navigationEntry.type) === "back_forward") {
            this.restoreApiCallOptions();
            this.getProducts();
        }
        else {
            sessionStorage.removeItem("ApiCallOptions");
        }
    }
    async getProducts() {
        this.saveApiCallOptions();
        const url = `/User/ProductBrowser/GetChoosenProducts?${this.getFilteringString()}`;
        this.spinnerDiv.style.display = "flex";
        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
            });
            if (!response.ok) {
                throw new Error(`Failed to fetch data: ${response.statusText}`);
            }
            const data = await response.json();
            this.displayProducts(data.data.items);
            if (this.setItemsAmount) {
                this.setItemsAmount(data.data.totalItemCount);
            }
            if (this.initPaginationCallback) {
                this.initPaginationCallback();
            }
        }
        catch (error) {
            console.error('Error during API call:', error);
        }
        finally {
            this.spinnerDiv.style.display = "none";
        }
    }
    getFilteringString() {
        const queryParams = Object.entries(this.ApiCallOptions)
            .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
        return queryParams.join("&");
    }
    displayProducts(products) {
        this.cleanItemDisplayDiv();
        if (products.length > 0) {
            products.forEach(p => {
                this.itemDisplayDiv.appendChild(this.cardGenerator.generateProductCard(p));
                this.informationP.innerText = "";
            });
        }
        else
            this.informationP.innerText = "Products not found...";
    }
    cleanItemDisplayDiv() {
        this.itemDisplayDiv.innerHTML = "";
    }
}
//# sourceMappingURL=ProductBrowserAPIManager.js.map