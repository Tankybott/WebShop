
interface InputData {
    cartItemId: string,
    quantity: string
}
class CartItemsQuantityValidator {
    private inputs: NodeListOf<HTMLInputElement> | null = null
    private readonly loadingScreen: HTMLDivElement

    constructor(
        loadingScreenSelector: string,
        private readonly inputSelector: string,
        private readonly cartItemIdSelector: string,
        private readonly warningParagraphSelector: string,
        private readonly sweetAlert: SweetAlertDisplayer
    ) {
        this.loadingScreen = document.querySelector(loadingScreenSelector) as HTMLDivElement;
    }

    private gatherInputsData(): InputData[] {
        this.inputs = document.querySelectorAll(this.inputSelector) as NodeListOf<HTMLInputElement>;
        let inputsData: InputData[] = [];
        this.inputs.forEach(input => {
            const inputAncestor = this.gatherInputAncestorDiv(input);
            const cartItemIdMeta = inputAncestor.querySelector(this.cartItemIdSelector) as HTMLMetaElement;
            const cartItemId = cartItemIdMeta.content;

            const inputData: InputData = {
                cartItemId: cartItemId,
                quantity: input.value,
            }
            inputsData.push(inputData);
        })

        return inputsData;
    }

    private gatherInputAncestorDiv(input: HTMLInputElement): HTMLDivElement {
        return input.parentElement?.parentElement as HTMLDivElement;
    }

    public async validateCartQuantities(): Promise<boolean> {
        let isValid: boolean = true;
        const inputsData = this.gatherInputsData();

        if (inputsData.length === 0) {
            isValid = false;
            return isValid;
        }
        this.loadingScreen.style.display = "flex";

        const formData = new FormData();
        inputsData.forEach((item, index) => {
            Object.keys(item).forEach((key) => {
                formData.append(`CollectionOfDTOs[${index}].${key}`, (item as Record<string, any>)[key]);
            });
        });

        try {
            const response = await fetch("/User/Cart/ValidateCartQuantity", {
                method: "PATCH",
                body: formData,
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const result = await response.json();

            if (result.success) {
                this.handleUpdatedQuantities(result.itemsWithMaxQuantity);
            } else {
                isValid = false;
                toastr.error("Something went wrong, try again later.");
            }
        } catch (error) {
            isValid = false;
            toastr.error("Something went wrong, try again later.");
        } finally {
            this.loadingScreen.style.display = "none";
        }

        return isValid;
    }

    private handleUpdatedQuantities(updatedItems: InputData[]) {
        let foundAny: boolean = false;
        updatedItems.forEach(updatedItem => {
            const foundPair = Array.from(this.inputs!)
                .map(i => ({
                    input: i,
                    ancestor: this.gatherInputAncestorDiv(i)
                }))
                .find(({ ancestor }) => {
                    const itemIdMeta = ancestor.querySelector(this.cartItemIdSelector) as HTMLMetaElement;
                    console.log(itemIdMeta.content)
                    return itemIdMeta && itemIdMeta.content === updatedItem.cartItemId.toString();
                });

            if (foundPair) {
                const { input, ancestor } = foundPair;
                const cartItemWarning = ancestor.querySelector(this.warningParagraphSelector) as HTMLParagraphElement;
                cartItemWarning.innerText = "";
                if (Number(input.value) > Number(updatedItem.quantity)) {
                    input.value = updatedItem.quantity;
                    cartItemWarning.innerText = "Desired quantity is out of stock. The product quantity has been adjusted to the maximum available.";
                    foundAny = true;
                }
            }
        });

        if (foundAny) {
            this.loadingScreen.style.display = "none";
            this.sweetAlert.FireSweetAlert("Update", "Some of your items are out of stock at the desired quantity. Their quantities have been adjusted in your cart.", () => {})
        }
        this.loadingScreen.style.display = "none";
    }
}
