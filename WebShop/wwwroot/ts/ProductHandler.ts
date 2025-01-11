class ProductHandler {
    private productIdInput: HTMLInputElement | null;
    private discountIdInput: HTMLInputElement | null;
    private isDiscountChanged: boolean = false;
    constructor(
        productId: string,
        discountId: string,
        private nameSelector: string,
        private categorySelector: string,
        private descriptionSelector: string,
        private editorSelector: string,
        private photoUploader: PhotoUploader,
        private discountHandler: DiscountHandler,
        private saveButtonSelector: string,

        private discountStartTimeInput: HTMLInputElement,
        private discountEndTimeInput: HTMLInputElement,
        private discountPercentageInput: HTMLInputElement,
    ) {
        this.attachSaveButtonHandler();
        this.productIdInput = document.querySelector(`#${productId}`);
        this.discountIdInput = document.querySelector(`#${discountId}`);

        this.discountStartTimeInput = document.getElementById("discount-start") as HTMLInputElement;
        this.discountEndTimeInput = document.getElementById("discount-end") as HTMLInputElement;
        this.discountPercentageInput = document.getElementById("discount-percentage") as HTMLInputElement;
        this.discountStartTimeInput.addEventListener('change', () => {
            this.isDiscountChanged = true
            console.log(this.isDiscountChanged);
        });
        this.discountEndTimeInput.addEventListener('change', () => this.isDiscountChanged = true);
        this.discountPercentageInput.addEventListener('change', () => this.isDiscountChanged = true);
    }


    private attachSaveButtonHandler(): void {
        const saveButton = document.querySelector("#save-button") as HTMLButtonElement;

        saveButton?.addEventListener("click", async (event) => {
            event.preventDefault();

            if (this.validateInputs()) {
                await this.handleProductUpsert();
            }
        });
    }


    validateInputs(): boolean {
        let isValid = true;
        isValid = this.discountHandler.validateInputs();
        const inputs = document.querySelectorAll("[data-validation]");
        inputs.forEach((input) => {
            const field = input as HTMLInputElement;
            const errorParagraph = field.nextElementSibling as HTMLParagraphElement;

            const value = field.value.trim();
            const validationType = field.getAttribute("data-validation");

            let errorMessage = "";

            if (validationType === "product-name") {
                if (!value || value.length < 1 || value.length > 100) {
                    errorMessage = "Product name must be between 1 and 100 characters.";
                }
            } else if (validationType === "short-description") {
                if (!value || value.length < 1 || value.length > 500) {
                    errorMessage = "Short description must be between 1 and 500 characters.";
                }
            } else if (validationType === "category") {
                if (!value || value === "") {
                    errorMessage = "Please select a valid category.";
                }
            }

            if (errorMessage) {
                isValid = false;
                errorParagraph.textContent = errorMessage; // Display error message in <p>
                field.classList.add("is-invalid"); // Add invalid styling
            } else {
                errorParagraph.textContent = ""; // Clear any previous error message
                field.classList.remove("is-invalid"); // Remove invalid styling
            }
        });

        return isValid;
    }

    getEditorContent(): string {
        const editorElement = document.querySelector(this.editorSelector) as HTMLElement;
        return editorElement ? editorElement.innerHTML.trim() : '';
    }

    async prepareFormData(): Promise<FormData> {
        const nameInput = document.querySelector(this.nameSelector) as HTMLInputElement;
        const descriptionInput = document.querySelector(this.descriptionSelector) as HTMLInputElement;
        const categorySelect = document.querySelector(this.categorySelector) as HTMLSelectElement;
        // Add event listeners for change detection


        // Get content from Quill editor
        const editorContent = this.getEditorContent();

        // Await the asynchronous photo upload preparation
        const formData = this.photoUploader.preparePhotosToUpload();
        const discountFormData = this.discountHandler.prepareDiscountToUpload();

        for (const [key, value] of discountFormData.entries()) {
            formData.append(key, value)
        }

        formData.append("Id", this.productIdInput!.value.trim());
        formData.append("DiscountId", this.discountIdInput!.value.trim());

        formData.append("Name", nameInput.value.trim());
        formData.append("ShortDescription", descriptionInput.value.trim());
        formData.append("FullDescription", editorContent);
        formData.append("CategoryId", categorySelect.value.trim());

        formData.append("DiscountStartDate", this.discountStartTimeInput.value.trim());
        formData.append("DiscountEndDate", this.discountEndTimeInput.value.trim());
        formData.append("DiscountPercentage", this.discountPercentageInput.value.trim());
        formData.append("IsDisocuntChanged", this.isDiscountChanged.toString());


        return formData;
    }

    private async handleProductUpsert(): Promise<void> {
        try {
            const formData = await this.prepareFormData();

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

            console.log("Product upserted successfully.");
        } catch (error) {
            console.error("Error during product upsert:", error);
        } finally {
            window.location.href = "/Admin/Product/Index";
        }
    }
}