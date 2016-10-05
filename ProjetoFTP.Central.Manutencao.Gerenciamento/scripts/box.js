(function ($) {
    $.fn.box = function (options) {
        var defaults = {
            icon: "/img/alerta.png",
            title: "box title",
            width : 500,
            height : 300,
            onClose : function() { },
            onCreate: function() { }
        };
        var identifier = this.attr('href');
        var html = $(identifier).html();
        var values = $.extend(defaults, options);
        var header = "<div class='box_overlay'>" +
                        "<div class='box_container'>" +
                            "<div class='box_header'></div><div class='box_content'>";
        var footer = "</div></div></div>";
        var html = header + footer;
        $(identifier).wrap("<div class='box_overlay'></div>").wrap("<div class='box_container'></div>").wrap("<div class='box_content'></div>");
        //$(identifier).parent().parent().find(".box_header").html("<img src='" + values.icon + "' class='box_icon'><h1>" + values.title + "</h1><a class='anchor_close'><img src='/img/fechar2.png'></a>");
        $(identifier).parent().before("<div class='box_header'><img src='" + values.icon + "' class='box_icon'><h1>" + values.title + "</h1><a class='anchor_close'><img src='/img/fechar2.png'></a></div>");
        var container = $(identifier).parent().parent();//alert(html);
        container.find(".anchor_close").click(function() { 
            $(this).parent().parent().parent().fadeToggle("medium",function() { eval(values.onClose()); });
        });
        eval(values.onCreate());
        //$(identifier).remove();
        //anchor click
        this.click(function() {
            $(identifier).parent().parent().parent().fadeToggle("medium");
            container.css('minWidth',values.width);
            container.css('minHeight',values.height);
            var width = container.width();
            var height = container.height();
            container.css('marginLeft', "-" + width/2 + "px");
            container.css('marginTop', "-" + height/2 + "px");
        });
        
        
    }
})(jQuery);

function createBox(title, content, options)
{
        var defaults = {
            icon: "/img/alerta.png",
            width : 500,
            height : 300,
        };
        var values = $.extend(defaults, options);
        var header = "<div class='box_overlay' id='box_id'>" +
                        "<div class='box_container'>" +
                            "<div class='box_header'><img src='" + values.icon + "' class='box_icon'><h1>" + title + "</h1><a class='anchor_close'><img src='/img/fechar2.png'></a></div><div class='box_content'>";
        var footer = "</div></div></div>";
        $(header + content + footer).appendTo($("body"));
        //close click
        $("#box_id").fadeToggle("medium");
        var container = $("#box_id").find(".box_container");
        container.css('minWidth',values.width);
        container.css('minHeight',values.height);
        var width = container.width();
        var height = container.height();
        container.css('marginLeft', "-" + width/2 + "px");
        container.css('marginTop', "-" +  height/2 + "px");
        $("#box_id").find(".anchor_close").click(function () {
            $(this).parent().parent().parent().fadeToggle("medium",function() { $(this).remove();});
        });

}

function showHideBox(box) {
    $(box).parent().parent().parent().fadeToggle("medium");
}