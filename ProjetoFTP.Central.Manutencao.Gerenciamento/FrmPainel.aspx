    <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmPainel.aspx.cs" Inherits="ProjetoFTP.Central.Manutencao.Gerenciamento.FrmPainel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/painel.css" rel="stylesheet" type="text/css" />
    <link href="estilos/rsAlert.css" rel="stylesheet" type="text/css" />
    <script src="scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="scripts/slider.js" type="text/javascript"></script>
    <script src="scripts/label.js" type="text/javascript"></script>
    <script src="scripts/alert.js" type="text/javascript"></script>
    <script src="scripts/carro.js" type="text/javascript"></script>
    <script src="scripts/download.js" type="text/javascript"></script>
    <link href="estilos/downloads.css" rel="stylesheet" type="text/css" />
    <link rel="shortcut icon" type="image/x-icon" href="/favicon.ico" />
    <link rel="icon" type="image/x-icon" href="/favicon.ico" />
    <script>
        $(window).load(function () {
            var s = new slider();
            s.start();
            $(".item").click(function () {
                var idCarro = $(this).attr("id");
                removeCarro(idCarro);
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManager1"></asp:ScriptManager>
        <asp:Timer runat="server" ID="tmrTimer" Interval="2000" Enabled="true"></asp:Timer>
        <asp:UpdatePanel runat="server" ID="updTimer">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="tmrTimer" />
            </Triggers>
        </asp:UpdatePanel>
        <ul id="contador"></ul>
        <div class="content">
            <div id="top-shadow"></div>
            <div align="center" class="viewport">
                <table class="grid" id="slider" cellpadding="0" cellspacing="0">
                </table>
            </div>
            <div id="bottom-shadow"></div>
        </div>
        <!--
            <asp:UpdatePanel runat="server" ID="updTransf" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <asp:GridView runat="server" ID="gvTransferencias" AutoGenerateColumns="false" GridLines="None" ShowHeader="false" Width="92%" OnRowDataBound="gvTransferencias_DataBound">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class="itemTransf">
                                <div class="esquerda">
                                    <asp:Label runat="server" ID="lblCarro" CssClass="idCarro"></asp:Label>
                                    <img src="carros_img/carro_branco.png" class="esquerda" height="50px" width="110px"/>
                                </div>
                                <div class="esquerda header">
                                    <asp:Label runat="server" ID="lblArquivos"></asp:Label><br />
                                    <asp:Label runat="server" ID="lblVolume"></asp:Label><br />
                                    <asp:Label runat="server" ID="lblDataInicial"></asp:Label>
                                </div>
                                <div style="clear : both;"></div>
                                <asp:Panel runat="server" ID="pnlUpdContainer" CssClass="updContainer" Width="100%">
                                    <asp:Panel runat="server" ID="pnlUpd" CssClass="updProgress"></asp:Panel>
                                </asp:Panel>
                                <h5 class="esquerda"><asp:Label runat="server" ID="lblVelocidade"></asp:Label></h5>
                                <h5 class="direita"><asp:Label runat="server" ID="lblTempoRestante"></asp:Label></h5>
                                <div style="clear  : both;"></div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div>
                        <img src="img/busvision.png" width="280px" id="busvision_logo"/>
                        <h2>Não existe nenhuma cópia sendo realizada no momento!</h2>
                        <h3>Aguardando a entrada de carros para transferência.</h3>
                        <div id="preloader"><img src="img/loader.gif" /></div>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
            -->
    </form>
</body>
</html>
