"use strict";
class LightBox {
    constructor(LightboxSelector, nextButtonSelector, prevButtonSelector, modalSelector, modalCloseButtonSelector) {
        this.currentlyActivePhotoIndex = 0;
        this.lightbox = document.querySelector(LightboxSelector);
        this.nextButton = document.querySelector(nextButtonSelector);
        this.prevButton = document.querySelector(prevButtonSelector);
        this.modal = document.querySelector(modalSelector);
        this.modalCloseButton = document.querySelector(modalCloseButtonSelector);
        this.lightboxImages = this.lightbox.querySelectorAll('img');
        this.addButtonsEventListeners();
        this.modalCloseButton.addEventListener('click', () => this.closeModal());
    }
    activePhoto(photoIndex) {
        this.currentlyActivePhotoIndex = photoIndex;
        const img = this.lightboxImages[photoIndex];
        img.style.display = "block";
        img.classList.add('active-photo');
        this.handleButtonsEnability();
    }
    disactivePhoto(photoIndex) {
        const img = this.lightboxImages[photoIndex];
        img.classList.remove('active-photo');
    }
    switchVisiblePhoto(nextVisiblePhotoIndex) {
        const lastActivePhoto = this.lightboxImages[this.currentlyActivePhotoIndex];
        this.disactivePhoto(this.currentlyActivePhotoIndex);
        const onTransitionEnd = () => {
            this.activePhoto(nextVisiblePhotoIndex);
            lastActivePhoto.removeEventListener('transitionend', onTransitionEnd);
            lastActivePhoto.style.display = "none";
        };
        lastActivePhoto.addEventListener('transitionend', onTransitionEnd, { once: true });
    }
    moveRight() {
        if (this.currentlyActivePhotoIndex + 1 <= this.lightboxImages.length - 1) {
            this.switchVisiblePhoto(this.currentlyActivePhotoIndex + 1);
        }
    }
    moveLeft() {
        if (this.currentlyActivePhotoIndex - 1 >= 0) {
            this.switchVisiblePhoto(this.currentlyActivePhotoIndex - 1);
        }
    }
    addButtonsEventListeners() {
        this.nextButton.addEventListener('click', () => this.moveRight());
        this.prevButton.addEventListener('click', () => this.moveLeft());
    }
    handleButtonsEnability() {
        if (this.currentlyActivePhotoIndex == 0) {
            this.prevButton.classList.add("disabled");
        }
        else {
            this.prevButton.classList.remove("disabled");
        }
        if (this.currentlyActivePhotoIndex == this.lightboxImages.length - 1) {
            this.nextButton.classList.add("disabled");
        }
        else {
            this.nextButton.classList.remove("disabled");
        }
    }
    closeModal() {
        this.modal.style.display = "none";
        this.modal.classList.remove('show');
        this.disactivePhoto(this.currentlyActivePhotoIndex);
        this.lightboxImages[this.currentlyActivePhotoIndex].style.display = "none";
        this.currentlyActivePhotoIndex = 0;
    }
    openModal() {
        this.modal.style.display = "block";
        this.modal.classList.add('show');
    }
    openWithActivePhto(photoIndex) {
        this.openModal();
        this.activePhoto(photoIndex);
    }
}
