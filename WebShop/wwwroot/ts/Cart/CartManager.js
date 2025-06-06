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
class CartManager {
    constructor() {
        this.itemQuantitySpan = document.getElementById('cartQuantity');
        this.refreshCartQuantity();
    }
    refreshCartQuantity() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                const response = yield fetch("/User/Cart/GetCartItemsQuantity");
                if (!response.ok) {
                    throw new Error(`API request failed: ${response.status}`);
                }
                const result = yield response.json();
                if (result.success == true) {
                    if (this.itemQuantitySpan)
                        this.itemQuantitySpan.innerText = `${result.newQuantity}`;
                }
                else {
                    if (this.itemQuantitySpan)
                        this.itemQuantitySpan.innerText = `0`;
                }
            }
            catch (error) {
                console.error("Failed to fetch cart quantity:", error);
            }
        });
    }
}
