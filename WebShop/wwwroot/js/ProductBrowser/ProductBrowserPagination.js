"use strict";
class ProductBrowserPagination {
    constructor(paginationArrowSelector, paginationSpacerSelector, paginationEdgeSelector, paginationNumberSelector, paginationBoxSelector, apiManager) {
        this.apiManager = apiManager;
        this.PAGE_SIZE = 20;
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
        this.apiManager.setInitPaginationCallback(this.initPagination.bind(this));
        this.apiManager.setItemsAmountCallback(this.setItemsAmount.bind(this));
        this.addEventListenersToNumbers();
        this.addEventListenersToEdges();
        this.addEventListenerToArrows();
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
        this.activePageNumber = 1;
        this.paginationValues.PageNumber = "1";
        this.paginationValues.PageSize = this.PAGE_SIZE.toString();
        this.initValues();
        this.initPagination();
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
        if (this.itemsAmount > 0) {
            this.paginationUl.style.display = 'flex';
        }
        else {
            this.paginationUl.style.display = 'none';
        }
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
        if (this.pagesAmount > 4 && this.activePageNumber < this.pagesAmount - 1) {
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
        this.paginationNumbersParents[1].style.display = "block";
        this.paginationNumbers[1].innerText = (this.activePageNumber - 2).toString();
        this.paginationNumbersParents[2].style.display = "block";
        this.paginationNumbers[2].innerText = (this.activePageNumber - 1).toString();
    }
    setupForBeforeLastActive() {
        this.paginationNumbersParents[1].style.display = "block";
        this.paginationNumbers[1].innerText = (this.activePageNumber - 1).toString();
        this.paginationNumbersParents[2].style.display = "block";
        this.paginationNumbers[2].innerText = (this.activePageNumber).toString();
    }
    setupPaginationSpacers(pagesAmount) {
        if (pagesAmount <= 4) {
            this.paginationSpacersParents[0].style.display = "none";
        }
        if (this.activePageNumber < 4) {
            this.paginationSpacersParents[0].style.display = "none";
        }
        if (pagesAmount <= 4 || this.activePageNumber > pagesAmount - 3) {
            this.paginationSpacersParents[1].style.display = "none";
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
        if (this.apiManager.ApiCallOptions.PageNumber != "")
            this.activePageNumber = Number(this.apiManager.ApiCallOptions.PageNumber);
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
        this.paginationSpacersParents.forEach(p => p.style.display = "block");
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
            n.addEventListener("click", async () => {
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
                    await this.apiManager.getProducts();
                }
                catch (error) {
                    console.error("Error fetching products:", error);
                }
            });
        });
    }
    addEventListenersToEdges() {
        this.paginationEdges[0].addEventListener("click", async () => {
            try {
                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                this.apiManager.ApiCallOptions.PageNumber = "1";
                this.activePageNumber = 1;
                await this.apiManager.getProducts();
            }
            catch (error) {
                console.error("Error fetching products:", error);
            }
        });
        this.paginationEdges[1].addEventListener("click", async () => {
            try {
                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                this.apiManager.ApiCallOptions.PageNumber = `${this.pagesAmount}`;
                this.activePageNumber = this.pagesAmount;
                await this.apiManager.getProducts();
            }
            catch (error) {
                console.error("Error fetching products:", error);
            }
        });
    }
    addEventListenerToArrows() {
        this.paginationArrows[0].addEventListener("click", async () => {
            try {
                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                this.apiManager.ApiCallOptions.PageNumber = `${this.activePageNumber - 1}`;
                this.activePageNumber = this.activePageNumber - 1;
                await this.apiManager.getProducts();
            }
            catch (error) {
                console.error("Error fetching products:", error);
            }
        });
        this.paginationArrows[1].addEventListener("click", async () => {
            try {
                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                this.apiManager.ApiCallOptions.PageNumber = `${this.activePageNumber + 1}`;
                this.activePageNumber = this.activePageNumber + 1;
                await this.apiManager.getProducts();
            }
            catch (error) {
                console.error("Error fetching products:", error);
            }
        });
    }
}
//# sourceMappingURL=ProductBrowserPagination.js.map