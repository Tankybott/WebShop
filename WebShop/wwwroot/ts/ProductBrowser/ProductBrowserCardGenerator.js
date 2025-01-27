"use strict";
class ProductBrowserCardGenerator {
    generateProductCard(productDto) {
        const colDiv = this.createMainContainer();
        const cardDiv = this.createCard(productDto);
        colDiv.appendChild(cardDiv);
        return colDiv;
    }
    createMainContainer() {
        const colDiv = document.createElement("div");
        colDiv.className = "col-lg-3 col-sm-6 col-12 d-flex align-items-stretch w";
        return colDiv;
    }
    createCard(productDto) {
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
    createImage(productDto) {
        const img = document.createElement("img");
        img.src = productDto.mainPhotoUrl;
        img.alt = productDto.name;
        img.className = "card-img-top rounded";
        img.style.maxWidth = "220px";
        return img;
    }
    createCardBody(productDto) {
        const cardBody = document.createElement("div");
        cardBody.className = "card-body pb-0 flex-grow-1";
        const title = document.createElement("p");
        title.className = "card-title h7 text-dark opacity-75 text-uppercase text-center";
        title.textContent = productDto.name;
        const category = document.createElement("p");
        category.className = "card-title text-black text-center";
        category.textContent = productDto.categoryName;
        const price = document.createElement("p");
        price.className = "text-dark text-opacity-75 text-center";
        price.innerHTML = `Price: <span>${productDto.price}</span>`;
        cardBody.appendChild(title);
        cardBody.appendChild(category);
        cardBody.appendChild(price);
        return cardBody;
    }
    createButtonDiv(productDto) {
        const buttonDiv = document.createElement("div");
        const detailsButton = document.createElement("a");
        detailsButton.className = "btn btn-primary bg-gradient border-0 form-control";
        detailsButton.href = `details?productId=${productDto.id}`;
        detailsButton.textContent = "Details";
        buttonDiv.appendChild(detailsButton);
        return buttonDiv;
    }
}
