interface IFullFilterModel extends IFilterModel {
    categoryIdFilter: string
}

class ProductBrowserCategoryHandler{
    private readonly navigationUl: HTMLUListElement;
    private lastActiveLink: HTMLAnchorElement | null = null
    private allCategoryAnchors: NodeListOf<HTMLAnchorElement>;
    constructor(
        navigationULSelector: string,
        private readonly productBrowserFilter: ProductBrowserFilter,
        private readonly productBrowserApiManager: ProductBrowserApiManager
    ) {
        this.navigationUl = document.querySelector(navigationULSelector) as HTMLUListElement;
        this.allCategoryAnchors = this.navigationUl.querySelectorAll('a');
        this.initNavigation();

        this.handleBackNavigation();
    }

    private initNavigation(): void {
        const allLinks: NodeListOf<HTMLAnchorElement> = this.navigationUl.querySelectorAll('a');

        allLinks.forEach(link => {
            link.addEventListener('click', async () => { 
                this.productBrowserFilter.resetFilters();
                this.setCategoryLinkActive(link);
                this.productBrowserApiManager.ApiCallOptions.categoryIDFilter = link.nextElementSibling?.innerHTML as string;
                await this.productBrowserApiManager.getProducts();
            });
        })
    }

    private setCategoryLinkActive(link: HTMLAnchorElement): void {
        this.lastActiveLink?.classList.remove("text-success");
        if (this.lastActiveLink && this.lastActiveLink.innerText.toUpperCase() != "ALL") this.lastActiveLink?.classList.remove("border-bottom");
        link.classList.add("text-success");
        if (link.innerText.toUpperCase() != "ALL") link.classList.add("border-bottom");
        this.lastActiveLink = link;
    }

    private updateCategoryStatusAfterReload(): void {
        console.log('dziala')
        this.allCategoryAnchors.forEach(a => {
            if (a.nextElementSibling?.innerHTML.trim() == this.productBrowserApiManager.ApiCallOptions.categoryIDFilter.trim()) {
                this.setCategoryLinkActive(a);
            }
        })
    }

    private handleBackNavigation(): void {
        const navigationEntry = performance.getEntriesByType("navigation")[0] as PerformanceNavigationTiming;
        if (navigationEntry?.type === "back_forward") {
            this.updateCategoryStatusAfterReload();
        } 
    }
}