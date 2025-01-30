class LightBox {
    private readonly lightbox: HTMLDivElement;
    private readonly nextButton: HTMLButtonElement;
    private readonly prevButton: HTMLButtonElement;
    private readonly lightboxImages: NodeListOf<HTMLImageElement>;
    private readonly modal: HTMLDivElement;
    private readonly modalCloseButton: HTMLButtonElement;

    private currentlyActivePhotoIndex: number = 0;

    constructor(
        LightboxSelector: string,
        nextButtonSelector: string,
        prevButtonSelector: string,
        modalSelector: string,
        modalCloseButtonSelector: string,
    ) {
        this.lightbox = document.querySelector(LightboxSelector) as HTMLDivElement;
        this.nextButton = document.querySelector(nextButtonSelector) as HTMLButtonElement;
        this.prevButton = document.querySelector(prevButtonSelector) as HTMLButtonElement;
        this.modal = document.querySelector(modalSelector) as HTMLDivElement;
        this.modalCloseButton = document.querySelector(modalCloseButtonSelector) as HTMLButtonElement;

        this.lightboxImages = this.lightbox.querySelectorAll('img');
        this.addButtonsEventListeners();
        this.modalCloseButton.addEventListener('click', () => this.closeModal());
    }

    public activePhoto(photoIndex: number): void {
        this.currentlyActivePhotoIndex = photoIndex;
        const img = this.lightboxImages[photoIndex];
        img.style.display = "block";
        img.classList.add('active-photo');
        this.handleButtonsEnability();
    }

    private disactivePhoto(photoIndex: number): void {
        const img = this.lightboxImages[photoIndex];
        img.classList.remove('active-photo');
    }

    private switchVisiblePhoto(nextVisiblePhotoIndex: number): void {
        const lastActivePhoto: HTMLImageElement = this.lightboxImages[this.currentlyActivePhotoIndex];
        this.disactivePhoto(this.currentlyActivePhotoIndex);

        const onTransitionEnd = () => {
            this.activePhoto(nextVisiblePhotoIndex);
            lastActivePhoto.removeEventListener('transitionend', onTransitionEnd);
            lastActivePhoto.style.display = "none";
        };

        lastActivePhoto.addEventListener('transitionend', onTransitionEnd, { once: true });
    }

    private moveRight(): void {
        if (this.currentlyActivePhotoIndex + 1 <= this.lightboxImages.length - 1) {
            this.switchVisiblePhoto(this.currentlyActivePhotoIndex + 1);
        }
    }

    private moveLeft(): void {
        if (this.currentlyActivePhotoIndex - 1 >= 0) {
            this.switchVisiblePhoto(this.currentlyActivePhotoIndex - 1);
        }
    }

    private addButtonsEventListeners(): void {
        this.nextButton.addEventListener('click', () => this.moveRight());
        this.prevButton.addEventListener('click', () => this.moveLeft());
    }

    private handleButtonsEnability(): void {
        if (this.currentlyActivePhotoIndex == 0) {
            this.prevButton.classList.add("disabled")
        } else {
            this.prevButton.classList.remove("disabled") 
        }

        if (this.currentlyActivePhotoIndex == this.lightboxImages.length - 1) {
            this.nextButton.classList.add("disabled")
        } else {
            this.nextButton.classList.remove("disabled")
        }
    }

    private closeModal(): void {
        this.modal.style.display = "none";
        this.modal.classList.remove('show');
        this.disactivePhoto(this.currentlyActivePhotoIndex);
        this.lightboxImages[this.currentlyActivePhotoIndex].style.display = "none";
        this.currentlyActivePhotoIndex = 0;
    }

    private openModal(): void {
        this.modal.style.display = "block"; 
        this.modal.classList.add('show'); 
    }

    public openWithActivePhto(photoIndex: number): void {
        this.openModal();
        this.activePhoto(photoIndex);
    } 
}