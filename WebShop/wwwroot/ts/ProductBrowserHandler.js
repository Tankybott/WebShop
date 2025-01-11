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
class ProductBrowserHandler {
    constructor(navigationULSelector, itemDisplayDivSelector, productBrowserFilter) {
        this.productBrowserFilter = productBrowserFilter;
        this.choosenCategory = "";
        this.filterModel = {
            categoryFilter: "",
            typedTextFilter: "",
            minimalPriceFilter: "",
            maximalPriceFilter: "",
            showOnlyDiscountFilter: "",
            sortByValueFilter: ""
        };
        this.navigationUl = document.querySelector(navigationULSelector);
        this.itemDisplayDiv = document.querySelector(itemDisplayDivSelector);
        this.initNavigation();
    }
    initNavigation() {
        const allLinks = this.navigationUl.querySelectorAll('a');
        allLinks.forEach(link => {
            const linkContent = link.innerText;
            link.addEventListener('click', () => {
                this.handleApiCall(linkContent);
                this.productBrowserFilter.cleanInputs();
                this.setCategoryLinkActive(link);
            });
        });
    }
    handleApiCall(categoryFilter) {
        return __awaiter(this, void 0, void 0, function* () {
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
                this.displayProducts(data.data);
            }
            catch (error) {
                console.error('Error during API call:', error);
            }
        });
    }
    setCategoryLinkActive(link) {
        link.style.color = "blue";
        this.choosenCategory = link.innerText.trim();
    }
    setBrowsingMessege(messege) {
    }
    displayProducts(products) {
        if (products.length > 0) {
            products.map(p => this.renderProductCard(p));
        }
    }
    renderProductCard(product) {
        var p = document.createElement('p');
        p.innerText = product.name;
        this.itemDisplayDiv.appendChild(p);
    }
    getFilteringString() {
        this.initFilterModel();
        const queryParams = Object.entries(this.filterModel)
            .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
        return queryParams.join("&");
    }
    initFilterModel() {
        this.filterModel = Object.assign({ categoryFilter: this.choosenCategory }, this.productBrowserFilter.getFilters());
        console.log(this.filterModel);
    }
}
