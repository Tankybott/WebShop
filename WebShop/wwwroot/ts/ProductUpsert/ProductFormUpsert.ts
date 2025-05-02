class ProductFormUpsert {
    private readonly productIdInput: HTMLInputElement;
    private readonly discountIdInput: HTMLInputElement;
    private readonly nameInput: HTMLInputElement;
    private readonly categoryInput: HTMLInputElement;
    private readonly priceInput: HTMLInputElement;
    private readonly shippingPriceFactorInput: HTMLInputElement;
    private readonly stockQuantityInput: HTMLInputElement;
    private readonly textEditor: HTMLElement;
    private readonly saveButton: HTMLButtonElement;
    constructor(
        productId: string,
        discountId: string,
        nameId: string,
        categoryId: string,
        priceId: string,
        shippingPriceFactorInputId: string,
        stockQuantity: string,
        textEditorSelector: string,
        saveButtonSelector: string,
        private photoUploader: PhotoUploader,
        private discountHandler: DiscountHandler,
    ) {
        this.productIdInput = document.querySelector(`#${productId}`) as HTMLInputElement;
        this.discountIdInput = document.querySelector(`#${discountId}`) as HTMLInputElement;
        this.nameInput = document.querySelector(`#${nameId}`) as HTMLInputElement;
        this.categoryInput = document.querySelector(`#${categoryId}`) as HTMLInputElement;
        this.priceInput = document.querySelector(`#${priceId}`) as HTMLInputElement;
        this.shippingPriceFactorInput = document.querySelector(`#${shippingPriceFactorInputId}`) as HTMLInputElement;
        this.stockQuantityInput = document.querySelector(`#${stockQuantity}`) as HTMLInputElement;
        this.textEditor = document.querySelector(textEditorSelector) as HTMLElement;
        this.saveButton = document.querySelector(saveButtonSelector) as HTMLButtonElement;

        this.attachSaveButtonHandler();

        console.log('ok')
    }

    private attachSaveButtonHandler(): void {
        this.saveButton.addEventListener("click", async (event) => {
            event.preventDefault();

            if (this.validateInputs()) {
                await this.handleProductUpsert();
            }
        });
    }

    private validateInputs(): boolean {
        let isValid: boolean = true;
        if (!this.photoUploader.validateMainPhoto()) isValid = false;
        if (!this.discountHandler.validateInputs()) isValid = false;
        if (!this.validateName()) isValid = false;
        if (!this.validateCategory()) isValid = false;
        if (!this.validatePrice()) isValid = false;
        if (!this.validateShippingPriceFactor()) isValid = false;
        if (!this.validateStockQuantity()) isValid = false;

        return isValid;
    }

    private validateName(): boolean {
        let isValid = true;
        const nameValue: string = this.nameInput.value;
        const nameValidationParagraph: HTMLParagraphElement = this.nameInput.parentElement?.querySelector('p') as HTMLParagraphElement;
        nameValidationParagraph.innerText = "";

        if (!nameValue || nameValue.length < 1 || nameValue.length > 100) {
            nameValidationParagraph.innerText = "Product name must be between 1 and 100 characters.";
            isValid = false;
        }
        return isValid;
    }

    private validateCategory(): boolean {
        let isValid = true;
        const categoryValue: string = this.categoryInput.value;
        const cateogryValidationParagraph: HTMLParagraphElement = this.categoryInput.parentElement?.querySelector('p') as HTMLParagraphElement;
        cateogryValidationParagraph.innerText = "";
        if (!categoryValue || categoryValue === "") {
            cateogryValidationParagraph.innerText = "Please select a valid category.";
            isValid = false;
        }
        return isValid;
    }

    private validatePrice(): boolean {
        let isValid = true;
        const priceValue: string = this.priceInput.value;
        const priceValidationParagraph: HTMLParagraphElement = this.priceInput.parentElement?.querySelector('p') as HTMLParagraphElement;
        priceValidationParagraph.innerText = ""; 

        if (!priceValue || isNaN(Number(priceValue)) || parseFloat(priceValue) <= 0) {
            priceValidationParagraph.innerText = "Price must be a number greater than 0.";
            isValid = false;
        }

        return isValid;
    }

    private validateShippingPriceFactor(): boolean {
        let isValid = true;
        const priceValue: string = this.shippingPriceFactorInput.value;
        const priceValidationParagraph: HTMLParagraphElement = this.shippingPriceFactorInput.parentElement?.querySelector('p') as HTMLParagraphElement;
        priceValidationParagraph.innerText = "";

        if (!priceValue || isNaN(Number(priceValue)) || parseFloat(priceValue) <= 0) {
            priceValidationParagraph.innerText = "Shipping price factor must be a number greater than 0.";
            isValid = false;
        }

        return isValid;
    }

    private validateStockQuantity(): boolean {
        let isValid = true;
        const stockQuantityInputValue: string = this.stockQuantityInput.value;
        const stockQuantityParagraph: HTMLParagraphElement = this.stockQuantityInput.parentElement?.querySelector('p') as HTMLParagraphElement;
        stockQuantityParagraph.innerText = "";

        if (!stockQuantityInputValue || isNaN(Number(stockQuantityInputValue)) || parseFloat(stockQuantityInputValue) < 0) {
            stockQuantityParagraph.innerText = "Stock quantity cannot be lower than 0.";
            isValid = false;
        }
        return isValid;
    }

    private getEditorContent(): string {
        // Clone the editor to prevent modifying the original DOM
        const editorClone = this.textEditor.cloneNode(true) as HTMLElement;

        // Remove tooltips
        const tooltips = editorClone.querySelectorAll(".ql-tooltip");
        tooltips.forEach(tooltip => tooltip.remove());

        // Remove contenteditable attributes from all elements
        const editableElements = editorClone.querySelectorAll("[contenteditable]");
        editableElements.forEach(el => el.removeAttribute("contenteditable"));

        // Optionally sanitize other attributes if needed (e.g., remove styles)
        const elementsWithStyles = editorClone.querySelectorAll("[style]");
        elementsWithStyles.forEach(el => el.removeAttribute("style"));

        // Return the cleaned HTML
        return editorClone.innerHTML.trim();
    }

    async prepareFormData(): Promise<FormData> {
        const editorContent = this.getEditorContent();

        const formData = this.photoUploader.preparePhotosToUpload();
        const discountFormData = this.discountHandler.prepareDiscountToUpload();
        for (const [key, value] of discountFormData.entries()) {
            formData.append(key, value)
        }

        formData.append("Id", this.productIdInput!.value.trim());
        formData.append("DiscountId", this.discountIdInput!.value.trim());

        formData.append("Name", this.nameInput.value.trim());
        formData.append("Price", this.priceInput.value.trim());
        formData.append("FullDescription", editorContent);
        formData.append("CategoryId", this.categoryInput.value.trim());
        formData.append("StockQuantity", this.stockQuantityInput.value.trim());
        formData.append("ShippingPriceFactor", this.shippingPriceFactorInput.value.trim());
        return formData;
    }

    private async handleProductUpsert(): Promise<void> {
        try {
            const formData = await this.prepareFormData();
            console.log(formData)

            const response = await fetch("/Admin/Product/UpsertAjax", {
                method: "POST",
                body: formData,
                headers: {
                    "X-Requested-With": "XMLHttpRequest",
                },
            });

            if (!response.ok) {
                throw new Error("Failed to upsert product.");
            }
        } catch (error) {
            console.error("Error during product upsert:", error);
        } finally {
            window.location.href = "/Admin/Product/Index";
        }
    }
}

