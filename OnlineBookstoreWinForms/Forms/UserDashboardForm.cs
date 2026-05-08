using System;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Controls;
using OnlineBookstoreWinForms.Helpers;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Forms;

public class UserDashboardForm : Form
{
    private FlowLayoutPanel _flpBooks   = null!;
    private ProgressBar     _progress   = null!;
    private TextBox         _txtSearch  = null!;

    public UserDashboardForm()
    {
        Text            = "Online Bookstore — Browse";
        ClientSize      = new Size(980, 700);
        MinimumSize     = new Size(980, 700);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Theme.Background;
        Font            = Theme.FontBase;

        BuildUI();
    }

    private void BuildUI()
    {
        SuspendLayout();

        // ── NavBar ──────────────────────────────────────────
        var pnlNav = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = Theme.NavHeight,
            BackColor = Theme.NavBar,
        };

        var lblBrand = new Label
        {
            Text      = "📚  Online Bookstore",
            Font      = Theme.FontBold,
            ForeColor = Color.White,
            AutoSize  = true,
            Location  = new Point(16, 15),
        };

        var btnLogout = new Button
        {
            Text   = "Logout",
            Size   = new Size(80, 32),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        UIHelper.StyleSecondary(btnLogout);
        btnLogout.Location = new Point(980 - 16 - 80, 10);
        btnLogout.Click += (s, e) =>
        {
            SessionManager.Logout();
            new LoginForm().Show();
            Close();
        };

        var btnOrders = new Button
        {
            Text   = "Order History",
            Size   = new Size(110, 32),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        UIHelper.StyleSecondary(btnOrders);
        btnOrders.Location = new Point(btnLogout.Left - 8 - 110, 10);
        btnOrders.Click += (s, e) => new OrderHistoryForm().ShowDialog(this);

        var btnWishlist = new Button
        {
            Text   = "Wishlist",
            Size   = new Size(80, 32),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };
        UIHelper.StyleSecondary(btnWishlist);
        btnWishlist.Location = new Point(btnOrders.Left - 8 - 80, 10);
        btnWishlist.Click += (s, e) => new WishlistForm().ShowDialog(this);

        pnlNav.Controls.AddRange(new Control[] { lblBrand, btnWishlist, btnOrders, btnLogout });

        // ── Search bar ──────────────────────────────────────
        var pnlSearch = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 60,
            BackColor = Theme.Background,
            Padding   = new Padding(12),
        };

        _txtSearch = new TextBox
        {
            Location = new Point(12, 12),
            Size     = new Size(860, Theme.InputHeight),
            Anchor   = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
        };
        UIHelper.StyleTextBox(_txtSearch);

        var btnSearch = new Button
        {
            Text     = "Search",
            Size     = new Size(80, Theme.InputHeight),
            Location = new Point(880, 12),
            Anchor   = AnchorStyles.Top | AnchorStyles.Right,
        };
        UIHelper.StylePrimary(btnSearch);
        btnSearch.Click += async (s, e) =>
        {
            var term = _txtSearch.Text.Trim();
            var books = await AppServices.Books.SearchBooksAsync(term, null, null, null, null);
            PopulateBooks(books);
        };

        pnlSearch.Controls.AddRange(new Control[] { _txtSearch, btnSearch });

        // ── Loading bar ─────────────────────────────────────
        _progress = new ProgressBar
        {
            Dock    = DockStyle.Top,
            Height  = 3,
            Style   = ProgressBarStyle.Marquee,
            Visible = false,
        };

        // ── Book cards flow panel ────────────────────────────
        _flpBooks = new FlowLayoutPanel
        {
            Dock          = DockStyle.Fill,
            AutoScroll    = true,
            WrapContents  = true,
            BackColor     = Theme.Background,
            Padding       = new Padding(12),
            FlowDirection = FlowDirection.LeftToRight,
        };

        Controls.AddRange(new Control[] { _flpBooks, _progress, pnlSearch, pnlNav });

        // If user closes via X (without logout), log out and exit cleanly
        FormClosed += (s, e) =>
        {
            if (SessionManager.IsLoggedIn)
            {
                SessionManager.Logout();
                Application.Exit();
            }
        };

        ResumeLayout();

        Load += async (s, e) =>
        {
            _progress.Visible = true;
            var books = await AppServices.Books.GetAllBooksAsync();
            _progress.Visible = false;
            PopulateBooks(books);
        };
    }

    private void PopulateBooks(System.Collections.Generic.List<Book> books)
    {
        _flpBooks.SuspendLayout();
        _flpBooks.Controls.Clear();

        foreach (var book in books)
        {
            var card = new BookCard { Book = book };
            card.ViewDetails += (s, b) =>
            {
                using var dlg = new BookDetailForm(b);
                dlg.ShowDialog(this);
            };
            _flpBooks.Controls.Add(card);
        }

        _flpBooks.ResumeLayout();
    }
}
