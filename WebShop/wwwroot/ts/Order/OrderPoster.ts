class OrderPoster {
    private readonly postButton: HTMLButtonElement;

    private readonly nameInput: HTMLInputElement;
    private readonly phoneInput: HTMLInputElement;
    private readonly streetInput: HTMLInputElement;
    private readonly cityInput: HTMLInputElement;
    private readonly regionInput: HTMLInputElement;
    private readonly postalInput: HTMLInputElement;
    private readonly countryInput: HTMLInputElement;
    private readonly carrierSelect: HTMLSelectElement;
    private readonly cartIdMeta: HTMLMetaElement;

    constructor(
        postButtonSelector: string,
        private readonly cartTextQuantityValidator: CartTextQuantityValidator,
        private readonly orderInputValidator: OrderInputsValidator,

        nameId: string,
        phoneId: string,
        streetId: string,
        cityId: string,
        regionId: string,
        postalCodeId: string,
        countryId: string,
        carrierSelectId: string,
        cartIdMetaSelector: string
    ) {
        this.postButton = document.querySelector(postButtonSelector) as HTMLButtonElement;

        this.nameInput = document.getElementById(nameId) as HTMLInputElement;
        this.phoneInput = document.getElementById(phoneId) as HTMLInputElement;
        this.streetInput = document.getElementById(streetId) as HTMLInputElement;
        this.cityInput = document.getElementById(cityId) as HTMLInputElement;
        this.regionInput = document.getElementById(regionId) as HTMLInputElement;
        this.postalInput = document.getElementById(postalCodeId) as HTMLInputElement;
        this.countryInput = document.getElementById(countryId) as HTMLInputElement;
        this.carrierSelect = document.getElementById(carrierSelectId) as HTMLSelectElement;
        this.cartIdMeta = document.querySelector(cartIdMetaSelector) as HTMLMetaElement;

        this.postButton.addEventListener("click", (e) => {
            e.preventDefault();
            this.postOrder();
        });
    }

    private async postOrder(): Promise<void> {
        let isValid: boolean = await this.cartTextQuantityValidator.validateCartQuantities();
        isValid = this.orderInputValidator.validateForm();
        if (!isValid) return;

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
            } else {
                const msg = result.message || "Something went wrong. Please try again.";
                toastr.error(msg);
            }
        } catch (error) {
            console.error("Unexpected error posting order:", error);
            toastr.error("Unexpected error occurred while submitting your order.");
        }
    }


    private gatherOrderFormData(): FormData {
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
