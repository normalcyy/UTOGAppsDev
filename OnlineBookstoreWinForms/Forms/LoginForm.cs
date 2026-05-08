using System;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;

namespace OnlineBookstoreWinForms.Forms;

public class LoginForm : Form
{
    private TextBox _txtUsername = null!;
    private TextBox _txtPassword = null!;
    private Panel   _pnlError    = null!;
    private Label   _lblError    = null!;

    public LoginForm()
    {
        Text            = "Online Bookstore — Sign In";
        ClientSize      = new Size(420, 480);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Theme.Background;
        Font            = Theme.FontBase;

        BuildUI();
    }

    private void BuildUI()
    {
        // Card panel
        var pnlCard = new Panel
        {
            Location  = new Point(30, 50),
            Size      = new Size(360, 380),
            BackColor = Color.White,
        };
        pnlCard.Paint += UIHelper.PaintCard;

        int pad = 20;
        int w   = 360 - pad * 2; // 320
        int y   = pad;

        // Emoji
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

        // Heading
        var lblHeading = new Label
        {
            Text      = "Online Bookstore",
            Font      = Theme.FontHeading,
            ForeColor = Theme.TextMain,
            AutoSize  = false,
            Size      = new Size(w, 30),
            Location  = new Point(pad, y),
            TextAlign = ContentAlignment.MiddleCenter,
        };
        y += 32;

        // Subtext
        var lblSub = new Label
        {
            Text      = "Sign in to your account",
            Font      = Theme.FontBase,
            ForeColor = Theme.TextMuted,
            AutoSize  = false,
            Size      = new Size(w, 20),
            Location  = new Point(pad, y),
            TextAlign = ContentAlignment.MiddleCenter,
        };
        y += 28;

        // Username label
        var lblUsername = new Label
        {
            Text      = "Username",
            Font      = Theme.FontLabel,
            ForeColor = Theme.TextSubtle,
            AutoSize  = true,
            Location  = new Point(pad, y),
        };
        y += 20;

        // Username textbox
        _txtUsername = new TextBox { Location = new Point(pad, y), Size = new Size(w, Theme.InputHeight) };
        UIHelper.StyleTextBox(_txtUsername);
        y += Theme.InputHeight + 8;

        // Password label
        var lblPassword = new Label
        {
            Text      = "Password",
            Font      = Theme.FontLabel,
            ForeColor = Theme.TextSubtle,
            AutoSize  = true,
            Location  = new Point(pad, y),
        };
        y += 20;

        // Password textbox
        _txtPassword = new TextBox
        {
            Location     = new Point(pad, y),
            Size         = new Size(w, Theme.InputHeight),
            PasswordChar = '●',
        };
        UIHelper.StyleTextBox(_txtPassword);
        y += Theme.InputHeight + 8;

        // Error panel
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

        // Sign In button
        var btnSignIn = new Button
        {
            Text     = "Sign In",
            Location = new Point(pad, y),
            Size     = new Size(w, Theme.ButtonHeight),
        };
        UIHelper.StylePrimary(btnSignIn);
        btnSignIn.Click += BtnSignIn_Click;
        y += Theme.ButtonHeight + 8;

        // Register link
        var btnRegister = new Button
        {
            Text      = "Don't have an account? Register",
            Location  = new Point(pad, y),
            Size      = new Size(w, 30),
            FlatStyle = FlatStyle.Flat,
            ForeColor = Theme.Primary,
            BackColor = Color.Transparent,
            Font      = Theme.FontBase,
            Cursor    = Cursors.Hand,
        };
        btnRegister.FlatAppearance.BorderSize = 0;
        btnRegister.Click += (s, e) =>
        {
            // Show register as a modal dialog — LoginForm stays open behind it
            using var reg = new RegisterForm();
            reg.ShowDialog(this);
        };

        pnlCard.Controls.AddRange(new Control[]
        {
            lblEmoji, lblHeading, lblSub,
            lblUsername, _txtUsername,
            lblPassword, _txtPassword,
            _pnlError, btnSignIn, btnRegister
        });

        Controls.Add(pnlCard);

        // Apply rounded corners after layout
        Load += (s, e) => UIHelper.RoundCorners(btnSignIn);

        // If user closes the login window without logging in, exit the app
        FormClosed += (s, e) =>
        {
            if (!SessionManager.IsLoggedIn)
                Application.Exit();
        };

        // Allow Enter key to submit
        AcceptButton = btnSignIn;
    }

    private async void BtnSignIn_Click(object? sender, EventArgs e)
    {
        _pnlError.Visible = false;

        var username = _txtUsername.Text.Trim();
        var password = _txtPassword.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Please enter username and password.");
            return;
        }

        var (user, error) = await AppServices.Auth.LoginAsync(username, password);
        if (error != null)
        {
            ShowError(error);
            return;
        }

        SessionManager.Login(user!);

        if (SessionManager.IsAdmin)
            new AdminPanelForm().Show();
        else
            new UserDashboardForm().Show();

        // Hide (don't Close) so Application.Run() keeps the message loop alive.
        Hide();
    }

    private void ShowError(string message)
    {
        _lblError.Text    = message;
        _pnlError.Visible = true;
    }
}
