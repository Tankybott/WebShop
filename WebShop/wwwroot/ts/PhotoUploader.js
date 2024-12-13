"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class PhotoUploader2 {
    constructor(mainPhotoAreaSelector, mainPhotoUploadSelector, mainPhotoUpdateSelector, otherPhotosContainerSelector, downloadedMainPhotoUrl = null, downloadedOtherPhotoUrls = []) {
        this.mainPhotoAreaSelector = mainPhotoAreaSelector;
        this.mainPhotoUploadSelector = mainPhotoUploadSelector;
        this.mainPhotoUpdateSelector = mainPhotoUpdateSelector;
        this.otherPhotosContainerSelector = otherPhotosContainerSelector;
        this.downloadedMainPhotoUrl = downloadedMainPhotoUrl;
        this.downloadedOtherPhotoUrls = downloadedOtherPhotoUrls;
        this.currentMainPhotoUrl = null;
        this.photoToDeleteUrls = [];
        this.photoToUploadUrls = [];
        this.currentMainPhotoUrl = downloadedMainPhotoUrl;
        this.mainPhotoUploadInput = document.querySelector(this.mainPhotoUploadSelector);
        this.mainPhotoUpdateContainer = document.querySelector(this.mainPhotoUpdateSelector);
        // do poprawy id na data 
        this.otherPhotosContainer = document.querySelector(this.otherPhotosContainerSelector);
        this.mainPhotoImg = this.mainPhotoUpdateContainer.querySelector('img');
    }
    init() {
        var _a, _b;
        (_a = this.mainPhotoUploadInput) === null || _a === void 0 ? void 0 : _a.addEventListener("change", (e) => {
            this.handleMainPhotoUpload(this.mainPhotoUploadInput);
        });
        (_b = this.mainPhotoUpdateContainer) === null || _b === void 0 ? void 0 : _b.addEventListener("change", (e) => {
            e.preventDefault();
            this.handleMainPhotoUpload(this.mainPhotoUpdateContainer.querySelector('input'));
        });
        this.downloadedMainPhotoUrl && this.addOtherPhotosInput();
    }
    handleMainPhotoUpload(fileInput) {
        var _a;
        const file = (_a = fileInput === null || fileInput === void 0 ? void 0 : fileInput.files) === null || _a === void 0 ? void 0 : _a[0];
        if (file) {
            if (!this.isFileTooBig(file)) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    var _a;
                    this.currentMainPhotoUrl && this.processPhotoForDeletion();
                    console.log("run");
                    const newPhotoUrl = (_a = e.target) === null || _a === void 0 ? void 0 : _a.result;
                    this.photoToUploadUrls.push(newPhotoUrl);
                    this.mainPhotoImg.src = newPhotoUrl;
                    this.currentMainPhotoUrl = newPhotoUrl;
                    this.addOtherPhotosInput();
                };
                reader.readAsDataURL(file);
            }
        }
        this.switchUploadForUpdateMainPhoto();
    }
    processPhotoForDeletion() {
        if (this.isBase64(this.mainPhotoImg.src)) {
            this.photoToUploadUrls = this.photoToUploadUrls.filter(url => url !== this.mainPhotoImg.src);
        }
        else {
            this.photoToDeleteUrls.push(this.mainPhotoImg.src);
        }
    }
    switchUploadForUpdateMainPhoto() {
        console.log("startuje");
        if (this.mainPhotoUploadInput.parentElement && this.mainPhotoUploadInput.parentElement.style.display != "none") {
            const mainPhotoInputParent = this.mainPhotoUploadInput.parentElement;
            mainPhotoInputParent.style.display = "none";
            console.log("usuwa");
        }
        if (this.mainPhotoUpdateContainer.style.display == "none") {
            this.mainPhotoUpdateContainer.style.display = "inline-block";
            console.log("dodaje");
        }
    }
    addOtherPhotosInput() {
        const existingInput = this.otherPhotosContainer.querySelector("input[type='file']");
        if (!existingInput) {
            const input = document.createElement("input");
            input.type = "file";
            input.addEventListener("change", () => {
                this.handleOtherPhotoUpload(input);
            });
            this.otherPhotosContainer.appendChild(input);
        }
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
            this.SwitchPhotoWithMain(img);
        });
        deleteButton.addEventListener("click", (event) => {
            event.preventDefault();
            this.deletePhoto(imageWrapper, img);
        });
    }
    SwitchPhotoWithMain(img) {
        const temp = img.src;
        img.src = this.currentMainPhotoUrl;
        this.mainPhotoImg.src = temp;
        this.currentMainPhotoUrl = temp;
    }
    deletePhoto(otherPhotoContainer, img) {
        if (this.isBase64(img.src)) {
            this.photoToUploadUrls.filter(url => url !== img.src);
        }
        else {
            this.normalizeUrlsArr(this.downloadedOtherPhotoUrls).filter(url => url == this.normalizeUrl(img.src));
        }
        otherPhotoContainer.remove();
    }
    //utilities
    isFileTooBig(file) {
        const MAX_SIZE_MB = 8;
        const MAX_SIZE_BYTES = MAX_SIZE_MB * 1024 * 1024;
        if (file.size > MAX_SIZE_BYTES) {
            toastr.error("Photo size is too large.");
            return true;
        }
        return false;
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
