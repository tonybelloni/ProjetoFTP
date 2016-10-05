var haveAlert = false;

function busAlert(title, message) {
    if (!haveAlert) {
        haveAlert = true;
        var element = "<div id='busAlert'>" +
                                "<div class='info'>" +
                                    "<div style='float : right;'><img src='/img/logo_carro.png' height='60px'/></div>" +
                                    "<div class='valor'>" + title + "</div>" +
                                "</div>" +
                       "</div>";
        $("body").append(element);
        var meio = ($(document).width() / 2) - ($("#busAlert").width() / 2) + 35;
        var finalWidth = $(document).width() - meio + 35;
        $("#busAlert").delay(200).animate({ "left": meio + "px",
            "opacity": "1"
        }, 200).delay(1600).animate({ "left": finalWidth + "px",
            "opacity": "0"
        }, 200,
            function () {
                $(this).remove();
                haveAlert = false;
            });

    }
    else {
        setTimeout(
        function () {
            busAlert(title, message);
        }, 1000);
    }

}

function rsAlert(title, message) {
    if (!haveAlert) {
        haveAlert = true;
        var element = "<div class='update' style=''><div id='rsAlert'>" +
                                    "<div class='topo'><a href='#'><div class='fechar'>X</div></a><h1>" + title + "</h1></div>" +
                                    "<div class='main'>" + message  + "</div>" +
                                    "<div class='rodape'>" + 
                                        "<img src='img/busvision.png' height='50px' style='float : right;'>" +    
                                    "</div>" +
                       "</div></div>";
        $("body").append(element);
        var meio = ($(document).width() / 2) - ($("#rsAlert").width() / 2) + 35;
        var finalWidth = $(document).width() - meio + 35;
        $("#rsAlert").delay(200).animate({ "left": meio + "px",
            "opacity": "1"
        }, 200, function () {
            $(this).find(".fechar").click(function () {
                $("#rsAlert").animate({ "left": finalWidth + "px",
                    "opacity": "0"
                }, 200,
            function () {
                $(this).remove();
                $(".update").fadeOut(function () { $(this).remove(); });
                haveAlert = false;
            });
            });
        });

    }
    else {
        setTimeout(
        function () {
            rsAlert(title, message);
        }, 1000);
    }

}