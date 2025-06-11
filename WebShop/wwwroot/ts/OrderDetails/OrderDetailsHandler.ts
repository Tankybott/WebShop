class OrderDetailHandler {
    private readonly processingButton: HTMLButtonElement;
    private readonly idInput: HTMLInputElement;
    private readonly sweetAlert: SweetAlertDisplayer;
    private readonly form: HTMLFormElement;
    private readonly formInputs: NodeListOf<HTMLInputElement>;
    private readonly isValidToSend: boolean;
    private readonly sendButton: HTMLButtonElement;
    private readonly printButton: HTMLButtonElement;

    constructor(
        orderIdInpuId: string,
        processingButtonId: string,
        formId: string,
        sendButtonId: string,
        printButtonId: string,
        sweetAlert: SweetAlertDisplayer
    ) {
        this.processingButton = document.querySelector(processingButtonId) as HTMLButtonElement;
        this.idInput = document.querySelector(orderIdInpuId) as HTMLInputElement;
        this.form = document.querySelector(formId) as HTMLFormElement;
        this.formInputs = this.form.querySelectorAll("input");
        this.sweetAlert = sweetAlert;
        this.sendButton = document.querySelector(sendButtonId) as HTMLButtonElement;
        this.printButton = document.querySelector(printButtonId) as HTMLButtonElement;

        this.isValidToSend = this.checkInputsValidity();
        if (this.processingButton) this.processingButton.addEventListener("click", this.handleStartProcessingClick.bind(this));
        if (this.sendButton) this.sendButton.addEventListener("click", this.handleSendClick.bind(this));
        if (this.printButton) this.printButton.addEventListener("click", this.handlePrintClick.bind(this));
        
    }

    private checkInputsValidity(): boolean {
        for (const input of this.formInputs) {
            if (input.type === "date") {
                if (!input.value || input.value === "0001-01-01") {
                    return false;
                }
            } else {
                if (!input.value.trim()) {
                    return false;
                }
            }
        }
        return true;
    }

    private async handleStartProcessingClick(e: Event): Promise<void> {
        e.preventDefault();

        const orderId = this.idInput?.value;
        if (!orderId) return;

        try {
            const baseUrl = window.location.origin;
            const response = await fetch(`${baseUrl}/User/Order/StartProcessing`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(orderId)
            });

            if (response.redirected || response.url.includes("/AccessDenied") || response.url.includes("/Login")) {
                toastr.error("Access denied.");
                return;
            }

            const result = await response.json();
            if (!result.success) {
                toastr.error("Something went wrong, please try again later.");
            }

            location.reload();
        } catch {
            location.reload();
            toastr.error("Something went wrong, please try again later.");
        } 
    }

    private async handleSendClick(e: Event): Promise<void> {
        e.preventDefault();

        const orderId = this.idInput?.value;
        if (!orderId) return;
        if (!this.isValidToSend) {
            this.sweetAlert.FireSweetAlert("Update order info!!!", "You must first update order information to set order sent...", () => { });
            return;
        }

        try {
            const baseUrl = window.location.origin;
            const response = await fetch(`${baseUrl}/User/Order/SetOrderSent`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(orderId)
            });

            if (response.redirected || response.url.includes("/AccessDenied") || response.url.includes("/Login")) {
                toastr.error("Access denied.");
                return;
            }

            const result = await response.json();
            if (!result.success) {
                toastr.error("Something went wrong, please try again later.");
            } 

            location.reload();
        } catch {location.reload();
            toastr.error("Something went wrong, please try again later.");
        } 
    }

    private handlePrintClick(e: Event): void {
        e.preventDefault();

        const orderId = this.idInput?.value;
        if (!orderId) return;

        const baseUrl = window.location.origin;
        const downloadUrl = `${baseUrl}/User/Order/DownloadOrderPdf/${orderId}`;

        const anchor = document.createElement("a");
        anchor.href = downloadUrl;
        anchor.download = `order_${orderId}.pdf`;
        document.body.appendChild(anchor);
        anchor.click();
        document.body.removeChild(anchor);
    }
}
