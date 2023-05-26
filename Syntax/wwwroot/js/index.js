$(function () {

    $('.main-page-post').on('click', (e) => {

        var target = $(e.delegateTarget);

        window.location.href = target.data('view-post-url');
    });

    $('.main-page-post').on('mouseenter', (e) => {

        $(e.delegateTarget).addClass("main-page-post-hover");
    });
    
    $('.main-page-post').on('mouseleave', (e) => {

        $(e.delegateTarget).removeClass("main-page-post-hover");
    });


});