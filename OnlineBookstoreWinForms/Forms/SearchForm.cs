using System;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Forms;

public class SearchForm : Form
{
    private TextBox      _txtTitle    = null!;
    private TextBox      _txtAuthor   = null!;
    private ComboBox     _cbCategory  = null!;
    private TextBox      _txtMinPrice = null!;
    private TextBox      _txtMaxPrice = null!;
    private DataGridView _dgvResults  = null!;

    public SearchForm()
    {
        Text            = "Search Books";
        ClientSize      = new Size(940, 660);
        MinimumSize     = new Size(940, 660);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Theme.Background;
        Font            = Theme.FontBase;

        BuildUI();
    }

    private void BuildUI()
    {
        SuspendLayout();

        // ── Header ──────────────────────────────────────────
        var pnlHeader = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 56,
            BackColor = Theme.Background,
            Padding   = new Padding(16, 12, 16, 0),
        };
        var lblTitle = new Label
        {
            Text      = "Search Books",
            Font      = Theme.FontHeading,
            ForeColor = Theme.TextMain,
            AutoSize  = true,
            Location  = new Point(16, 14),
        };
        pnlHeader.Controls.Add(lblTitle);

        // ── Filter panel ────────────────────────────────────
        var pnlFilter = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 80,
            BackColor = Color.White,
            Padding   = new Padding(16, 12, 16, 12),
        };
        pnlFilter.Paint += UIHelper.PaintCard;

        int fx = 16, fy = 14;
        int fieldW = 148;

        _txtTitle = new TextBox { Location = new Point(fx, fy + 20), Size = new Size(fieldW, Theme.InputHeight) };
        UIHelper.StyleTextBox(_txtTitle);
        var llTitle = new Label { Text = "Title", Font = Theme.FontLabel, ForeColor = Theme.TextSubtle, Location = new Point(fx, fy), AutoSize = true };

        fx += fieldW + 8;
        _txtAuthor = new TextBox { Location = new Point(fx, fy + 20), Size = new Size(fieldW, Theme.InputHeight) };
        UIHelper.StyleTextBox(_txtAuthor);
        var llAuthor = new Label { Text = "Author", Font = Theme.FontLabel, ForeColor = Theme.TextSubtle, Location = new Point(fx, fy), AutoSize = true };

        fx += fieldW + 8;
        _cbCategory = new ComboBox
        {
            Location     = new Point(fx, fy + 20),
            Size         = new Size(140, Theme.InputHeight),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = Theme.FontBase,
        };
        _cbCategory.Items.Add("All Categories");
        var llCat = new Label { Text = "Category", Font = Theme.FontLabel, ForeColor = Theme.TextSubtle, Location = new Point(fx, fy), AutoSize = true };

        fx += 148;
        _txtMinPrice = new TextBox { Location = new Point(fx, fy + 20), Size = new Size(80, Theme.InputHeight) };
        UIHelper.StyleTextBox(_txtMinPrice);
        var llMin = new Label { Text = "Min $", Font = Theme.FontLabel, ForeColor = Theme.TextSubtle, Location = new Point(fx, fy), AutoSize = true };

        fx += 88;
        _txtMaxPrice = new TextBox { Location = new Point(fx, fy + 20), Size = new Size(80, Theme.InputHeight) };
        UIHelper.StyleTextBox(_txtMaxPrice);
        var llMax = new Label { Text = "Max $", Font = Theme.FontLabel, ForeColor = Theme.TextSubtle, Location = new Point(fx, fy), AutoSize = true };

        fx += 88;
        var btnSearch = new Button { Text = "Search", Location = new Point(fx, fy + 20), Size = new Size(80, Theme.ButtonHeight) };
        UIHelper.StylePrimary(btnSearch);
        btnSearch.Click += BtnSearch_Click;

        fx += 88;
        var btnClear = new Button { Text = "Clear", Location = new Point(fx, fy + 20), Size = new Size(68, Theme.ButtonHeight) };
        UIHelper.StyleSecondary(btnClear);
        btnClear.Click += (s, e) =>
        {
            _txtTitle.Clear(); _txtAuthor.Clear(); _cbCategory.SelectedIndex = 0;
            _txtMinPrice.Clear(); _txtMaxPrice.Clear();
            _ = LoadResultsAsync(null, null, null, null, null);
        };

        var btnBack = new Button { Text = "Back", Location = new Point(fx + 76, fy + 20), Size = new Size(60, Theme.ButtonHeight) };
        UIHelper.StyleSecondary(btnBack);
        btnBack.Click += (s, e) => Close();

        pnlFilter.Controls.AddRange(new Control[]
        {
            llTitle, _txtTitle, llAuthor, _txtAuthor,
            llCat, _cbCategory, llMin, _txtMinPrice,
            llMax, _txtMaxPrice, btnSearch, btnClear, btnBack
        });

        // ── Results grid ─────────────────────────────────────
        _dgvResults = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGrid(_dgvResults);
        _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title",    HeaderText = "Title",    DataPropertyName = "Title"    });
        _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Author",   HeaderText = "Author",   DataPropertyName = "AuthorName" });
        _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", DataPropertyName = "CategoryName" });
        _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price",    HeaderText = "Price",    DataPropertyName = "Price"    });
        _dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock",    HeaderText = "Stock",    DataPropertyName = "Stock"    });
        _dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _dgvResults.CellDoubleClick += DgvResults_CellDoubleClick;

        Controls.AddRange(new Control[] { _dgvResults, pnlFilter, pnlHeader });

        ResumeLayout();

        Load += async (s, e) =>
        {
            var cats = await AppServices.CategoryRepo.GetAllAsync();
            foreach (var c in cats) _cbCategory.Items.Add(c);
            _cbCategory.DisplayMember = "Name";
            _cbCategory.SelectedIndex = 0;
            await LoadResultsAsync(null, null, null, null, null);
        };
    }

    private async void BtnSearch_Click(object? sender, EventArgs e)
    {
        string? title  = string.IsNullOrWhiteSpace(_txtTitle.Text)    ? null : _txtTitle.Text.Trim();
        string? author = string.IsNullOrWhiteSpace(_txtAuthor.Text)   ? null : _txtAuthor.Text.Trim();
        int?    catId  = _cbCategory.SelectedIndex > 0 && _cbCategory.SelectedItem is Category c ? c.Id : (int?)null;

        decimal? minP = decimal.TryParse(_txtMinPrice.Text, out var mn) ? mn : (decimal?)null;
        decimal? maxP = decimal.TryParse(_txtMaxPrice.Text, out var mx) ? mx : (decimal?)null;

        await LoadResultsAsync(title, author, catId, minP, maxP);
    }

    private async System.Threading.Tasks.Task LoadResultsAsync(
        string? title, string? author, int? catId, decimal? minP, decimal? maxP)
    {
        var books = await AppServices.Books.SearchBooksAsync(title, author, catId, minP, maxP);
        _dgvResults.DataSource = null;
        _dgvResults.DataSource = books;
    }

    private void DgvResults_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (_dgvResults.DataSource is System.Collections.Generic.List<Book> books)
        {
            var book = books[e.RowIndex];
            using var dlg = new BookDetailForm(book);
            dlg.ShowDialog(this);
        }
    }
}
