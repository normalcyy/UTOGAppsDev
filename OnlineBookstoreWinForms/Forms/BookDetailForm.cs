using System;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Forms;

public class BookDetailForm : Form
{
    private readonly Book _book;
    private Panel  _pnlMessage = null!;
    private Label  _lblMessage = null!;
    private Button _btnWishlist = null!;

    public BookDetailForm(Book book)
    {
        _book           = book;
        Text            = book.Title;
        ClientSize      = new Size(540, 540);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = Theme.Background;
        Font            = Theme.FontBase;

        BuildUI();
    }

    private void BuildUI()
    {
        SuspendLayout();

        var pnlCard = new Panel
        {
            Location  = new Point(20, 20),
            Size      = new Size(500, 500),
            BackColor = Color.White,
        };
        pnlCard.Paint += UIHelper.PaintCard;

        int pad = 24;
        int w   = 500 - pad * 2;
        int y   = pad;

        // Title
        var lblTitle = new Label
        {
            Text      = _book.Title,
            Font      = Theme.FontHeading,
            ForeColor = Theme.TextMain,
            AutoSize  = false,
            Size      = new Size(w, 40),
            Location  = new Point(pad, y),
        };
        y += 44;

        // Meta grid (2 columns)
        void AddMeta(string label, string value, ref int yRef, Font? valueFont = null)
        {
            var lbl = new Label
            {
                Text      = label + ":",
                Font      = Theme.FontLabel,
                ForeColor = Theme.TextMuted,
                AutoSize  = false,
                Size      = new Size(90, 22),
                Location  = new Point(pad, yRef),
            };
            var val = new Label
            {
                Text      = value,
                Font      = valueFont ?? Theme.FontBase,
                ForeColor = Theme.TextMain,
                AutoSize  = false,
                Size      = new Size(w - 96, 22),
                Location  = new Point(pad + 96, yRef),
                AutoEllipsis = true,
            };
            pnlCard.Controls.Add(lbl);
            pnlCard.Controls.Add(val);
            yRef += 26;
        }

        AddMeta("Author",   _book.AuthorName,   ref y);
        AddMeta("Category", _book.CategoryName, ref y);

        // Price (special styling)
        var lblPriceLbl = new Label { Text = "Price:", Font = Theme.FontLabel, ForeColor = Theme.TextMuted, AutoSize = false, Size = new Size(90, 22), Location = new Point(pad, y) };
        var lblPrice    = new Label { Text = $"${_book.Price:F2}", Font = Theme.FontSubhead, ForeColor = Theme.Primary, AutoSize = false, Size = new Size(w - 96, 22), Location = new Point(pad + 96, y) };
        pnlCard.Controls.Add(lblPriceLbl);
        pnlCard.Controls.Add(lblPrice);
        y += 26;

        AddMeta("Stock", _book.Stock.ToString(), ref y);
        AddMeta("ISBN",  _book.ISBN,             ref y);

        // Separator
        y += 4;
        var sep = new Panel { Location = new Point(pad, y), Size = new Size(w, 1), BackColor = Theme.Border };
        pnlCard.Controls.Add(sep);
        y += 8;

        // Description
        var lblDescHeader = new Label
        {
            Text      = "Description",
            Font      = Theme.FontBold,
            ForeColor = Theme.TextSubtle,
            AutoSize  = true,
            Location  = new Point(pad, y),
        };
        y += 22;

        var lblDesc = new Label
        {
            Text      = _book.Description,
            Font      = Theme.FontBase,
            ForeColor = Theme.TextMuted,
            AutoSize  = false,
            Size      = new Size(w, 64),
            Location  = new Point(pad, y),
        };
        y += 72;

        // Message panel
        _pnlMessage = new Panel
        {
            Location  = new Point(pad, y),
            Size      = new Size(w, 32),
            Visible   = false,
        };
        _lblMessage = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = Theme.FontSm,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(6, 0, 0, 0),
        };
        _pnlMessage.Controls.Add(_lblMessage);
        y += 40;

        // Buttons row
        int btnW = (w - 8) / 2;
        _btnWishlist = new Button
        {
            Text     = "Add to Wishlist",
            Location = new Point(pad, y),
            Size     = new Size(btnW, Theme.ButtonHeight),
        };
        UIHelper.StylePrimary(_btnWishlist);
        _btnWishlist.Click += BtnWishlist_Click;

        var btnBack = new Button
        {
            Text     = "Back",
            Location = new Point(pad + btnW + 8, y),
            Size     = new Size(btnW, Theme.ButtonHeight),
        };
        UIHelper.StyleSecondary(btnBack);
        btnBack.Click += (s, e) => Close();

        pnlCard.Controls.AddRange(new Control[]
        {
            lblTitle, lblDescHeader, lblDesc,
            sep, _pnlMessage, _btnWishlist, btnBack
        });

        Controls.Add(pnlCard);
        ResumeLayout();

        Load += async (s, e) =>
        {
            if (SessionManager.IsLoggedIn)
            {
                bool inList = await AppServices.Wishlist.IsInWishlistAsync(
                    SessionManager.CurrentUser!.Id, _book.Id);
                UpdateWishlistButton(inList);
            }
        };
    }

    private void UpdateWishlistButton(bool isInWishlist)
    {
        if (isInWishlist)
        {
            _btnWishlist.Text = "Remove from Wishlist";
            _btnWishlist.BackColor = Theme.Destructive;
            _btnWishlist.FlatAppearance.MouseOverBackColor = Theme.DestructiveHov;
        }
        else
        {
            _btnWishlist.Text = "Add to Wishlist";
            _btnWishlist.BackColor = Theme.Primary;
            _btnWishlist.FlatAppearance.MouseOverBackColor = Theme.PrimaryHover;
        }
    }

    private async void BtnWishlist_Click(object? sender, EventArgs e)
    {
        if (!SessionManager.IsLoggedIn) return;

        int userId = SessionManager.CurrentUser!.Id;
        bool isIn  = await AppServices.Wishlist.IsInWishlistAsync(userId, _book.Id);

        if (isIn)
        {
            await AppServices.Wishlist.RemoveFromWishlistAsync(userId, _book.Id);
            ShowMessage("Removed from wishlist.", success: false);
            UpdateWishlistButton(false);
        }
        else
        {
            var (ok, err) = await AppServices.Wishlist.AddToWishlistAsync(userId, _book.Id);
            if (ok)
            {
                ShowMessage("Added to wishlist!", success: true);
                UpdateWishlistButton(true);
            }
            else
            {
                ShowMessage(err ?? "Error.", success: false);
            }
        }
    }

    private void ShowMessage(string msg, bool success)
    {
        _lblMessage.Text            = msg;
        _pnlMessage.BackColor       = success ? Theme.SuccessBg  : Theme.ErrorBg;
        _lblMessage.ForeColor       = success ? Theme.Success     : Theme.Destructive;
        _pnlMessage.Visible         = true;
    }
}
