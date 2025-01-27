"use strict";
class DiscountHandler {
    constructor(collapseButtonSelector, clearButtonSelector, discountStartTimeInputId, discountEndTimeInputId, discountPercentageInputId) {
        var _a, _b, _c;
        this.arrowUp = '<i class="bi bi-chevron-compact-up"></i>';
        this.buttonContent = "";
        this.discountStartTimeInput = document.querySelector(`#${discountStartTimeInputId}`);
        this.startTimeValidationParagraph = (_a = this.discountStartTimeInput.parentElement) === null || _a === void 0 ? void 0 : _a.querySelector('p');
        this.discountEndTimeInput = document.querySelector(`#${discountEndTimeInputId}`);
        this.endTimeValidationParagraph = (_b = this.discountEndTimeInput.parentElement) === null || _b === void 0 ? void 0 : _b.querySelector('p');
        this.discountPercentageInput = document.querySelector(`#${discountPercentageInputId}`);
        this.percentageValidationParagraph = (_c = this.discountPercentageInput.parentElement) === null || _c === void 0 ? void 0 : _c.querySelector('p');
        this.collapseButton = document.querySelector(collapseButtonSelector);
        this.collapseButtonInnerDiv = this.collapseButton.querySelector('div');
        this.buttonContent = this.collapseButtonInnerDiv.innerHTML;
        this.clearButton = document.querySelector(clearButtonSelector);
        this.initialStartTime = this.discountStartTimeInput.value;
        this.initialEndTime = this.discountEndTimeInput.value;
        this.initialPercentage = this.discountPercentageInput.value;
        this.init();
    }
    init() {
        this.collapseButton.addEventListener('click', this.changeButtonContent.bind(this));
        this.clearButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.handleClearButtonClick();
        });
    }
    changeButtonContent() {
        this.collapseButtonInnerDiv.classList.remove('show');
        this.collapseButton.style.pointerEvents = "none";
        setTimeout(() => {
            if (this.collapseButtonInnerDiv.innerHTML.trim() === this.buttonContent.trim()) {
                this.collapseButtonInnerDiv.innerHTML = this.arrowUp;
            }
            else {
                this.collapseButtonInnerDiv.innerHTML = this.buttonContent;
            }
            this.collapseButtonInnerDiv.classList.add('show');
            this.collapseButton.style.pointerEvents = "auto";
        }, 150);
    }
    validateDiscountInputs() {
        let isValid = true;
        return isValid;
    }
    isDiscountChanged() {
        if (this.initialStartTime == this.discountStartTimeInput.value
            && this.initialEndTime == this.discountEndTimeInput.value
            && this.initialPercentage == this.discountPercentageInput.value) {
            return false;
        }
        else {
            return true;
        }
    }
    handleClearButtonClick() {
        this.discountStartTimeInput.value = "";
        this.discountEndTimeInput.value = "";
        this.discountPercentageInput.value = "";
        this.startTimeValidationParagraph.innerText = "";
        this.endTimeValidationParagraph.innerText = "";
        this.percentageValidationParagraph.innerText = "";
    }
    validateInputs() {
        let isValid = true;
        if (this.isDiscountChanged()) {
            if (!this.validateStartTime()) {
                isValid = false;
            }
            if (!this.validateEndTime()) {
                isValid = false;
            }
            if (!this.validatePercentage()) {
                isValid = false;
            }
        }
        return isValid;
    }
    IsDiscountCleaned() {
        if (this.discountStartTimeInput.value == ""
            && this.discountEndTimeInput.value == ""
            && this.discountPercentageInput.value == "") {
            return true;
        }
        else {
            return false;
        }
    }
    validateStartTime() {
        let isValid = true;
        const startTimeValue = this.discountStartTimeInput.value;
        this.startTimeValidationParagraph.innerText = "";
        if (!this.IsDiscountCleaned()) {
            if (startTimeValue == "") {
                this.startTimeValidationParagraph.innerText = "The input cannot be empty. Please provide a valid value or clear all discount fields.";
                isValid = false;
            }
            else if (isNaN(Date.parse(startTimeValue))) {
                this.startTimeValidationParagraph.innerText = "The date format is invalid. Please provide a valid date and time or clear all discount fields.";
                isValid = false;
            }
        }
        return isValid;
    }
    validateEndTime() {
        let isValid = true;
        const currenctDate = new Date();
        const startTimeDate = new Date(this.discountStartTimeInput.value);
        const endTimeDate = new Date(this.discountEndTimeInput.value);
        const endTimeValue = this.discountEndTimeInput.value;
        this.endTimeValidationParagraph.innerText = "";
        if (!this.IsDiscountCleaned()) {
            if (endTimeValue == "") {
                this.endTimeValidationParagraph.innerText = "The input cannot be empty. Please provide a valid value or clear all discount fields.";
                isValid = false;
            }
            else if (isNaN(Date.parse(endTimeValue))) {
                this.endTimeValidationParagraph.innerText = "The date format is invalid. Please provide a valid date and time or clear all discount fields.";
                isValid = false;
            }
            else if (endTimeDate < currenctDate) {
                this.endTimeValidationParagraph.innerText = "The discount end time cannot be in the past. Please provide a valid future date.";
                isValid = false;
            }
            else if (endTimeDate < startTimeDate) {
                this.endTimeValidationParagraph.innerText = "The discount end time cannot be erlier than start time. Please provide a valid value or clear all discount fields.";
                isValid = false;
            }
        }
        return isValid;
    }
    validatePercentage() {
        let isValid = true;
        const percentageValue = this.discountPercentageInput.value;
        const percentage = parseFloat(percentageValue);
        this.percentageValidationParagraph.innerText = "";
        if (!this.IsDiscountCleaned()) {
            if (percentageValue === "") {
                this.percentageValidationParagraph.innerText =
                    "The discount percentage cannot be empty. Please provide a valid percentage or clear all discount fields.";
                isValid = false;
            }
            else if (isNaN(percentage) || percentage < 1 || percentage > 99) {
                this.percentageValidationParagraph.innerText =
                    "The discount percentage must be a number between 1 and 99. Please provide a valid value.";
                isValid = false;
            }
        }
        return isValid;
    }
    prepareDiscountToUpload() {
        const formData = new FormData();
        formData.append("DiscountStartDate", this.discountStartTimeInput.value.trim());
        formData.append("DiscountEndDate", this.discountEndTimeInput.value.trim());
        formData.append("DiscountPercentage", this.discountPercentageInput.value.trim());
        formData.append("IsDisocuntChanged", this.isDiscountChanged().toString());
        return formData;
    }
}
