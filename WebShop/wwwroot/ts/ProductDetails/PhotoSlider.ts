class PhotoSlider {
    private readonly photoSlider: HTMLDivElement;
    private readonly slidingContainer: HTMLDivElement;
    private readonly sliderItems: NodeListOf<HTMLDivElement>;
    private readonly previousButton: HTMLButtonElement;
    private readonly nextButton: HTMLButtonElement;
    private itemWidth: number = 0;
    private activeSlide = 0;

    constructor(
        photoSliderSelector: string,
        slidingContainerSelector: string,
        sliderItemSelector: string,
        previousButtonSelector: string,
        nextButtonSelector: string,
    ) {
        this.photoSlider = document.querySelector(photoSliderSelector) as HTMLDivElement;
        this.slidingContainer = this.photoSlider.querySelector(slidingContainerSelector) as HTMLDivElement;
        this.sliderItems = this.photoSlider.querySelectorAll(sliderItemSelector) as NodeListOf<HTMLDivElement>;
        this.previousButton = document.querySelector(previousButtonSelector) as HTMLButtonElement;
        this.nextButton = document.querySelector(nextButtonSelector) as HTMLButtonElement;

        this.adjustSizes();
        document.addEventListener('resize', () => this.adjustSizes());
        this.previousButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.slideLeft();
        })
        this.nextButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.slideRight();
        })
    }

    private getWidthOfItem(item: HTMLDivElement): number {
        return item.offsetWidth;
    }

    private slideRight(): void {
        if (this.activeSlide < this.sliderItems.length - 1) {
            this.activeSlide += 1;
            const translateValue = -this.activeSlide * this.itemWidth;
            this.slidingContainer.style.transform = `translateX(${translateValue}px)`;
        }
    }

    private slideLeft(): void {
        if (this.activeSlide > 0) {
            this.activeSlide -= 1;
            const translateValue = -this.activeSlide * this.itemWidth;
            this.slidingContainer.style.transform = `translateX(${translateValue}px)`;
        }
    }

    private adjustSizes(): void {
        this.itemWidth = this.getWidthOfItem(this.sliderItems[0]);
        this.photoSlider.style.maxWidth = `${this.itemWidth}px`;
    }
}