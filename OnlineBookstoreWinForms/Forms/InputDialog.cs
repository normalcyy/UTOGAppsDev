using System;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;

namespace OnlineBookstoreWinForms.Forms;

/// <summary>
/// Simple single-field input dialog — replaces Microsoft.VisualBasic.Interaction.InputBox.
/// Returns null if the user cancels.
/// </summary>
public class InputDialog : Form
{
    private readonly TextBox _txt;
    public string? Value => DialogResult == DialogResult.OK ? _txt.Text.Trim() : null;

    public InputDialog(string prompt, string title, string defaultValue = "")
    {
        Text            = title;
        ClientSize      = new Size(360, 130);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = Theme.Background;
        Font            = Theme.FontBase;

        var lbl = new Label { Text = prompt, AutoSize = true, Location = new Point(16, 16), Font = Theme.FontBase, ForeColor = Theme.TextSubtle };

        _txt = new TextBox { Location = new Point(16, 40), Size = new Size(328, Theme.InputHeight), Text = defaultValue };
        UIHelper.StyleTextBox(_txt);

        var btnOk = new Button { Text = "OK",     Location = new Point(16, 84),  Size = new Size(148, Theme.ButtonHeight) };
        var btnCc = new Button { Text = "Cancel", Location = new Point(172, 84), Size = new Size(172, Theme.ButtonHeight) };
        UIHelper.StylePrimary(btnOk);
        UIHelper.StyleSecondary(btnCc);

        btnOk.Click += (s, e) => { DialogResult = DialogResult.OK;     Close(); };
        btnCc.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[] { lbl, _txt, btnOk, btnCc });
        Load += (s, e) => { UIHelper.RoundCorners(btnOk); UIHelper.RoundCorners(btnCc); };
        AcceptButton = btnOk;
        CancelButton = btnCc;
    }
}
