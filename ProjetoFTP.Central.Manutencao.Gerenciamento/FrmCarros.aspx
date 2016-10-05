<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmCarros.aspx.cs" Inherits="ProjetoFTP.Web.FrmCarros" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="scripts/jquery.min.js" type="text/javascript"></script>
    <script src="http://code.highcharts.com/highcharts.js"></script>
    <script src="scripts/download.js" type="text/javascript"></script>
    <link href="estilos/downloads.css" rel="stylesheet" type="text/css" />
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
    <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
    <div class="pagina">
        <div class="topo">
            <img src="/img/busvision.png" height="40px" id="logo_topo" />
        </div>
        <div class="esquerda" id="menu">
            <ul>
                <li><a href="FrmCarros.aspx">Carros</a></li>
                <li><a href="FrmAdicionaCarro.aspx">Adicionar Carro</a></li>
                <li><a href="Default.aspx">Voltar</a></li>
            </ul>
        </div>
        <div class="direita" id="conteudo">
            <asp:UpdateProgress runat="server" ID="updProgress" AssociatedUpdatePanelID="updCarros">
                <ProgressTemplate>
                    <div class="update mini">
                        <div class="upd_desc"><img src="/img/pl.gif" /></div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <asp:UpdatePanel runat="server" ID="updCarros">
                <ContentTemplate>
                    <h1>Carros</h1>
                    <asp:TextBox runat="server" ID="txtProcuraCarro" CssClass="txt" Width="200px" style="margin-left : 10px;"></asp:TextBox>
                    <asp:Button runat="server" ID="btnProcurar" CssClass="btn" Text="Pesquisar" OnClick="btnProcurar_Click"/>
                    <asp:Button runat="server" ID="btnAdicionaCarro" CssClass="btn" PostBackUrl="FrmAdicionaCarro.aspx" Text="Adicionar novo carro" />
                    <asp:GridView runat="server" ID="gvCarros" CssClass="grid cabecalho" Width="100%" 
                    AutoGenerateColumns="false" OnPageIndexChanging="gvCarros_PageIndexChanging" AllowSorting="true" AllowPaging="true" PageSize="15" PagerStyle-HorizontalAlign="Center"
                    GridLines="None" OnRowDataBound="gvCarros_RowDataBound"
                    onRowCommand="gvCarros_RowCommand">
                        <Columns>
                            <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn" Text="Editar" CommandName="Edit" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center"/>
                            <asp:ButtonField ButtonType="Button" Text="Forçar Cópia" CommandName="Copiar" ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundField HeaderText="NUMERO DO CARRO" DataField="ID_CARRO" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField HeaderText="IP" DataField="IP"  ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundField HeaderText="USUÁRIO" DataField="USUARIO"  ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundField HeaderText="SENHA" DataField="SENHA"  ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundField HeaderText="ULTIMA CÓPIA" DataField="ULTIMA_COPIA"  ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundField HeaderText="ULTIMO ARQUIVO COPIADO" DataField="ULTIMO_ARQUIVO_COPIADO"  ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundField HeaderText="VERIFICADO" DataField="VERIFICADO"  ItemStyle-HorizontalAlign="Center"/>
                            <asp:BoundField HeaderText="ULTIMA VERIFICAÇÃO" DataField="ULTIMA_VERIFICACAO"  ItemStyle-HorizontalAlign="Center"/>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div id="download-list"></div>
    </form>
</body>
</html>
