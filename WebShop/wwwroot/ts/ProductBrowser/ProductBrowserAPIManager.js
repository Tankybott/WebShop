"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class ProductBrowserApiManager {
    setItemsAmountCallback(callback) {
        this.setItemsAmount = (totalItems) => {
            callback.call(this, totalItems);
        };
    }
    setInitPaginationCallback(callback) {
        this.initPaginationCallback = callback;
    }
    constructor(itemsDisplayDivSelector, informationPSelector, cardGenerator) {
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
        this.handleBackNavigation();
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
    getProducts() {
        return __awaiter(this, void 0, void 0, function* () {
            this.saveApiCallOptions();
            const url = `/User/ProductBrowser/GetChoosenProducts?${this.getFilteringString()}`;
            try {
                const response = yield fetch(url, {
                    method: 'GET',
                    headers: { 'Content-Type': 'application/json' },
                });
                if (!response.ok) {
                    throw new Error(`Failed to fetch data: ${response.statusText}`);
                }
                const data = yield response.json();
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
        });
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
