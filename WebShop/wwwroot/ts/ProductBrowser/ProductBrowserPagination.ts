interface IPaginationValues {
    PageNumber: string;
    PageSize: string;
}
class ProductBrowserPagination {
    private readonly PAGE_SIZE: number = 20;
    private itemsAmount: number = 0;
    private activePageNumber: number = 1;
    private pagesAmount: number = 0;

    private paginationValues: IPaginationValues = {
        PageNumber: "",
        PageSize: ""
    }

    private readonly paginationArrows: NodeListOf<HTMLAnchorElement>;
    private readonly paginationSpacers: NodeListOf<HTMLAnchorElement>;
    private readonly paginationEdges: NodeListOf<HTMLAnchorElement>;
    private readonly paginationNumbers: NodeListOf<HTMLAnchorElement>;
    private readonly paginationUl: HTMLUListElement;

    private readonly paginationArrowsParents: HTMLElement[] = [];
    private readonly paginationSpacersParents: HTMLElement[] = [];
    private readonly paginationEdgesParents: HTMLElement[] = [];
    private readonly paginationNumbersParents: HTMLElement[] = [];

    constructor(
        paginationArrowSelector: string,
        paginationSpacerSelector: string,
        paginationEdgeSelector: string,
        paginationNumberSelector: string,
        paginationBoxSelector: string,
        private readonly apiManager: ProductBrowserApiManager
    ) {
        this.paginationArrows = document.querySelectorAll(paginationArrowSelector);
        this.paginationSpacers = document.querySelectorAll(paginationSpacerSelector);
        this.paginationEdges = document.querySelectorAll(paginationEdgeSelector)
        this.paginationNumbers = document.querySelectorAll(paginationNumberSelector)
        this.paginationUl = document.querySelector(paginationBoxSelector) as HTMLUListElement;
                this.gatherParentElements();

        this.apiManager.setInitPaginationCallback(this.initPagination.bind(this));
        this.apiManager.setItemsAmountCallback(this.setItemsAmount.bind(this));

        this.addEventListenersToNumbers();
        this.addEventListenersToEdges();
        this.addEventListenerToArrows();
    }

    private gatherParentElements(): void {
        this.paginationArrows.forEach((arrow) => {
            if (arrow.parentElement) this.paginationArrowsParents.push(arrow.parentElement);
        });
        this.paginationSpacers.forEach((spacer) => {
            if (spacer.parentElement) this.paginationSpacersParents.push(spacer.parentElement);
        });
        this.paginationEdges.forEach((edge) => {
            if (edge.parentElement) this.paginationEdgesParents.push(edge.parentElement);
        });
        this.paginationNumbers.forEach((number) => {
            if (number.parentElement) this.paginationNumbersParents.push(number.parentElement);
        });
    }

    public resetToDefault() {
        this.activePageNumber = 1;
        this.paginationValues.PageNumber = "1";
        this.paginationValues.PageSize = this.PAGE_SIZE.toString();

        this.initValues();
        this.initPagination();
    }

    private initValues() {
        this.apiManager.ApiCallOptions.PageNumber = this.paginationValues.PageNumber;
        this.apiManager.ApiCallOptions.PageSize = this.paginationValues.PageSize;
    }

    public setItemsAmount(itemsAmount: string): void {
        const itemsAmountNumber = Number(itemsAmount);
        console.log(itemsAmount);
        if (!isNaN(itemsAmountNumber)) {
            this.itemsAmount = itemsAmountNumber;
        } else {
            console.error("The value cannot be converted to a number");
        }
    }

