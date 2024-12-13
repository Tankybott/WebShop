class ExtraTopicUploader {
    private extraTopicArea: HTMLDivElement;
    private addExtraTopicButton: HTMLButtonElement;

    constructor(
        extraTopicAreaSelector: string,
        addExtraTopicButtonSelector: string,
        private deleteExtraTopicButtonSelector: string, 
        private saveExtraTopicButtonSelector: string, 
        private editExtraTopicButtonSelector: string,
        private extraTopicContainerSelector: string,
        private extraTopicSavedClass: string,
    ) {
        this.extraTopicArea = document.querySelector(extraTopicAreaSelector) as HTMLDivElement;
        this.addExtraTopicButton = document.querySelector(addExtraTopicButtonSelector) as HTMLButtonElement;

        this.init();
    }

    public init(): void {
        this.addExtraTopicButton.addEventListener("click", (e) => {
            e.preventDefault();
            this.buildExtraTopicStructure();
        })
        this.handleExistingTopicEventHandlers();
    }

    private buildExtraTopicStructure(): void {
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

    private createDeleteButtonContainer(extraTopicDiv: HTMLDivElement): HTMLDivElement {
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

    private createTitleSection(): { titleInput: HTMLInputElement; titleFormGroup: HTMLDivElement } {
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

    private createInfoSection(): { infoTextarea: HTMLTextAreaElement; infoFormGroup: HTMLDivElement } {
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

    private createButtonsSection(extraTopicDiv: HTMLDivElement, titleInput: HTMLInputElement, infoTextarea: HTMLTextAreaElement): HTMLDivElement {
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

    private handleExistingTopicEventHandlers(): void {
        const deleteButtons = this.extraTopicArea.querySelectorAll(this.deleteExtraTopicButtonSelector);
        const saveButtons = this.extraTopicArea.querySelectorAll(this.saveExtraTopicButtonSelector);
        const editButtons = this.extraTopicArea.querySelectorAll(this.editExtraTopicButtonSelector);

        deleteButtons.forEach(btn => {
            const extraTopicContainer = btn.closest(this.extraTopicContainerSelector) as HTMLDivElement;
            btn.addEventListener('click', e => {
                e.preventDefault;
                this.deleteExtraTopic(extraTopicContainer)
            })
        })

        saveButtons.forEach(btn => {
            const extraTopicContainer = btn.closest(this.extraTopicContainerSelector) as HTMLDivElement;
            const extraTopicTitleInput = extraTopicContainer?.querySelector('input') as HTMLInputElement;
            const extraTopicInfoTextArea = extraTopicContainer.querySelector('textarea') as HTMLTextAreaElement;
            btn.addEventListener('click', e => {
                e.preventDefault;
                this.saveTopic(extraTopicContainer, extraTopicTitleInput, extraTopicInfoTextArea)
            })
        })

        editButtons.forEach(btn => {
            const extraTopicContainer = btn.closest(this.extraTopicContainerSelector) as HTMLDivElement;
            const extraTopicTitleInput = extraTopicContainer?.querySelector('input') as HTMLInputElement;
            const extraTopicInfoTextArea = extraTopicContainer.querySelector('textarea') as HTMLTextAreaElement;
            btn.addEventListener('click', e => {
                e.preventDefault;
                this.editTopic(extraTopicContainer, extraTopicTitleInput, extraTopicInfoTextArea)
            })
        })
    }

    private saveTopic(extraTopicContainer: HTMLDivElement, input: HTMLInputElement, textArea: HTMLTextAreaElement): void {
        !extraTopicContainer.classList.contains(this.extraTopicSavedClass) && extraTopicContainer.classList.add(this.extraTopicSavedClass);
        input.disabled = true;
        textArea.disabled = true;
    }

    private editTopic(extraTopicContainer: HTMLDivElement, input: HTMLInputElement, textArea: HTMLTextAreaElement): void {
        extraTopicContainer.classList.contains(this.extraTopicSavedClass) && extraTopicContainer.classList.remove(this.extraTopicSavedClass);
        input.disabled = false;
        textArea.disabled = false;
    }

    private deleteExtraTopic(extraTopicContainer: HTMLDivElement): void {
        extraTopicContainer.remove();
    }

    public prepareTopicsToUpload(): FormData {
        const formData = new FormData();
        const topics: { TopicTitle: string; TopicInfo: string }[] = [];

        this.extraTopicArea.querySelectorAll(`.${this.extraTopicSavedClass}`).forEach((topicDiv) => {
            const titleInput = topicDiv.querySelector("input") as HTMLInputElement;
            const infoTextarea = topicDiv.querySelector("textarea") as HTMLTextAreaElement;

            if (titleInput?.value.trim() && infoTextarea?.value.trim()) {
                topics.push({ TopicTitle: titleInput.value.trim(), TopicInfo: infoTextarea.value.trim() });
            }
        });

        formData.append("ExtraTopics", JSON.stringify(topics));

        return formData;
    }
}