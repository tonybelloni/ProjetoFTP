<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmEditarCarro.aspx.cs" Inherits="ProjetoFTP.Web.FrmEditarCarro" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/estilos/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="380px">
            <tr>
                <td>Número: </td>
                <td><asp:TextBox runat="server" ID="txtNumero" CssClass="txt" Enabled="false"></asp:TextBox></td>
            </tr>
            <tr>
                <td>IP: </td>
                <td><asp:TextBox runat="server" ID="txtIp" CssClass="txt"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Usuário: </td>
                <td><asp:TextBox runat="server" ID="txtUsuario" CssClass="txt"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Senha: </td>
                <td><asp:TextBox runat="server" ID="txtSenha" CssClass="txt"></asp:TextBox></td>
            </tr>
            <tr>
                <td></td>
                <td><asp:Button runat="server" ID="btnEditar" CssClass="btn" Text="Editar" OnClick="btnEditar_Click"/></td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
