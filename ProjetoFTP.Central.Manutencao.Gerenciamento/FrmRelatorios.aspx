<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmRelatorios.aspx.cs" Inherits="ProjetoFTP.Web.FrmRelatorios" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/scripts/download.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.core.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.mouse.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.draggable.js" type="text/javascript"></script>
    <link href="/estilos/downloads.css" rel="stylesheet" type="text/css" />
    <link href="/estilos/relatorios.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery.ui.core.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.mouse.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.resizable.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.draggable.js" type="text/javascript"></script>
    <script>
        $(document).ready(function () {
            $("#menu").height($(window).height() - $(".topo").height() - 43);
            $(".pagina #conteudo").width($(document).width() - $("#menu").width() - 1);
            $(".pagina #conteudo #rela").width($(document).width() - $("#menu").width() - 1);
            $(".pagina #conteudo #rela").height(600);
            $(".pagina #conteudo #rela .grid-carros").draggable({"axis" : "y", "cursor" : "crosshair"});
            download();
        });
    </script>
	<style>
	#abc { width: 150px; height: 150px; padding: 0.5em; }
	</style>
    
</head>
<body>
    <form id="form1" runat="server">
    <div id="sis_info"></div>
    <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
    <div class="pagina" id="pag">
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

            <asp:UpdateProgress runat="server" ID="updProgressRelatorio" AssociatedUpdatePanelID="updRelatorio">
                <ProgressTemplate>
                    <div class="update">Carregando...</div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <div id="rela">
                <asp:Timer runat="server" ID="tmrUpdate" Interval="5000" Enabled="true"></asp:Timer>
                <asp:UpdatePanel runat="server" ID="updRelatorio" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="tmrUpdate" />
                    </Triggers>
                    <ContentTemplate>
                        <script type="text/javascript">
                            var prm = Sys.WebForms.PageRequestManager.getInstance();
                            prm.add_endRequest(function () {
                                $(".pagina #conteudo #rela .grid-carros").draggable({ "axis": "y" });
                            }); 
                        </script>
                        <asp:DataList runat="server" ID="dlGridCarros" CssClass="grid-carros" RepeatColumns="15" RepeatDirection="Horizontal" OnItemDataBound="dlGridCarros_ItemDataBound">
                            <ItemTemplate>
                                 <asp:Panel runat="server" ID="pnlItem" CssClass='<%# Eval("CssClass") %>'>
                                    <asp:Label runat="server" ID="lblnumero" Text='<%# Eval("Numero") %>'></asp:Label>
                                 </asp:Panel>
                            </ItemTemplate>
                        </asp:DataList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <!--<div id="painel_controle" class="esquerda">
                <div class="painel_conteudo">
                    <h2>Opções:</h2>
                    Ultima Cópia: 03/05/2012 11:00:00<br />
                    Ultima Verificação: 03/05/2012 11:00:00<br />
                    <asp:Button runat="server" ID="btnForcarCopia" CssClass="btn" Text="Forçar Cópia" />
                    <asp:Button runat="server" ID="Button1" CssClass="btn vermelho" Text="Cancelar" />
                </div>
            </div>-->
            <div>
                <h1>Legenda:</h1>
                <ul id="legenda" class="esquerda">
                    <li><div class="esquerda cor branco"></div><div class="desc">Pendente de Verificação</div></li>
                    <li><div class="esquerda cor amarelo"></div><div class="desc">Pendente de Cópia</div></li>
                    <li><div class="esquerda cor laranja"></div><div class="desc">Pendente de Cópia e Verificação</div></li>
                </ul>
            </div>
            <!--
            <div id="downloads">
                <h2>Carros pendentes de cópia</h2>
                <asp:DataList runat="server" ID="dlCarros" CssClass="data-list">
                    <ItemTemplate>
                        <div class="item">
                            <img src="carros_img/carro_branco.png" width="92" />
                            <span class="numero"><asp:Label runat="server" ID="lblnumero" Text='<%# Eval("ID_CARRO") %>'></asp:Label></span>
                        </div>
                    </ItemTemplate>
                </asp:DataList>
            </div>
            -->
        </div>
    </div>
    <div id="download-list"></div>
    </form>
</body>
</html>
