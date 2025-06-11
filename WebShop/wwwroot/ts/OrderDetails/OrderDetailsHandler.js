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
class OrderDetailHandler {
    constructor(orderIdInpuId, processingButtonId, formId, sendButtonId, printButtonId, sweetAlert) {
        this.processingButton = document.querySelector(processingButtonId);
        this.idInput = document.querySelector(orderIdInpuId);
        this.form = document.querySelector(formId);
        this.formInputs = this.form.querySelectorAll("input");
        this.sweetAlert = sweetAlert;
        this.sendButton = document.querySelector(sendButtonId);
        this.printButton = document.querySelector(printButtonId);
        this.isValidToSend = this.checkInputsValidity();
        if (this.processingButton)
            this.processingButton.addEventListener("click", this.handleStartProcessingClick.bind(this));
        if (this.sendButton)
            this.sendButton.addEventListener("click", this.handleSendClick.bind(this));
        if (this.printButton)
            this.printButton.addEventListener("click", this.handlePrintClick.bind(this));
    }
    checkInputsValidity() {
        for (const input of this.formInputs) {
            if (input.type === "date") {
                if (!input.value || input.value === "0001-01-01") {
                    return false;
                }
            }
            else {
                if (!input.value.trim()) {
                    return false;
                }
            }
        }
        return true;
    }
    handleStartProcessingClick(e) {
        return __awaiter(this, void 0, void 0, function* () {
            var _a;
            console.log("wysyla");
            e.preventDefault();
            const orderId = (_a = this.idInput) === null || _a === void 0 ? void 0 : _a.value;
            if (!orderId)
                return;
            try {
                const baseUrl = window.location.origin;
                const response = yield fetch(`${baseUrl}/User/Order/StartProcessing`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(orderId)
                });
                const result = yield response.json();
                if (!result.success) {
                    this.sweetAlert.FireSweetAlert("Something went wrong", "Please try again later.", () => { });
                }
            }
            catch (_b) {
                this.sweetAlert.FireSweetAlert("Something went wrong", "Please try again later.", () => { });
            }
            finally {
                location.reload();
            }
        });
    }
    handleSendClick(e) {
        return __awaiter(this, void 0, void 0, function* () {
            var _a;
            console.log("leci");
            e.preventDefault();
            const orderId = (_a = this.idInput) === null || _a === void 0 ? void 0 : _a.value;
            if (!orderId)
                return;
            if (!this.isValidToSend) {
                this.sweetAlert.FireSweetAlert("Update order info!!!", "You must first update order information to set order sent...", () => { });
                return;
            }
            try {
                const baseUrl = window.location.origin;
                const response = yield fetch(`${baseUrl}/User/Order/SetOrderSent`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(orderId)
                });
                const result = yield response.json();
                if (!result.success) {
                    this.sweetAlert.FireSweetAlert("Something went wrong", "Please try again later.", () => { });
                }
            }
            catch (_b) {
                this.sweetAlert.FireSweetAlert("Something went wrong", "Please try again later.", () => { });
            }
            finally {
                location.reload();
            }
        });
    }
    handlePrintClick(e) {
        var _a;
        e.preventDefault();
        const orderId = (_a = this.idInput) === null || _a === void 0 ? void 0 : _a.value;
        if (!orderId)
            return;
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
