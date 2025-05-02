interface InputData {
    cartItemId: string;
    quantity: string;
}

abstract class BaseCartQuantityValidator<T extends HTMLElement> {
    protected readonly loadingScreen: HTMLDivElement;
    protected elements: NodeListOf<T> | null = null;
    private isQuantityValid: boolean = true;

    constructor(
        loadingScreenSelector: string,
        protected readonly elementWithQuantity: string,
        protected readonly cartItemIdMetaSelector: string,
        protected readonly warningParagraphSelector: string,
        protected readonly sweetAlert: SweetAlertDisplayer
    ) {
        this.loadingScreen = document.querySelector(loadingScreenSelector) as HTMLDivElement;
    }

    protected gatherElementsData(): InputData[] {
        this.elements = document.querySelectorAll(this.elementWithQuantity) as NodeListOf<T>;
        const data: InputData[] = [];
        this.elements.forEach(element => {
            const ancestor = this.gatherAncestor(element);
            const cartItemIdMeta = ancestor.querySelector(this.cartItemIdMetaSelector) as HTMLMetaElement;
            const cartItemId = cartItemIdMeta.content;
            data.push({ cartItemId, quantity: this.getElementValue(element) });
        });
        return data;
    }

    protected gatherAncestor(element: T): HTMLDivElement {
        return element.parentElement?.parentElement as HTMLDivElement;
    }

    public async validateCartQuantities(): Promise<boolean> {
        this.isQuantityValid = true;
        const inputsData = this.gatherElementsData();
        if (inputsData.length === 0) return false;

        this.loadingScreen.style.display = "flex";

        const formData = new FormData();
        inputsData.forEach((item, index) => {
            Object.entries(item).forEach(([key, value]) => {
                formData.append(`CollectionOfDTOs[${index}].${key}`, value);
            });
        });

        try {
            const response = await fetch("/User/Cart/ValidateCartQuantity", {
                method: "PATCH",
                body: formData,
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            const result = await response.json();

            if (result.success) {
                this.handleUpdatedQuantities(result.itemsWithMaxQuantity);
            } else {
                this.isQuantityValid = false;
                toastr.error("Something went wrong, try again later.");
            }
        } catch (e) {
            this.isQuantityValid = false;
            toastr.error("Something went wrong, try again later.");
        } finally {
            this.loadingScreen.style.display = "none";
            return this.isQuantityValid;
        }
    }

    private handleUpdatedQuantities(updatedItems: InputData[]) {
        let foundAny = false;

        updatedItems.forEach(updatedItem => {
            console.log(this.elements)
            const match = Array.from(this.elements!)
                .map(el => ({ el, ancestor: this.gatherAncestor(el) }))
                .find(({ ancestor }) => {
                    const meta = ancestor.querySelector(this.cartItemIdMetaSelector) as HTMLMetaElement;
                    return meta && meta.content == updatedItem.cartItemId;
                });

            if (match) {
                const { el, ancestor } = match;
                const warning = ancestor.querySelector(this.warningParagraphSelector) as HTMLParagraphElement;
                warning.innerText = "";

                if (Number(this.getElementValue(el)) > Number(updatedItem.quantity)) {
                    this.setElementValue(el, updatedItem.quantity);
                    warning.innerText = "Desired quantity is out of stock. The quantity has been adjusted.";
                    foundAny = true;
                }
            }
        });

        if (foundAny) {
            this.loadingScreen.style.display = "none";
            this.isQuantityValid = false;
            this.sweetAlert.FireSweetAlert(
                "Update",
                "Some of your items were adjusted to match available stock.",
                () => { }
            );
        }
    }

    protected abstract getElementValue(element: T): string;
    protected abstract setElementValue(element: T, value: string): void;
}


class CartInputQuantityValidator extends BaseCartQuantityValidator<HTMLInputElement> {
    protected getElementValue(element: HTMLInputElement): string {
        return element.value;
    }

    protected setElementValue(element: HTMLInputElement, value: string): void {
        element.value = value;
    }
}

class CartTextQuantityValidator extends BaseCartQuantityValidator<HTMLElement> {
    protected getElementValue(element: HTMLElement): string {
        return element.innerText;
    }

    protected setElementValue(element: HTMLElement, value: string): void {
        element.innerText = value;
    }
}