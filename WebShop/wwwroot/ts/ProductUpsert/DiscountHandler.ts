class DiscountHandler {
    private readonly collapseButton: HTMLButtonElement;
    private readonly clearButton: HTMLButtonElement;
    private readonly collapseButtonInnerDiv: HTMLDivElement;
    private readonly discountStartTimeInput: HTMLInputElement;
    private readonly discountEndTimeInput: HTMLInputElement;
    private readonly discountPercentageInput: HTMLInputElement;
    private readonly startTimeValidationParagraph: HTMLParagraphElement
    private readonly endTimeValidationParagraph: HTMLParagraphElement;
    private readonly percentageValidationParagraph: HTMLParagraphElement;


    private readonly arrowUp: string = '<i class="bi bi-chevron-compact-up"></i>';
    private readonly buttonContent: string = "";

    private readonly initialStartTime: string;
    private readonly initialEndTime: string;
    private readonly initialPercentage: string;
    constructor(
        collapseButtonSelector: string,
        clearButtonSelector: string,
        discountStartTimeInputId: string,
        discountEndTimeInputId: string,
        discountPercentageInputId: string
    ) {
        this.discountStartTimeInput = document.querySelector(`#${discountStartTimeInputId}`) as HTMLInputElement;
        this.startTimeValidationParagraph = this.discountStartTimeInput.parentElement?.querySelector('p') as HTMLParagraphElement;
        this.discountEndTimeInput = document.querySelector(`#${discountEndTimeInputId}`) as HTMLInputElement;
        this.endTimeValidationParagraph = this.discountEndTimeInput.parentElement?.querySelector('p') as HTMLParagraphElement;
        this.discountPercentageInput = document.querySelector(`#${discountPercentageInputId}`) as HTMLInputElement;
        this.percentageValidationParagraph = this.discountPercentageInput.parentElement?.querySelector('p') as HTMLParagraphElement;
        this.collapseButton = document.querySelector(collapseButtonSelector) as HTMLButtonElement;
        this.collapseButtonInnerDiv = this.collapseButton.querySelector('div') as HTMLDivElement;
        this.buttonContent = this.collapseButtonInnerDiv.innerHTML;
        this.clearButton = document.querySelector(clearButtonSelector) as HTMLButtonElement;
        this.initialStartTime = this.discountStartTimeInput.value;
        this.initialEndTime = this.discountEndTimeInput.value;
        this.initialPercentage = this.discountPercentageInput.value;

        this.init();
    }

    private init(): void {
        this.collapseButton.addEventListener('click', this.changeButtonContent.bind(this));
        this.clearButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.handleClearButtonClick();
        })
    }

    private changeButtonContent(): void {
        this.collapseButtonInnerDiv.classList.remove('show');
        this.collapseButton.style.pointerEvents = "none"; 
        setTimeout(() => {
            if (this.collapseButtonInnerDiv.innerHTML.trim() === this.buttonContent.trim()) {
                this.collapseButtonInnerDiv.innerHTML = this.arrowUp;
            } else {
                this.collapseButtonInnerDiv.innerHTML = this.buttonContent;
            }

            this.collapseButtonInnerDiv.classList.add('show');
            this.collapseButton.style.pointerEvents = "auto";
        }, 150);
    }

    public validateDiscountInputs(): Boolean {
        let isValid: boolean = true;

        return isValid;
    }

    private isDiscountChanged(): boolean {
        if (this.initialStartTime == this.discountStartTimeInput.value
            && this.initialEndTime == this.discountEndTimeInput.value
            && this.initialPercentage == this.discountPercentageInput.value) {
            return false;
        } else {
            return true;
        }
    }

    private handleClearButtonClick(): void {
        this.discountStartTimeInput.value = "";
        this.discountEndTimeInput.value = "";
        this.discountPercentageInput.value = "";
        this.startTimeValidationParagraph.innerText = "";
        this.endTimeValidationParagraph.innerText = "";
        this.percentageValidationParagraph.innerText = "";
    }

    public validateInputs(): boolean {
        let isValid: boolean = true;
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


    private IsDiscountCleaned(): boolean {
        if (this.discountStartTimeInput.value == ""
            && this.discountEndTimeInput.value == ""
            && this.discountPercentageInput.value == "") {
            return true;
        } else {
            return false;
        }
    }

    private validateStartTime(): boolean {
        let isValid: boolean = true;
        const startTimeValue: string = this.discountStartTimeInput.value;
        this.startTimeValidationParagraph.innerText = "";
        if (!this.IsDiscountCleaned())
        {
            if (startTimeValue == "") {
                this.startTimeValidationParagraph.innerText = "The input cannot be empty. Please provide a valid value or clear all discount fields."
                isValid = false;
            } else if (isNaN(Date.parse(startTimeValue))) {
                this.startTimeValidationParagraph.innerText = "The date format is invalid. Please provide a valid date and time or clear all discount fields.";
                isValid = false;
            }
        } 
        return isValid;
    }

    private validateEndTime(): boolean {
        let isValid: boolean = true;
        const currenctDate: Date = new Date();
        const startTimeDate: Date = new Date(this.discountStartTimeInput.value);
        const endTimeDate: Date = new Date(this.discountEndTimeInput.value);
        const endTimeValue: string = this.discountEndTimeInput.value;
        this.endTimeValidationParagraph.innerText = "";

        if (!this.IsDiscountCleaned()) {
            if (endTimeValue == "") {
                this.endTimeValidationParagraph.innerText = "The input cannot be empty. Please provide a valid value or clear all discount fields."
                isValid = false;
            } else if (isNaN(Date.parse(endTimeValue))) {
                this.endTimeValidationParagraph.innerText = "The date format is invalid. Please provide a valid date and time or clear all discount fields.";
                isValid = false;
            } else if (endTimeDate < currenctDate) {
                this.endTimeValidationParagraph.innerText = "The discount end time cannot be in the past. Please provide a valid future date.";
                isValid = false;
            } else if (endTimeDate < startTimeDate) {
                this.endTimeValidationParagraph.innerText = "The discount end time cannot be erlier than start time. Please provide a valid value or clear all discount fields."
                isValid = false;
            }
        }
        return isValid;
    }

    private validatePercentage(): boolean {
        let isValid: boolean = true;
        const percentageValue: string = this.discountPercentageInput.value;
        const percentage: number = parseFloat(percentageValue);
        this.percentageValidationParagraph.innerText = "";

        if (!this.IsDiscountCleaned()) {
            if (percentageValue === "") {
                this.percentageValidationParagraph.innerText =
                    "The discount percentage cannot be empty. Please provide a valid percentage or clear all discount fields.";
                isValid = false;
            } else if (isNaN(percentage) || percentage < 1 || percentage > 99) {
                this.percentageValidationParagraph.innerText =
                    "The discount percentage must be a number between 1 and 99. Please provide a valid value.";
                isValid = false;
            }
        }
        return isValid;
    }

    public prepareDiscountToUpload(): FormData {
        const formData: FormData = new FormData();
        formData.append("DiscountStartDate", this.discountStartTimeInput.value.trim());
        formData.append("DiscountEndDate", this.discountEndTimeInput.value.trim());
        formData.append("DiscountPercentage", this.discountPercentageInput.value.trim());
        formData.append("IsDisocuntChanged", this.isDiscountChanged().toString());
        return formData
    }
}

