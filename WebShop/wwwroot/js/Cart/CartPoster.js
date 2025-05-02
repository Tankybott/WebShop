"use strict";
class CartPoster {
    constructor(postButtonSelector, cartItemsQuantityValidator, synchronizationChecker) {
        this.cartItemsQuantityValidator = cartItemsQuantityValidator;
        this.synchronizationChecker = synchronizationChecker;
        this.postButton = document.querySelector(postButtonSelector);
        this.postButton.addEventListener('click', () => this.handlePostAsync());
    }
    async handlePostAsync() {
        let isValid = await this.synchronizationChecker.synchronize();
        if (isValid)
            isValid = await this.cartItemsQuantityValidator.validateCartQuantities();
        if (isValid) {
            window.location.href = '/User/Order/CreateNewOrder';
        }
    }
}
//# sourceMappingURL=CartPoster.js.map