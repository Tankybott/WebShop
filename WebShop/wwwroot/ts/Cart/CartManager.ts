class CartManager {
    private itemQuantitySpan: HTMLSpanElement | null;

    constructor() {
        this.itemQuantitySpan = document.getElementById('cartQuantity');
        this.refreshCartQuantity(); 
    }
    
    async refreshCartQuantity(): Promise<void> {
        try {
            const response = await fetch("/User/Cart/GetCartItemsQuantity");
            if (!response.ok) {
                throw new Error(`API request failed: ${response.status}`);
            }

            const result = await response.json();
            if (result.success == true) {
                if (this.itemQuantitySpan) this.itemQuantitySpan.innerText = `${result.newQuantity}`
            } else {
                if (this.itemQuantitySpan) this.itemQuantitySpan.innerText = `0`
            }
        } catch (error) {
            console.error("Failed to fetch cart quantity:", error);
        }
    }
}
