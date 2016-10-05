<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProjetoFTP.Web.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/estilos/style.css" rel="stylesheet" type="text/css" />
    <link href="/estilos/default.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/jquery-2.0.3.min.js" type="text/javascript"></script>
    <script src="/scripts/workspace.js" type="text/javascript"></script>
    <link href="/estilos/workspace.css" rel="stylesheet" type="text/css" />
    <link href="/estilos/downloads.css" rel="stylesheet" type="text/css" />
    <script src="/scripts/download.js" type="text/javascript"></script>
    <link rel="shortcut icon" type="image/x-icon" href="/favicon.ico" />
    <link rel="icon" type="image/x-icon" href="/favicon.ico" />
    <script>
        $(document).ready(function () {
            localStorage.setItem(TAG_DOWNLOAD_TAB_OPEN, true);
            download();
            adicionaItem("#content_main table tr:nth-child(1) td:first-child", { width: 90,
                height: 90,
                title: 'Painel',
                src: 'img/painel.png',
                href: 'painel'
            });
            adicionaItem("#content_main table tr:nth-child(1) td:nth-child(2)", {
                width: 90,
                height: 90,
                title: 'Relatorios',
                src: 'img/relatorio.png',
                href: 'relatorio'
            });
            adicionaItem("#content_main table tr:nth-child(1) td:nth-child(3)", {
                width: 90,
                height: 90,
                title: 'Notificações',
                src: 'img/notificacao.png',
                href: ''
            });
            adicionaItem("#content_main table tr:nth-child(1) td:nth-child(4)", { width: 90,
                height: 90,
                title: 'Carros',
                src: 'img/carro.png',
                href: 'carros'
            });
            adicionaItem("#content_main table tr:nth-child(1) td:nth-child(5)", { width: 90,
                height: 90,
                title: 'Estações',
                src: 'img/estacao.png',
                href: 'estacoes'
            });
            adicionaItem("#content_main table tr:nth-child(1) td:nth-child(6)", { width: 90,
                height: 90,
                title: 'Sair',
                src: 'img/sair.png',
                href: 'FrmLogout.aspx'
            });
        });
    </script>
    <style>
        #download-show-hide
        {
            display: none;
        }
        
#download-list
{
    z-index: 1;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="viewport">
            <div id="page_bar" runat="server">
            </div>
            <div id="content">
                <div id="content_header">
                    <img src="/img/busvision.png" height="40px"/>
                </div>
                <div id="content_main" align="left">
                    <table>
                        <tr>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </div>
                <!--<div align="left">
                    <div class="rodape">Desenvolvido por Rio Service Tecnologia</div>
                </div>-->
            </div>
        </div>
    </form>
    <div id="download-list"></div>
</body>
</html>
