export function downloadFile(mimeType, base64String, fileName) {

    var fileDataUrl = "data:" + mimeType + ";base64," + base64String;
    fetch(fileDataUrl)
        .then(response => response.blob())
        .then(blob => {

            //create a link
            var link = window.document.createElement("a");
            link.href = window.URL.createObjectURL(blob, { type: mimeType });
            link.download = fileName;

            //add, click and remove
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
    
}

export function setScroll() {
    //let's fix scroll here
    var divMessageContainerBase = document.getElementById('divMessageContainerBase');
    if (divMessageContainerBase != null)
    {
        divMessageContainerBase.scrollTop = divMessageContainerBase.scrollHeight;
    }
}