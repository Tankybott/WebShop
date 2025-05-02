interface Carrier {
    name: string;
    isPricePerKg: boolean;
    minimalShippingPrice: number;
    price: number;
}

class ShippingPriceCalculator {
    private readonly carrierSelect: HTMLSelectElement;
    private readonly priceParagraph: HTMLParagraphElement;
    private readonly loadingScreen: HTMLDivElement;
    private readonly totalProductsPriceMeta: HTMLMetaElement;
    private readonly totalPriceParagraph: HTMLParagraphElement;


    constructor(
        selectId: string,
        priceParagraphSelector: string,
        loadingScreenSelector: string,
        totalProductsPriceMetaSelector: string,
        totalPriceParagraphSelector: string,
        private readonly cartItemSelector: string,
        private readonly ShippingPriceFactorMeta: string,
        private readonly itemQuantityMetaSelector: string,
    ) {
        this.carrierSelect = document.querySelector(selectId) as HTMLSelectElement;
        this.priceParagraph = document.querySelector(priceParagraphSelector) as HTMLParagraphElement;
        this.loadingScreen = document.querySelector(loadingScreenSelector) as HTMLDivElement;
        this.totalProductsPriceMeta = document.querySelector(totalProductsPriceMetaSelector) as HTMLMetaElement;
        this.totalPriceParagraph = document.querySelector(totalPriceParagraphSelector) as HTMLParagraphElement;


        this.carrierSelect.addEventListener('change', (event: Event) => this.setShippingPrice(event))
    }

    private async setShippingPrice(event: Event): Promise<void> {
        const select = event.target as HTMLSelectElement;
        const carrierId = select.value
        const carrier: Carrier | null = await this.fetchCarrier(carrierId);
        let price: number = 0;
        if (carrier != null) {

            if (!carrier.isPricePerKg) {
                price = carrier.price
            } else {
                console.log(carrier.price)
                price = this.calcShippingPrice(carrier.price)
                console.log(price)
                if (price < carrier.minimalShippingPrice && carrier.minimalShippingPrice > 0) {
                    price = carrier.minimalShippingPrice
                }
                console.log(price)
            }
        }
        this.priceParagraph.innerText = price.toString();
        this.setTotalPrice(price);
    }

    private async fetchCarrier(carrierId: string): Promise<Carrier | null> {
        this.loadingScreen.style.display = "flex";
        try {
            const response = await fetch(`/User/Order/GetCarrier?carrierId=${carrierId}`);
            const data = await response.json();

            if (data.success) {
                return data.carrier as Carrier;
            } else {
                console.error("Failed to fetch carrier data");
                return null;
            }
        } catch (error) {
            console.error("Error fetching carrier:", error);
            return null;
        } finally {
            this.loadingScreen.style.display = "none";
        }
    }

    private setTotalPrice(shippingPrice: number) {
        const productsPrice = +this.totalProductsPriceMeta.content;
        const totalPrice = productsPrice + shippingPrice;

        this.totalPriceParagraph.innerText = totalPrice.toString();
    }

    public calcShippingPrice(pricePerKg: number): number {
        let totalPrice: number = 0;
        console.log(pricePerKg)
        const cartItems = document.querySelectorAll(this.cartItemSelector) as NodeListOf<HTMLDivElement>;

        cartItems.forEach(item => {
            const shippingPriceFactorMeta = item.querySelector(this.ShippingPriceFactorMeta) as HTMLMetaElement;
            const quantityMeta = item.querySelector(this.itemQuantityMetaSelector) as HTMLMetaElement;

            if (shippingPriceFactorMeta && quantityMeta) {
                const factor = parseFloat(shippingPriceFactorMeta.content);
                const quantity = parseInt(quantityMeta.content);
                console.log(quantity + " " + factor + " " + pricePerKg)

                if (!isNaN(factor) && !isNaN(quantity)) {
                    totalPrice += factor * quantity * pricePerKg;
                }
            }
        });

        return Math.round(totalPrice * 10) / 10;
    }

}
