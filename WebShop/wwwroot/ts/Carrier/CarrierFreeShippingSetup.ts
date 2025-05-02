class CarrierFreeShippingSetup {
    private readonly saveButton: HTMLButtonElement;
    private readonly enableButton: HTMLButtonElement;
    private readonly disableButton: HTMLButtonElement;
    private readonly input: HTMLInputElement;
    private readonly overlay: HTMLDivElement;
    private readonly loadingScreen: HTMLDivElement;
    private readonly freeShippingPriceMeta: HTMLMetaElement;

    private wasFreeShippingPriceSet: boolean = false;
    private readonly apiUrl: string = "/admin/Carrier/SetFreeShippingFromPrice";

    constructor(
        saveButtonSelector: string,
        enableButtonSelector: string,
        disableButtonSelector: string,
        inputSelector: string,
        overlaySelector: string,
        loadingScreenSelector: string,
        freeShippingPriceMetaSelector: string
    ) {
        this.saveButton = document.querySelector(saveButtonSelector) as HTMLButtonElement;
        this.enableButton = document.querySelector(enableButtonSelector) as HTMLButtonElement;
        this.disableButton = document.querySelector(disableButtonSelector) as HTMLButtonElement;
        this.input = document.querySelector(inputSelector) as HTMLInputElement;
        this.overlay = document.querySelector(overlaySelector) as HTMLDivElement;
        this.loadingScreen = document.querySelector(loadingScreenSelector) as HTMLDivElement;
        this.freeShippingPriceMeta = document.querySelector(freeShippingPriceMetaSelector) as HTMLMetaElement;

        this.addEventListeners();
        this.init();
    }

    private init() {
        if (this.freeShippingPriceMeta.content !== '') {
            this.enableFreeShipping();
            this.input.value = this.freeShippingPriceMeta.content;
            this.wasFreeShippingPriceSet = true;
        } else {
            this.disableButton.disabled = true;
        }
    }

    private enableFreeShipping(): void {
        this.overlay.classList.add("d-none");
        this.overlay.classList.remove("d-flex");
        this.saveButton.disabled = false;
        this.enableButton.disabled = true;
        this.disableButton.disabled = false;
        this.input.disabled = false;
    }

    private disableFreeShipping(): void {
        this.overlay.classList.remove("d-none");
        this.overlay.classList.add("d-flex");
        this.disableButton.disabled = true;
        this.enableButton.disabled = false;
        this.saveButton.disabled = true;
        this.input.disabled = true;
        this.input.value = '';
        if (this.wasFreeShippingPriceSet) {
            this.updateFreeShippingPrice(null);
            this.wasFreeShippingPriceSet = false;
        }
    }

    private async updateFreeShippingPrice(price: number | null): Promise<void> {
        try {
            this.loadingScreen.style.display = 'flex'; 

            const response = await fetch(this.apiUrl, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(price)
            });

            const data = await response.json();
            if (data.success) {
                toastr.success(price === null ? "Free shipping disabled" : "Free shipping price updated");
                if (price === null) {
                    this.freeShippingPriceMeta.content = "";
                } else {
                    this.freeShippingPriceMeta.content = price.toString();
                }
            } else {
                toastr.error("Failed to update free shipping price.");
            }
        } catch (error) {
           
            toastr.error("An error occurred while updating the price.");
        } finally {
            this.loadingScreen.style.display = 'none'; 
        }
    }

    private addEventListeners(): void {
        this.disableButton.addEventListener('click', () => this.disableFreeShipping());
        this.enableButton.addEventListener('click', () => this.enableFreeShipping());
        this.saveButton.addEventListener('click', () => {
            const price = parseFloat(this.input.value);
            if (!isNaN(price) && price > 0) {
                this.updateFreeShippingPrice(price);
                this.wasFreeShippingPriceSet = true;
            } else {
                toastr.error("Please enter a valid price.");
            }
        });
    }
}
