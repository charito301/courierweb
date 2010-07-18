function OpenTable(trRow, imgId) {
    object = document.getElementById(trRow);

    if (object.style.display == "none") {
        object.style.display = "";
        document.getElementById(imgId).src = "images/Minus.gif";
    }
    else {
        object.style.display = "none";
        document.getElementById(imgId).src = "images/Plus.gif";
    }

    return false;
}