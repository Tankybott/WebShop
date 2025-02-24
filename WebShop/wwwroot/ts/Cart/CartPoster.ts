class CartPoster {
    private readonly postButton: HTMLButtonElement;

    constructor(
        postButtonSelector: string,
        private readonly cartItemsQuantityValidator: CartItemsQuantityValidator,
        private readonly synchronizationChecker: CartPricesSynchronizationChecker
    ) {
        this.postButton = document.querySelector(postButtonSelector) as HTMLButtonElement;

        this.postButton.addEventListener('click', () => this.handlePostAsync())
    }

    private async handlePostAsync(): Promise<void> {
        this.synchronizationChecker.initialize();
        let isValid = await this.cartItemsQuantityValidator.validateCartQuantities()
        if (isValid) {
            console.log('poszlo')
        } 
    }
}
