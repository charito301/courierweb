function isValidEmail(email) {
    return email.match('^.+@.+[.].+$');
}

function isDigitKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;

    return true;

}

// Variables to hold mouse x-y pos.s
var mouseLocation = new Object();
ino_MouseLocation.X = 0;
ino_MouseLocation.Y = 0;

// Main function to retrieve mouse x-y pos.s
function getMouseXY(e) {
    // Detect if the browser is IE or not.
    // If it is not IE, we assume that the browser is NS.
    var IE = document.all ? true : false;

    if (IE) { // grab the x-y pos.s if browser is IE
        mouseLocation.X = window.event.clientX + document.body.scrollLeft;
        mouseLocation.Y = window.event.clientY + document.body.scrollTop;
    } else {  // grab the x-y pos.s if browser is NS
        mouseLocation.X = e.pageX;
        mouseLocation.Y = e.pageY;
    }
    // catch possible negative values in NS4
    if (mouseLocation.X < 0) { mouseLocation.X = 0; }
    if (mouseLocation.Y < 0) { mouseLocation.Y = 0; }
}

function findPosX(obj) {
    var curleft = 0;
    if (obj.offsetParent) {
        while (obj.offsetParent) {
            curleft += obj.offsetLeft
            obj = obj.offsetParent;
        }
    }
    else if (obj.x)
        curleft += obj.x;
    return curleft;
}

function inoFindPosY(obj) {
    var curtop = 0;
    if (obj.offsetParent) {
        while (obj.offsetParent) {
            curtop += obj.offsetTop
            obj = obj.offsetParent;
        }
    }
    else if (obj.y)
        curtop += obj.y;
    return curtop;
}