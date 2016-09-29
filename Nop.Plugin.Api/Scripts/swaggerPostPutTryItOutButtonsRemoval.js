$(function () {

    function removeButtons(buttons) {
        [].forEach.call(buttons, function (btn) {
            btn.remove();
        });
    }

    var putbuttons = document.querySelectorAll(".put .submit");
    removeButtons(putbuttons);

    var postbuttons = document.querySelectorAll(".post .submit");
    removeButtons(postbuttons);

});