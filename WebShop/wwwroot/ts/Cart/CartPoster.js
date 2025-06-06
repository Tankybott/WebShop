"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class CartPoster {
    constructor(postButtonSelector, cartItemsQuantityValidator, synchronizationChecker) {
        this.cartItemsQuantityValidator = cartItemsQuantityValidator;
        this.synchronizationChecker = synchronizationChecker;
        this.postButton = document.querySelector(postButtonSelector);
        this.postButton.addEventListener('click', () => this.handlePostAsync());
    }
    handlePostAsync() {
        return __awaiter(this, void 0, void 0, function* () {
            let isValid = yield this.synchronizationChecker.synchronize();
            if (isValid)
                isValid = yield this.cartItemsQuantityValidator.validateCartQuantities();
            if (isValid) {
                window.location.href = '/User/Order/CreateNewOrder';
            }
        });
    }
}
