class CartPricesSynchronizationChecker {
    private readonly loadingScreenDiv: HTMLDivElement;
    private readonly cartId: string;
    private readonly inputs: NodeListOf<HTMLInputElement>;
    private isSynchronized: boolean = true;

    constructor(
        inputsSelector: string,
        loadingScreenSelector: string,
        cartIdMetaSelector: string,
        private readonly cartItemIdMetaSelector: string,
        private readonly warningParagraphSelector: string,
        private readonly sweetAlert: SweetAlertDisplayer,
    ) {
        this.loadingScreenDiv = document.querySelector(loadingScreenSelector) as HTMLDivElement;
        this.inputs = document.querySelectorAll(inputsSelector) as NodeListOf<HTMLInputElement>;

        const cartIdMeta = document.querySelector(cartIdMetaSelector) as HTMLMetaElement;
        this.cartId = cartIdMeta.content;

        this.checkForModifiedItemsInSession();
    }

    private async initialize(): Promise<void> {
        await this.synchronizePricesWithServerAsync();
    }

    public async synchronize(): Promise<boolean> {
        await this.synchronizePricesWithServerAsync();
        return this.isSynchronized
    }

    private async synchronizePricesWithServerAsync(): Promise<void> {
        this.isSynchronized = true;
        this.loadingScreenDiv.style.display = "flex";
        try {
            const response = await fetch(`Cart/SynchronizeCartPrices?cartId=${this.cartId}`, {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json"
                },
            });

            const result = await response.json();
            if (result.success) {
                if (result.modifiedCartItemsIds.length > 0) {
                    sessionStorage.setItem("modifiedCartItemsIds", JSON.stringify(result.modifiedCartItemsIds));

                    this.sweetAlert.FireSweetAlert("Update", "Prices of some items have changed. Your cart will be updated accordingly.", () => {
                        location.reload();
                    });
                    this.isSynchronized = false;
                }
            } else {
                console.error("There was an error when synchronizing cart prices");
                window.location.href = "/User/Home";
            }
        } catch (error) {
            console.error("There was an error when synchronizing cart prices");
            window.location.href = "/User/Home";
        } finally {
            this.loadingScreenDiv.style.display = "none";
        }
    }

    private checkForModifiedItemsInSession(): void {
        const storedModifiedIds = sessionStorage.getItem("modifiedCartItemsIds");
        if (!storedModifiedIds) return;

        const modifiedIds: string[] = JSON.parse(storedModifiedIds);
        if (modifiedIds.length === 0) return;

        this.inputs.forEach(input => {
            const cartItemId = this.getCartItemId(input);
            console.log(cartItemId)
            if (modifiedIds.map(String).includes(cartItemId)) {
                const warningParagraph = this.getInputWarningParagraph(input);
                console.log(warningParagraph)
                if (warningParagraph) {
                    console.log(warningParagraph)
                    warningParagraph.textContent = "This item's price was adjusted due to price updates.";
                }
            }
        });

        sessionStorage.removeItem("modifiedCartItemsIds");
    }

    private getInputWarningParagraph(input: HTMLInputElement): HTMLParagraphElement {
        const inputAncestor = this.getInputAncestor(input);
        return inputAncestor.querySelector(this.warningParagraphSelector) as HTMLParagraphElement;
    }

    private getCartItemId(input: HTMLInputElement): string {
        const inputAncestor = this.getInputAncestor(input);
        const cartItemIdMeta = inputAncestor.querySelector(this.cartItemIdMetaSelector) as HTMLMetaElement;
        return cartItemIdMeta.content;
    }

    private getInputAncestor(input: HTMLInputElement): HTMLDivElement {
        return input.parentElement?.parentElement as HTMLDivElement;
    }
}