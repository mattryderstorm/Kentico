using System;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.ExtendedControls;
using CMS.SiteProvider;

public partial class CMSModules_Content_CMSDesk_Edit_ImageEditor : CMSModalPage
{
    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            if (!imageEditor.IsUndoRedoPossible())
            {
                CurrentMaster.Body.Attributes["onunload"] = GetRefresh();
            }

            string title = GetString("general.editimage");
            Page.Title = title;
            CurrentMaster.Title.TitleText = title;
            CurrentMaster.Title.TitleImage = GetImageUrl("Design/Controls/ImageEditor/Title.png");

            CurrentMaster.Title.HelpTopicName = "imageeditor";
            CurrentMaster.Title.HelpName = "helpTopic";

            // Register postback
            ControlsHelper.RegisterPostbackControl(btnRedo);
            ControlsHelper.RegisterPostbackControl(btnUndo);
            ControlsHelper.RegisterPostbackControl(btnSave);
            ControlsHelper.RegisterPostbackControl(btnClose);

            btnSave.Click += btnSave_Click;
            btnClose.Click += btnClose_Click;
            btnUndo.Click += btnUndo_Click;
            btnRedo.Click += btnRedo_Click;

            btnUndo.Visible = imageEditor.IsUndoRedoPossible();
            btnRedo.Visible = imageEditor.IsUndoRedoPossible();
            btnSave.Visible = imageEditor.IsUndoRedoPossible();

            AddNoCacheTag();
        }
        else
        {
            // Hide all controls
            imageEditor.Visible = false;
            btnUndo.Visible = false;
            btnRedo.Visible = false;
            btnSave.Visible = false;
            btnClose.Visible = false;

            string url = ResolveUrl(String.Format("~/CMSMessages/Error.aspx?title={0}&text={1}&cancel=1", GetString("dialogs.badhashtitle"), GetString("dialogs.badhashtext")));
            ltlScript.Text = ScriptHelper.GetScript(String.Format("window.location = '{0}';", url));
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            btnUndo.Enabled = imageEditor.IsUndoEnabled();
            btnRedo.Enabled = imageEditor.IsRedoEnabled();

            if (btnUndo.Enabled || btnRedo.Enabled)
            {
                string confirmScript = String.Format("function discardchanges() {{ return confirm({0}); }}", ScriptHelper.GetString(GetString("imageeditor.discardchangesconfirmation")));
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "DiscardChanges", ScriptHelper.GetScript(confirmScript));

                btnClose.OnClientClick = "return discardchanges();";
            }
        }

        // Enable or disable save button
        btnSave.Enabled = imageEditor.Enabled;
    }

    #endregion


    #region "Buttons handling"

    protected void btnRedo_Click(object sender, EventArgs e)
    {
        imageEditor.ProcessRedo();
    }


    protected void btnUndo_Click(object sender, EventArgs e)
    {
        imageEditor.ProcessUndo();
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        imageEditor.SaveCurrentVersion();
        if (!imageEditor.SavingFailed)
        {
            ltlScript.Text = ScriptHelper.GetScript("setTimeout('window.skipCloseConfirm = true;Close()',0);");
            if (imageEditor.IsUndoRedoPossible())
            {
                ltlScript.Text += ScriptHelper.GetScript(GetRefresh());
            }
        }
    }


    protected void btnClose_Click(object sender, EventArgs e)
    {
        TempFileInfoProvider.DeleteTempFiles(TempFileInfoProvider.IMAGE_EDITOR_FOLDER, imageEditor.InstanceGUID);
        ltlScript.Text = ScriptHelper.GetScript("setTimeout('window.skipCloseConfirm = true;Close()',0);");
    }

    #endregion


    #region "Private methods"

    private static string GetRefresh()
    {
        if (QueryHelper.GetBoolean("refresh", false))
        {
            string attachmentGuid = QueryHelper.GetString("attachmentguid", "");
            string filepath = QueryHelper.GetString("filepath", "");
            string clientid = QueryHelper.GetString("clientid", "");
            int nodeId = QueryHelper.GetInteger("nodeId", 0);

            if (!String.IsNullOrEmpty(filepath))
            {
                return String.Format("if (wopener.imageEdit_FileSystemRefresh) {{ wopener.imageEdit_FileSystemRefresh({0}); }}", ScriptHelper.GetString(String.Format("filepath|{0}", filepath)));
            }
            else if (nodeId > 0)
            {
                return String.Format("if (wopener.imageEdit_ContentRefresh) {{ wopener.imageEdit_ContentRefresh({0}); }}", ScriptHelper.GetString(String.Format("attachmentguid|{0}|nodeId|{1}", attachmentGuid, nodeId)));
            }
            else
            {
                return String.Format("if (wopener.imageEdit_AttachmentRefresh) {{ wopener.imageEdit_AttachmentRefresh('attachmentguid|{0}'); }} else {{ InitRefresh({1}, true, true, '{0}', 'refresh'); }}", ScriptHelper.GetString(attachmentGuid, false), ScriptHelper.GetString(clientid));
            }
        }

        return String.Empty;
    }

    #endregion
}