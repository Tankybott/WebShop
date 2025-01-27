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
class ProductBrowserPagination {
    constructor(paginationArrowSelector, paginationSpacerSelector, paginationEdgeSelector, paginationNumberSelector, paginationBoxSelector, apiManager) {
        this.apiManager = apiManager;
        this.PAGE_SIZE = 1;
        this.itemsAmount = 0;
        this.activePageNumber = 1;
        this.pagesAmount = 0;
        this.paginationValues = {
            PageNumber: "",
            PageSize: ""
        };
        this.paginationArrowsParents = [];
        this.paginationSpacersParents = [];
        this.paginationEdgesParents = [];
        this.paginationNumbersParents = [];
        this.paginationArrows = document.querySelectorAll(paginationArrowSelector);
        this.paginationSpacers = document.querySelectorAll(paginationSpacerSelector);
        this.paginationEdges = document.querySelectorAll(paginationEdgeSelector);
        this.paginationNumbers = document.querySelectorAll(paginationNumberSelector);
        this.paginationUl = document.querySelector(paginationBoxSelector);
        this.gatherParentElements();
        this.resetToDefault();
        this.initValues();
        this.apiManager.setInitPaginationCallback(this.initPagination.bind(this));
        this.apiManager.setItemsAmountCallback(this.setItemsAmount.bind(this));
        this.addEventListenersToNumbers();
        this.addEventListenersToEdges();
    }
    gatherParentElements() {
        this.paginationArrows.forEach((arrow) => {
            if (arrow.parentElement)
                this.paginationArrowsParents.push(arrow.parentElement);
        });
        this.paginationSpacers.forEach((spacer) => {
            if (spacer.parentElement)
                this.paginationSpacersParents.push(spacer.parentElement);
        });
        this.paginationEdges.forEach((edge) => {
            if (edge.parentElement)
                this.paginationEdgesParents.push(edge.parentElement);
        });
        this.paginationNumbers.forEach((number) => {
            if (number.parentElement)
                this.paginationNumbersParents.push(number.parentElement);
        });
    }
    resetToDefault() {
        this.paginationValues.PageNumber = this.activePageNumber.toString();
        this.paginationValues.PageSize = this.PAGE_SIZE.toString();
    }
    initValues() {
        this.apiManager.ApiCallOptions.PageNumber = this.paginationValues.PageNumber;
        this.apiManager.ApiCallOptions.PageSize = this.paginationValues.PageSize;
    }
    setItemsAmount(itemsAmount) {
        const itemsAmountNumber = Number(itemsAmount);
        console.log(itemsAmount);
        if (!isNaN(itemsAmountNumber)) {
            this.itemsAmount = itemsAmountNumber;
        }
        else {
            console.error("The value cannot be converted to a number");
        }
    }
    initPagination() {
        this.pagesAmount = Math.ceil(this.itemsAmount / this.PAGE_SIZE);
        this.resetPaginationInterface();
        this.paginationEdges[1].innerText = this.pagesAmount.toString();
        if (this.pagesAmount === 1) {
            this.setupForOnePage();
        }
        if (this.pagesAmount === 3) {
            this.setupForThreePages();
        }
        if (this.pagesAmount === 4) {
            this.setupForFourPages();
        }
        if (this.pagesAmount > 4 && this.activePageNumber === this.pagesAmount - 1) {
            this.setupForBeforeLastActive();
        }
        if (this.pagesAmount > 4 && this.activePageNumber === this.pagesAmount) {
            this.setupForLastActive();
        }
        if (this.pagesAmount > 4 && this.activePageNumber < this.pagesAmount - 2) {
            this.setupForOverFourPages();
        }
        this.setupPaginationArrows(this.pagesAmount);
        this.setupPaginationSpacers(this.pagesAmount);
        this.setupActivePage(this.pagesAmount);
    }
    setupForOnePage() {
        this.paginationEdgesParents[1].style.display = "none";
    }
    setupForThreePages() {
        this.paginationNumbers[1].innerText = "2";
        this.paginationNumbersParents[1].style.display = "block";
    }
    setupForFourPages() {
        this.paginationNumbersParents[0].style.display = "block";
        this.paginationNumbersParents[1].style.display = "block";
        this.paginationNumbers[0].innerText = "2";
        this.paginationNumbers[1].innerText = "3";
    }
    setupForOverFourPages() {
        this.paginationNumbersParents[0].style.display = "block";
        this.paginationNumbersParents[1].style.display = "block";
        this.paginationNumbersParents[2].style.display = "none";
        if (this.activePageNumber == 1) {
            this.paginationNumbers[0].innerText = "2";
            this.paginationNumbers[1].innerText = "3";
        }
        if (this.activePageNumber == 2) {
            this.paginationNumbers[0].innerText = "2";
            this.paginationNumbers[1].innerText = "3";
        }
        if (this.activePageNumber > 2) {
            this.paginationNumbersParents[2].style.display = "block";
            this.paginationNumbers[0].innerText = `${this.activePageNumber - 1}`;
            this.paginationNumbers[1].innerText = `${this.activePageNumber}`;
            this.paginationNumbers[2].innerText = `${this.activePageNumber + 1}`;
        }
    }
    setupForLastActive() {
        const parent0 = this.paginationNumbers[0].parentElement;
        const parent1 = this.paginationNumbers[1].parentElement;
        const parent2 = this.paginationNumbers[2].parentElement;
        if (parent0) {
            parent0.style.display = "block";
            this.paginationNumbers[0].innerText = (this.activePageNumber - 3).toString();
        }
        if (parent1) {
            parent1.style.display = "block";
            this.paginationNumbers[1].innerText = (this.activePageNumber - 2).toString();
        }
        if (parent2) {
            parent2.style.display = "block";
            this.paginationNumbers[2].innerText = (this.activePageNumber - 1).toString();
        }
    }
    setupForBeforeLastActive() {
        this.paginationNumbersParents[0].style.display = "block";
        this.paginationNumbers[0].innerText = (this.activePageNumber - 2).toString();
        this.paginationNumbersParents[1].style.display = "block";
        this.paginationNumbers[1].innerText = (this.activePageNumber - 1).toString();
        this.paginationNumbersParents[2].style.display = "block";
        this.paginationNumbers[2].innerText = (this.activePageNumber).toString();
    }
    setupPaginationSpacers(pagesAmount) {
        var _a, _b;
        const parent0 = (_a = this.paginationSpacers[0]) === null || _a === void 0 ? void 0 : _a.parentElement;
        const parent1 = (_b = this.paginationSpacers[1]) === null || _b === void 0 ? void 0 : _b.parentElement;
        if (parent0 && this.activePageNumber <= 4)
            parent0.style.display = "none";
        if (parent1 && (pagesAmount <= 4 || this.activePageNumber > pagesAmount - 3)) {
            parent1.style.display = "none";
        }
    }
    setupPaginationArrows(pagesAmount) {
        if (this.activePageNumber === 1) {
            this.paginationArrows[0].classList.add("disabled");
        }
        if (this.activePageNumber === pagesAmount) {
            this.paginationArrows[1].classList.add("disabled");
        }
    }
    setupActivePage(pagesAmount) {
        if (this.activePageNumber === 1) {
            this.paginationEdges[0].classList.add("active");
        }
        else if (this.activePageNumber === pagesAmount) {
            this.paginationEdges[1].classList.add("active");
        }
        else {
            this.paginationNumbers.forEach((n) => {
                if (n.innerText.trim() === this.activePageNumber.toString()) {
                    n.classList.add("active");
                }
            });
        }
    }
    resetPaginationInterface() {
        this.paginationArrows.forEach((a) => a.classList.remove("disabled"));
        this.paginationNumbers.forEach((n) => {
            n.classList.remove("active");
            const parent = n.parentElement;
            if (parent)
                parent.style.display = "none";
        });
        this.paginationEdges.forEach((e) => {
            e.classList.remove("active");
            const parent = e.parentElement;
            if (parent)
                parent.style.display = "block";
        });
        this.paginationEdges[0].innerText = "1";
        this.paginationNumbers[0].innerText = (this.activePageNumber - 1).toString();
        this.paginationNumbers[1].innerText = this.activePageNumber.toString();
        this.paginationNumbers[2].innerText = (this.activePageNumber + 1).toString();
    }
    addEventListenersToNumbers() {
        this.paginationNumbers.forEach((n) => {
            n.addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
                try {
                    console.log('proba');
                    this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                    this.apiManager.ApiCallOptions.PageNumber = n.innerText.trim();
                    const activePageNumber = Number(n.innerText);
                    if (!isNaN(activePageNumber)) {
                        this.activePageNumber = activePageNumber;
                    }
                    else {
                        console.error("The value cannot be converted to a number");
                    }
                    yield this.apiManager.getProducts();
                }
                catch (error) {
                    console.error("Error fetching products:", error);
                }
            }));
        });
    }
    addEventListenersToEdges() {
        this.paginationEdges[0].addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
            try {
                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                this.apiManager.ApiCallOptions.PageNumber = "1";
                this.activePageNumber = 1;
                yield this.apiManager.getProducts();
            }
            catch (error) {
                console.error("Error fetching products:", error);
            }
        }));
        this.paginationEdges[1].addEventListener("click", () => __awaiter(this, void 0, void 0, function* () {
            try {
                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                this.apiManager.ApiCallOptions.PageNumber = `${this.pagesAmount}`;
                this.activePageNumber = this.pagesAmount;
                yield this.apiManager.getProducts();
            }
            catch (error) {
                console.error("Error fetching products:", error);
            }
        }));
    }
}
