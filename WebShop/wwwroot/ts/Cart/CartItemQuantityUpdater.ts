class CartItemQuantityUpdater {
    private readonly quantityInputs: NodeListOf<HTMLInputElement>;
    private readonly loadingScreenDiv: HTMLDivElement;


    private lastStateOfInput: string | null = null;
    constructor(
        quantityInputSelector: string,
        private readonly idMetaSelector: string,
        loadingScreenSelector: string,
    ) {
        this.quantityInputs = document.querySelectorAll(quantityInputSelector) as NodeListOf<HTMLInputElement>;
        this.loadingScreenDiv = document.querySelector(loadingScreenSelector) as HTMLDivElement;
        this.addEventListenrsToInputs();

        document.addEventListener("keydown", (event) => {
            if (event.key === "Enter") {
                const activeElement = document.activeElement as HTMLElement;
                if (activeElement && activeElement.tagName === "INPUT") {
                    activeElement.blur();

                    if (document.body.style.pointerEvents == "none") document.body.style.pointerEvents = "auto";
                }
            }
        });
    }

    private gatherInputAncestorDiv(input: HTMLInputElement): HTMLDivElement {
        return input.parentElement?.parentElement as HTMLDivElement;
    }

    private handleInputFocus(input: HTMLInputElement) {
        this.lastStateOfInput = input.value;
        document.body.style.pointerEvents = "none";
    }

    private async handleInputFocusLostAsync(input: HTMLInputElement): Promise<void> {
        document.body.style.pointerEvents = "none";
        const inputValue = input.value;
        if (inputValue === this.lastStateOfInput) {
            document.body.style.pointerEvents = "auto";
            return;
        };

        this.postChangedInputValue(input);
        document.body.style.pointerEvents = "auto";
    }

    private assingLastStateToInput(input: HTMLInputElement): void {
        input.value = this.lastStateOfInput as string;
    }

    private async postChangedInputValue(input: HTMLInputElement): Promise<void> {
        const inputParentElement = this.gatherInputAncestorDiv(input);
        const productIdMeta = inputParentElement.querySelector(this.idMetaSelector) as HTMLMetaElement;
        const cartItemId = productIdMeta.content;
        const newQuantity = parseInt(input.value, 10);

        if (isNaN(newQuantity)) {
            console.error("Error: Invalid quantity value.");
            return;
        }

        if (newQuantity <= 0) {
            toastr.error('Item quantity cannot be less than 1')
            this.assingLastStateToInput(input);
            return;
        }

        this.loadingScreenDiv.style.display = "flex";
        try {
            const response = await fetch(`Cart/ChangeCartItemQuantity?cartItemId=${cartItemId}&newQuantity=${newQuantity}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
            });

            const result = await response.json();

            if (result.success) {
                toastr.success("Cart item quantity upadated sucessfully")
            } else {
                if (result.quantityLeft != null) {
                    toastr.error("Items out of stock, item quantity is automaticly assigned to maximum quantity of this item")
                    input.value = result.quantityLeft;
                    this.postChangedInputValue(input);
                } else {
                    toastr.error("There was en error when updating items quantity")
                    this.assingLastStateToInput(input);
                }
            }
        } catch (error) {
            toastr.error("There was en error when updating items quantity")
            this.assingLastStateToInput(input);
        } finally {
            this.loadingScreenDiv.style.display = "none";
        }
    }

    private addEventListenrsToInputs(): void {
        this.quantityInputs.forEach(input => {
            input.addEventListener("focusin", () => this.handleInputFocus(input));
            input.addEventListener("focusout", () => this.handleInputFocusLostAsync(input));
        })
    }
}
