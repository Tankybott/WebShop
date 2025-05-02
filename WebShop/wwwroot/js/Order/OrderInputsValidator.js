"use strict";
class OrderInputsValidator {
    constructor(nameId, phoneId, streetId, cityId, regionId, postalCodeId, countryId, carrierSelectId) {
        this.nameInput = document.getElementById(nameId);
        this.phoneInput = document.getElementById(phoneId);
        this.streetInput = document.getElementById(streetId);
        this.cityInput = document.getElementById(cityId);
        this.regionInput = document.getElementById(regionId);
        this.postalInput = document.getElementById(postalCodeId);
        this.countryInput = document.getElementById(countryId);
        this.carrierSelect = document.getElementById(carrierSelectId);
    }
    validateForm() {
        let isValid = true;
        if (!this.validateRequired(this.nameInput, 1, 100, "Full name is required (1–100 characters)."))
            isValid = false;
        if (!this.validatePhone(this.phoneInput))
            isValid = false;
        if (!this.validateRequired(this.streetInput, 3, 150, "Street address must be 3–150 characters."))
            isValid = false;
        if (!this.validateRequired(this.cityInput, 1, 100, "City is required."))
            isValid = false;
        if (!this.validateRequired(this.regionInput, 1, 100, "Region is required."))
            isValid = false;
        if (!this.validatePostalCode(this.postalInput))
            isValid = false;
        if (!this.validateRequired(this.countryInput, 1, 100, "Country is required."))
            isValid = false;
        if (!this.validateSelect(this.carrierSelect, "Please select a carrier."))
            isValid = false;
        return isValid;
    }
    validateRequired(input, min, max, message) {
        const value = input.value.trim();
        const isValid = value.length >= min && value.length <= max;
        this.displayValidationMessage(input, isValid ? "" : message);
        return isValid;
    }
    validatePhone(input) {
        const value = input.value.trim();
        const isValid = /^\+?[0-9\s\-()]{6,20}$/.test(value);
        this.displayValidationMessage(input, isValid ? "" : "Please enter a valid phone number.");
        return isValid;
    }
    validatePostalCode(input) {
        const value = input.value.trim();
        const isValid = /^[A-Za-z0-9\s\-]{3,12}$/.test(value);
        this.displayValidationMessage(input, isValid ? "" : "Please enter a valid postal code.");
        return isValid;
    }
    validateSelect(select, message) {
        const isValid = !!select.value;
        this.displayValidationMessage(select, isValid ? "" : message);
        return isValid;
    }
    displayValidationMessage(element, message) {
        var _a;
        const parent = (_a = element.closest(".form-floating")) !== null && _a !== void 0 ? _a : element.parentElement;
        if (!parent)
            return;
        let p = parent.querySelector("p");
        if (!p) {
            p = document.createElement("p");
            p.classList.add("text-danger", "mt-1");
            parent.appendChild(p);
        }
        p.textContent = message;
    }
}
//# sourceMappingURL=OrderInputsValidator.js.map