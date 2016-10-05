<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmEmails.aspx.cs" Inherits="ProjetoFTP.Web.FrmEmails" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.ui.core.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.mouse.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.resizable.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.draggable.js" type="text/javascript"></script>
    <script src="scripts/jquery.min.js" type="text/javascript"></script>
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
        <div class="esquerda" id="menu">
            <ul>
                <li><a href="FrmEmails.aspx">Emails</a></li>
                <li><a href="FrmAdicionaEmail.aspx">Adicionar Email</a></li>
                <li><a href="Default.aspx">Voltar</a></li>
            </ul>
        </div>
        <div class="direita" id="conteudo">
            <h1>Emails</h1>
            <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
            <asp:UpdateProgress runat="server" ID="updProgressEmails" AssociatedUpdatePanelID="updEmails">
                <ProgressTemplate>
                    <div class="update">
                        <div class="upd_desc">Carregando...</div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <asp:UpdatePanel runat="server" ID="updEmails">
                <ContentTemplate>
                    <asp:TextBox runat="server" ID="txtProcurar" CssClass="txt" style="margin-left : 10px;"></asp:TextBox>
                    <asp:Button runat="server" ID="btnProcurar" CssClass="btn" Text="Procurar" OnClick="btnProcurar_Click" />
                    <asp:GridView runat="server" ID="gvEmails" AutoGenerateColumns="false" GridLines="None" CssClass="grid"
                         Width="100%" OnRowCommand="gvEmails_RowCommand" ShowHeader="false">
                        <Columns>
                            <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn" Text="Remover" ItemStyle-Width="120px" CommandName="Remover"/>
                            <asp:BoundField DataField="Endereco" ItemStyle-HorizontalAlign="Left" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    </form>
</body>
</html>
