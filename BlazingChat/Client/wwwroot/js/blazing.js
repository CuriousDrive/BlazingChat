function getMessageInput(){
    var txtMessageInput = document.getElementById("txtMessageInput").value;
    document.getElementById("txtMessageInput").value = '';
    return txtMessageInput;
}

function setScroll() {
    
    //var data = document.getElementById("btn-input").value;    
    
    //var chatLog = document.getElementById("chatLog");
    //chatLog.innerHTML += '<div class="row msg_container base_sent"><div class="col-md-10 col-xs-10"><div class="messages msg_receive"><p>' + data + '</p></div></div></div><div class="row msg_container base_receive"><div class="col-md-10 col-xs-10"><div class="messages msg_receive"><p>' + data + '</p></div></div></div>';

    //let's fix scroll here
    var divMessageContainerBase = document.getElementById('divMessageContainerBase');
    divMessageContainerBase.scrollTop = divMessageContainerBase.scrollHeight;

  };
  
//   function clearInput() {
//     $("#myForm :input").each(function() {
//       $(this).val(''); //hide form values
//     });
//   }
