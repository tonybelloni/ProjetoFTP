function RelatorioCopiaClick() {
    $(".copias_rel tr .detalhes").click(function () {
        var html = '';
        var statusHtml = '';
        var transfHtml = '';
        //var codigo = $(this).parent().find(".codigo_status").text().split('/')[0];
        var codigo = 200;
        var estacao = $(this).parent().parent().find(".estacao").text();
        var id_copia = $(this).find("img").attr('id');
        var relQueryString = "?id=" + id_copia;
        $.ajax({
            url: '/queries/GetTransfInfo.ashx' + relQueryString,
            accpets: 'application/json',
            beforeSend: function () {
                $("body").append("<div id='loader' class='loader_rel'><img src='/img/preloader.gif'></div>");
            },
            success: function (data) {
                transfHtml = "<div class='copia_info_container'>" +
                                 "<div class='copia_info'>Carro: <span class='copia_info_data'>" + data.NumeroCarro + "</span></div>" +
                                 "<div class='copia_info'>Estação: <span class='copia_info_data'>" + estacao + "</span></div>" +
                                 "<div class='copia_info'>Tipo de Filtro: <span class='copia_info_data'>" + data.TipoCopia + "</span></div>" +
                                 "<div class='copia_info'>Início da cópia: <span class='copia_info_data'>" + data.DataInicialString + "</span></div>" +
                                 "<div class='copia_info'>Fim da cópia: <span class='copia_info_data'>" + data.DataFinalString + "</span></div>" +
                                 "<div class='copia_info'>Duração: <span class='copia_info_data'>" + data.Intervalo.toFixed(2) + " minutos</span></div>" +
                                 "<div class='copia_info'>Velocidade Média: <span class='copia_info_data'>" + data.VelocidadeMedia.toFixed(2) + " MB/min</span></div>" +
                                 "<div class='copia_info'>Arquivos no Equipamento: <span class='copia_info_data'>" + data.QuantidadeArquivosTotal + " arquivos</span></div>" +
                                 "<div class='copia_info'>Arquivos Válidos para o Sistema: <span class='copia_info_data'>" + data.QuantidadeArquivosValidos + " arquivos</span></div>" +
                                 "<div class='copia_info'>Arquivos Copiados: <span class='copia_info_data'>" + data.QuantidadeArquivosCopiados + " arquivos</span></div>" +
                                 "<div class='copia_info'>Volume de Dados Total: <span class='copia_info_data'>" + data.VolumeArquivosTotal + " MB</span></div>" +
                                 "<div class='copia_info'>Volume de Dados Copiados: <span class='copia_info_data'>" + data.VolumeArquivosCopiados + " MB</span></div>" +
                                 "<div class='copia_info'>Período Inicial dos Arquivos: <span class='copia_info_data'>" + data.PeriodoInicialString + "</span></div>" +
                                 "<div class='copia_info'>Período Final dos Arquivos: <span class='copia_info_data'>" + data.PeriodoFinalString + "</span></div>" +
                             "</div>";
                $.ajax({
                    url: 'queries/GetProcedimentos.ashx?cod=' + data.Codigo,
                    accepts: 'application/json',
                    beforeSend: function () {
                    },
                    success: function (data) {
                        if (data.Codigo != null) {
                            statusHtml = "<h2>Status da Transferência</h2>" +
                                         "<div class='copia_info_container'>" +
                                         "<div class='copia_id_carro'>Status:" + data.Codigo + "/" + data.Status + "</div>" +
                                         "<div class='copia_id_carro'>Mensagem:" + data.Resposta + "</div>";
                            if (data.Procedimentos != null) {
                                if (data.Procedimentos.length > 0) {
                                    statusHtml += "<div>Procedimentos:<ul class='lista_procedimentos'>";
                                    $.each(data.Procedimentos, function (i, procedimento) {
                                        statusHtml += "<li>" + procedimento + "</li>";
                                    });
                                    statusHtml += "</ul></div></div>";
                                }
                            }
                        }
                        html = "<div class='copia_info_container'>" +
                                   "<h2>Dados da Transferência</h2>" +
                                   transfHtml +
                                   statusHtml +
                               "</div>";
                        createBox("Relatório Detalhado", html, { icon: "/img/relatorio_icon.png" });
                    }
                });
            },
            error: function () {

            },
            complete: function () {
                $(".loader_rel").remove();
            }
        });
    });

}