<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Busvision Server - Painel</title>
    <link href="/estilos/style.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <link href="/estilos/carrospainel.css" rel="stylesheet" type="text/css" />
    <link href="/estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/download.js" type="text/javascript"></script>
    <script>
        var first = true;
        var lastId = 0;
        var oldestDate = "";
        var oldestId = 0;
        var content = null;
        var firstDate = "";
        var lastDate = "";
        $(document).ready(function () {
            download();
            content = $(".notificacoes_container");
            setInterval(function () {
                updateNotificacoes();
            }, 1000);
            updateCarros();
            setInterval(function () {
                updateCarros();
            }, 1000);
            var oldestBlocked = false;
            content.scroll(function () {
                var div = $(this);
                if (div[0].scrollHeight - div.scrollTop() == div.outerHeight() && !oldestBlocked) {
                    var aux = oldestDate.replace("/", "").replace("/", "").substr(0, 8);
                    var d = aux.substr(4, 4) + aux.substr(2, 2) + aux.substr(0, 2);
                    oldestBlocked = true;
                    var length = 0;
                    $.ajax({
                        url: "/queries/GetNotificacoes.ashx",
                        data: {
                            i: $(".notificacao_item").last().attr("id").split('_')[1],
                            a: "oldest",
                            d: d
                        },
                        success: function (data) {
                            if (data != null) {
                                length = data.notifications.length;
                                $.each(data.notifications, function (i, o) {
                                    var obj = $("#not_" + o.id)
                                    if (obj.length == 0) {
                                        setTimeout(function () {
                                            obj = generateItemNotificacaoHtmlObj(o.id, o.t, o.d, o.c, o.m, o.s);
                                            if (o.d.split(' ')[0] != oldestDate.split(' ')[0]) {
                                                content.append("<div class='notification_date_separator'>" + o.d.split(' ')[0] + "</div>");
                                            }
                                            content.append(obj);
                                            oldestDate = o.d;
                                            showItemNotificacao(obj);
                                        }, i * 100);
                                    }
                                });
                            }
                        },
                        complete: function () {
                            setTimeout(function () { oldestBlocked = false; }, length * 100);
                        }
                    });
                }
            });
        });

        function showMessage(message, status) {
            var elem = $("#main_message");
            var d = 0;
            if (elem.hasClass('visible')) {
                d = 5000;
            }
            setTimeout(function () {
                elem.attr('class', 'message visible ' + status);
                elem.find(".message_content").html(message);
                setTimeout(function () {
                    elem.attr('class', 'message');
                }, 5000);
            }, d);
        }
        function updateNotificacoes() {
            $.ajax({
                url: "/queries/GetNotificacoes.ashx",
                data: { i: lastId, a: "newest" },
                success: function (data) {
                    $.each(data.notifications, function (i, o) {
                        var obj = $("#not_" + o.id);
                        if (obj.length == 0) {
                            obj = generateItemNotificacaoHtmlObj(o.id, o.t, o.d, o.c, o.m, o.s);
                            if (oldestDate == "" || oldestDate.localeCompare(o.d) == 1) {
                                oldestDate = o.d;
                            }
                            if (o.d.split(' ')[0] != firstDate.split(' ')[0] && firstDate != "") {
                                content.prepend("<div class='notification_date_separator'>" + firstDate.split(' ')[0] + "</div>");
                            }
                            content.prepend(obj);
                            showItemNotificacao(obj);
                            showAlertIfNeed(o);
                            firstDate = o.d;
                        }
                    });
                    if(data.notifications.length > 0)
                    lastId = data.notifications[data.notifications.length - 1].id;
                }
            });
        }
        function generateItemNotificacaoHtmlObj(id_elem, tipo, date, carro, message, status) {
            date = date.split(' ')[1].substring(0, 5);
            var class_elem = "notificacao_item " + ((tipo == "s") ? "notificacao_sinal" : (tipo == "c") ? "notificacao_camera" : "notificacao_transferencia") + " new_notificacao_item " + ((status == 0) ? "notificacao_negativa" : "notificacao_positiva");
            obj = $("<div id='not_" + id_elem + "' class='" + class_elem + "'>" +
                        "<span class='notificacao_date'>" + date + "</span>" +
                        "<h2>" + carro + "</h2>" +
                        message +
                    "</div>");
            return obj;
        }
        function showItemNotificacao(obj) {
            setTimeout(function () {
                obj.removeClass("new_notificacao_item");
            }, 200);
        }
        function showAlertIfNeed(o) {
            if (lastId > 0) {
                showMessage(o.c + " - " + o.m, o.s == 0 ? "negative" : "positive");
            }
        }
        function updateCarros() {
            $.ajax({
                url: "/queries/GetCarros.ashx",
                success: function (data) {
                    $.each(data, function (i, o) {
                        var obj = $("#" + o.n);
                        var carros = $("#carros");
                        if (obj.length == 0) {
                            obj = $("<div class='carro' id='" + o.n + "'>" +
                                        "<spam class='car_id'>" + o.n + "</spam>" +
                                    "</div>");
                            obj.appendTo(carros);
                        }
                        var css_class = '';
                        if (o.p == 0) {
                            if (o.e == 1) {
                                css_class = 'carro carro_transparente';
                            }
                            else {
                                css_class = 'carro carro_semitransparente';
                            }
                        }
                        else {
                            if (o.g == 1) {
                                css_class = 'carro carro_verde';
                            }
                            else {
                                css_class = 'carro carro_vermelho';
                            }
                        }

                        if (obj.attr('class') != css_class) {
                            obj.attr("class", css_class);
                            if (!first) {
                                obj.css('-webkit-animation-name', 'carro_animation');
                                setTimeout(function () {
                                    obj.css('-webkit-animation-name', '');
                                }, 4000);
                            }
                        }
                    });
                    first = false;
                }
            });
        }
    </script>
    <style>
        #download-container {
            top: 0;
        }

        #download-show-hide {
            display: none;
        }
    </style>
</head>
<body align="center">
    <header id="top_shadow">
        <a href="Default.aspx">
            <img src='/img/busvision.png' id="logo_busvision"/>
        </a>
    </header>
    <section id="carros_container">
        <div id="carros">
        </div>
    </section>
    <aside class="notificacoes_wrapper">
        <div class="notificacoes_content">
            <header id="notificacoes_header">
                <h1>Notificações</h1>
            </header>
            <div class="notificacoes_container"></div>
        </div>
    </aside>
        <div id="main_message" class="message">
            <div class="message_content"></div>
        </div>
    <div id="download-list"></div>
        <div id="dialog">
            <h1 style="text-align: left;">Carro: <span id="dialog_id_car">123</span></h1>
            <img src="/img/fechar2.png" class="fechar" />
            <div id="dialog_info">
                Código do Equipamento: <span id="dialog_pendrive_code"></span>
                <br />
                Quantidade de arquivos no equipamento: <span id="dialog_qt_arqs_pendrive"></span>
                <br />
                Quantidade de arquivos válidos: <span id="dialog_qt_valid_arqs"></span>
                <br />
                Status: <span></span>
                <spam></spam>
                <div id="dialog_bottom"></div>
            </div>
        </div>
</body>
</html>
<!--<div class="notificacao_item notificacao_camera">
                <span class="notificacao_date">14:22</span>
                <h2>42565</h2>
                O carro estava gravando, mas parou de gravar.
            </div>
            <div class="notificacao_item notificacao_sinal">
                <span class="notificacao_date">14:21</span>
                <h2>42585</h2>
                O carro havia sido localizado, mas perdemos o sinal dele.
            </div>
        </div>-->
