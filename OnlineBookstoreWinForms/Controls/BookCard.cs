using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Controls;

public class BookCard : UserControl
{
    private Book? _book;

    private readonly Panel  _iconPanel;
    private readonly Label  _lblTitle;
    private readonly Label  _lblAuthor;
    private readonly Label  _lblCategory;
    private readonly Label  _lblPrice;
    private readonly Label  _lblStock;
    private readonly Button _btnView;

    public event EventHandler<Book>? ViewDetails;

    public Book? Book
    {
        get => _book;
        set
        {
            _book = value;
            if (_book != null) Populate();
        }
    }

    public BookCard()
    {
        Size        = new Size(200, 260);
        BackColor   = Theme.Card;
        BorderStyle = BorderStyle.FixedSingle;
        Padding     = new Padding(12);
        Cursor      = Cursors.Default;

        int pad = 12;
        int w   = 200 - pad * 2;
        int y   = pad;

        // Icon placeholder
        _iconPanel = new Panel
        {
            Location  = new Point(pad, y),
            Size      = new Size(w, 56),
            BackColor = Theme.SecondaryBg,
        };
        var lblIcon = new Label
        {
            Text      = "📖",
            Font      = new Font("Segoe UI Emoji", 20f),
            AutoSize  = false,
            Size      = new Size(w, 56),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
        };
        _iconPanel.Controls.Add(lblIcon);
        y += 60;

        // Title
        _lblTitle = new Label
        {
            Location  = new Point(pad, y),
            Size      = new Size(w, 36),
            Font      = Theme.FontBold,
            ForeColor = Theme.TextMain,
            AutoSize  = false,
            AutoEllipsis = true,
        };
        y += 40;

        // Author
        _lblAuthor = new Label
        {
            Location  = new Point(pad, y),
            Size      = new Size(w, 18),
            Font      = Theme.FontSm,
            ForeColor = Theme.TextMuted,
            AutoSize  = false,
            AutoEllipsis = true,
        };
        y += 20;

        // Category
        _lblCategory = new Label
        {
            Location  = new Point(pad, y),
            Size      = new Size(w, 16),
            Font      = Theme.FontSm,
            ForeColor = Theme.TextMuted,
            AutoSize  = false,
        };
        y += 18;

        // Price
        _lblPrice = new Label
        {
            Location  = new Point(pad, y),
            Size      = new Size(w, 24),
            Font      = Theme.FontSubhead,
            ForeColor = Theme.Primary,
            AutoSize  = false,
        };
        y += 26;

        // Stock
        _lblStock = new Label
        {
            Location  = new Point(pad, y),
            Size      = new Size(w, 16),
            Font      = Theme.FontSm,
            ForeColor = Theme.TextMuted,
            AutoSize  = false,
        };

        // View Details button (pinned to bottom)
        _btnView = new Button
        {
            Text     = "View Details",
            Location = new Point(pad, 260 - pad - Theme.ButtonHeight),
            Size     = new Size(w, Theme.ButtonHeight),
        };
        UIHelper.StylePrimary(_btnView);
        _btnView.Click += (s, e) => { if (_book != null) ViewDetails?.Invoke(this, _book); };

        Controls.AddRange(new Control[]
        {
            _iconPanel, _lblTitle, _lblAuthor, _lblCategory,
            _lblPrice, _lblStock, _btnView
        });

        // Re-apply rounded corners after resize
        Load += (s, e) => UIHelper.RoundCorners(_btnView);
    }

    private void Populate()
    {
        _lblTitle.Text    = _book!.Title;
        _lblAuthor.Text   = _book.AuthorName;
        _lblCategory.Text = _book.CategoryName;
        _lblPrice.Text    = $"${_book.Price:F2}";
        _lblStock.Text    = $"Stock: {_book.Stock}";
    }
}
