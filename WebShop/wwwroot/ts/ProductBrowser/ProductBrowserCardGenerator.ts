class ProductBrowserCardGenerator {
    public generateProductCard(productDto: IProductDTO): HTMLDivElement {
        const colDiv = this.createMainContainer();

        const cardDiv = this.createCard(productDto);

        colDiv.appendChild(cardDiv);
        this.styleDiscountedCard(productDto, cardDiv);
        const stockQuantityNumber: number = Number(productDto.stockQuantity)
        if (!isNaN(stockQuantityNumber) && stockQuantityNumber <= 0) {
            const overlay = this.handleOutOfStock();
            cardDiv.appendChild(overlay as HTMLDivElement);
        } 
        return colDiv;
    }

    private styleDiscountedCard(productDto: IProductDTO, cardDiv: HTMLDivElement): void {
        const discountPercentage = Number(productDto.discountPercentage);
        if (!isNaN(discountPercentage) && discountPercentage > 0) {
            cardDiv.classList.add("border-primary")
            cardDiv.classList.add("border-bottom")
        }
    }

    private createMainContainer(): HTMLDivElement {
        const colDiv = document.createElement("div");
        colDiv.className = "col-lg-3 col-sm-6 col-12 d-flex align-items-stretch w";
        return colDiv;
    }

    private createCard(productDto: IProductDTO): HTMLDivElement {
        const cardDiv = document.createElement("div");
        cardDiv.className = "card border-0 p-3 shadow border-top border-5 rounded d-flex flex-column align-items-center h-100 w-100 m-3 mb-3";

        const img = this.createImage(productDto);
        const cardBody = this.createCardBody(productDto);
        const buttonDiv = this.createButtonDiv(productDto);

        cardDiv.appendChild(img);
        cardDiv.appendChild(cardBody);
        cardDiv.appendChild(buttonDiv);

        return cardDiv;
    }

    private handleOutOfStock(): HTMLDivElement | null {
        const overlay = document.createElement("div");
        overlay.className = "position-absolute top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center";
        overlay.style.backgroundColor = "rgba(108, 117, 125, 0.5)";
        overlay.style.backdropFilter = "blur(1.5px)"; 
        overlay.style.zIndex = "10";

        const outOfStockMessage = document.createElement("p");
        outOfStockMessage.className = "text-white fw-bold text-center fs-3";
        outOfStockMessage.textContent = "Out of Stock";

        overlay.appendChild(outOfStockMessage);
        return overlay;
    }

    private createImage(productDto: IProductDTO): HTMLDivElement {
        const imgContainer = document.createElement('div');
        imgContainer.style.width = "80%";
        imgContainer.style.height = "50%";
        imgContainer.style.minHeight = "200px";
        imgContainer.className = "d-flex align-items-center bg-dark"
        const img = document.createElement("img");
        img.src = productDto.mainPhotoUrl;
        img.alt = productDto.name;
        img.className = "thumbnail-photo";
        img.loading = "lazy";

        imgContainer.appendChild(img)
        return imgContainer;
    }

    private createCardBody(productDto: IProductDTO): HTMLDivElement {
        const cardBody = document.createElement("div");
        cardBody.className = "card-body pb-0 flex-grow-1";

        const title = document.createElement("p");
        title.className = "card-title h7 text-black text-uppercase text-center fw-bold";
        title.textContent = productDto.name;

        const category = document.createElement("p");
        category.className = "card-title text-dark text-center border-bottom border-top border-primary p-1";
        category.textContent = productDto.categoryName;

        const price = this.createPrice(productDto);

        cardBody.appendChild(title);
        cardBody.appendChild(category);
        cardBody.appendChild(price);

        return cardBody;
    }

    private createPrice(productDto: IProductDTO): HTMLDivElement {
        const priceContainer = document.createElement("div");
        priceContainer.className = "d-flex flex-column align-items-center";

        const price = Number(productDto.price);
        const discountPercentage = Number(productDto.discountPercentage);

        if (!isNaN(price) && !isNaN(discountPercentage) && discountPercentage > 0) {
            const originalPrice = document.createElement("p");
            originalPrice.className = "text-muted text-decoration-line-through text-center";
            originalPrice.textContent = `Price: ${price.toFixed(2)}`;

            const discountSign = document.createElement("p");
            discountSign.className = "text-uppercase fw-bold text-primary"
            discountSign.innerText = `Discount: ${productDto.discountPercentage}%`

            const discountedPriceValue = price * (1 - discountPercentage / 100);

            const discountedPrice = document.createElement("p");
            discountedPrice.className = "text-dark text-center fw-bold";
            discountedPrice.textContent = `Price now: ${discountedPriceValue.toFixed(2)}`;

            priceContainer.appendChild(originalPrice);
            priceContainer.appendChild(discountSign);
            priceContainer.appendChild(discountedPrice);
        } else {
            const normalPrice = document.createElement("p");
            normalPrice.className = "text-dark text-center fw-bold";
            normalPrice.textContent = `Price: ${price.toFixed(2)}`;

            priceContainer.appendChild(normalPrice);
        }

        return priceContainer;
    }

    private createButtonDiv(productDto: IProductDTO): HTMLDivElement {
        const buttonDiv = document.createElement("div");

        const detailsButton = document.createElement("a");
        detailsButton.className = "btn btn-primary bg-gradient border-0 form-control";
        detailsButton.href = `/User/ProductDetails/Details?productId=${productDto.id}`;
        detailsButton.textContent = "Details";

        buttonDiv.appendChild(detailsButton);

        return buttonDiv;
    }


}