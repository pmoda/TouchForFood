/**
Removes the row based on the id that was just passed in
**/
function deleteTableRow(row) {
    //we had a succesful deletion
    if (row >= 1) {
        $('tr#' + row).remove();
    } else {
        //do some error message stuff
    }
}

function onRemoveMenuCategorySuccess(data, status, exf) {
    var result = parseInt(data);
    if (result != 0) {
        window.location.reload()
        return;
    }
    $("#ajaxError").addClass("warning");
    $("#ajaxError").html("The menu is currently active! Cannot delete category.");
}

function onRemoveMenuItemSuccess(data, status, exf) {
    var result = parseInt(data);
    if (result != 0) {
        window.location.reload()
        return;
    }
    $("#ajaxError").addClass("warning");
    $("#ajaxError").html("The menu is currently active! Cannot delete item.");
}