/*The visibility code can be found here: http://css-tricks.com/snippets/javascript/showhide-element/ */

$(function () {
    $('#request-toggle').click(function () 
    {
        $('#OpenRequests').toggle();
        $('#AllRequests').toggle();
        if ($(this).text() == "Show Cancelled & Completed") {
            $(this).text("Hide Cancelled & Completed");
        }
        else {
            $(this).text("Show Cancelled & Completed");
        }; 
    });
});
