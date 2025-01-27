"use strict";
class ProductBrowserCategoryHandler {
    constructor(navigationULSelector, productBrowserFilter, productBrowserApiManager) {
        this.productBrowserFilter = productBrowserFilter;
        this.productBrowserApiManager = productBrowserApiManager;
        this.lastActiveLink = null;
        this.navigationUl = document.querySelector(navigationULSelector);
        this.allCategoryAnchors = this.navigationUl.querySelectorAll('a');
        this.initNavigation();
        this.handleBackNavigation();
    }
    initNavigation() {
        const allLinks = this.navigationUl.querySelectorAll('a');
        allLinks.forEach(link => {
            link.addEventListener('click', async () => {
                var _a;
                this.productBrowserFilter.resetFilters();
                this.setCategoryLinkActive(link);
                this.productBrowserApiManager.ApiCallOptions.categoryIDFilter = (_a = link.nextElementSibling) === null || _a === void 0 ? void 0 : _a.innerHTML;
                await this.productBrowserApiManager.getProducts();
            });
        });
    }
    setCategoryLinkActive(link) {
        var _a, _b;
        (_a = this.lastActiveLink) === null || _a === void 0 ? void 0 : _a.classList.remove("text-success");
        if (this.lastActiveLink && this.lastActiveLink.innerText.toUpperCase() != "ALL")
            (_b = this.lastActiveLink) === null || _b === void 0 ? void 0 : _b.classList.remove("border-bottom");
        link.classList.add("text-success");
        if (link.innerText.toUpperCase() != "ALL")
            link.classList.add("border-bottom");
        this.lastActiveLink = link;
    }
    updateCategoryStatusAfterReload() {
        console.log('dziala');
        this.allCategoryAnchors.forEach(a => {
            var _a;
            if (((_a = a.nextElementSibling) === null || _a === void 0 ? void 0 : _a.innerHTML.trim()) == this.productBrowserApiManager.ApiCallOptions.categoryIDFilter.trim()) {
                this.setCategoryLinkActive(a);
            }
        });
    }
    handleBackNavigation() {
        const navigationEntry = performance.getEntriesByType("navigation")[0];
        if ((navigationEntry === null || navigationEntry === void 0 ? void 0 : navigationEntry.type) === "back_forward") {
            this.updateCategoryStatusAfterReload();
        }
    }
}
//# sourceMappingURL=ProductBrowserCategoryHandler.js.map