"use strict";
class CartPoster {
    constructor(postButtonSelector, cartItemsQuantityValidator, synchronizationChecker) {
        this.cartItemsQuantityValidator = cartItemsQuantityValidator;
        this.synchronizationChecker = synchronizationChecker;
        this.postButton = document.querySelector(postButtonSelector);
        this.postButton.addEventListener('click', () => this.handlePostAsync());
    }
    async handlePostAsync() {
        this.synchronizationChecker.initialize();
        let isValid = await this.cartItemsQuantityValidator.validateCartQuantities();
        if (isValid) {
            console.log('poszlo');
        }
    }
}
//# sourceMappingURL=CartPoster.js.map