/// <reference path="_references.js" />
$(function () {
    var $menu = $('#menu-list');
    var $menuOffsetTop = $menu.offset().top;

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

    $("#aPreview").click(function () {
        var src = $("textarea#comment-editor").val();
        var html = marked(src);
        $("#comment-preview").html(html);
        $("pre code").each(function (i, block) {
            hljs.highlightBlock(block);
        });
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
                        hinata.insertAtCaret("comment-editor", "![" + result.OriginalFileName + "](" + result.Url + ")");
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

    createMenu = function () {
        var $idCount = {};
        $('#item-body').children('h1,h2,h3,h4,h5,h6').each(function () {
            var $header = $(this);
            var $text = $header.text();
            var $aId = '';
            $header.addClass('menu-anchor');
            if ($idCount[$text]) {
                var currentCount = parseInt($idCount[$text]);
                $aId = $text + '-' + (++currentCount).toString();
                $idCount[$text] = currentCount;
            }
            else {
                $aId = $text;
                $idCount[$text] = 1;
            }
            $header.attr('id', $aId);

            var $a = $('<li><a href="#' + $aId + '" aId="' + $aId + '">' + $text + '</a></li>');
            $menu.append($a);

            var $nestLevel = parseInt($header.prop('tagName').replace('H', ''));
            $a.addClass('menu-item');
            $a.css('padding-left', ($nestLevel * 1).toString() + 'em')
        });
    };
    floatMenu = function () {
        if ($(window).scrollTop() > $menuOffsetTop) {
            $menu.addClass('fixed-menu');
        } else {
            $menu.removeClass('fixed-menu');
        }
    };

    isVisibleInViewport = function (e) {
        var el = $(e);
        var top = $(window).scrollTop();
        var bottom = top + $(window).height();

        var eltop = el.offset().top;
        var elbottom = eltop + el.height();

        return (elbottom <= bottom) && (eltop >= top);
    };

    getFirstVisibleMenuAnchor = function () {
        var $firstAnchor;
        $('.menu-anchor').each(function (i, e) {
            var $h = $(e);
            if (isVisibleInViewport($h)) {
                $firstAnchor = $h;
                return false;
            }
        });

        return $firstAnchor;
    };

    setActiveMenuItem = function () {
        $firstAnchor = getFirstVisibleMenuAnchor();
        if (!$firstAnchor || $firstAnchor.length <= 0) return;

        var $menuItems = $menu.find('.menu-item');
        $menuItems.each(function (i, e) {
            var $li = $(e);
            $li.removeClass('active');

            var $a = $li.children('a');
            if ($firstAnchor[0].id == $a.attr('aId')) {
                $li.addClass('active');
            }
        });
    };

    $(window).scroll(function () {
        floatMenu();
        setActiveMenuItem();
    });

    createMenu();
});