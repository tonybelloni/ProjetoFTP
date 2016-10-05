<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmEditaEstacao.aspx.cs" Inherits="ProjetoFTP.Web.FrmEditaEstacao" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="scripts/jquery.min.js" type="text/javascript"></script>
    <link href="estilos/rsAlert.css" rel="stylesheet" type="text/css" />
    <script src="scripts/alert.js" type="text/javascript"></script>
    <script src="scripts/download.js" type="text/javascript"></script>
    <link href="estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.ui.core.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.mouse.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.resizable.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.draggable.js" type="text/javascript"></script>
    <script>
        $(document).ready(function () {
            $("#menu").height($(document).height() - $(".topo").height() - 43);
            $(".pagina #conteudo").width($(document).width() - $("#menu").width() - 1);
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
            <img src="/img/busvision.png" height="40px" id="logo_topo" />
        </div>
        <div class="esquerda" id="menu">
            <ul>
                <li><a href="FrmEstacoes.aspx">Estações</a></li>
                <li><a href="Default.aspx">Voltar</a></li>
            </ul>
        </div>
        <div class="direita" id="conteudo">
            <h1>Editar Estação</h1>
            <h2>Estação <asp:Label runat="server" ID="lblEstacao"></asp:Label></h2>
            <asp:Panel runat="server" ID="pnlMensagem">
                <asp:Label runat="server" ID="lblMensagem"></asp:Label>
            </asp:Panel>
            <table class="form">
                <tr>
                    <td>Nome da máquina:</td>
                    <td><asp:TextBox runat="server" ID="txtNome" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Button runat="server" ID="btnEditar" CssClass="btn" Text="Editar" OnClick="btnEditar_Click"/></td>
                </tr>
            </table>
        </div>
    </div>
    <div id="download-list"></div>
    </form>
</body>
</html>
