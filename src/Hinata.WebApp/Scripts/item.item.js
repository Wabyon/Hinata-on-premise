/// <reference path="_references.js" />
$(function() {
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

    $("#comment-editor").tabIndent();

    $("#aPreview").click(function() {
        var src = $("textarea#comment-editor").val();
        var html = marked(src);
        $("#comment-preview").html(html);
        $("pre code").each(function (i, block) {
            hljs.highlightBlock(block);
        });
    });
});