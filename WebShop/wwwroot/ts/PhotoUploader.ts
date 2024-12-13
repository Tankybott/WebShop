class PhotoUploader {
    private currentMainPhotoUrl: string | null = null;
    private photoToDeleteUrls: string[] = [];
    private photoToUploadUrls: string[] = [];
    private mainPhotoUploadInput: HTMLInputElement;
    private mainPhotoUpdateContainer: HTMLDivElement;
    private otherPhotosContainer: HTMLDivElement;
    private mainPhotoImg: HTMLImageElement;

    private MAX_PHOTOS_TO_ADD: number = 7;
    private MAX_PHOTO_SIZE_IN_MB: number = 8;
    constructor(
        mainPhotoUploadSelector: string,
        mainPhotoUpdateSelector: string,
        otherPhotosContainerSelector: string,
        private downloadedMainPhotoUrl: string | null = null,
        private downloadedOtherPhotoUrls: string[] = []
    ) {
        this.currentMainPhotoUrl = downloadedMainPhotoUrl
        this.mainPhotoUploadInput = document.querySelector(mainPhotoUploadSelector) as HTMLInputElement;
        this.mainPhotoUpdateContainer = document.querySelector(mainPhotoUpdateSelector) as HTMLInputElement;
        this.otherPhotosContainer = document.querySelector(otherPhotosContainerSelector) as HTMLDivElement; 
        this.mainPhotoImg = this.mainPhotoUpdateContainer.querySelector('img') as HTMLImageElement;

        this.init();
    }


    public init(): void {
        this.mainPhotoUploadInput?.addEventListener("change", () => {
            this.handleMainPhotoUpload(this.mainPhotoUploadInput);
        });
        this.mainPhotoUpdateContainer?.addEventListener("change", (e) => {
            e.preventDefault();
            this.handleMainPhotoUpload(this.mainPhotoUpdateContainer.querySelector('input') as HTMLInputElement);
        });

        this.downloadedOtherPhotoUrls.length > 0 && this.handleDownloadedOtherImagesEvents();

        this.downloadedMainPhotoUrl && this.handleOtherPhotosInput();
    }

    private handleMainPhotoUpload(fileInput: HTMLInputElement): void {
        const file = fileInput?.files?.[0];
        if (file) {
            if (!this.isFileTooBig(file)) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.currentMainPhotoUrl && this.deletePhotoFromServer(this.currentMainPhotoUrl);

                    const newPhotoUrl = e.target?.result as string;
                    this.photoToUploadUrls.push(newPhotoUrl);
                    this.mainPhotoImg.src = newPhotoUrl;
                    this.currentMainPhotoUrl = newPhotoUrl;

                    this.handleOtherPhotosInput();
                };
                reader.readAsDataURL(file);
            }
        }

        !this.downloadedMainPhotoUrl && this.switchUploadForUpdateMainPhoto();
    }

    private switchUploadForUpdateMainPhoto(): void
    {
        if (this.mainPhotoUploadInput && this.mainPhotoUploadInput.parentElement && this.mainPhotoUploadInput.parentElement.style.display != "none") {
            const mainPhotoInputParent = this.mainPhotoUploadInput.parentElement as HTMLElement;
            mainPhotoInputParent.style.display = "none";
        }

        if (this.mainPhotoUpdateContainer.style.display == "none") {
            this.mainPhotoUpdateContainer.style.display = "inline-block"
        }
    }

    private handleOtherPhotosInput(): void {
        const existingInput = this.otherPhotosContainer.querySelector("input[type='file']") as HTMLElement;
        if (this.countAllOtherPhotos() < this.MAX_PHOTOS_TO_ADD) {
            if (!existingInput) {
                const input = document.createElement("input");
                input.type = "file";
                input.accept = "image/png, image/jpeg, image/jpg";
                input.addEventListener("change", () => {
                    this.handleOtherPhotoUpload(input);
                });
                this.otherPhotosContainer.appendChild(input);
            }
        } else {
            if(existingInput) existingInput.style.display = "none"
        }
    }

    private countAllOtherPhotos(): number{
        const allOtherPhotos = this.otherPhotosContainer.querySelectorAll('img');
        return allOtherPhotos.length;
    }

    private handleOtherPhotoUpload(fileInput: HTMLInputElement): void {
        const file = fileInput?.files?.[0];
        if (file) {
            if (!this.isFileTooBig(file)) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    const url = e.target?.result as string;
                    this.buildOtherPhotoStructure(url);
                    this.photoToUploadUrls.push(url)
                    this.handleOtherPhotosInput();
                };
                reader.readAsDataURL(file);
            }
        }
    }

    private buildOtherPhotoStructure(url: string): void {

        const imageWrapper = document.createElement("div") as HTMLDivElement;

        //zmienic z klasa css
        const img = document.createElement("img");
        img.src = url;
        img.style.width = "100px";
        img.style.height = "100px";

        const controlsDiv = document.createElement("div");

        const deleteButton = document.createElement("button");
        deleteButton.textContent = "Delete";

        const switchButton = document.createElement("button");
        switchButton.textContent = "Switch with Main Photo";

        controlsDiv.appendChild(deleteButton);
        controlsDiv.appendChild(switchButton);

        imageWrapper.appendChild(img);
        imageWrapper.appendChild(controlsDiv);

        this.otherPhotosContainer.appendChild(imageWrapper);

        switchButton.addEventListener("click", (event) => {
            event.preventDefault();
            this.switchPhotoWithMain(img);
        });

        deleteButton.addEventListener("click", (event) => {
            event.preventDefault();
            this.deleteOtherPhoto(imageWrapper, img);
        })
    }

    private switchPhotoWithMain(img: HTMLImageElement): void {
        const temp = img.src;
        if (!this.isBase64(this.currentMainPhotoUrl as string)) this.downloadedOtherPhotoUrls.push(this.currentMainPhotoUrl as string);
        if (!this.isBase64(temp)) this.downloadedOtherPhotoUrls = this.downloadedOtherPhotoUrls.filter(url => this.normalizeUrl(url) !== this.normalizeUrl(temp));
        img.src = this.currentMainPhotoUrl as string;
        this.mainPhotoImg.src = temp;
        this.currentMainPhotoUrl = temp;
    }

    private deleteOtherPhoto(otherPhotoContainer: HTMLDivElement, img: HTMLImageElement): void {
        this.deletePhotoFromServer(img.src);
        otherPhotoContainer.remove();
    }

    private deletePhotoFromServer(imgSrc: string): void {
        if (this.isBase64(imgSrc)) {
            this.photoToUploadUrls = this.photoToUploadUrls.filter(url => url.trim() !== imgSrc.trim());
        } else {
            this.photoToDeleteUrls.push(this.getRelativeUrl(imgSrc));
        }
    }



    handleDownloadedOtherImagesEvents(): void {
        this.otherPhotosContainer.addEventListener("click", (event) => {
            const eventTarget = event.target as HTMLElement;

            // Handle "Delete" button click
            if (eventTarget.classList.contains("delete-photo-button")) {
                console.log("delte invoked")
                const photoUrl = eventTarget.getAttribute("data-photo-url");
                if (photoUrl) {
                    const imageWrapper = eventTarget.closest(".image-wrapper") as HTMLDivElement;
                    const image = imageWrapper.querySelector("img") as HTMLImageElement;
                    if (imageWrapper) this.deleteOtherPhoto(imageWrapper, image);
                }
            }

            // Handle "Switch with Main Photo" button click
            if (eventTarget.classList.contains("switch-photo-button")) {
                console.log("switch invoked")
                const photoUrl = eventTarget.getAttribute("data-photo-url");
                if (photoUrl) {
                    const imageWrapper = eventTarget.closest(".image-wrapper") as HTMLDivElement;
                    const image = imageWrapper.querySelector("img") as HTMLImageElement;
                    this.switchPhotoWithMain(image);
                }
            }
        });
    }

    public preparePhotosToUpload(): FormData {
        const formData = new FormData();

        if (this.currentMainPhotoUrl)
        {
            if (this.isBase64(this.currentMainPhotoUrl)) {
                formData.append("MainPhoto", this.base64ToFile(this.currentMainPhotoUrl));
                this.photoToUploadUrls = this.photoToUploadUrls.filter(url => url !== this.currentMainPhotoUrl)
            } else {
                formData.append("MainPhotoUrl", this.getRelativeUrl(this.currentMainPhotoUrl));
            }
        }

        if (this.photoToUploadUrls.length > 0) {
            for (const url of this.photoToUploadUrls) {
                const file = this.base64ToFile(url);
                formData.append("OtherPhotos", file);
            }
        }

        if (this.photoToDeleteUrls.length > 0) {
            for (const url of this.photoToDeleteUrls) {
                formData.append("UrlsToDelete", url);
            }
        }

        if (this.downloadedOtherPhotoUrls.length > 0) {
            for (const url of this.downloadedOtherPhotoUrls) {
                const isNotDeleted = this.normalizeUrlsArr(this.photoToDeleteUrls)
                    .every(normalizedUrl => normalizedUrl !== this.normalizeUrl(url));

                if (isNotDeleted) {
                    formData.append("OtherPhotosUrls", url);
                }
            }
        }

       return formData;
    }

    private base64ToFile(base64: string): File {
        const [prefix, data] = base64.split(',');
        const contentType = prefix?.match(/data:(.*?);base64/)?.[1] || '';
        const extension = contentType.split('/')[1] || 'bin'; 

        const byteCharacters = atob(data);
        const byteNumbers = new Array(byteCharacters.length);

        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);

        const uniqueId = Date.now() + Math.random().toString(36).substring(2, 15);
        const fileName = `file_${uniqueId}.${extension}`;
        return new File([byteArray], fileName, { type: contentType });
    }





    //utilities

    private isFileTooBig(file: File): boolean {
        const MAX_SIZE_MB = this.MAX_PHOTO_SIZE_IN_MB;
        const MAX_SIZE_BYTES = MAX_SIZE_MB * 1024 * 1024;

        if (file.size > MAX_SIZE_BYTES) {
            toastr.error("Photo size is too large.");
            return true;
        }
        return false;
    }

    private getRelativeUrl(url: string): string{
        const tempUrl = new URL(url, window.location.origin);
        return tempUrl.pathname;
    }

    private normalizeUrl(url: string): string{
        return new URL(url, window.location.origin).pathname
    }

    private normalizeUrlsArr(urls: string[]): string[] {
        return urls.map(url => new URL(url, window.location.origin).pathname)
    }


    private isBase64(str: string): boolean {
        const base64Regex = /^(data:[a-zA-Z0-9/+:;=]+,)?[a-zA-Z0-9+/]+={0,2}$/;

        try {
            return base64Regex.test(str) && !!atob(str.split(',').pop() || '');
        } catch (e) {
            return false; 
        }
    }

}
