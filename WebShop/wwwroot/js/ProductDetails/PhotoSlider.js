"use strict";
class PhotoSlider {
    constructor(photoSliderSelector, slidingContainerSelector, sliderItemSelector, previousButtonSelector, nextButtonSelector) {
        this.itemWidth = 0;
        this.activeSlide = 0;
        this.photoSlider = document.querySelector(photoSliderSelector);
        this.slidingContainer = this.photoSlider.querySelector(slidingContainerSelector);
        this.sliderItems = this.photoSlider.querySelectorAll(sliderItemSelector);
        this.previousButton = document.querySelector(previousButtonSelector);
        this.nextButton = document.querySelector(nextButtonSelector);
        this.adjustSizes();
        document.addEventListener('resize', () => this.adjustSizes());
        this.previousButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.slideLeft();
        });
        this.nextButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.slideRight();
        });
    }
    getWidthOfItem(item) {
        return item.offsetWidth;
    }
    slideRight() {
        if (this.activeSlide < this.sliderItems.length - 1) {
            this.activeSlide += 1;
            const translateValue = -this.activeSlide * this.itemWidth;
            this.slidingContainer.style.transform = `translateX(${translateValue}px)`;
        }
    }
    slideLeft() {
        if (this.activeSlide > 0) {
            this.activeSlide -= 1;
            const translateValue = -this.activeSlide * this.itemWidth;
            this.slidingContainer.style.transform = `translateX(${translateValue}px)`;
        }
    }
    adjustSizes() {
        this.itemWidth = this.getWidthOfItem(this.sliderItems[0]);
        this.photoSlider.style.maxWidth = `${this.itemWidth}px`;
    }
}
//# sourceMappingURL=PhotoSlider.js.map