class CartTotalPriceCounter {
    private readonly inputs: NodeListOf<HTMLInputElement>
    private readonly totalPrice: HTMLHeadingElement;

    constructor(
        inputSelector: string,
        totalPriceSelector: string,
        private readonly itemCurrentPriceSelector: string
    ) {
        this.inputs = document.querySelectorAll(inputSelector) as NodeListOf<HTMLInputElement>;
        this.totalPrice = document.querySelector(totalPriceSelector) as HTMLHeadingElement;

        this.countPrice();
    }

    private getInputAncestor(input: HTMLInputElement): HTMLDivElement {
        return input.parentElement?.parentElement as HTMLDivElement;
    }

    public countPrice(): void {
        let summaryPrice: number = 0;
        this.inputs.forEach(input => {
            summaryPrice += this.countItemSummaryPrice(input)
        })

        this.setSummaryPrice(summaryPrice)
    }

    private countItemSummaryPrice(input: HTMLInputElement): number {
        const inputAncestor = this.getInputAncestor(input);
        const itemCurrentPriceMeta = inputAncestor.querySelector(this.itemCurrentPriceSelector) as HTMLMetaElement;
        const itemCurrentPrice = itemCurrentPriceMeta.content;

        return Number(input.value) * Number(itemCurrentPrice);
    }

    private setSummaryPrice(price: number): void {
        this.totalPrice.innerText = price.toString();
    }
}
