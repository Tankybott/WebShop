"use strict";
class PhotoUploader {
    constructor(mainPhotoUploadSelector, mainPhotoUpdateSelector, otherPhotosContainerSelector, downloadedMainPhotoUrl = null, downloadedOtherPhotoUrls = []) {
        this.downloadedMainPhotoUrl = downloadedMainPhotoUrl;
        this.downloadedOtherPhotoUrls = downloadedOtherPhotoUrls;
        this.currentMainPhotoUrl = null;
        this.photoToDeleteUrls = [];
        this.photoToUploadUrls = [];
        this.MAX_PHOTOS_TO_ADD = 7;
        this.MAX_PHOTO_SIZE_IN_MB = 8;
        this.currentMainPhotoUrl = downloadedMainPhotoUrl;
        this.mainPhotoUploadInput = document.querySelector(mainPhotoUploadSelector);
        this.mainPhotoUpdateContainer = document.querySelector(mainPhotoUpdateSelector);
        this.otherPhotosContainer = document.querySelector(otherPhotosContainerSelector);
        this.mainPhotoImg = this.mainPhotoUpdateContainer.querySelector('img');
        this.init();
    }
    init() {
        var _a, _b;
        (_a = this.mainPhotoUploadInput) === null || _a === void 0 ? void 0 : _a.addEventListener("change", () => {
            this.handleMainPhotoUpload(this.mainPhotoUploadInput);
        });
        (_b = this.mainPhotoUpdateContainer) === null || _b === void 0 ? void 0 : _b.addEventListener("change", (e) => {
            e.preventDefault();
            this.handleMainPhotoUpload(this.mainPhotoUpdateContainer.querySelector('input'));
        });
        this.downloadedOtherPhotoUrls.length > 0 && this.handleDownloadedOtherImagesEvents();
        this.downloadedMainPhotoUrl && this.handleOtherPhotosInput();
    }
    handleMainPhotoUpload(fileInput) {
        var _a;
        const file = (_a = fileInput === null || fileInput === void 0 ? void 0 : fileInput.files) === null || _a === void 0 ? void 0 : _a[0];
        if (file) {
            if (!this.isFileTooBig(file)) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    var _a;
                    this.currentMainPhotoUrl && this.deletePhotoFromServer(this.currentMainPhotoUrl);
                    const newPhotoUrl = (_a = e.target) === null || _a === void 0 ? void 0 : _a.result;
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
    switchUploadForUpdateMainPhoto() {
        if (this.mainPhotoUploadInput && this.mainPhotoUploadInput.parentElement && this.mainPhotoUploadInput.parentElement.style.display != "none") {
            const mainPhotoInputParent = this.mainPhotoUploadInput.parentElement;
            mainPhotoInputParent.style.display = "none";
        }
        if (this.mainPhotoUpdateContainer.style.display == "none") {
            this.mainPhotoUpdateContainer.style.display = "inline-block";
        }
    }
    handleOtherPhotosInput() {
        const existingInput = this.otherPhotosContainer.querySelector("input[type='file']");
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
        }
        else {
            if (existingInput)
                existingInput.style.display = "none";
        }
    }
    countAllOtherPhotos() {
        const allOtherPhotos = this.otherPhotosContainer.querySelectorAll('img');
        return allOtherPhotos.length;
    }
    handleOtherPhotoUpload(fileInput) {
        var _a;
        const file = (_a = fileInput === null || fileInput === void 0 ? void 0 : fileInput.files) === null || _a === void 0 ? void 0 : _a[0];
        if (file) {
            if (!this.isFileTooBig(file)) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    var _a;
                    const url = (_a = e.target) === null || _a === void 0 ? void 0 : _a.result;
                    this.buildOtherPhotoStructure(url);
                    this.photoToUploadUrls.push(url);
                    this.handleOtherPhotosInput();
                };
                reader.readAsDataURL(file);
            }
        }
    }
    buildOtherPhotoStructure(url) {
        const imageWrapper = document.createElement("div");
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
        });
    }
    switchPhotoWithMain(img) {
        const temp = img.src;
        if (!this.isBase64(this.currentMainPhotoUrl))
            this.downloadedOtherPhotoUrls.push(this.currentMainPhotoUrl);
        if (!this.isBase64(temp))
            this.downloadedOtherPhotoUrls = this.downloadedOtherPhotoUrls.filter(url => this.normalizeUrl(url) !== this.normalizeUrl(temp));
        img.src = this.currentMainPhotoUrl;
        this.mainPhotoImg.src = temp;
        this.currentMainPhotoUrl = temp;
    }
    deleteOtherPhoto(otherPhotoContainer, img) {
        this.deletePhotoFromServer(img.src);
        otherPhotoContainer.remove();
    }
    deletePhotoFromServer(imgSrc) {
        if (this.isBase64(imgSrc)) {
            this.photoToUploadUrls = this.photoToUploadUrls.filter(url => url.trim() !== imgSrc.trim());
        }
        else {
            this.photoToDeleteUrls.push(this.getRelativeUrl(imgSrc));
        }
    }
    handleDownloadedOtherImagesEvents() {
        this.otherPhotosContainer.addEventListener("click", (event) => {
            const eventTarget = event.target;
            // Handle "Delete" button click
            if (eventTarget.classList.contains("delete-photo-button")) {
                console.log("delte invoked");
                const photoUrl = eventTarget.getAttribute("data-photo-url");
                if (photoUrl) {
                    const imageWrapper = eventTarget.closest(".image-wrapper");
                    const image = imageWrapper.querySelector("img");
                    if (imageWrapper)
                        this.deleteOtherPhoto(imageWrapper, image);
                }
            }
            // Handle "Switch with Main Photo" button click
            if (eventTarget.classList.contains("switch-photo-button")) {
                console.log("switch invoked");
                const photoUrl = eventTarget.getAttribute("data-photo-url");
                if (photoUrl) {
                    const imageWrapper = eventTarget.closest(".image-wrapper");
                    const image = imageWrapper.querySelector("img");
                    this.switchPhotoWithMain(image);
                }
            }
        });
    }
    preparePhotosToUpload() {
        const formData = new FormData();
        if (this.currentMainPhotoUrl) {
            if (this.isBase64(this.currentMainPhotoUrl)) {
                formData.append("MainPhoto", this.base64ToFile(this.currentMainPhotoUrl));
                this.photoToUploadUrls = this.photoToUploadUrls.filter(url => url !== this.currentMainPhotoUrl);
            }
            else {
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
    base64ToFile(base64) {
        var _a;
        const [prefix, data] = base64.split(',');
        const contentType = ((_a = prefix === null || prefix === void 0 ? void 0 : prefix.match(/data:(.*?);base64/)) === null || _a === void 0 ? void 0 : _a[1]) || '';
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
    isFileTooBig(file) {
        const MAX_SIZE_MB = this.MAX_PHOTO_SIZE_IN_MB;
        const MAX_SIZE_BYTES = MAX_SIZE_MB * 1024 * 1024;
        if (file.size > MAX_SIZE_BYTES) {
            toastr.error("Photo size is too large.");
            return true;
        }
        return false;
    }
    getRelativeUrl(url) {
        const tempUrl = new URL(url, window.location.origin);
        return tempUrl.pathname;
    }
    normalizeUrl(url) {
        return new URL(url, window.location.origin).pathname;
    }
    normalizeUrlsArr(urls) {
        return urls.map(url => new URL(url, window.location.origin).pathname);
    }
    isBase64(str) {
        const base64Regex = /^(data:[a-zA-Z0-9/+:;=]+,)?[a-zA-Z0-9+/]+={0,2}$/;
        try {
            return base64Regex.test(str) && !!atob(str.split(',').pop() || '');
        }
        catch (e) {
            return false;
        }
    }
}
