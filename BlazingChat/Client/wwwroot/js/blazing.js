function getMessageInput() {
  var txtMessageInput = document.getElementById("txtMessageInput").value;
  document.getElementById("txtMessageInput").value = '';
  return txtMessageInput;
}

function setScroll() {

  //let's fix scroll here
  var divMessageContainerBase = document.getElementById('divMessageContainerBase');
  divMessageContainerBase.scrollTop = divMessageContainerBase.scrollHeight;

}

function changeTitle() {

  var currentTitle = document.title;
  var splitCurrentTitle = currentTitle.split(' ');

  if (splitCurrentTitle.length > 2) {

    var currentCount = parseInt(splitCurrentTitle[0].replace('(','').replace(')',''));
    var newCount = currentCount + 1;

    document.title = '(' + newCount + ') Blazing Chat';

  }
  else {
    document.title = '(1) Blazing Chat';
  }

}