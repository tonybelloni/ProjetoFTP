<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmRelatorioTransferencias.aspx.cs" Inherits="ProjetoFTP.Web.FrmRelatorioTransferencias" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="estilos/style.css" rel="stylesheet" type="text/css" />
    <link href="estilos/relatorio_transferencia.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <script src="scripts/ScrollableGrid.js" type="text/javascript"></script>
    <script src="/scripts/box.js" type="text/javascript"></script>
    <link href="/estilos/box.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/mask.js" type="text/javascript"></script>
    <script src="/scripts/relatoriodetalhado.js" type="text/javascript"></script>
    <script src="/scripts/download.js" type="text/javascript"></script>
    <link href="/estilos/downloads.css" rel="stylesheet" type="text/css" />
    <title></title>
    <script>
        $(document).ready(function () {
            RelatorioCopiaClick();
            download();
            var height = $(window).height() - 260;
            $("#<%=gvRelatorio.ClientID %>").Scrollable({ ScrollHeight: height });
            $(".box").box({ icon: "/img/busca.png",
                title: "Busca Avançada"
            });

            $(".box2").box({ icon: "/img/email.png",
                title: "Enviar E-mail"
            });
            $(".data").mask("99/99/9999");
        });
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
                    <a id="action_send_email" class="box2" href="#email_box"><img src="/img/email.png"/></a>
                </div>
            </div>
            <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
            <div id="content">
                <div id="content_header">
                <h1 id="content_title">Transferências</h1>
                    <div id="content_header_options">
                        <h1><asp:Label runat="server" ID="lblPaginas" Text="1 de 10" CssClass="lblPaginas"></asp:Label></h1>
                        <div class="seta_paginacao">
                            <asp:ImageButton runat="server" ID="btnAnterior" ImageUrl="/img/seta_esquerda.png" OnClick="btnAnterior_Click" Width="32"/>
                            <asp:ImageButton runat="server" ID="btnProximo"  ImageUrl="/img/seta_direita.png" OnClick="btnProximo_Click" Width="32"/>
                        </div>
                    </div>
                </div>
                <div id="content_main">
                    <div id="relatorio_header">
                    </div>
                    <div class="relatorio">
                        <asp:UpdatePanel runat="server" ID="updPanel">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnFiltrar"/>
                            </Triggers>
                            <ContentTemplate>
                                <script type="text/javascript">
                                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                                    prm.add_endRequest(function () {
                                        $(document).ready(function () {
                                            RelatorioCopiaClick();
                                            var height = $(window).height() - 260;
                                            $("#<%=gvRelatorio.ClientID %>").Scrollable({ ScrollHeight: height });

                                            $(".data").mask("99/99/9999");
                                        });
                                    });
                                </script>
                        <asp:GridView runat="server" ID="gvRelatorio" AutoGenerateColumns="false" AllowSorting="true" Width="100%" GridLines="None"
                                onrowdatabound="gvRelatorios_RowDataBound" PageSize="100" OnRowCommand="gvRelatorios_RowCommand" AllowPaging="true" 
                                ShowHeader="true" ShowFooter="false" CssClass="grid copias_rel">
                            <PagerStyle CssClass="paging_hidden" HorizontalAlign="Right" />  
                            <HeaderStyle CssClass="fixed_header" />
                            <Columns>
                                <asp:TemplateField ItemStyle-Width="28px">
                                    <ItemTemplate>
                                        <a class="relatorio_icon detalhes">
                                            <img id="<%# Eval("ID") %>" src="/img/detalhes.png" width="28px">
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:ButtonField ButtonType="Image" ImageUrl="/img/download.png" CommandName="Copiar" ControlStyle-Width="28px" ItemStyle-CssClass="relatorio_icon" ItemStyle-Width="28px"/>
                                <asp:BoundField HeaderText="Carro" DataField="NumeroCarro" ItemStyle-CssClass="id_veiculo" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField HeaderText="Estação" DataField="Estacao" ItemStyle-CssClass="estacao" ItemStyle-HorizontalAlign="Center"/>
                                <asp:BoundField HeaderText="Inicio da cópia" DataField="DataInicial" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="data_inicial" DataFormatString="{0:dd/MM/yy HH:mm}" ItemStyle-Width="100px" />
                                <asp:BoundField HeaderText="Fim da cópia" DataField="DataFinal" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="data_final" DataFormatString="{0:dd/MM/yy HH:mm}" ItemStyle-Width="100px" />
                                <asp:BoundField HeaderText="Duração" DataField="Intervalo" DataFormatString="{0:0.0} min" ItemStyle-HorizontalAlign="Right" ItemStyle-CssClass="intervalo_tempo" ItemStyle-Width="60px" />
                                <asp:BoundField HeaderText="Arquivos validos" DataField="QuantidadeArquivosValidos" ItemStyle-HorizontalAlign="Right" ItemStyle-CssClass="arquivos_validos" HeaderStyle-HorizontalAlign="Right" />
                                <asp:BoundField HeaderText="Arquivos copiados" DataField="QuantidadeArquivosCopiados" ItemStyle-HorizontalAlign="Right" ItemStyle-CssClass="arquivos_copiados" />
                                <asp:BoundField HeaderText="Volume total" DataField="VolumeArquivosTotal" DataFormatString="{0:0} MB" ItemStyle-HorizontalAlign="Right" ItemStyle-CssClass="volume_total"  />
                                <asp:BoundField HeaderText="Volume copiados" DataField="VolumeArquivosCopiados" DataFormatString="{0:0} MB" ItemStyle-HorizontalAlign="Right" ItemStyle-CssClass="volume_copiados" />
                                <asp:BoundField HeaderText="Velocidade média" DataField="VelocidadeMedia" DataFormatString="{0:0} MB/min" ItemStyle-HorizontalAlign="Right" ItemStyle-CssClass="velocidade_media" />
                                <asp:BoundField HeaderText="Filtro" DataField="TipoCopia" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="codigo"/>
                            </Columns>
                        </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    <asp:UpdateProgress runat="server" ID="updProgressRelatorio" AssociatedUpdatePanelID="updPanel">
                        <ProgressTemplate>
                            <div id="loader">
                                <img src="/img/preloader.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    </div>
                </div>
                <div id="content_footer">
                
                </div>
            </div>
        </div>
        <div id="search_box">
            <table>
                <tr>
                    <td>Número do Veículo:</td>
                    <td><asp:TextBox runat="server" ID="txtNumero" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Data Inicial:</td>
                    <td><asp:TextBox runat="server" ID="txtDataInicial" CssClass="txt data"></asp:TextBox></td>
                    <td><img src="/img/calendario.png" width="28px" /></td>
                </tr>
                <tr>
                    <td>Data Final:</td>
                    <td><asp:TextBox runat="server" ID="txtDataFinal" CssClass="txt data"></asp:TextBox></td>
                    <td><img src="/img/calendario.png" width="28px" /></td>
                </tr>
                <tr>
                    <td>Velocidade mínima(MB/min):</td>
                    <td><asp:TextBox runat="server" ID="txtVelMediaMin" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Velocidade máxima(MB/min):</td>
                    <td><asp:TextBox runat="server" ID="txtVelMediaMax" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Duração mínima(min):</td>
                    <td><asp:TextBox runat="server" ID="txtDurMin" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Duração máxima(min):</td>
                    <td><asp:TextBox runat="server" ID="txtDurMax" CssClass="txt"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Estação: </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlEstacao" CssClass="ddl"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Tipo de Cópia:</td>
                    <td><asp:DropDownList runat="server" ID="ddlTipo" CssClass="ddl">
                                <asp:ListItem Text="Todas" Value="-999" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Guia" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="Ultimo Arq" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Completa" Value="1"></asp:ListItem>
                        </asp:DropDownList></td>
                </tr>
            </table>
            <asp:Button runat="server" ID="btnFiltrar" CssClass="btn" Text="Buscar" OnClick="btnFiltrar_Click" OnClientClick="showHideBox('#search_box');"/>
        </div>
        <div id="email_box">
            <asp:UpdatePanel runat="server" ID="UpdatePanelEmails">
                <ContentTemplate>
                    <asp:TextBox runat="server" ID="txtAdicionarEmail" CssClass="txt" Width="290px"></asp:TextBox><asp:Button runat="server" ID="btnAdicionarEmail" CssClass="btn" Text="Adicionar e-mail" OnClick="btnAdicionarEmail_Click"/>
                    <asp:GridView runat="server" ID="gvEmails" CssClass="grid" OnRowCommand="gvEmails_RowCommand" GridLines="None" AutoGenerateColumns="false">
                        <Columns>
                            <asp:ButtonField ButtonType="Image" ImageUrl="/img/enviar.png" CommandName="Enviar" ControlStyle-Width="32px"/>
                            <asp:BoundField DataField="Endereco" ItemStyle-Width="370px"/>
                            <asp:ButtonField ButtonType="Image" ImageUrl="/img/fechar2.png" CommandName="remover" ControlStyle-Width="32px" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdateProgress runat="server" ID="UpdateProgressEmails" AssociatedUpdatePanelID="UpdatePanelEmails">
                <ProgressTemplate>
                    <div id="loader">
                        <img src="/img/preloader.gif"/>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
        <div id="download-list"></div>
    </form>
</body>
</html>
