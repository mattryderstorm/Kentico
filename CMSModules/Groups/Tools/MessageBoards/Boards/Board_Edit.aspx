<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_MessageBoards_Boards_Board_Edit"
    CodeFile="Board_Edit.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Message Boards - Default</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<frameset border="0" runat="server" rows="64, *" id="rowsFrameset">
    <frame name="boardEditMenu" src="Board_Header.aspx<%= Request.Url.Query %>" scrolling="no"
        frameborder="0" noresize="noresize" />
    <frame name="boardEditContent" src="../Messages/Message_List.aspx<%= Request.Url.Query %>"
        frameborder="0" noresize="noresize" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
