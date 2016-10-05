<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmRelatorioCopiasGraf.aspx.cs" Inherits="ProjetoFTP.Central.Manutencao.Gerenciamento.FrmRelatorioCopiasGraf" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="estilos/site.css" rel="stylesheet" type="text/css" />
    <script src="scripts/jquery.min.js" type="text/javascript"></script>
    <script src="scripts/highcharts.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.core.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.mouse.js" type="text/javascript"></script>
    <script src="/scripts/jquery.ui.resizable.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.draggable.js" type="text/javascript"></script>
    <script>
        $(function () {
            var chart;
            $(document).ready(function () {
                chart = new Highcharts.Chart({
                    chart: {
                        renderTo: 'container',
                        type: 'line',
                        marginRight: 100,
                        marginBottom: 25
                    },
                    title: {
                        text: 'Velocidade Média de cópia dos carros',
                        x: -20 //center
                    },
                    xAxis: {
                        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
                    },
                    yAxis: {
                        title: {
                            text: 'Velocidade (MB/min)'
                        },
                        plotLines: [{
                            value: 0,
                            width: 1,
                            color: '#808080'
                        }]
                    },
                    tooltip: {
                        formatter: function () {
                            return '<b>' + this.series.name + '</b><br/>' +
                        this.x + ': ' + this.y + 'MB/min';
                        }
                    },
                    legend: {
                        layout: 'vertical',
                        align: 'right',
                        verticalAlign: 'top',
                        x: -20,
                        y: 0,
                        borderWidth: 0
                    },
                    series: [{
                        name: 'Carros',
                        data: [27.0, 32.9, 32.5, 34.5, 25.2, 21.5, 25.2, 26.5, 23.3, 18.3, 19.9, 20.6]
                    }],
                    colors : [ "#0a2" ],
                    symbols : [ "triangle" ]
            });
        });

    });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="update"><div class="upd_desc">Em Contrução</div></div>
        <div id="container" style="min-width: 400px; height: 400px; margin: 0 auto"></div>
    </div>
    </form>
</body>
</html>
