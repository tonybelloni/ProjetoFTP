<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmLogin.aspx.cs" Inherits="ProjetoFTP.Web.FrmLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/estilos/style.css" rel="stylesheet" type="text/css" />
    <link href="/estilos/default.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <script src="/scripts/box.js" type="text/javascript"></script>
    <link href="/estilos/box.css" rel="stylesheet" type="text/css" />
    <link rel="shortcut icon" type="image/x-icon" href="/favicon.ico" />
    <link rel="icon" type="image/x-icon" href="/favicon.ico" />
    <link href="/estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/download.js" type="text/javascript"></script>
    <style>
    </style>
    <script>
        $(document).ready(function () {
            localStorage.setItem(TAG_DOWNLOAD_TAB_OPEN, true);
            download();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
    <div class="pagina">
    <div id="viewport">
        <div id="page_bar" runat="server"></div>
        <div id="content">
            <div id="content_header">
                <a href="http://www.busvision.com.br" target="_blank"><img src="/img/busvision.png" height="60px"/></a>
            </div>
            <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                <ContentTemplate>
                    <div id="content_main" align="center">
                        <div style=" width : 600px;">
                            <img src="/img/logo_rioservice2.png" height="320px" style="float : left;"/>
                            <table style="float : right; margin-top : 80px;">
                                <tr>
                                    <td><img src="/img/usuario2.png" height="36"/></td>
                                    <td><asp:TextBox runat="server" ID="txtUsuario" CssClass="txt dark" Width="200px"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td><img src="img/cadeado2.png" height="36"/></td>
                                    <td><asp:TextBox runat="server" ID="txtSenha" CssClass="txt dark" TextMode="Password" Width="200px"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td><asp:Button runat="server" ID="btnLogar" CssClass="btn" Text="Entrar" OnClick="btnLogar_Click" Width="226px"></asp:Button></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td><asp:Button runat="server" ID="btnPainel" CssClass="btn" Text="Ver Painel" PostBackUrl="/painel" Width="226px"></asp:Button></td>
                                </tr>
                            </table>
                            <div style="clear : both;"></div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    </form>
    <div id="download-list"></div>
</body>
</html>
