<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmVeiculos.aspx.cs" Inherits="ProjetoFTP.Web.FrmVeiculos" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/estilos/style.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <script src="scripts/ScrollableGrid.js" type="text/javascript"></script>
    <script src="/scripts/box.js" type="text/javascript"></script>
    <link href="/estilos/box.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/download.js" type="text/javascript"></script>
    <link href="/estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/watermark.js" type="text/javascript"></script>
    <script>
        $(document).ready(function () {
            var height = $(window).height() - 220;
            $("#<%=gvCarros.ClientID %>").Scrollable({ ScrollHeight: height });
            download();
            $(".box").box({ icon: "/img/busca.png",
                title: "Buscar Carro",
                height: "auto",
                onClose: function () {
                    $("#search_box .txt").val("").blur();
                }
            });

            $(".box2").box({ icon: "/img/mais.png",
                title: "Adicionar Carro",
                width: "auto",
                onClose: function () {
                    $("#aviso").hide();
                    $("#add_box .txt").val("");
                }
            });

            $(".box3").box({ title: "Editar Carro", icon: "/img/editar.png", width: "auto", height: "auto" });

            $(".pesquisa").watermark("Número do carro");

            $("#btnCancelarAddCarro").click(function () {
                showHideBox("#add_box");
                $("#add_box .txt").val("").blur();
            });


            $(".numero_add").blur(function () {
                if ($(".numero_add").val().trim() != "") {
                    var number = $(".numero_add").val();
                    var tokens = [];
                    tokens[0] = "10";
                    tokens[1] = number.substr(0, 2);
                    if (number.substr(3) > 255) {
                        tokens[2] = number.substr(2, 2);
                        tokens[3] = number.substr(4);
                    }
                    else {
                        tokens[2] = number.substr(2, 1);
                        tokens[3] = number.substr(3);
                    }
                    var addr = "";
                    for (var i = 0; i < tokens.length; i++) {
                        addr += tokens[i];
                        if (i < tokens.length - 1)
                            addr += ".";
                    }
                    $(".ip").val(addr);
                }
            });
        });

        function showAlert(element, title, content, theme) {
            var e = $(element);
            if (theme == 'good_thing') {
                e.addClass('good_thing');
            }
            else if (theme == 'bad_thing') {
                e.addClass('bad_thing');
            }
            e.delay(1000).slideUp("medium", function () { $(this).html("<h1>" + title + "</h1><div>" + content + "</div>").slideDown("slow"); }); 
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    
    <div id="viewport">
        <div id="page_bar" runat="server">
                
        </div>
        <div id="action_bar">
            <a id="action_bar_anchor" href="/principal"><img id="action_bar_icon" src="/img/busvision.png" /></a>
            <div id="action_bar_intent_actions">
                <a id="action_search" class="box" href="#search_box"><img src="/img/busca.png"/></a>
                <a id="action_new" class="box2" href="#add_box"><img src="/img/mais.png"/></a>
                <a id="action_edit" class="box3" href="#edit_box"><img src="/img/editar.png"/></a>
            </div>
        </div>
    <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
    
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnBuscarCarro" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnAdicionarCarro" EventName="Click" />
        </Triggers>
        <ContentTemplate>
        <script>
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            var postBackElement;

            prm.add_initializeRequest(InitializeRequest);
            prm.add_endRequest(EndRequest);

            function InitializeRequest(sender, args) {
                if (prm.get_isInAsyncPostBack()) {
                    args.set_cancel(true);
                }
                postBackElement = args.get_postBackElement();
                if (postBackElement.id == 'btnAdicionarCarro' || postBackElement.id == 'btnBuscarCarro') {
                    $get('updProgressRelatorio').style.display = 'block';
                }
            }
            function EndRequest(sender, args) {

                $(document).ready(function () {
                    var height = $(window).height() - 220;
                    $("#<%=gvCarros.ClientID %>").Scrollable({ ScrollHeight: height });
                });

                if (postBackElement.id == 'btnAdicionarCarro' || postBackElement.id == 'btnBuscarCarro') {
                    $get('updProgressRelatorio').style.display = 'none';
                }
            }

    </script>
        <div id="content">
            <div id="content_header">
                <span><img id="content_icon" src="/img/grid_icon.png"/><h1 id="content_title">Carros</h1></span>
                <div id="content_header_options">
                    <h1><asp:Label runat="server" ID="lblPaginas" Text="1 de 10" CssClass="lblPaginas"></asp:Label></h1>
                    <div class="seta_paginacao">
                        <asp:ImageButton runat="server" ID="btnAnterior" ImageUrl="/img/seta_esquerda.png" OnClick="btnAnterior_Click" Width="32"/>
                        <asp:ImageButton runat="server" ID="btnProximo"  ImageUrl="/img/seta_direita.png" OnClick="btnProximo_Click" Width="32"/>
                    </div>
                </div>
            </div>
            <div id="content_main">
                <asp:GridView runat="server" ID="gvCarros" CssClass="grid" AutoGenerateColumns="false" Width="100%" GridLines="None" OnRowCommand="gvCarros_RowCommand" AllowPaging="true" PageSize="100" OnRowDataBound="gvCarros_RowDataBound">
                    <PagerStyle CssClass="paging_hidden" HorizontalAlign="Right" />  
                    <AlternatingRowStyle BackColor="#EBEBEB" />
                    <RowStyle BackColor="#E3E3E3" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbEditar" CommandName="Editar" CommandArgument='<%# Container.DataItemIndex %>'><img src="/img/editar.png" width="28px" /></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Image" ImageUrl="/img/download.png" CommandName="Copiar" ControlStyle-Width="28px" ItemStyle-Width="28px" ItemStyle-HorizontalAlign="Center"/>
                        <asp:BoundField HeaderText="Número do Carro" DataField="ID_CARRO" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField HeaderText="IP" DataField="IP"  ItemStyle-HorizontalAlign="Center"/>
                        <asp:BoundField HeaderText="Última Cópia" DataField="ULTIMA_COPIA"  ItemStyle-HorizontalAlign="Center"/>
                        <asp:BoundField HeaderText="Último Arquivo" DataField="ULTIMO_ARQUIVO_COPIADO"  ItemStyle-HorizontalAlign="Center"/>
                        <asp:BoundField HeaderText="Última Verificação" DataField="ULTIMA_VERIFICACAO"  ItemStyle-HorizontalAlign="Center"/>
                        <asp:BoundField HeaderText="Ativo" DataField="ATIVO" ItemStyle-HorizontalAlign="Center"/>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress runat="server" ID="updProgressRelatorio" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
                <div id="loader">
                    <img src="/img/preloader.gif" />
                </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
        <div id="add_box">
            <div id="aviso"></div>
            <table>
                <tr>
                    <td>Número do Carro: </td>
                    <td><asp:TextBox runat="server" ID="txtNumeroCarroAdd" CssClass="txt numero_add"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Ip: </td>
                    <td><asp:TextBox runat="server" ID="txtIp" CssClass="txt ip"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Usuário: </td>
                    <td><asp:TextBox runat="server" ID="txtUsuario" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Senha: </td>
                    <td><asp:TextBox runat="server" ID="txtSenha" CssClass="txt" TextMode="Password"></asp:TextBox></td>
                    <td><img src="/img/cadeado.png" width="28px" /></td>
                </tr>
                <tr>
                    <td>Repetir Senha: </td>
                    <td><asp:TextBox runat="server" ID="txtRepetirSenha" CssClass="txt" TextMode="Password"></asp:TextBox></td>
                    <td><img src="/img/cadeado.png" width="28px" /></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button runat="server" ID="btnAdicionarCarro" CssClass="btn" Text="Adiconar" OnClick="btnAdicionarCarro_Click" />
                        <asp:Button runat="server" ID="btnCancelarAddCarro" CssClass="btn" Text="Cancelar" OnClick="btnCancelarAddCarro_Click" />
                    </td>
                    <td></td>
                </tr>
            </table>
        </div>

        <div id="edit_box">
            <asp:UpdatePanel runat="server" ID="updEditar">
                <ContentTemplate>
            <table width="380px">
            <tr>
                <td>Número: </td>
                <td><asp:TextBox runat="server" ID="txtAltNumero" CssClass="txt" Enabled="false"></asp:TextBox></td>
            </tr>
            <tr>
                <td>IP: </td>
                <td><asp:TextBox runat="server" ID="txtAltIp" CssClass="txt"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Usuário: </td>
                <td><asp:TextBox runat="server" ID="txtAltUsuario" CssClass="txt"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Senha: </td>
                <td><asp:TextBox runat="server" ID="txtAltSenha" CssClass="txt" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Senha: </td>
                <td><asp:TextBox runat="server" ID="txtAltRepetirSenha" CssClass="txt" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                 <td>Status:</td>
                 <td><asp:DropDownList runat="server" ID="ddlStatusEditar" CssClass="ddl">
                        <asp:ListItem Text="Ativo" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Inativo" Value="0"></asp:ListItem>
                    </asp:DropDownList></td>
                </tr>
            <tr>
                <td></td>
                <td><asp:Button runat="server" ID="Button1" CssClass="btn" Text="Editar" OnClick="btnEditar_Click"/></td>
            </tr>
        </table>
                         </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div id="search_box">
            <asp:TextBox runat="server" ID="txtNumeroCarro" CssClass="txt pesquisa" Width="350px"></asp:TextBox>
            <asp:Button runat="server" ID="btnBuscarCarro" CssClass="btn" Text="Buscar" OnClick="btnBuscarCarro_Click" OnClientClick="showHideBox('#search_box');"/>
        </div>
        <div id="download-list"></div>

    </div>
    
    </form>
</body>
</html>
