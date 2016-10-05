<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmRelatorioVerificacoes.aspx.cs" Inherits="ProjetoFTP.Web.FrmRelatorioVerificacoes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="scripts/jquery.min.js" type="text/javascript"></script>
    <script src="scripts/download.js" type="text/javascript"></script>
    <link href="estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.ui.core.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.mouse.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.resizable.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.draggable.js" type="text/javascript"></script>
    <script>
        $(document).ready(function () {
            $("#menu").height($(window).height() - $(".topo").height() - 43);
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
                <li><a href="FrmRelatorios.aspx">Resumo do dia</a></li>
                <li><a href="FrmRelatorioCopias.aspx">Cópias</a></li>
                <li><a href="FrmRelatorioVerificacoes.aspx">Verificação</a></li>
                <li><a href="Default.aspx">Voltar</a></li>
            </ul>
        </div>
        <div class="esquerda" id="conteudo">
            <asp:UpdateProgress runat="server" ID="updProgress" AssociatedUpdatePanelID="updPanel">
                <ProgressTemplate>
                    <div class="update mini">
                        <div class="upd_desc">
                            <img src="/img/pl.gif" />
                        </div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <asp:UpdatePanel runat="server" ID="updPanel">
                <ContentTemplate>
                
            <h1>Relatório de Verificação</h1>
            <div>
                <table class="form esquerda pequeno">
                    <tr>
                        <td>Número do carro:</td>
                        <td><asp:TextBox runat="server" ID="txtNumero" CssClass="txt"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Data Inicial:</td>
                        <td><asp:TextBox runat="server" ID="txtDtInicial" CssClass="txt"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Data Final:</td>
                        <td><asp:TextBox runat="server" ID="txtDtFinal" CssClass="txt"></asp:TextBox></td>
                    </tr>
                </table>
                <table class="form esquerda pequeno">
                    <tr>
                        <td>Cam 1(%) mín:</td>
                        <td><asp:TextBox ID="txtC1Min" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                        <td>max:</td>
                        <td><asp:TextBox ID="txtC1Max" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Cam 2(%) mín:</td>
                        <td><asp:TextBox ID="txtC2Min" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                        <td>max:</td>
                        <td><asp:TextBox ID="txtC2Max" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Cam 3(%) mín:</td>
                        <td><asp:TextBox ID="txtC3Min" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                        <td>max:</td>
                        <td><asp:TextBox ID="txtC3Max" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                    </tr>
                </table>
                <table class="form esquerda pequeno">
                    <tr>
                        <td>Cam 4(%) mín:</td>
                        <td><asp:TextBox ID="txtC4Min" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                        <td>max:</td>
                        <td><asp:TextBox ID="txtC4Max" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Todas(%) mín:</td>
                        <td><asp:TextBox ID="txtCTodasMin" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                        <td>max:</td>
                        <td><asp:TextBox ID="txtCTodasMax" CssClass="txt" runat="server" Width="30px"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="4"><asp:Button runat="server" ID="btnFiltrar" CssClass="btn" Text="Filtrar" OnClick="btnFiltrar_Click"/></td>
                    </tr>
                </table>
            </div>
            <div style="clear : both;"></div>
                    <asp:GridView runat="server" ID="gvRelatorio" CssClass="grid cabecalho" Width="100%" PageSize="20" AllowPaging="true" 
                        AllowSorting="true" AutoGenerateColumns="false" GridLines="None" OnPageIndexChanging="gvRelatorios_PageIndexChanging" 
                        OnRowDataBound="gvRelatorios_RowDataBound">
                        <Columns>
                            <asp:BoundField HeaderText="Número do Carro" DataField="NumeroCarro" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Código do Equipamento" DataField="CodigoEquipamento" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Data da Verificação" DataField="DataVerificacao" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Câmera 1 (Quantidade de Arquivos)" DataField="QtCam1" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Câmera 1 (% - imagens/dia)" DataField="Cam1" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.0}%" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Câmera 2 (Quantidade de Arquivos)" DataField="QtCam2" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Câmera 2 (% - imagens/dia)" DataField="Cam2" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.0}%" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Câmera 3 (Quantidade de Arquivos)" DataField="QtCam3" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Câmera 3 (% - imagens/dia)" DataField="Cam3" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.0}%" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Câmera 4 (Quantidade de Arquivos)" DataField="QtCam4" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="100px"/>
                            <asp:BoundField HeaderText="Câmera 4 (% - imagens/dia)" DataField="Cam4" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.0}%" ItemStyle-Width="100px"/>
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
