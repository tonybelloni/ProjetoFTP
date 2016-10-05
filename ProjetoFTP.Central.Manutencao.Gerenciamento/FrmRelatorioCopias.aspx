<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmRelatorioCopias.aspx.cs" Inherits="ProjetoFTP.Web.FrmRelatorioCopias" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="scripts/jquery.min.js" type="text/javascript"></script>
    <script src="scripts/download.js" type="text/javascript"></script>
    <link href="estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.ui.core.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.mouse.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.resizable.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.draggable.js" type="text/javascript"></script>
    <script>
        $(document).ready(function () {
            $("#menu").height($(window).height() - $(".topo").height() - 43);
            $(".pagina #conteudo").width($(window).width());
            $("#rel-target").height($(document).height() - 91);
            $("#rel").click(function () {
                $("#rel-target").attr('src', 'FrmRelatorioCopiasRel.aspx');
            });
            $("#graf").click(function () {
                $("#rel-target").attr('src', 'FrmRelatorioCopiasGraf.aspx');
            });
            download();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="sis_info"></div>
    <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
    <div class="pagina">
        <div class="topo">
            <a href="Default.aspx">
                <img src="/img/busvision.png" height="40px" id="logo_topo" />
            </a>
        </div>
        <div class="esquerda" id="conteudo">
            <!--<div class="hor-menu">
                <a id='rel' href="#">Relatórios</a>
                <a id='graf' href="#">Gráficos</a>
            </div>-->
            </div>
            <div id="download-list"></div>
    </form>
</body>
</html>
