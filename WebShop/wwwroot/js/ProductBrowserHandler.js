"use strict";
class ProductBrowserHandler {
    constructor(navigationULSelector, itemDisplayDivSelector, productBrowserFilter) {
        this.productBrowserFilter = productBrowserFilter;
        this.choosenCategoryId = "";
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
            link.addEventListener('click', async () => {
                this.setCategoryLinkActive(link);
                await this.handleApiCall();
                this.productBrowserFilter.cleanInputs();
            });
        });
    }
    async handleApiCall() {
        const url = `/User/ProductBrowser/GetChoosenProducts?${this.getFilteringString()}`;
        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
            });
            if (!response.ok) {
                throw new Error(`Failed to fetch data: ${response.statusText}`);
            }
            const data = await response.json();
            this.displayProducts(data.data);
        }
        catch (error) {
            console.error('Error during API call:', error);
        }
    }
    setCategoryLinkActive(link) {
        var _a;
        link.style.color = "blue";
        this.choosenCategoryId = (_a = link.nextElementSibling) === null || _a === void 0 ? void 0 : _a.innerHTML; //fetches id from p inside list item
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
        this.filterModel = Object.assign({ categoryFilter: this.choosenCategoryId }, this.productBrowserFilter.getFilters());
    }
}
//# sourceMappingURL=ProductBrowserHandler.js.map