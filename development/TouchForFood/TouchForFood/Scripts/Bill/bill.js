$(document).ready(function () {

    $(".food-menu-button").click(function (event) {
		//var menu_item_id = parseInt($(this).attr('id'));
		toggle($(this));
    });
	
	$(".bill-item").click(function(event) {
		var bill_item_id = parseInt($(this).attr('id'));
		var found = false;
		var cntr = 0;
		var total = $(".menu-item-bill-selected").size();
		$(".menu-item-bill-selected").each(function(index){
			found = true;
			var order_item_id = parseInt($(this).attr('id'));
			$.ajax({
				url: "/Bill/AddOrderItemToBill?billId="+bill_item_id+"&orderItemId=" + order_item_id,
				context: document.body,
				type: 'POST'
				}).done(function() {	
					cntr++;
					if (cntr == total){
						location.reload();
					}
				});
		});
		
	});	
});

function toggle(elem){
	if (elem.hasClass("menu-item-bill-unselected"))
	{
		elem.removeClass("menu-item-bill-unselected");
		elem.addClass("menu-item-bill-selected");
	}
	else
	{
		elem.removeClass("menu-item-bill-selected");
		elem.addClass("menu-item-bill-unselected");
	}
}

function onCreateBillForOrder(data) {
    if (data == 0) {
        $("#ajaxError").addClass("error");
        $("#ajaxError").html("You are trying to create a bill but have no more items.");
    }
    else {
        window.location.reload();
    }
}