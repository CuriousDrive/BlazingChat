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

function setTheme(themeName) {
  
  // Build the new css link
  let newLink = document.createElement("link");
  newLink.setAttribute("id", "theme");
  newLink.setAttribute("rel", "stylesheet");
  newLink.setAttribute("type", "text/css");
  newLink.setAttribute("href", `css/${themeName}.css`);
  // Remove and replace the theme
  let head = document.getElementsByTagName("head")[0];
  head.querySelector("#theme").remove();
  head.appendChild(newLink);

}