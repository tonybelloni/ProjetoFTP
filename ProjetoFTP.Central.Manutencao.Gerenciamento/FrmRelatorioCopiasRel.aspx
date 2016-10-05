<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmRelatorioCopiasRel.aspx.cs" Inherits="ProjetoFTP.Web.FrmRelatorioCopiasRel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.min.js" type="text/javascript"></script>
    <link href="/estilos/relatorioscopiasrel.css" rel="stylesheet" type="text/css" />
    <script src="/fancybox/jquery.fancybox.js" type="text/javascript"></script>
    <link href="/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <link href="/estilos/jquery.alerts.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.alerts.js" type="text/javascript"></script>
    <script src="/scripts/relatoriodetalhado.js" type="text/javascript"></script>
    <script src="/scripts/download.js" type="text/javascript"></script>
    <link href="estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="scripts/iscroll.js" type="text/javascript"></script>
    <style>

        #updateHolder {
            width: 100%;  /* <= For Width: change the width value */
            color: #333;
            margin-top: 10px;
            overflow: hidden;
            border:1px solid #CCCCCC;
	    }

	    #updateContainer {
            background-color: white;
            position: relative;
            cursor: pointer;
	    }
	    
        #updateContent {
            background: #fff;
            position: absolute;
            padding-right: 10px;
	    }

	    #updateScollBar {
            background: #E2E2E2;
            border-left: 1px solid #D6D6D6;
            position: absolute;
            width: 10px;
            right: 0;
            bottom: 0;
            cursor: default;
            zoom: 1;
    		filter: alpha(opacity=50); /* For IE8 and earlier */
    		opacity: 0.5;	  
	    }

	    #updateScollScrubber {
            background-color: #b0b0b0;
            width: 10px;
            padding-left : 0px;
            height: 50px;
            position: absolute;
            border-left:1px solid #444;
            border-top:1px solid #444;
        }

        #updateScollBar, #updateContainer, #updateHolder {        
	    }

    </style>
    <script>
        $(document).ready(function () {
            download();
            $(".fancybox").fancybox({ width: 650, padding: 2 });
            RelatorioCopiaClick();
            $("#filtro-toggle,#fechar_busca_avancada").click(function () {
                $("#filtro").fadeToggle("medium");
            });
            //var height = $(window).height() - 110;
            //height = ($(".copias_rel").height() > height) ? height : $(".copias_rel").height();
            //$("#updateScollBar, #updateContainer, #updateHolder").height(height);
        });

        var myScroll;
        function loaded() {
            myScroll = new iScroll('wrapper', { vScrollbar: true });
        }

        document.addEventListener('DOMContentLoaded', loaded, false);
    </script>
    <!--<script>

        $(document).ready(function () {

            var _offsetY = 0,
    _startY = 0,
    scrollStep = 10,
    isScrollBarClick = false,
    contentDiv,
    scrubber,
    scrollHeight,
    contentHeight,
    scrollFaceHeight,
    initPosition,
    initContentPos,
    moveVal,
    scrubberY = 0;

            element = document.getElementById("updateHolder");
            if (element.addEventListener)
            /** DOMMouseScroll is for mozilla. */
                element.addEventListener('DOMMouseScroll', wheel, false);
            /** IE/Opera. */
            element.onmousewheel = document.onmousewheel = wheel;

            // To resize the height of the scroll scrubber when scroll height increases. 
            setScrubberHeight();

            contentDiv = document.getElementById('updateContainer');

            scrubber = $('#updateScollScrubber');

            scrollHeight = $('#updateScollBar').outerHeight();

            contentHeight = $('#updateContent').outerHeight();

            scrollFaceHeight = scrubber.outerHeight();

            initPosition = 0;

            initContentPos = $('#updateHolder').offset().top;

            // Calculate the movement ration with content height and scrollbar height
            moveVal = (contentHeight - scrollHeight) / (scrollHeight - scrollFaceHeight);

            $('#updateHolder').bind('mousewheel', wheel);

            $("#updateScollScrubber").mouseover(function () {
                // Enable Scrollbar only when the content height is greater then the view port area.
                isScrollBarClick = false;
                if (contentHeight > scrollHeight) {
                    // Show scrollbar on mouse over
                    scrubber.bind("mousedown", onMouseDown);
                }
            }).mouseout(function () {
                isScrollBarClick = false;
                if (contentHeight > scrollHeight) {
                    // Hide Scrollbar on mouse out.
                    $('#updateHolder').unbind("mousemove", onMouseMove);
                    scrubber.unbind("mousedown", onMouseDown);
                }
            });


            $("#updateScollBar").mousedown(function () {
                isScrollBarClick = true;
            }).mouseout(function () {
                isScrollBarClick = false;
            }).mouseup(function (event) {
                if (isScrollBarClick == false)
                    return;
                if ((event.pageY - initContentPos) > (scrollHeight - scrubber.outerHeight())) {
                    scrubber.css({ top: (scrollHeight - scrubber.outerHeight()) });
                } else {
                    scrubber.css({ top: (event.pageY - initContentPos) - 5 });
                }
                $('#updateContent').css({ top: ((initContentPos - scrubber.offset().top) * moveVal) });
            });


            function onMouseDown(event) {
                $('#updateHolder').bind("mousemove", onMouseMove);
                $('#updateHolder').bind("mouseup", onMouseUp);
                _offsetY = scrubber.offset().top;
                _startY = event.pageY + initContentPos;
                // Disable the text selection inside the update area. Otherwise the text will be selected while dragging on the scrollbar.
                contentDiv.onselectstart = function () { return false; } // ie
                contentDiv.onmousedown = function () { return false; } // mozilla
            }

            function onMouseMove(event) {

                isScrollBarClick = false;
                // Checking the upper and bottom limit of the scroll area
                if ((scrubber.offset().top >= initContentPos) && (scrubber.offset().top <= (initContentPos + scrollHeight - scrollFaceHeight))) {
                    // Move the scrubber on mouse drag
                    scrubber.css({ top: (_offsetY + event.pageY - _startY) });
                    // Move the content area according to the scrubber movement.
                    $('#updateContent').css({ top: ((initContentPos - scrubber.offset().top) * moveVal) });
                } else {
                    // Reset when upper and lower limits are excced.
                    if (scrubber.offset().top <= initContentPos) {
                        scrubber.css({ top: 0 });
                        $('#updateContent').css({ top: 0 });
                    }
                    if (scrubber.offset().top > (initContentPos + scrollHeight - scrollFaceHeight)) {
                        scrubber.css({ top: (scrollHeight - scrollFaceHeight - 2) });
                        $('#updateContent').css({ top: (scrollHeight - contentHeight + initPosition) });
                    }
                    $('#updateHolder').trigger('mouseup');
                }
            }

            function onMouseUp(event) {
                $('#updateHolder').unbind("mousemove", onMouseMove);
                contentDiv.onselectstart = function () { return true; } // ie
                contentDiv.onmousedown = function () { return true; } // mozilla
            }

            function setScrubberHeight() {
                cH = $('#updateContent').outerHeight();
                sH = $('#updateScollBar').outerHeight();
                if (cH > sH) {
                    // Set the min height of the scroll scrubber to 20
                    if (sH / (cH / sH) < 20) {
                        $('#updateScollScrubber').css({ height: 20 });
                    } else {
                        $('#updateScollScrubber').css({ height: sH / (cH / sH) });
                    }
                }
            }

            function onMouseWheel(dir) {
                scrubberY = scrubber.offset().top + (scrollStep * dir) - initContentPos;
                if ((scrubberY) > (scrollHeight - scrubber.outerHeight())) {
                    scrubber.css({ top: (scrollHeight - scrubber.outerHeight()) });
                } else {
                    if (scrubberY < 0) scrubberY = 0;
                    scrubber.css({ top: scrubberY });
                }
                $('#updateContent').css({ top: ((initContentPos - scrubber.offset().top) * moveVal) });
            }

            /** This is high-level function.
            * It must react to delta being more/less than zero.
            */
            function handle(delta) {
                if (delta < 0) {
                    onMouseWheel(1);
                }
                else {
                    onMouseWheel(-1);
                }
            }

            /** Event handler for mouse wheel event.
            */
            function wheel(event) {
                var delta = 0;
                if (!event) /* For IE. */
                    event = window.event;
                if (event.wheelDelta) { /* IE/Opera. */
                    delta = event.wheelDelta / 120;
                } else if (event.detail) { /** Mozilla case. */
                    /** In Mozilla, sign of delta is different than in IE.
                    * Also, delta is multiple of 3.
                    */
                    delta = -event.detail / 3;
                }
                /** If delta is nonzero, handle it.
                * Basically, delta is now positive if wheel was scrolled up,
                * and negative, if wheel was scrolled down.
                */
                if (delta)
                    handle(delta);
                /** Prevent default actions caused by mouse wheel.
                * That might be ugly, but we handle scrolls somehow
                * anyway, so don't bother here..
                */
                if (event.preventDefault)
                    event.preventDefault();
                event.returnValue = false;
            }

        });

