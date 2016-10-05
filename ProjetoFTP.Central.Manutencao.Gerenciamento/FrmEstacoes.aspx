<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmEstacoes.aspx.cs" Inherits="ProjetoFTP.Web.FrmEstacoes" %>

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
                <li><a href="FrmAdicionaEstacao.aspx">Adicionar Estação</a></li>
                <li><a href="Default.aspx">Voltar</a></li>
            </ul>
        </div>
        <div class="direita" id="conteudo">
            <asp:UpdatePanel runat="server" ID="updEstacoes" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="txtPesquisar" />
                    <asp:AsyncPostBackTrigger ControlID="gvEstacoes" />
                </Triggers>
                <ContentTemplate>
                
                    <img id="page_icon" src="/img/estacoes_icon.png" /><h1>Estações</h1>
                    <asp:TextBox runat="server" ID="txtPesquisar" CssClass="txt" Width="200px"  style="margin-left : 10px;" ></asp:TextBox>
                    <asp:Button runat="server" ID="btnPesquisar" CssClass="btn" Text="Pesquisar" OnClick="btnPesquisar_Click"/>
                    <asp:Button runat="server" ID="btnAdicionar" CssClass="btn" PostBackUrl="FrmAdicionaEstacao.aspx" Text="Adiconar nova estação" /><br /><br />
                    <asp:GridView runat="server" ID="gvEstacoes" CssClass="grid cabecalho" AutoGenerateColumns="false" Width="100%"
                            AllowSorting="true" AllowPaging="true" PageSize="20" PagerStyle-HorizontalAlign="Center" OnRowDataBound="gvEstacoes_RowCreated"
                            GridLines="None" AlternatingRowStyle-BackColor="#cccccc" OnRowCommand="gvEstacoes_RowCommand">
                        <Columns>
                            <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn" Text="Ativar / Desativar" CommandName="Habilitar" ItemStyle-Width="60px"/>
                            <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn" Text="Editar" CommandName="Edit" ItemStyle-Width="60px"/>
                            <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn vermelho" Text="Deletar" CommandName="Delete" ItemStyle-Width="60px"/>
                            <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn verde" Text="Testar conexão" CommandName="Testar" ItemStyle-Width="70px"/>
                            <asp:BoundField HeaderText="NOME DA MÁQUINA" DataField="NOME_MAQUINA"/>
                            <asp:BoundField HeaderText="IP" DataField="IP" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Panel runat="server" ID="pnlAtiva" style="padding:3px 5px;" Width="100px">
                                        <asp:Label runat="server" ID="lblAtiva" Text='<%# Eval("HABILITADA") %>'></asp:Label>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>
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
