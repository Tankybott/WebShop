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
    constructor(startProcessingButtonSelector, orderIdInputSelector) {
        this.orderIdInputSelector = orderIdInputSelector;
        this.processingButton = document.querySelector(startProcessingButtonSelector);
        if (this.processingButton) {
            this.processingButton.addEventListener("click", this.handleStartProcessingClick.bind(this));
        }
    }
    handleStartProcessingClick(e) {
        return __awaiter(this, void 0, void 0, function* () {
            e.preventDefault();
            const orderIdInput = document.querySelector(this.orderIdInputSelector);
            const orderId = orderIdInput === null || orderIdInput === void 0 ? void 0 : orderIdInput.value;
            const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenElement === null || tokenElement === void 0 ? void 0 : tokenElement.value;
            if (!orderId || !token) {
                location.reload();
                return;
            }
            try {
                yield fetch('/Admin/Order/StartProcessing', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(orderId)
                });
            }
            finally {
                location.reload();
            }
        });
    }
}
