(function ($) {
    $.fn.watermark = function (mark) {
        this.val(mark).focus(function () {
            if ($(this).val() == mark) {
                $(this).val("");
            }
            $(this).removeClass("watermark_text");
        }).blur(function () {
            if ($(this).val().trim() == "" || $(this).val() == mark) {
                $(this).val(mark).addClass("watermark_text");
            }
        });
    };
})(jQuery);