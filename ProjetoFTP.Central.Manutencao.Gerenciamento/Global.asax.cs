using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;

namespace ProjetoFTP.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.Ignore("{resource}.axd/{*pathInfo}");
            RouteTable.Routes.Add("Principal", new Route("principal", new PageRouteHandler("~/Default.aspx")));
            RouteTable.Routes.Add("Painel", new Route("painel", new PageRouteHandler("~/FrmPainelCarros.aspx")));
            RouteTable.Routes.Add("Relatorio", new Route("relatorio", new PageRouteHandler("~/FrmRelatorioTransferencias.aspx")));
            RouteTable.Routes.Add("Carros", new Route("carros", new PageRouteHandler("~/FrmVeiculos.aspx")));
            RouteTable.Routes.Add("Estacoes", new Route("estacoes", new PageRouteHandler("~/FrmTerminais.aspx")));
            RouteTable.Routes.Add("Notificacoes", new Route("notificacoes", new PageRouteHandler("~/FrmNotificacoes.aspx")));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}