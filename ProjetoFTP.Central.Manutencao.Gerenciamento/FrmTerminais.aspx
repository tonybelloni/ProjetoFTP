<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmTerminais.aspx.cs" Inherits="ProjetoFTP.Web.FrmTerminais" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/style.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <script src="scripts/ScrollableGrid.js" type="text/javascript"></script>
    <script src="/scripts/box.js" type="text/javascript"></script>
    <link href="/estilos/box.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/download.js" type="text/javascript"></script>
    <link href="/estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/watermark.js" type="text/javascript"></script>
    <script>
        $(document).ready(function () {
            //$(".box").box({ title: "Buscar Estação", icon: "/img/busca.png", width: "auto", height: "auto" });
            $(".box2").box({ title: "Adicionar Estação", icon: "/img/mais.png", width: "auto", height: "auto" });
            $(".box3").box({ title: "Editar Estação", icon: "/img/editar.png", width: "auto", height: "auto" });
            download();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
    <div id="viewport">
        <div id="page_bar" runat="server"></div>
        <div id="action_bar">
            <a id="action_bar_anchor" href="Default.aspx"><img id="action_bar_icon" src="/img/busvision.png" /></a>
            <div id="action_bar_intent_actions">
                <!--<a id="action_search" class="box" href="#search_box"><img src="/img/busca.png"/></a>-->
                <a id="action_new" class="box2" href="#add_box"><img src="/img/mais.png"/></a>
                <a id="a1" class="box3" href="#edit_box" style="display: none;"><img src="/img/mais.png"/></a>
            </div>
        </div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <Triggers>
            </Triggers>
            <ContentTemplate>
        <div id="content">
            <div id="contet_header">
                <span><img id="content_icon" src="/img/estacoes_icon.png"/><h1 id="content_title">Estações</h1></span>
                <div id="content_header_options">
                    <h1><asp:Label runat="server" ID="lblPaginas" Text="1 de 10" CssClass="lblPaginas"></asp:Label></h1>
                    <div class="seta_paginacao">
                        <asp:ImageButton runat="server" ID="btnAnterior" ImageUrl="/img/seta_esquerda.png" OnClick="btnAnterior_Click" Width="32"/>
                        <asp:ImageButton runat="server" ID="btnProximo"  ImageUrl="/img/seta_direita.png" OnClick="btnProximo_Click" Width="32"/>
                    </div>
                </div>
            </div>
            <div id="content_main">
               <asp:GridView runat="server" ID="gvEstacoes" CssClass="grid" OnRowCommand="gvEstacoes_RowCommand" AllowPaging="true" PageSize="100" Width="100%" GridLines="None" AutoGenerateColumns="false" OnRowDataBound="gvEstacoes_RowDataBound">
                    <PagerStyle CssClass="paging_hidden" HorizontalAlign="Right" />  
                    <AlternatingRowStyle BackColor="#EBEBEB" />
                    <RowStyle BackColor="#E3E3E3" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbEditar" CommandName="Editar" CommandArgument='<%# Container.DataItemIndex %>'><img src="/img/editar.png" width="24px" /></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Nome da Máquina" DataField="NOME_MAQUINA" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Ip da Máquina" DataField="IP" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="Estado" DataField="HABILITADA" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
               </asp:GridView>
            </div>
        </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <div id="add_box">
            <table>
                <tr>
                    <td>Nome da estação: </td>
                    <td><asp:TextBox runat="server" ID="txtNomeMaquina" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Ip da máquina: </td>
                    <td><asp:TextBox runat="server" ID="txtIp" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlMaquinaAtiva" CssClass="ddl">
                            <asp:ListItem Selected="True" Text="Ativada" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Desativada" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button runat="server" ID="btnAdicionar" CssClass="btn" Text="Adicionar" OnClick="btnAdicionar_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="edit_box">
            <asp:UpdatePanel runat="server" ID="updEditar">
                <ContentTemplate>
                
            <table>
                <tr>
                    <td>Nome:</td>
                    <td><asp:TextBox runat="server" ID="txtNomeEditar" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>IP:</td>
                    <td><asp:TextBox runat="server" ID="txtIpEditar" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Status:</td>
                    <td><asp:DropDownList runat="server" ID="ddlStatusEditar" CssClass="ddl">
                        <asp:ListItem Text="Ativada" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Desativada" Value="0"></asp:ListItem>
                    </asp:DropDownList></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Button runat="server" ID="btnEditar" CssClass="btn" Text="Editar" OnClick="btnEditar_Click" /></td>
                </tr>
            </table>
            
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <!--
        <div id="search_box">
            <asp:TextBox runat="server" ID="txtBuscar" CssClass="txt" Width="350px"></asp:TextBox>
            <asp:Button runat="server" ID="btnBuscar" CssClass="btn" Text="Buscar" />
        </div>-->
        <div id="download-list"></div>
    </div>
    </form>
</body>
</html>
