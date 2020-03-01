function getMessageInput(){
    var txtMessageInput = document.getElementById("txtMessageInput").value;
    document.getElementById("txtMessageInput").value = '';
    return txtMessageInput;
}