</script>-->
</head>
<body>
    <div id="sis_info"></div>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
    <div class="pagina" id="relatorios">
        <div class="topo">
            <a href="Default.aspx">
                <img src="/img/busvision.png" height="40px" id="logo_topo" />
            </a>
        </div>
        <div id="conteudo">
            <asp:UpdatePanel runat="server" ID="updRelatorio">
                <ContentTemplate>
            <div id="gv_opt" class="direita">
                <asp:HyperLink runat="server" ID="hlEmal" CssClass="btn fancybox" NavigateUrl="#emails">Enviar E-mail</asp:HyperLink> 
                <asp:Button runat="server" ID="btnPDF" CssClass="btn" Text="Exportar PDF" />
                <div id="paginas_gridview">
                    <asp:Label runat="server" ID="lblPaginas" Text="1 de 10"></asp:Label>
                    <asp:Button runat="server" ID="btnAnterior" CssClass="btn cinza" Text="<" OnClick="btnAnterior_Click" />
                    <asp:Button runat="server" ID="btnProximo" CssClass="btn cinza" Text=">" OnClick="btnProximo_Click" />
                </div>
            </div>
            <img src="/img/relatorio_icon.png" id="page_icon"/><a id="filtro-toggle" href="#"><h1>Relatório de Cópias</h1></a>
            <div id="filtro">
                <div class=""><img id="page_icon" src="/img/busca.png"/><h1>Busca Avançada</h1><img src="/img/fechar2.png" class="direita" width="32px" id="fechar_busca_avancada"/></div>
                <table class="form esquerda pequeno" id="filtro1">
                    <tr>
                        <td>Data Inicial:</td>
                        <td><asp:TextBox runat="server" ID="txtDataInicial" CssClass="txt"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Data Final:</td>
                        <td><asp:TextBox runat="server" ID="txtDataFinal" CssClass="txt"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Número do Carro:</td>
                        <td><asp:TextBox runat="server" ID="txtNumero" CssClass="txt"></asp:TextBox></td>
                    </tr>
                </table>              
                <table class="form esquerda pequeno" id="filtro2">
                    <tr>
                        <td>Velocidade Mínima(MB/min):</td>
                        <td><asp:TextBox runat="server" ID="txtVelMediaMin" CssClass="txt"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Velocidade Máxima(MB/min):</td>
                        <td><asp:TextBox runat="server" ID="txtVelMediaMax" CssClass="txt"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Tipo de cópia:</td>
                        <td><asp:DropDownList runat="server" ID="ddlTipo" CssClass="ddl" Width="98px">
                                <asp:ListItem Text="Todas" Value="-999" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Guia" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="Ultimo Arq" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Completa" Value="1"></asp:ListItem>
                        </asp:DropDownList></td>
                    </tr>
                </table>             
                <table class="form esquerda pequeno" id="filtro3">
                    <tr>
                        <td>Duração Mínima(min):</td>
                        <td><asp:TextBox runat="server" ID="txtDurMin" CssClass="txt"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Duração Máxima(min):</td>
                        <td><asp:TextBox runat="server" ID="txtDurMax" CssClass="txt"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="2"><asp:Button runat="server" ID="btnFiltrar" CssClass="btn" Text="Filtar" OnClick="btnFiltrar_Click"/></td>
                    </tr>
                </table>
                <div class="esquerda" style="border-left : 1px solid #ababab; padding : 0px 5px;">
                    <ul id="legenda">
                        <li><div class="esquerda cor" id="verde"></div><div class="desc">Cópia completa</div></li>
                        <li><div class="esquerda cor" id="amarelo"></div><div class="desc">Cópia parcial</div></li>
                        <li><div class="esquerda cor" id="vermelho"></div><div class="desc">Erro na cópia</div></li>
                    </ul>
                </div>
            </div>
            <script>
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_endRequest(function () {
                    RelatorioCopiaClick();
                    $("#filtro-toggle").click(function () {
                        $("#filtro").fadeToggle("medium");
                    });
                }); 
            </script>

            <!--<div id="updateHolder">                <div id="updateContainer">                    <div id="updateContent">-->                    
                <div id="wrapper">
                    <div id="scroller">                        <asp:GridView runat="server" ID="gvRelatorio" AutoGenerateColumns="false" AllowSorting="true" Width="100%" GridLines="None"
                                onrowdatabound="gvRelatorios_RowDataBound" PageSize="100" OnRowCommand="gvRelatorios_RowCommand" AllowPaging="true" ShowHeader="false" ShowFooter="false"
                                onpageindexchanging="gvRelatorios_PageIndexChanging" CssClass="grid copias_rel">
                            <PagerStyle CssClass="pager" HorizontalAlign="Right" />  
                            <Columns>
                                <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn detalhes" Text="Detalhes" CommandName="ver detalhes" ItemStyle-Width="75px" ItemStyle-HorizontalAlign="Center"/>
                                <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn" Text="Copiar" CommandName="Copiar" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Left"/>
                                <asp:BoundField HeaderText="Nº do carro" DataField="NumeroCarro" ItemStyle-Width="50px" ItemStyle-CssClass="id_veiculo" />
                                <asp:BoundField HeaderText="Código de Status" DataField="Codigo" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="codigo_status"  />
                                <asp:BoundField HeaderText="Inicio da cópia" DataField="DataInicial" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="data_inicial" />
                                <asp:BoundField HeaderText="Fim da cópia" DataField="DataFinal" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="data_final" />
                                <asp:BoundField HeaderText="Código do Equipamento" DataField="PenDrive" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField HeaderText="Intervalo de tempo" DataField="Intervalo" DataFormatString="{0:0.0} min" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Right"  />
                                <asp:BoundField HeaderText="Nº de arquivos total" DataField="QuantidadeArquivosTotal" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField HeaderText="Nº de arquivos validos" DataField="QuantidadeArquivosValidos" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField HeaderText="Nº de arquivos copiados" DataField="QuantidadeArquivosCopiados" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField HeaderText="Volume de dados total" DataField="VolumeArquivosTotal" DataFormatString="{0:0} MB" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right"  />
                                <asp:BoundField HeaderText="Volume de dados copiados" DataField="VolumeArquivosCopiados" DataFormatString="{0:0} MB" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right"  />
                                <asp:BoundField HeaderText="Velocidade média (MB/min)" DataField="VelocidadeMedia" DataFormatString="{0:0.0} MB/min" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField HeaderText="Perido inicial dos arquivos" DataField="PeriodoInicial" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField HeaderText="Perido final dos arquivos" DataField="PeriodoFinal" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField HeaderText="Tipo de Cópia" DataField="TipoCopia" ItemStyle-Width="180px" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="codigo"/>
                            </Columns>
                        </asp:GridView>
			        </div>                    </div>
			<!--<div id="updateScollBar">
				<div id="updateScollScrubber">
				</div>
			</div>
                </div>
            </div>-->

                    
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdateProgress runat="server" ID="updProgressRelatorio" AssociatedUpdatePanelID="updRelatorio">
                <ProgressTemplate>
                    <div class="update">
                        <div class="upd_desc"><img src="/img/pl.gif" /></div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
                    <div id="emails" class="fb_cont">
                        <div class="fb_topo">
                            <img id="page_icon" src='/img/email.png'><h1>Emails</h1>
                        </div>
                        <div class="fb_main">
                            <asp:UpdatePanel runat="server" ID="updPanelEmails">
                                <ContentTemplate>
                                    <asp:GridView runat="server" ID="gvEmails" CssClass="grid" GridLines="None" ShowHeader="false" AutoGenerateColumns="false" Width="650px" ShowFooter="false" OnRowCommand="gvEmails_RowCommand">
                                        <Columns>
                                            <asp:ButtonField ButtonType="Image" ImageUrl="/img/enviar.png" CommandName="Enviar" ControlStyle-Width="32px"/>
                                            <asp:BoundField DataField="Endereco" ItemStyle-Width="585px"/>
                                            <asp:ButtonField ButtonType="Image" ImageUrl="/img/fechar2.png" CommandName="remover" ControlStyle-Width="32px" />
                                        </Columns>
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress runat="server" ID="updEmailsProgress" AssociatedUpdatePanelID="updPanelEmails">
                                <ProgressTemplate>
                                    Carregando
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </div>
                        <div class="fb_rodape">
                        </div>
                    </div>
        </div>
    </div>
    <div id="download-list"></div>
    </form>
</body>
</html>
