class OrderInputsValidator {
    private readonly nameInput: HTMLInputElement;
    private readonly phoneInput: HTMLInputElement;
    private readonly streetInput: HTMLInputElement;
    private readonly cityInput: HTMLInputElement;
    private readonly regionInput: HTMLInputElement;
    private readonly postalInput: HTMLInputElement;
    private readonly countryInput: HTMLInputElement;
    private readonly carrierSelect: HTMLSelectElement;

    constructor(
        nameId: string,
        phoneId: string,
        streetId: string,
        cityId: string,
        regionId: string,
        postalCodeId: string,
        countryId: string,
        carrierSelectId: string
    ) {
        this.nameInput = document.getElementById(nameId) as HTMLInputElement;
        this.phoneInput = document.getElementById(phoneId) as HTMLInputElement;
        this.streetInput = document.getElementById(streetId) as HTMLInputElement;
        this.cityInput = document.getElementById(cityId) as HTMLInputElement;
        this.regionInput = document.getElementById(regionId) as HTMLInputElement;
        this.postalInput = document.getElementById(postalCodeId) as HTMLInputElement;
        this.countryInput = document.getElementById(countryId) as HTMLInputElement;
        this.carrierSelect = document.getElementById(carrierSelectId) as HTMLSelectElement;
    }

    public validateForm(): boolean {
        let isValid = true;

        if (!this.validateRequired(this.nameInput, 1, 100, "Full name is required (1–100 characters).")) isValid = false;
        if (!this.validatePhone(this.phoneInput)) isValid = false;
        if (!this.validateRequired(this.streetInput, 3, 150, "Street address must be 3–150 characters.")) isValid = false;
        if (!this.validateRequired(this.cityInput, 1, 100, "City is required.")) isValid = false;
        if (!this.validateRequired(this.regionInput, 1, 100, "Region is required.")) isValid = false;
        if (!this.validatePostalCode(this.postalInput)) isValid = false;
        if (!this.validateRequired(this.countryInput, 1, 100, "Country is required.")) isValid = false;
        if (!this.validateSelect(this.carrierSelect, "Please select a carrier.")) isValid = false;

        return isValid;
    }

    private validateRequired(input: HTMLInputElement, min: number, max: number, message: string): boolean {
        const value = input.value.trim();
        const isValid = value.length >= min && value.length <= max;
        this.displayValidationMessage(input, isValid ? "" : message);
        return isValid;
    }

    private validatePhone(input: HTMLInputElement): boolean {
        const value = input.value.trim();
        const isValid = /^\+?[0-9\s\-()]{6,20}$/.test(value);
        this.displayValidationMessage(input, isValid ? "" : "Please enter a valid phone number.");
        return isValid;
    }

    private validatePostalCode(input: HTMLInputElement): boolean {
        const value = input.value.trim();
        const isValid = /^[A-Za-z0-9\s\-]{3,12}$/.test(value);
        this.displayValidationMessage(input, isValid ? "" : "Please enter a valid postal code.");
        return isValid;
    }

    private validateSelect(select: HTMLSelectElement, message: string): boolean {
        const isValid = !!select.value;
        this.displayValidationMessage(select, isValid ? "" : message);
        return isValid;
    }

    private displayValidationMessage(element: HTMLElement, message: string): void {
        const parent = element.closest(".form-floating") ?? element.parentElement;
        if (!parent) return;

        let p = parent.querySelector("p");
        if (!p) {
            p = document.createElement("p");
            p.classList.add("text-danger", "mt-1");
            parent.appendChild(p);
        }
        p.textContent = message;
    }
}
