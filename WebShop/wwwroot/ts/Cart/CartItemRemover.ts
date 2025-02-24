class CartItemRemover {
    private deleteButtons: NodeListOf<HTMLButtonElement>
    private readonly loadingScreen: HTMLDivElement;

    constructor(
        deleteButtonSelector: string,
        loadingScreenSelector: string,
        private readonly itemIdMetaSelector: string,
        private readonly sweetAlert: SweetAlertDisplayer
    ) {
        this.deleteButtons = document.querySelectorAll(deleteButtonSelector) as NodeListOf<HTMLButtonElement>;
        this.loadingScreen = document.querySelector(loadingScreenSelector) as HTMLDivElement;
        this.addEventListenersToButtons();
    }

    private async deleteItem(itemsDeleteButton: HTMLButtonElement): Promise<void> {
        const buttonParent = itemsDeleteButton.parentElement;
        const itemIdMeta = buttonParent?.parentElement?.querySelector(this.itemIdMetaSelector) as HTMLMetaElement;
        const itemId = itemIdMeta.content;

        await this.deleteCartItemFromServer(itemId);
    }

    private async deleteCartItemFromServer(id: string): Promise<void> {
        this.loadingScreen.style.display = "flex"
        try {
            const response = await fetch(`/User/Cart/DeleteCartItem?id=${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                const errorText = await response.text(); 
                throw new Error(`Failed to remove item: ${errorText}`);
            }
            this.loadingScreen.style.display = "none"
            location.reload(); 
        } catch (error) {
            console.error("Error:", error);
            toastr.error(`Something went wrong try again later`);
            this.loadingScreen.style.display = "none"
        }
    }




    private addEventListenersToButtons() {
        this.deleteButtons.forEach(button => {
            button.addEventListener('click', () => this.sweetAlert.FireSweetAlert("Warning", "Are you sure you want to delete this item from your cart?", () => this.deleteItem(button), () => {}, "Confirm"));
        })
    }
}
