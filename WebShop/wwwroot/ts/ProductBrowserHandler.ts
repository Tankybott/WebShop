interface ProductDTO {
    id: number;
    name: string;
    mainPhotoUrl: string;
    price: number;
    shortDescription: string;
    discountPercentage: number;
    categoryName: string;
}

interface IFullFilterModel extends IFilterModel {
    categoryFilter: string
}

class ProductBrowserHandler{
    private readonly navigationUl: HTMLUListElement;
    private readonly itemDisplayDiv: HTMLDivElement;
    private choosenCategoryId: string = "";
    private filterModel: IFullFilterModel = {
        categoryFilter: "",
        typedTextFilter: "",
        minimalPriceFilter: "",
        maximalPriceFilter: "",
        showOnlyDiscountFilter: "",
        sortByValueFilter: ""
    }
;
    constructor(
        navigationULSelector: string,
        itemDisplayDivSelector: string,
        private readonly productBrowserFilter: ProductBrowserFilter
    ) {
        this.navigationUl = document.querySelector(navigationULSelector) as HTMLUListElement;
        this.itemDisplayDiv = document.querySelector(itemDisplayDivSelector) as HTMLDivElement; 

        this.initNavigation();
    }

    private initNavigation(): void {
        const allLinks: NodeListOf<HTMLAnchorElement> = this.navigationUl.querySelectorAll('a');

        allLinks.forEach(link => {
            const linkContent: string = link.innerText;
            link.addEventListener('click', async () => { 
                this.setCategoryLinkActive(link);
                await this.handleApiCall();
                this.productBrowserFilter.cleanInputs();
            });
        })
    }

    private async handleApiCall(): Promise<void> {
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
        } catch (error) {
            console.error('Error during API call:', error);
        }
    }

    private setCategoryLinkActive(link: HTMLAnchorElement): void {
        link.style.color = "blue";
        this.choosenCategoryId = link.nextElementSibling?.innerHTML as string; //fetches id from p inside list item
    }

    private setBrowsingMessege(messege: string): void {
        
    }

    private displayProducts(products: ProductDTO[]): void {
        if (products.length > 0) {
            products.map(p => this.renderProductCard(p))
        }
    }

    private renderProductCard(product: ProductDTO) {
        var p = document.createElement('p');
        p.innerText = product.name;
        this.itemDisplayDiv.appendChild(p);
    }

    private getFilteringString(): string {
        this.initFilterModel();
        const queryParams = Object.entries(this.filterModel)
            .map(([key, value]: [string, string]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
        return queryParams.join("&");
    }

    private initFilterModel(): void {
        this.filterModel = {
            categoryFilter: this.choosenCategoryId,
            ...this.productBrowserFilter.getFilters()
        }
    }
}