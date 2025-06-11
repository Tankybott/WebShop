class CartItemAdder {
    private readonly productQuantityInput: HTMLInputElement;
    private readonly productIdMeta: HTMLMetaElement;
    private readonly productDiscountedMeta: HTMLMetaElement;
    private readonly productStockQuntityMeta: HTMLMetaElement;
    private readonly postButton: HTMLButtonElement;

    constructor(
        productQuantityInputSelector: string,
        productIdMetaSelector: string,
        productDiscountedMetaSelector: string,
        productStockQuantityMetaSelector: string,
        postButtonSelector: string,
        private readonly cartManager: CartManager

    ) {
        this.productQuantityInput = document.querySelector(productQuantityInputSelector) as HTMLInputElement;
        this.productIdMeta = document.querySelector(productIdMetaSelector) as HTMLMetaElement;
        this.productDiscountedMeta = document.querySelector(productDiscountedMetaSelector) as HTMLMetaElement;
        this.productStockQuntityMeta = document.querySelector(productStockQuantityMetaSelector) as HTMLMetaElement;
        this.postButton = document.querySelector(postButtonSelector) as HTMLButtonElement;

        this.postButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.addToCart()
        });
    }

    async addToCart(): Promise<void> {
        if (!this.validateQuantityInput()) return
        const formData = new FormData();

        formData.append("ProductId", this.productIdMeta.content);
        formData.append("ProductQuantity", this.productQuantityInput.value);
        formData.append("IsAddedWithDiscount", this.productDiscountedMeta.content === "true" ? "true" : "false");


        try {
            const response = await fetch("AddItemToCart", {
                method: "POST",
                body: formData
            });

            if (response.redirected) {
                const loginUrl = new URL(response.url);
                const returnUrl = loginUrl.searchParams.get("ReturnUrl");

                if (returnUrl && returnUrl.includes("AddItemToCart")) {
                    const productDetailsUrl = `/User/ProductDetails/Details?productId=${this.productIdMeta.content}`;
                    loginUrl.searchParams.set("ReturnUrl", productDetailsUrl);
                }

                window.location.href = loginUrl.toString(); 
                return;
            }

            const result = await response.json();

            if (result.success) {
                toastr.success(result.message);
                this.cartManager.refreshCartQuantity();
            } else {
                toastr.error(result.message);
            }
        } catch (error) {
            toastr.error("Something went wrong, try again later.");
            console.error("Error adding to cart:", error);
        }

        this.productQuantityInput.value = "1";
    }

    validateQuantityInput(): boolean {
        const stockQuantity = parseInt(this.productStockQuntityMeta.content, 10);
        const inputQuantity = parseInt(this.productQuantityInput.value, 10);

        var quantityErrorMessage = this.productQuantityInput.nextElementSibling as HTMLParagraphElement

        if (isNaN(inputQuantity) || inputQuantity <= 0) {
            quantityErrorMessage.textContent = "Quantity must be at least 1.";
            return false;
        }

        if (inputQuantity > stockQuantity) {
            quantityErrorMessage.textContent = `Only ${stockQuantity} items available in stock.`;
            return false;
        }

        quantityErrorMessage.textContent = "";
        return true;
    }
}