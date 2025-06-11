interface IProductDTO {
    id: string;
    name: string;
    mainPhotoUrl: string;
    price: string;
    shortDescription: string;
    discountPercentage: string;
    categoryName: string;
    stockQuantity: string;
}

interface ApiCallOptions extends IFilterModel, IPaginationValues {
    categoryIDFilter: string;
}

class ProductBrowserApiManager {
    private readonly spinnerDiv: HTMLDivElement;
    private readonly itemDisplayDiv: HTMLDivElement;
    private readonly informationP: HTMLParagraphElement;
    private setItemsAmount?: (totalItems: string) => void;
    public setItemsAmountCallback(callback: (totalItems: string) => void): void {
        this.setItemsAmount = (totalItems: string) => {
            callback.call(this, totalItems);
        };
    }
    private initPaginationCallback?: () => void;
    public setInitPaginationCallback(callback: () => void): void {
        this.initPaginationCallback = callback;
    }

    constructor(
        itemsDisplayDivSelector: string,
        informationPSelector: string,
        spinnerDivSelector:string,
        private readonly cardGenerator: ProductBrowserCardGenerator
    ) {
        this.itemDisplayDiv = document.querySelector(itemsDisplayDivSelector) as HTMLDivElement;
        this.informationP = document.querySelector(informationPSelector) as HTMLParagraphElement;
        this.spinnerDiv = document.querySelector(spinnerDivSelector) as HTMLDivElement;

        this.handleBackNavigation();
        this.getProducts();
    }

    public ApiCallOptions: ApiCallOptions = {
        categoryIDFilter: "",
        typedTextFilter: "",
        minimalPriceFilter: "",
        maximalPriceFilter: "",
        showOnlyDiscountFilter: "",
        sortByValueFilter: "",
        PageNumber: "",
        PageSize: ""
    }

    private saveApiCallOptions(): void {
        sessionStorage.setItem("ApiCallOptions", JSON.stringify(this.ApiCallOptions));
    }

    private restoreApiCallOptions(): void {
        const savedOptions = sessionStorage.getItem("ApiCallOptions");
        if (savedOptions) {
            this.ApiCallOptions = JSON.parse(savedOptions);
        }

    }

    private handleBackNavigation(): void {
        const navigationEntry = performance.getEntriesByType("navigation")[0] as PerformanceNavigationTiming;
        if (navigationEntry?.type === "back_forward") {
            this.restoreApiCallOptions();
            this.getProducts();
        } else {
            sessionStorage.removeItem("ApiCallOptions");
        }
    }

    public async getProducts(): Promise<void> {
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
        } catch (error) {
            console.error('Error during API call:', error);
        } finally {
            this.spinnerDiv.style.display = "none";
        }
    }


    private getFilteringString(): string {
        const queryParams = Object.entries(this.ApiCallOptions)
            .map(([key, value]: [string, string]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
        return queryParams.join("&");
    }

    private displayProducts(products: IProductDTO[]): void {
        this.cleanItemDisplayDiv();
        if (products.length > 0) {
            products.forEach(p => {
                this.itemDisplayDiv.appendChild(this.cardGenerator.generateProductCard(p));
                this.informationP.innerText = ""
            })
        } else this.informationP.innerText = "Products not found..."
    }

    private cleanItemDisplayDiv(): void {
        this.itemDisplayDiv.innerHTML = "";
    }
}
