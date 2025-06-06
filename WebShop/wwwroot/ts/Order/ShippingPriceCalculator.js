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
class ShippingPriceCalculator {
    constructor(selectId, priceParagraphSelector, loadingScreenSelector, totalProductsPriceMetaSelector, totalPriceParagraphSelector, cartItemSelector, ShippingPriceFactorMeta, itemQuantityMetaSelector) {
        this.cartItemSelector = cartItemSelector;
        this.ShippingPriceFactorMeta = ShippingPriceFactorMeta;
        this.itemQuantityMetaSelector = itemQuantityMetaSelector;
        this.carrierSelect = document.querySelector(selectId);
        this.priceParagraph = document.querySelector(priceParagraphSelector);
        this.loadingScreen = document.querySelector(loadingScreenSelector);
        this.totalProductsPriceMeta = document.querySelector(totalProductsPriceMetaSelector);
        this.totalPriceParagraph = document.querySelector(totalPriceParagraphSelector);
        this.carrierSelect.addEventListener('change', (event) => this.setShippingPrice(event));
    }
    setShippingPrice(event) {
        return __awaiter(this, void 0, void 0, function* () {
            const select = event.target;
            const carrierId = select.value;
            const carrier = yield this.fetchCarrier(carrierId);
            let price = 0;
            if (carrier != null) {
                if (!carrier.isPricePerKg) {
                    price = carrier.price;
                }
                else {
                    console.log(carrier.price);
                    price = this.calcShippingPrice(carrier.price);
                    console.log(price);
                    if (price < carrier.minimalShippingPrice && carrier.minimalShippingPrice > 0) {
                        price = carrier.minimalShippingPrice;
                    }
                    console.log(price);
                }
            }
            this.priceParagraph.innerText = price.toString();
            this.setTotalPrice(price);
        });
    }
    fetchCarrier(carrierId) {
        return __awaiter(this, void 0, void 0, function* () {
            this.loadingScreen.style.display = "flex";
            try {
                const response = yield fetch(`/User/Order/GetCarrier?carrierId=${carrierId}`);
                const data = yield response.json();
                if (data.success) {
                    return data.carrier;
                }
                else {
                    console.error("Failed to fetch carrier data");
                    return null;
                }
            }
            catch (error) {
                console.error("Error fetching carrier:", error);
                return null;
            }
            finally {
                this.loadingScreen.style.display = "none";
            }
        });
    }
    setTotalPrice(shippingPrice) {
        const productsPrice = +this.totalProductsPriceMeta.content;
        const totalPrice = productsPrice + shippingPrice;
        this.totalPriceParagraph.innerText = totalPrice.toString();
    }
    calcShippingPrice(pricePerKg) {
        let totalPrice = 0;
        console.log(pricePerKg);
        const cartItems = document.querySelectorAll(this.cartItemSelector);
        cartItems.forEach(item => {
            const shippingPriceFactorMeta = item.querySelector(this.ShippingPriceFactorMeta);
            const quantityMeta = item.querySelector(this.itemQuantityMetaSelector);
            if (shippingPriceFactorMeta && quantityMeta) {
                const factor = parseFloat(shippingPriceFactorMeta.content);
                const quantity = parseInt(quantityMeta.content);
                console.log(quantity + " " + factor + " " + pricePerKg);
                if (!isNaN(factor) && !isNaN(quantity)) {
                    totalPrice += factor * quantity * pricePerKg;
                }
            }
        });
        return Math.round(totalPrice * 10) / 10;
    }
}
