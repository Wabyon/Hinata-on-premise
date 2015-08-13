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

                $.ajax({
                    type: "POST",
                    url: $self.data("url"),
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        hinata.insertAtCaret("item-editor", "![" + result.OriginalFileName + "](" + result.Url + ")");

                        var src = $("#item-editor").val();
                        var html = marked(src);
                        $("#item-view").html(html);
                        $("pre code").each(function (i, block) {
                            hljs.highlightBlock(block);
                        });
                    },
                    error: function (xhr, status, p3, p4) {
                        if (xhr.responseText && xhr.responseText[0] === "{") {
                            var err = JSON.parse(xhr.responseText).Message;
                            alert(err);
                        }
                    },
                    complete: function () {
                        $self.val("");
                    }
                });
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
});
