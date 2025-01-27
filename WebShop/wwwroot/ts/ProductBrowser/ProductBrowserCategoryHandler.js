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
class ProductBrowserCategoryHandler {
    constructor(navigationULSelector, productBrowserFilter, productBrowserApiManager) {
        this.productBrowserFilter = productBrowserFilter;
        this.productBrowserApiManager = productBrowserApiManager;
        this.navigationUl = document.querySelector(navigationULSelector);
        this.initNavigation();
    }
    initNavigation() {
        const allLinks = this.navigationUl.querySelectorAll('a');
        allLinks.forEach(link => {
            link.addEventListener('click', () => __awaiter(this, void 0, void 0, function* () {
                var _a;
                this.productBrowserFilter.resetFilters();
                this.setCategoryLinkActive(link);
                this.productBrowserApiManager.ApiCallOptions.categoryID = (_a = link.nextElementSibling) === null || _a === void 0 ? void 0 : _a.innerHTML;
                yield this.productBrowserApiManager.getProducts();
            }));
        });
    }
    setCategoryLinkActive(link) {
        link.style.color = "blue";
    }
    setBrowsingMessege(messege) {
    }
}
