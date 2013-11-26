$(document).ready(function () {

    $(".table-number").click(function (event) {
        $(".order-wrapper").hide();
        var id = $(this).attr('id')
        $(".order-" + id).parent().show();
    });

});