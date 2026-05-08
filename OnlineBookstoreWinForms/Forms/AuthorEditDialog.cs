using System;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Forms;

public class AuthorEditDialog : Form
{
    private readonly Author? _existing;
    private TextBox _txtName = null!;
    private TextBox _txtBio  = null!;

    public Author? Result { get; private set; }

    public AuthorEditDialog(Author? existing)
    {
        _existing       = existing;
        Text            = existing == null ? "Add Author" : "Edit Author";
        ClientSize      = new Size(400, 220);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = Theme.Background;
        Font            = Theme.FontBase;

        BuildUI();
    }

    private void BuildUI()
    {
        int pad = 16, w = 368, y = 16;

        var lblName = new Label { Text = "Name", AutoSize = true, Location = new Point(pad, y), Font = Theme.FontLabel, ForeColor = Theme.TextSubtle };
        y += 20;
        _txtName = new TextBox { Location = new Point(pad, y), Size = new Size(w, Theme.InputHeight), Text = _existing?.Name ?? string.Empty };
        UIHelper.StyleTextBox(_txtName);
        y += Theme.InputHeight + 8;

        var lblBio = new Label { Text = "Bio", AutoSize = true, Location = new Point(pad, y), Font = Theme.FontLabel, ForeColor = Theme.TextSubtle };
        y += 20;
        _txtBio = new TextBox { Location = new Point(pad, y), Size = new Size(w, Theme.InputHeight), Text = _existing?.Bio ?? string.Empty };
        UIHelper.StyleTextBox(_txtBio);
        y += Theme.InputHeight + 16;

        var btnSave   = new Button { Text = "Save",   Location = new Point(pad, y), Size = new Size(174, Theme.ButtonHeight) };
        var btnCancel = new Button { Text = "Cancel", Location = new Point(pad + 182, y), Size = new Size(186, Theme.ButtonHeight) };
        UIHelper.StylePrimary(btnSave);
        UIHelper.StyleSecondary(btnCancel);

        btnSave.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text)) { MessageBox.Show("Name is required."); return; }
            Result = new Author { Id = _existing?.Id ?? 0, Name = _txtName.Text.Trim(), Bio = _txtBio.Text.Trim() };
            DialogResult = DialogResult.OK;
            Close();
        };
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[] { lblName, _txtName, lblBio, _txtBio, btnSave, btnCancel });
        Load += (s, e) => { UIHelper.RoundCorners(btnSave); UIHelper.RoundCorners(btnCancel); };
        AcceptButton = btnSave;
    }
}
