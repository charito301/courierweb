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

$(function() {
    alert('asd');
    $(".water").each(function() {
        $tb = $(this);
        if ($tb.val() != this.title) {
            $tb.removeClass("water");
        }
    });

    $(".water").focus(function() {
        $tb = $(this);
        if ($tb.val() == this.title) {
            $tb.val("");
            $tb.removeClass("water");
        }
    });

    $(".water").blur(function() {
        $tb = $(this);
        if ($.trim($tb.val()) == "") {
            $tb.val(this.title);
            $tb.addClass("water");
        }
    });
});   