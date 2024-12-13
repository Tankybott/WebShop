var quill = new Quill('#editor', {
    theme: 'snow',
    modules: {
        toolbar: [
            [{ font: [] }, { size: [] }],
            ['bold', 'italic', 'underline', 'strike'],
            [{ color: [] }, { background: [] }], 
            [{ list: 'ordered' }, { list: 'bullet' }], 
            ['link'] 
        ]
    }
});


$('form').submit(function () {
    $('#editorContent').val(quill.root.innerHTML);
});