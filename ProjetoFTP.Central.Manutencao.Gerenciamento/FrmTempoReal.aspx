<html>
    <head>
        <link href="estilos/temporeal.css" rel="stylesheet" type="text/css" />
        <script src="/scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
        <script type="text/javascript">
            $(document).ready(function () {
                alert('');
                /*setInterval(function () {
                    $.ajax({
                        url: "/queries/GetTransferencias.ashx",
                        success: function (data) {
                            var transferencias = $(".transferencias");
                            $.each(transferencias.find(".download_item"), function (i, o) {
                                var container = $(o);
                                var id = container.attr('id');
                                var aux = false;
                                for (var i = 0; i < data.length; i++) {
                                    if (id == data[i].Numero) {
                                        aux = true;
                                        break;
                                    }
                                }
                                if (!aux) {
                                    container.remove();
                                }
                            });
                            var html = "<ul>";
                            $.each(data, function (i, o) {
                                var container = $("#" + o.Numero);
                                if (container.length == 0) {
                                    html += "<li id='" + o.Numero + "' class='download_item'>" +
                                                "<div class='download_volume'>" + o.VolumeCopiado + " MB de " + o.VolumeTotal + " MB</div>" +
                                                "<div class='download_title'>" + o.Numero + "</div>" +
                                                "<div class='download_bar'><div></div></div>" +
                                                "<div class='download_speed_avg'>" + parseFloat(o.VelocidadeMedia).toFixed(2) + " MB/s</div>" +
                                            "</li>";
                                }
                                else {
                                    container.find(".download_bar > div").width(parseInt(o.Percent) + "%");
                                    container.find(".download_speed_avg").text(parseFloat(o.VelocidadeMedia).toFixed(2) + " MB/s");
                                    container.find(".download_volume").text(o.VolumeCopiado + " MB de " + o.VolumeTotal + " MB");
                                }
                            });
                            transferencias.append(html + "</ul>");
                            if (data.length == 0) {
                                $("#aviso_transferencia").addClass("visible");
                            }
                            else {
                                $("#aviso_transferencia").removeClass("visible");
                            }
                        }
                    });
                    $.ajax({
                        url: "/queries/GetTerminais.ashx",
                        success: function (data) {
                            var terminais = $("#terminais");
                            $.each(terminais.find(".terminal_item"), function (i, o) {
                                var container = $(o);
                                var id = container.attr('id');
                                var aux = false;
                                for (var i = 0; i < data.length; i++) {
                                    if (id == data[i].Nome) {
                                        aux = true;
                                        break;
                                    }
                                }
                                if (!aux) {
                                    container.remove();
                                }
                            });
                            var html = "<ul>";
                            var tokens = data.data_atual.split(' ');
                            var tokensDate = tokens[0].split('/');
                            var tokensTime = tokens[1].split(':');
                            var date = new Date(tokensDate[2], tokensDate[1], tokensDate[0], tokensTime[0], tokensTime[1], tokensTime[2] - 2);
                            var haveOneOnline = false;
                            $.each(data.terminais, function (i, o) {
                                var ts = o.ultimo_sinal.split(' ');
                                var td = ts[0].split('/');
                                var tt = ts[1].split(':');
                                var d = new Date(td[2], td[1], td[0], tt[0], tt[1], tt[2]);
                                var container = $("#" + o.nome.replace(/\s+/g, ''));
                                if (container.length == 0) {
                                    html += "<li id='" + o.nome.replace(/\s+/g, '') + "' class='download_item'>" +
                                                "<img src='" + ((date < d) ? "/img/terminal_ligado.png" : "/img/terminal_desligado.png") + "' class='terminal_image'>" +
                                                "<span class='terminal_queue'>" + o.fila + "</span>" +
                                                "<div class='terminal_title'>" + o.nome + "</div>" +
                                                "<div class='terminal_subtitle'>" + o.ip + "</div>" +
                                            "</li>";
                                }
                                else {
                                    container.find("img").attr('src', ((date < d) ? "/img/terminal_ligado.png" : "/img/terminal_desligado.png"));
                                    var last_queue = container.find(".terminal_queue").html();
                                    if (last_queue != o.fila) {
                                        var obj = container.find(".terminal_queue");
                                        obj.html(o.fila);
                                        obj.css('-webkit-animation-name', 'bounce');
                                        setTimeout(function () {
                                            obj.css('-webkit-animation-name', '');
                                        }, 1000);
                                    }
                                }
                                if (date < d)
                                    haveOneOnline = true;
                            });
                            if (!haveOneOnline) {
                                $("#aviso_terminal").addClass("visible");
                            }
                            else {
                                $("#aviso_terminal").removeClass("visible");
                            }
                            terminais.append(html + "</ul>");
                        }
                    });

                }, 1000);*/
            });

        </script>
    </head>
    <body>
        <div id="temporeal_container" align="left">
            <div id="wrapper">
                <div class="transferencias" id="scroller">
                    <div id="aviso_transferencia">
                        <h1>Nenhma cópia sendo feita!</h1>
                        <p>Por enquanto não existe nenhuma estação copiando arquivos.</p>
                    </div>
                </div>
            </div>
            <div id="terminais">
                <div id="aviso_terminal">
                    <span id="aviso_luz"></span>
                    <h1>As estações estão desligadas!</h1>
                    <p>O sistema identificou que não tem neste momento estações ligadas para fazer transferência. Ligue-as imediatamente para que o processo de cópias ocorra normalmente.</p>
                </div>
            </div>
        </div>
    </body>
</html>