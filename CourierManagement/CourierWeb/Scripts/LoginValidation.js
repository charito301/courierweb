function valSubmit() {
    var doc = document.forms[0];
    var msg = "";
    alert('1 ' + document.getElementById('dllOperators').Options[0].value);
    alert('2 ' + doc.dllOperators.Options[0].value);
    if (doc.ddlOperators.selectedIndex == 0) {
        msg += "Please choose operator username.n";
    }
    if (doc.password.value == "") {
        msg += "Please enter password.n";
    }
    if (msg == "") {
        doc.submit();
        return true;
    } else {
        alert(msg);
        return false;
    }
}
