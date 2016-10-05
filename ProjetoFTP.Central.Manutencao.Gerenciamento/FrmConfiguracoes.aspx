<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmConfiguracoes.aspx.cs" Inherits="ProjetoFTP.Web.FrmConfiguracoes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="scripts/alert.js" type="text/javascript"></script>
    <script src="scripts/jquery.min.js" type="text/javascript"></script>
    <link href="estilos/rsAlert.css" rel="stylesheet" type="text/css" />
    <link href="estilos/configuracoes.css" rel="stylesheet" type="text/css" />
    <script src="scripts/download.js" type="text/javascript"></script>
    <link href="estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.ui.core.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.mouse.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.resizable.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.draggable.js" type="text/javascript"></script>
    <script>
        $(document).ready(function () {
            $("#menu").height($(document).height() - $(".topo").height() - 21);
            $(".pagina #conteudo").width($(document).width() - $("#menu").width() - 1);
            download();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pagina">
        <div class="topo">
            <img src="/img/busvision.png" height="50px" style="margin : 0px 10px;" />
        </div>
        <div class="esquerda" id="menu">
            <ul>
                <li><a href="FrmConfiguracoes.aspx">Configurações</a></li>
                <li><a href="Default.aspx">Voltar</a></li>
            </ul>
        </div>
        <div class="direita" id="conteudo">
            <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
            <asp:UpdatePanel runat="server" ID="updPanel">
                <ContentTemplate>
                    <h1>Configuração</h1>
                    <table class="form">
                        <tr>
                            <td>Perído de sleep entre as verificações em minutos:</td>
                            <td><asp:TextBox runat="server" ID="txtSleep" CssClass="txt"></asp:TextBox></td>
                            <td><div class="box2"> Recomendado: 720min</div></td>
                        </tr>
                        <tr>
                            <td>Percentual de gravação considerável aceitavel:</td>
                            <td><asp:TextBox runat="server" ID="txtPercentAceitavel" CssClass="txt"></asp:TextBox></td>
                            <td><div class="box2"> Recomendado: 90%</div></td>
                        </tr>
                        <tr>
                            <td>Percentual minimo de gravação considerável preocupante:</td>
                            <td><asp:TextBox runat="server" ID="txtPercentPreocupante" CssClass="txt"></asp:TextBox></td>
                            <td><div class="box2"> Recomendado: 50%</div></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td colspan="2"><asp:Button runat="server" ID="btnSalvar" Text="Salvar" CssClass="btn" OnClick="btnSalvar_Click"/></td>
                        </tr>
                    </table>
                    <hr />
                    <h1>Ajustes do Sistema</h1>
                    <asp:Label runat="server" ID="lblStatus" CssClass="positiva" Text="O sistema está ligado!" Width="250px"></asp:Label><br />
                    <asp:Button runat="server" ID="btnTrocaStatus" CssClass="btn btnStatus" Text="Desligar Sistema"/>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div id="download-list"></div>
    </form>
</body>
</html>
