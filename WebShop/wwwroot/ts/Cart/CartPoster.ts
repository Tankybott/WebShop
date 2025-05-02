class CartPoster {
    private readonly postButton: HTMLButtonElement;

    constructor(
        postButtonSelector: string,
        private readonly cartItemsQuantityValidator: BaseCartQuantityValidator<HTMLInputElement>,
        private readonly synchronizationChecker: CartPricesSynchronizationChecker
    ) {
        this.postButton = document.querySelector(postButtonSelector) as HTMLButtonElement;

        this.postButton.addEventListener('click', () => this.handlePostAsync())
    }

    private async handlePostAsync(): Promise<void> {
        let isValid = await this.synchronizationChecker.synchronize();
        if (isValid) isValid = await this.cartItemsQuantityValidator.validateCartQuantities()
        if (isValid) {
            window.location.href = '/User/Order/CreateNewOrder';
        } 
    }
}
