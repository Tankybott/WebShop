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
class CarrierFreeShippingSetup {
    constructor(saveButtonSelector, enableButtonSelector, disableButtonSelector, inputSelector, overlaySelector, loadingScreenSelector, freeShippingPriceMetaSelector) {
        this.wasFreeShippingPriceSet = false;
        this.apiUrl = "/admin/Carrier/SetFreeShippingFromPrice";
        this.saveButton = document.querySelector(saveButtonSelector);
        this.enableButton = document.querySelector(enableButtonSelector);
        this.disableButton = document.querySelector(disableButtonSelector);
        this.input = document.querySelector(inputSelector);
        this.overlay = document.querySelector(overlaySelector);
        this.loadingScreen = document.querySelector(loadingScreenSelector);
        this.freeShippingPriceMeta = document.querySelector(freeShippingPriceMetaSelector);
        this.addEventListeners();
        this.init();
    }
    init() {
        if (this.freeShippingPriceMeta.content !== '') {
            this.enableFreeShipping();
            this.input.value = this.freeShippingPriceMeta.content;
            this.wasFreeShippingPriceSet = true;
        }
        else {
            this.disableButton.disabled = true;
        }
    }
    enableFreeShipping() {
        this.overlay.classList.add("d-none");
        this.overlay.classList.remove("d-flex");
        this.saveButton.disabled = false;
        this.enableButton.disabled = true;
        this.disableButton.disabled = false;
        this.input.disabled = false;
    }
    disableFreeShipping() {
        this.overlay.classList.remove("d-none");
        this.overlay.classList.add("d-flex");
        this.disableButton.disabled = true;
        this.enableButton.disabled = false;
        this.saveButton.disabled = true;
        this.input.disabled = true;
        this.input.value = '';
        if (this.wasFreeShippingPriceSet) {
            this.updateFreeShippingPrice(null);
            this.wasFreeShippingPriceSet = false;
        }
    }
    updateFreeShippingPrice(price) {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                this.loadingScreen.style.display = 'flex';
                const response = yield fetch(this.apiUrl, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(price)
                });
                const data = yield response.json();
                if (data.success) {
                    toastr.success(price === null ? "Free shipping disabled" : "Free shipping price updated");
                    if (price === null) {
                        this.freeShippingPriceMeta.content = "";
                    }
                    else {
                        this.freeShippingPriceMeta.content = price.toString();
                    }
                }
                else {
                    toastr.error("Failed to update free shipping price.");
                }
            }
            catch (error) {
                toastr.error("An error occurred while updating the price.");
            }
            finally {
                this.loadingScreen.style.display = 'none';
            }
        });
    }
    addEventListeners() {
        this.disableButton.addEventListener('click', () => this.disableFreeShipping());
        this.enableButton.addEventListener('click', () => this.enableFreeShipping());
        this.saveButton.addEventListener('click', () => {
            const price = parseFloat(this.input.value);
            if (!isNaN(price) && price > 0) {
                this.updateFreeShippingPrice(price);
                this.wasFreeShippingPriceSet = true;
            }
            else {
                toastr.error("Please enter a valid price.");
            }
        });
    }
}
