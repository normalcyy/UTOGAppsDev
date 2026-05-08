using System;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;

namespace OnlineBookstoreWinForms.Forms;

public class RegisterForm : Form
{
    private TextBox _txtUsername  = null!;
    private TextBox _txtEmail     = null!;
    private TextBox _txtPassword  = null!;
    private TextBox _txtConfirm   = null!;
    private Panel   _pnlError     = null!;
    private Label   _lblError     = null!;

    public RegisterForm()
    {
        Text            = "Online Bookstore — Register";
        ClientSize      = new Size(420, 560);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Theme.Background;
        Font            = Theme.FontBase;

        BuildUI();
    }

    private void BuildUI()
    {
        var pnlCard = new Panel
        {
            Location  = new Point(30, 40),
            Size      = new Size(360, 480),
            BackColor = Color.White,
        };
        pnlCard.Paint += UIHelper.PaintCard;

        int pad = 20;
        int w   = 320;
        int y   = pad;

        var lblEmoji = new Label
        {
            Text      = "📚",
            Font      = new Font("Segoe UI Emoji", 24f),
            AutoSize  = false,
            Size      = new Size(w, 44),
            Location  = new Point(pad, y),
            TextAlign = ContentAlignment.MiddleCenter,
        };
        y += 48;

        var lblHeading = new Label
        {
            Text      = "Create Account",
            Font      = Theme.FontHeading,
            ForeColor = Theme.TextMain,
            AutoSize  = false,
            Size      = new Size(w, 30),
            Location  = new Point(pad, y),
            TextAlign = ContentAlignment.MiddleCenter,
        };
        y += 32;

        var lblSub = new Label
        {
            Text      = "Join the Online Bookstore",
            Font      = Theme.FontBase,
            ForeColor = Theme.TextMuted,
            AutoSize  = false,
            Size      = new Size(w, 20),
            Location  = new Point(pad, y),
            TextAlign = ContentAlignment.MiddleCenter,
        };
        y += 28;

        Control MakeField(string labelText, bool isPassword = false)
        {
            var lbl = new Label
            {
                Text      = labelText,
                Font      = Theme.FontLabel,
                ForeColor = Theme.TextSubtle,
                AutoSize  = true,
                Location  = new Point(pad, y),
            };
            y += 20;
            var txt = new TextBox
            {
                Location     = new Point(pad, y),
                Size         = new Size(w, Theme.InputHeight),
                PasswordChar = isPassword ? '●' : '\0',
            };
            UIHelper.StyleTextBox(txt);
            y += Theme.InputHeight + 8;
            pnlCard.Controls.Add(lbl);
            return txt;
        }

        _txtUsername = (TextBox)MakeField("Username");
        _txtEmail    = (TextBox)MakeField("Email");
        _txtPassword = (TextBox)MakeField("Password", isPassword: true);
        _txtConfirm  = (TextBox)MakeField("Confirm Password", isPassword: true);

        _pnlError = new Panel
        {
            Location  = new Point(pad, y),
            Size      = new Size(w, 32),
            BackColor = Theme.ErrorBg,
            Visible   = false,
        };
        _lblError = new Label
        {
            Dock      = DockStyle.Fill,
            ForeColor = Theme.Destructive,
            Font      = Theme.FontSm,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(6, 0, 0, 0),
        };
        _pnlError.Controls.Add(_lblError);
        y += 40;

        var btnCreate = new Button
        {
            Text     = "Create Account",
            Location = new Point(pad, y),
            Size     = new Size(w, Theme.ButtonHeight),
        };
        UIHelper.StyleSuccess(btnCreate);
        btnCreate.Click += BtnCreate_Click;
        y += Theme.ButtonHeight + 8;

        var btnBack = new Button
        {
            Text      = "Already have an account? Sign In",
            Location  = new Point(pad, y),
            Size      = new Size(w, 30),
            FlatStyle = FlatStyle.Flat,
            ForeColor = Theme.Primary,
            BackColor = Color.Transparent,
            Font      = Theme.FontBase,
            Cursor    = Cursors.Hand,
        };
        btnBack.FlatAppearance.BorderSize = 0;
        btnBack.Click += (s, e) => Close(); // LoginForm is the parent dialog owner

        pnlCard.Controls.AddRange(new Control[]
        {
            lblEmoji, lblHeading, lblSub,
            _txtUsername, _txtEmail, _txtPassword, _txtConfirm,
            _pnlError, btnCreate, btnBack
        });

        Controls.Add(pnlCard);
        Load += (s, e) => UIHelper.RoundCorners(btnCreate);
        AcceptButton = btnCreate;
    }

    private async void BtnCreate_Click(object? sender, EventArgs e)
    {
        _pnlError.Visible = false;

        var username = _txtUsername.Text.Trim();
        var email    = _txtEmail.Text.Trim();
        var password = _txtPassword.Text;
        var confirm  = _txtConfirm.Text;

        if (password != confirm)
        {
            ShowError("Passwords do not match.");
            return;
        }

        var (success, error) = await AppServices.Auth.RegisterAsync(username, password, email);
        if (!success)
        {
            ShowError(error!);
            return;
        }

        MessageBox.Show("Account created! Please sign in.", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close(); // returns to LoginForm (the dialog owner)
    }

    private void ShowError(string message)
    {
        _lblError.Text    = message;
        _pnlError.Visible = true;
    }
}
