"use strict";
class OrderPoster {
    constructor(postButtonSelector, cartTextQuantityValidator, orderInputValidator, nameId, phoneId, streetId, cityId, regionId, postalCodeId, countryId, carrierSelectId, cartIdMetaSelector) {
        this.cartTextQuantityValidator = cartTextQuantityValidator;
        this.orderInputValidator = orderInputValidator;
        this.postButton = document.querySelector(postButtonSelector);
        this.nameInput = document.getElementById(nameId);
        this.phoneInput = document.getElementById(phoneId);
        this.streetInput = document.getElementById(streetId);
        this.cityInput = document.getElementById(cityId);
        this.regionInput = document.getElementById(regionId);
        this.postalInput = document.getElementById(postalCodeId);
        this.countryInput = document.getElementById(countryId);
        this.carrierSelect = document.getElementById(carrierSelectId);
        this.cartIdMeta = document.querySelector(cartIdMetaSelector);
        this.postButton.addEventListener("click", (e) => {
            e.preventDefault();
            this.postOrder();
        });
    }
    async postOrder() {
        let isValid = await this.cartTextQuantityValidator.validateCartQuantities();
        isValid = this.orderInputValidator.validateForm();
        if (!isValid)
            return;
        const formData = this.gatherOrderFormData();
        try {
            const baseUrl = window.location.origin;
            const response = await fetch(`${baseUrl}/User/Order/CreateOrder`, {
                method: "POST",
                body: formData
            });
            const result = await response.json();
            if (result.success && result.paymentRedirectionSuccess) {
                window.location.href = `${result.message}`;
                return;
            }
            else {
                const msg = result.message || "Something went wrong. Please try again.";
                toastr.error(msg);
            }
        }
        catch (error) {
            console.error("Unexpected error posting order:", error);
            toastr.error("Unexpected error occurred while submitting your order.");
        }
    }
    gatherOrderFormData() {
        const formData = new FormData();
        formData.append("Name", this.nameInput.value.trim());
        formData.append("PhoneNumber", this.phoneInput.value.trim());
        formData.append("StreetAdress", this.streetInput.value.trim());
        formData.append("City", this.cityInput.value.trim());
        formData.append("Region", this.regionInput.value.trim());
        formData.append("PostalCode", this.postalInput.value.trim());
        formData.append("Country", this.countryInput.value.trim());
        formData.append("carrierId", this.carrierSelect.value);
        formData.append("cartId", this.cartIdMeta.content);
        return formData;
    }
}
//# sourceMappingURL=OrderPoster.js.map