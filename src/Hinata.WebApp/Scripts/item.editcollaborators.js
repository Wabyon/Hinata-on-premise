/// <reference path="_references.js" />
$(function () {
    var searching = false;

	$('#search-collaborator').keydown(function (e) {
	    var $self = $(this);
		var code = e.keyCode || e.which;
		if (code === 13 && searching === false) {
		    searching = true;
			var data = {};
		    data.query = $self.val();

		    $.ajax({
				url: $self.data('url'),
				type: 'POST',
				dataType: 'html',
				data: data,
				success: function (ret) {
					$('#collarborator-searchresult-area').children().remove();
					$('#collarborator-searchresult-area').append(ret);
				},
				complete: function () {
				    searching = false;
				}
			});
		}
	});

	$(document).on('click', '#collarborator-searchresult-area button', function () {
    	var $self = $(this);
        $.ajax({
            url: $self.data('url'),
    		type: 'POST',
    		dataType: 'html',
    		success: function (ret) {
    			$('#collarborator-edit-area table').append(ret);
    			$self.remove();
		    }
    	});
    });

	$(document).on('click', '#collarborator-edit-area button', function () {
	    var $self = $(this);
	    $.ajax({
	        url: $self.data('url'),
	        type: 'POST',
	        success: function (ret) {
	            $('#collarborator-edit-area table').append(ret);
	            $self.closest('tr').remove();
	        }
	    });
	});

	$(document).on('click', '#collarborator-edit-area a.list-group-item', function () {
	    var $self = $(this);
	    $.ajax({
	        url: $self.data('url'),
	        type: 'POST',
	        success: function (ret) {
	            $self.closest('div.btn-group').find('button.dropdown-toggle').text($self.data('roletype'));
	            $self.closest('ul.dropdown-menu').find('a.list-group-item').removeClass('active');
	            $self.addClass('active');
	        }
	    });
	});
});