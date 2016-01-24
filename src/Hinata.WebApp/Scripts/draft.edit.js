/// <reference path="_references.js" />
$(function () {

    marked.setOptions({
        gfm: true,
        tables: true,
        breaks: true,
        pedantic: false,
        sanitize: false,
        smartLists: true,
        silent: false,
        highlight: function (code) {
            return hljs.highlightAuto(code).value;
        },
        langPrefix: "",
        smartypants: false,
        headerPrefix: "",
        renderer: new window.marked.Renderer(),
        xhtml: false
    });

    $("#item-editor").tabIndent();

    $("#item-editor").typeCharacter(function () {
        var src = $(this).val();
        var html = marked(src);
        $("#item-view").html(html);
        $("pre code").each(function (i, block) {
            hljs.highlightBlock(block);
        });
    });

    $("#mainForm button[type='button']").on("click", function () {
        var $self = $(this);
        $("#mainForm").attr("action", $self.data("action"));
        $("#mainForm").submit();
    });

    $("#update-file").on("change", function (e) {
        var $self = $(this);
        var files = e.target.files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }

                uploadImage($self.data("url"), data, uploadImageSuccessCallback, uploadImageErrorCallback, function () { $self.val(""); });
            } else {
                alert("This browser doesn't support HTML5 file uploads!");
            }
        }
    });

    var oldValue = $("#item-editor").val();
    setInterval(function () {
        var $editor = $("#item-editor");
        if ($editor.val() !== oldValue) {
            $.ajax({
                url: $editor.data("url"),
                type: "POST",
                dataType: "json",
                data: $("form").serialize(),
                success: function (ret) {
                    history.replaceState("", "", ret.Url);
                },
                complete: function () {
                    oldValue = $editor.val();
                }
            });
        }
    }, 20000);

    $('#aDiff').click(function () {
        var diff = hinata.diff($('#PublishedBody').val(), $('#item-editor').val());
        $('#diff-view').html(diff);
    });

    $('.datetime').datetimepicker({
        format: 'YYYY/MM/DD HH:mm',
        useCurrent: false,
        // Number of minutes the up/down arrow's will move the minutes value in the time picker
        stepping: 15,
        // Shows the picker side by side when using the time and date together.
        sideBySide: true,
        locale: 'ja',
        showTodayButton: true,
        showClear: true
    });

    $("#item-editor").bind("paste", function (e) {
        var $self = $(this);
        if (!e.originalEvent.clipboardData) return;

        var $items = e.originalEvent.clipboardData.items;
        var $hasImage = false;
        var $data = new FormData();
        for (var i = 0 ; i < $items.length ; i++) {
            var item = $items[i];
            if (item.type.indexOf("image") == -1) continue;

            var file = item.getAsFile();
            var fileName = hinata.getGuid() + file.type.replace('image/', '.');
            $data.append("file", file, fileName);
            $hasImage = true;
        }

        if ($hasImage === true) {
            uploadImage($self.data("upload-url"), $data, uploadImageSuccessCallback, uploadImageErrorCallback);
        }
    });

    uploadImage = function (url, data, successCallback, errorCallback, completeCallback) {
        $.ajax({
            type: "POST",
            url: url,
            contentType: false,
            processData: false,
            data: data,
            success: successCallback,
            error: errorCallback,
            complete: completeCallback,
        });
    };

    uploadImageSuccessCallback = function (result) {
        hinata.insertAtCaret("item-editor", "![" + result.OriginalFileName + "](" + result.Url + ")");

        var src = $("#item-editor").val();
        var html = marked(src);
        $("#item-view").html(html);
        $("pre code").each(function (i, block) {
            hljs.highlightBlock(block);
        });
    };

    uploadImageErrorCallback = function (xhr, status, p3, p4) {
        if (xhr.responseText && xhr.responseText[0] === "{") {
            var err = JSON.parse(xhr.responseText).Message;
            alert(err);
        }
    }
});