    private initPagination(): void {

        this.pagesAmount = Math.ceil(this.itemsAmount / this.PAGE_SIZE);
        if (this.itemsAmount > 0) {
            this.paginationUl.style.display = 'flex'
        } else {
            this.paginationUl.style.display = 'none'
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

    private setupForOnePage(): void {
        this.paginationEdgesParents[1].style.display = "none";
    }

    private setupForThreePages(): void {
        this.paginationNumbers[1].innerText = "2";
        this.paginationNumbersParents[1].style.display = "block";
    }

    private setupForFourPages(): void {
        this.paginationNumbersParents[0].style.display = "block";
        this.paginationNumbersParents[1].style.display = "block";
        this.paginationNumbers[0].innerText = "2"
        this.paginationNumbers[1].innerText = "3"
    }

    private setupForOverFourPages(): void {
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
            this.paginationNumbers[0].innerText = `${this.activePageNumber -1}`;
            this.paginationNumbers[1].innerText = `${this.activePageNumber}`;
            this.paginationNumbers[2].innerText = `${this.activePageNumber + 1}`;
        }
    }

    private setupForLastActive(): void {
        this.paginationNumbersParents[1].style.display = "block";
        this.paginationNumbers[1].innerText = (this.activePageNumber - 2).toString();

        this.paginationNumbersParents[2].style.display = "block";
        this.paginationNumbers[2].innerText = (this.activePageNumber - 1).toString();
    }

    private setupForBeforeLastActive(): void {
        this.paginationNumbersParents[1].style.display = "block";
        this.paginationNumbers[1].innerText = (this.activePageNumber - 1).toString();

        this.paginationNumbersParents[2].style.display = "block";
        this.paginationNumbers[2].innerText = (this.activePageNumber).toString();
    }

    private setupPaginationSpacers(pagesAmount: number): void {
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

    private setupPaginationArrows(pagesAmount: number): void {
        if (this.activePageNumber === 1) {
            this.paginationArrows[0].classList.add("disabled");
        }
        if (this.activePageNumber === pagesAmount) {
            this.paginationArrows[1].classList.add("disabled");
        }
    }

    private setupActivePage(pagesAmount: number): void {
        if (this.apiManager.ApiCallOptions.PageNumber != "") this.activePageNumber = Number(this.apiManager.ApiCallOptions.PageNumber);

        if (this.activePageNumber === 1) {
            this.paginationEdges[0].classList.add("active");
        } else if (this.activePageNumber === pagesAmount) {
            this.paginationEdges[1].classList.add("active");
        } else {
            this.paginationNumbers.forEach((n) => {
                if (n.innerText.trim() === this.activePageNumber.toString()) {
                    n.classList.add("active");
                }
            });
        }
    }

    private resetPaginationInterface(): void {
        this.paginationArrows.forEach((a) => a.classList.remove("disabled"));
        this.paginationSpacersParents.forEach(p => p.style.display = "block")
        this.paginationNumbers.forEach((n) => {
            n.classList.remove("active");
            const parent = n.parentElement;
            if (parent) parent.style.display = "none";
        });
        this.paginationEdges.forEach((e) => {
            e.classList.remove("active");
            const parent = e.parentElement;
            if (parent) parent.style.display = "block";
        });
        this.paginationEdges[0].innerText = "1"
        this.paginationNumbers[0].innerText = (this.activePageNumber - 1).toString();
        this.paginationNumbers[1].innerText = this.activePageNumber.toString();
        this.paginationNumbers[2].innerText = (this.activePageNumber + 1).toString();
    }

    private addEventListenersToNumbers(): void {
        this.paginationNumbers.forEach((n) => {
            n.addEventListener("click", async () => {
                try {
                    console.log('proba')
                    this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                    this.apiManager.ApiCallOptions.PageNumber = n.innerText.trim();
                    const activePageNumber = Number(n.innerText)
                    if (!isNaN(activePageNumber)) {
                        this.activePageNumber = activePageNumber;
                    } else {
                        console.error("The value cannot be converted to a number");
                    }
                    await this.apiManager.getProducts();
                } catch (error) {
                    console.error("Error fetching products:", error);
                }
            });
        });
    }

    private addEventListenersToEdges(): void {
        this.paginationEdges[0].addEventListener("click", async () => {
            try {
                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                this.apiManager.ApiCallOptions.PageNumber = "1";
                this.activePageNumber = 1;
                await this.apiManager.getProducts();
            } catch (error) {
                console.error("Error fetching products:", error);
            }
        });
        this.paginationEdges[1].addEventListener("click", async () => {
            try {
                
                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString(); 
                this.apiManager.ApiCallOptions.PageNumber = `${this.pagesAmount}`;
                this.activePageNumber = this.pagesAmount;
                await this.apiManager.getProducts();
            } catch (error) {
                console.error("Error fetching products:", error);
            }
        });
    }

    private addEventListenerToArrows(): void {
        this.paginationArrows[0].addEventListener("click", async () => {
            try {
                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                this.apiManager.ApiCallOptions.PageNumber = `${this.activePageNumber - 1}`;
                this.activePageNumber = this.activePageNumber - 1;
                await this.apiManager.getProducts();
            } catch (error) {
                console.error("Error fetching products:", error);
            }
        });
        this.paginationArrows[1].addEventListener("click", async () => {
            try {

                this.apiManager.ApiCallOptions.PageSize = this.PAGE_SIZE.toString();
                this.apiManager.ApiCallOptions.PageNumber = `${this.activePageNumber + 1}`;
                this.activePageNumber = this.activePageNumber + 1;
                await this.apiManager.getProducts();
            } catch (error) {
                console.error("Error fetching products:", error);
            }
        });
    }
}