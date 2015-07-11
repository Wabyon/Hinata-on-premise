/// <reference path="_references.js" />
$(function () {
    $(document).on("click", "a.list-group-item", function () {
        var $self = $(this);
        $("a.list-group-item").removeClass("active");
        $self.addClass("active");

        $.ajax({
            url: $self.data("url"),
            type: "GET",
            dataType: "html",
            success: function (html) {
                $("#draft-preview").html(html);
            }
        });
    });
});
