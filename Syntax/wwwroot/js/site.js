function createObjectURL(object) {
    return (window.URL)
        ? window.URL.createObjectURL(object)
        : window.webkitURL.createObjectURL(object);
}

function revokeObjectURL(url) {
    return (window.URL)
        ? window.URL.revokeObjectURL(url)
        : window.webkitURL.revokeObjectURL(url);
}

$("#profile-picture-input").on('change', (e) => {

    // There should be only 1 file
    if (e.target.files.length == 1) {

        var file = e.target.files[0];

        var src = createObjectURL(file);
        var image = new Image();
        image.src = src;
        $('#img').attr('src', src);
    }
});