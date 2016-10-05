<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmAdicionaCarro.aspx.cs" Inherits="ProjetoFTP.Web.FrmAdicionaCarro" %>

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
                <li><a href="FrmCarros.aspx">Carros</a></li>
                <li><a href="FrmAdicionaCarro.aspx">Adicionar Carro</a></li>
                <li><a href="FrmCarros.aspx">Voltar</a></li>
            </ul>
        </div>
        <div class="direita" id="conteudo">
            <h1>Adicionar Carro</h1>
            <asp:UpdatePanel runat="server" ID="updAdiciona">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlMensagem" Visible="false">
                        <asp:Label runat="server" ID="lblMensagem"></asp:Label>
                    </asp:Panel>
                    <table class="form">
                        <tr>
                            <td>Número do carro:</td>
                            <td><asp:TextBox runat="server" ID="txtNumero" CssClass=" txt"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>IP:</td>
                            <td><asp:TextBox runat="server" ID="txtIp" CssClass="txt"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>Usuário:</td>
                            <td><asp:TextBox runat="server" ID="txtUsuario" CssClass="txt"></asp:TextBox></td>
                        </tr>
                        <tr>
                        <tr>
                            <td>Senha:</td>
                            <td><asp:TextBox runat="server" ID="txtSenha" CssClass="txt"></asp:TextBox></td>
                        </tr>
                            <td>Confirmar senha:</td>
                            <td><asp:TextBox runat="server" ID="txtSenha2" CssClass="txt"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><asp:Button runat="server" ID="btnAdicionar" CssClass="btn" Text="Adicionar" OnClick="btnAdicionar_Click"/></td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div id="download-list"></div>
    </form>
</body>
</html>
