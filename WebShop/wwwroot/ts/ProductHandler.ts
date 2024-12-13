class ProductHandler {
    private productIdInput: HTMLInputElement | null;
    constructor(
        private productIdSelector: string,
        private nameSelector: string,
        private categorySelector: string,
        private descriptionSelector: string,
        private editorSelector: string,
        private photoUploader: PhotoUploader,
        private extraTopicUploader: ExtraTopicUploader,
        private saveButtonSelector: string
    ) {
        this.attachSaveButtonHandler();
        this.productIdInput = document.querySelector(productIdSelector);

    }


    private attachSaveButtonHandler(): void {
        const saveButton = document.querySelector(this.saveButtonSelector) as HTMLButtonElement;

        saveButton?.addEventListener("click", async (event) => {
            event.preventDefault();

            if (this.validateInputs()) {
                await this.handleProductUpsert();
            }
        });
    }
    /**
     * Validates the input fields (Product Name and Short Description).
     * @returns {boolean} - Returns true if valid, otherwise false.
     */
    validateInputs(): boolean {
        let isValid = true;

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

    /**
     * Retrieves the content from the Quill editor container.
     * @returns {string} - The content of the editor.
     */
    getEditorContent(): string {
        const editorElement = document.querySelector(this.editorSelector) as HTMLElement;
        return editorElement ? editorElement.innerHTML.trim() : '';
    }

    /**
     * Prepares the form data for submission.
     * @returns {FormData} - The complete form data including product details and photos.
     */
    async prepareFormData(): Promise<FormData> {
        const nameInput = document.querySelector(this.nameSelector) as HTMLInputElement;
        const descriptionInput = document.querySelector(this.descriptionSelector) as HTMLInputElement;
        const categorySelect = document.querySelector(this.categorySelector) as HTMLSelectElement;

        const discountStartTimeInput = document.getElementById("discount-start") as HTMLInputElement;
        const discountEndTimeInput = document.getElementById("discount-end") as HTMLInputElement;
        const discountPercentageInput = document.getElementById("discount-percentage") as HTMLInputElement;

        // Get content from Quill editor
        const editorContent = this.getEditorContent();

        // Await the asynchronous photo upload preparation
        const formData = this.photoUploader.preparePhotosToUpload();
        const extraTopicsFormData = this.extraTopicUploader.prepareTopicsToUpload();

        for (const [key, value] of extraTopicsFormData.entries()) {
            formData.append(key, value);
        }

        for (const [key, value] of formData.entries()) {
            console.log(`${key}: ${value}`);
        }

        formData.append("Id", this.productIdInput!.value.trim());
        formData.append("Name", nameInput.value.trim());
        formData.append("ShortDescription", descriptionInput.value.trim());
        formData.append("FullDescription", editorContent);
        formData.append("CategoryId", categorySelect.value.trim());

        formData.append("Discount.StartTime", discountStartTimeInput.value.trim());
        formData.append("Discount.EndTime", discountEndTimeInput.value.trim());
        formData.append("Discount.Percentage", discountPercentageInput.value.trim());

        return formData;
    }

    private async handleProductUpsert(): Promise<void> {
        try {
            const formData = await this.prepareFormData(); // Await the resolved FormData

            const response = await fetch("/Admin/Product/UpsertAjax", {
                method: "POST",
                body: formData, // Resolved FormData passed here
                headers: {
                    "X-Requested-With": "XMLHttpRequest",
                },
            });

            if (!response.ok) {
                throw new Error("Failed to upsert product.");
            }

            const data = await response.json();
            console.log("Product upserted successfully:", data);
            window.location.href = "/Admin/Product/Index";
        } catch (error) {
            console.error("Error during product upsert:", error);
            window.location.href = "/Admin/Product/Index";
        }
    }

}