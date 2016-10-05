<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmLogs.aspx.cs" Inherits="ProjetoFTP.Central.Manutencao.Gerenciamento.FrmLogs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="scripts/jquery-1.7.2.js" type="text/javascript"></script>
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
    <style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="pagina">
        <div class="topo">
            <img src="/img/busvision.png" height="50px" style="margin : 0px 10px;" />
        </div>
        <div class="esquerda" id="menu">
            <ul>
                <li><a href="FrmLogs.aspx">Logs</a></li>
                <li><a href="Default.aspx">Voltar</a></li>
            </ul>
        </div>
        <div class="esquerda" id="conteudo">
            <h1>Logs</h1>
            <asp:GridView runat="server" ID="gvLogs" CssClass="grid cabecalho" GridLines="None" Width="100%" AllowPaging="true" PageSize="20">
                <Columns>
                    <asp:BoundField HeaderText="Data" />
                    <asp:BoundField HeaderText="Horário" />
                    <asp:BoundField HeaderText="Tag" />
                    <asp:BoundField HeaderText="Mensagem" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    </form>
</body>
</html>
