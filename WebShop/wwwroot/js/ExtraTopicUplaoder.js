"use strict";
class ExtraTopicUploader {
    constructor(extraTopicAreaSelector, addExtraTopicButtonSelector, deleteExtraTopicButtonSelector, saveExtraTopicButtonSelector, editExtraTopicButtonSelector, extraTopicContainerSelector, extraTopicSavedClass) {
        this.deleteExtraTopicButtonSelector = deleteExtraTopicButtonSelector;
        this.saveExtraTopicButtonSelector = saveExtraTopicButtonSelector;
        this.editExtraTopicButtonSelector = editExtraTopicButtonSelector;
        this.extraTopicContainerSelector = extraTopicContainerSelector;
        this.extraTopicSavedClass = extraTopicSavedClass;
        this.extraTopicArea = document.querySelector(extraTopicAreaSelector);
        this.addExtraTopicButton = document.querySelector(addExtraTopicButtonSelector);
        this.init();
    }
    init() {
        this.addExtraTopicButton.addEventListener("click", (e) => {
            e.preventDefault();
            this.buildExtraTopicStructure();
        });
        this.handleExistingTopicEventHandlers();
    }
    buildExtraTopicStructure() {
        const extraTopicDiv = document.createElement("div");
        extraTopicDiv.classList.add("extra-topic");
        extraTopicDiv.appendChild(this.createDeleteButtonContainer(extraTopicDiv));
        const { titleInput, titleFormGroup } = this.createTitleSection();
        extraTopicDiv.appendChild(titleFormGroup);
        const { infoTextarea, infoFormGroup } = this.createInfoSection();
        extraTopicDiv.appendChild(infoFormGroup);
        extraTopicDiv.appendChild(this.createButtonsSection(extraTopicDiv, titleInput, infoTextarea));
        this.extraTopicArea.appendChild(extraTopicDiv);
    }
    createDeleteButtonContainer(extraTopicDiv) {
        const deleteButtonContainer = document.createElement("div");
        const deleteButton = document.createElement("button");
        deleteButton.textContent = "Delete";
        deleteButton.addEventListener("click", (e) => {
            e.preventDefault();
            this.deleteExtraTopic(extraTopicDiv);
        });
        deleteButtonContainer.appendChild(deleteButton);
        return deleteButtonContainer;
    }
    createTitleSection() {
        const titleFormGroup = document.createElement("div");
        titleFormGroup.classList.add("form-group");
        const titleLabel = document.createElement("label");
        titleLabel.textContent = "Title";
        const titleInput = document.createElement("input");
        titleInput.classList.add("form-control");
        titleInput.type = "text";
        titleFormGroup.appendChild(titleLabel);
        titleFormGroup.appendChild(titleInput);
        return { titleInput, titleFormGroup };
    }
    createInfoSection() {
        const infoFormGroup = document.createElement("div");
        infoFormGroup.classList.add("form-group");
        const infoLabel = document.createElement("label");
        infoLabel.textContent = "Info";
        const infoTextarea = document.createElement("textarea");
        infoTextarea.classList.add("form-control");
        infoFormGroup.appendChild(infoLabel);
        infoFormGroup.appendChild(infoTextarea);
        return { infoTextarea, infoFormGroup };
    }
    createButtonsSection(extraTopicDiv, titleInput, infoTextarea) {
        const topicButtonsDiv = document.createElement("div");
        topicButtonsDiv.classList.add("topic-buttons");
        const saveButton = document.createElement("button");
        saveButton.textContent = "Save";
        saveButton.addEventListener("click", (e) => {
            e.preventDefault();
            this.saveTopic(extraTopicDiv, titleInput, infoTextarea);
        });
        const editButton = document.createElement("button");
        editButton.textContent = "Edit";
        editButton.addEventListener("click", (e) => {
            e.preventDefault();
            this.editTopic(extraTopicDiv, titleInput, infoTextarea);
        });
        topicButtonsDiv.appendChild(saveButton);
        topicButtonsDiv.appendChild(editButton);
        return topicButtonsDiv;
    }
    handleExistingTopicEventHandlers() {
        const deleteButtons = this.extraTopicArea.querySelectorAll(this.deleteExtraTopicButtonSelector);
        const saveButtons = this.extraTopicArea.querySelectorAll(this.saveExtraTopicButtonSelector);
        const editButtons = this.extraTopicArea.querySelectorAll(this.editExtraTopicButtonSelector);
        deleteButtons.forEach(btn => {
            const extraTopicContainer = btn.closest(this.extraTopicContainerSelector);
            btn.addEventListener('click', e => {
                e.preventDefault;
                this.deleteExtraTopic(extraTopicContainer);
            });
        });
        saveButtons.forEach(btn => {
            const extraTopicContainer = btn.closest(this.extraTopicContainerSelector);
            const extraTopicTitleInput = extraTopicContainer === null || extraTopicContainer === void 0 ? void 0 : extraTopicContainer.querySelector('input');
            const extraTopicInfoTextArea = extraTopicContainer.querySelector('textarea');
            btn.addEventListener('click', e => {
                e.preventDefault;
                this.saveTopic(extraTopicContainer, extraTopicTitleInput, extraTopicInfoTextArea);
            });
        });
        editButtons.forEach(btn => {
            const extraTopicContainer = btn.closest(this.extraTopicContainerSelector);
            const extraTopicTitleInput = extraTopicContainer === null || extraTopicContainer === void 0 ? void 0 : extraTopicContainer.querySelector('input');
            const extraTopicInfoTextArea = extraTopicContainer.querySelector('textarea');
            btn.addEventListener('click', e => {
                e.preventDefault;
                this.editTopic(extraTopicContainer, extraTopicTitleInput, extraTopicInfoTextArea);
            });
        });
    }
    saveTopic(extraTopicContainer, input, textArea) {
        !extraTopicContainer.classList.contains(this.extraTopicSavedClass) && extraTopicContainer.classList.add(this.extraTopicSavedClass);
        input.disabled = true;
        textArea.disabled = true;
    }
    editTopic(extraTopicContainer, input, textArea) {
        extraTopicContainer.classList.contains(this.extraTopicSavedClass) && extraTopicContainer.classList.remove(this.extraTopicSavedClass);
        input.disabled = false;
        textArea.disabled = false;
    }
    deleteExtraTopic(extraTopicContainer) {
        extraTopicContainer.remove();
    }
    prepareTopicsToUpload() {
        const formData = new FormData();
        const topics = [];
        this.extraTopicArea.querySelectorAll(`.${this.extraTopicSavedClass}`).forEach((topicDiv) => {
            const titleInput = topicDiv.querySelector("input");
            const infoTextarea = topicDiv.querySelector("textarea");
            if ((titleInput === null || titleInput === void 0 ? void 0 : titleInput.value.trim()) && (infoTextarea === null || infoTextarea === void 0 ? void 0 : infoTextarea.value.trim())) {
                topics.push({ TopicTitle: titleInput.value.trim(), TopicInfo: infoTextarea.value.trim() });
            }
        });
        formData.append("ExtraTopics", JSON.stringify(topics));
        return formData;
    }
}
//# sourceMappingURL=ExtraTopicUplaoder.js.map