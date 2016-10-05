<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmAdicionaEmail.aspx.cs" Inherits="ProjetoFTP.Web.FrmAdicionaEmail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="scripts/jquery.min.js" type="text/javascript"></script>
    <link href="estilos/rsAlert.css" rel="stylesheet" type="text/css" />
    <script src="scripts/alert.js" type="text/javascript"></script>
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
    <div class="pagina">
        <div class="topo">
            <img src="/img/busvision.png" height="40px" id="logo_topo" />
        </div>
        <div id="menu" class="esquerda">
            <ul>
                <li><a href="FrmEmails.aspx">Emails</a></li>
                <li><a href="FrmAdicionaEmail.aspx">Adicionar Email</a></li>
                <li><a href="Default.aspx">Voltar</a></li>
            </ul>
        </div>
        <div id="conteudo" class="esquerda">
            <h1>Adicionar Email</h1>
            <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
            <asp:UpdatePanel runat="server" ID="updEmails">
                <ContentTemplate>
                    <table class="form">
                        <tr>
                            <td>Email:</td>
                            <td><asp:TextBox runat="server" ID="txtEmail" CssClass=" txt"></asp:TextBox></td>
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
    </form>
</body>
</html>
