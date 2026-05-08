using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Forms;

public class AdminPanelForm : Form
{
    private Panel        _pnlError    = null!;
    private Label        _lblError    = null!;

    // Books tab
    private DataGridView _dgvBooks    = null!;

    // Authors tab
    private DataGridView _dgvAuthors  = null!;

    // Categories tab
    private DataGridView _dgvCats     = null!;

    // Orders tab
    private DataGridView _dgvOrders   = null!;
    private DataGridView _dgvOrderItems = null!;
    private ComboBox     _cbStatus    = null!;

    public AdminPanelForm()
    {
        Text            = "📚 BookStore · Admin";
        ClientSize      = new Size(1060, 740);
        MinimumSize     = new Size(1060, 740);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Theme.Background;
        Font            = Theme.FontBase;

        BuildUI();
    }

    private void BuildUI()
    {
        SuspendLayout();

        // ── NavBar ──────────────────────────────────────────
        var pnlNav = new Panel { Dock = DockStyle.Top, Height = Theme.NavHeight, BackColor = Theme.NavBar };
        var lblBrand = new Label { Text = "📚  BookStore · Admin", Font = Theme.FontBold, ForeColor = Color.White, AutoSize = true, Location = new Point(16, 15) };
        var btnLogout = new Button { Text = "Logout", Size = new Size(80, 32), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        UIHelper.StyleSecondary(btnLogout);
        btnLogout.Location = new Point(1060 - 16 - 80, 10);
        btnLogout.Click += (s, e) =>
        {
            SessionManager.Logout();
            new LoginForm().Show();
            Close();
        };
        pnlNav.Controls.AddRange(new Control[] { lblBrand, btnLogout });

        // ── Error panel ─────────────────────────────────────
        _pnlError = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Theme.ErrorBg, Visible = false };
        _lblError = new Label { Dock = DockStyle.Fill, ForeColor = Theme.Destructive, Font = Theme.FontBase, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(16, 0, 0, 0) };
        _pnlError.Controls.Add(_lblError);

        // ── Tab control ─────────────────────────────────────
        var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(12, 6) };
        UIHelper.StyleTabControl(tabs);

        var tabBooks  = new TabPage("Books")       { BackColor = Color.White, Padding = new Padding(8) };
        var tabAuthors= new TabPage("Authors")     { BackColor = Color.White, Padding = new Padding(8) };
        var tabCats   = new TabPage("Categories")  { BackColor = Color.White, Padding = new Padding(8) };
        var tabOrders = new TabPage("Orders")      { BackColor = Color.White, Padding = new Padding(8) };

        BuildBooksTab(tabBooks);
        BuildAuthorsTab(tabAuthors);
        BuildCategoriesTab(tabCats);
        BuildOrdersTab(tabOrders);

        tabs.TabPages.AddRange(new[] { tabBooks, tabAuthors, tabCats, tabOrders });

        Controls.AddRange(new Control[] { tabs, _pnlError, pnlNav });

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
            await RefreshBooksAsync();
            await RefreshAuthorsAsync();
            await RefreshCatsAsync();
            await RefreshOrdersAsync();
        };
    }

    // ────────────────────────────────────────────────────────
    // BOOKS TAB
    // ────────────────────────────────────────────────────────
    private void BuildBooksTab(TabPage tab)
    {
        var pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.White };

        var btnAdd = new Button { Text = "Add Book", Size = new Size(100, Theme.ButtonHeight), Location = new Point(8, 8) };
        UIHelper.StyleSuccess(btnAdd);
        btnAdd.Click += BtnBookAdd_Click;

        var btnEdit = new Button { Text = "Edit", Size = new Size(80, Theme.ButtonHeight), Location = new Point(116, 8) };
        UIHelper.StylePrimary(btnEdit);
        btnEdit.Click += BtnBookEdit_Click;

        var btnDel = new Button { Text = "Delete", Size = new Size(80, Theme.ButtonHeight), Location = new Point(204, 8) };
        UIHelper.StyleDestructive(btnDel);
        btnDel.Click += BtnBookDelete_Click;

        pnlToolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDel });

        _dgvBooks = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGrid(_dgvBooks);
        _dgvBooks.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID",         DataPropertyName = "Id",           Name = "Id"       });
        _dgvBooks.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Title",      DataPropertyName = "Title",        Name = "Title"    });
        _dgvBooks.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Author",     DataPropertyName = "AuthorName",   Name = "Author"   });
        _dgvBooks.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Category",   DataPropertyName = "CategoryName", Name = "Category" });
        _dgvBooks.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Price",      DataPropertyName = "Price",        Name = "Price"    });
        _dgvBooks.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock",      DataPropertyName = "Stock",        Name = "Stock"    });
        _dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        tab.Controls.AddRange(new Control[] { _dgvBooks, pnlToolbar });
    }

    private async void BtnBookAdd_Click(object? sender, EventArgs e)
    {
        var authors    = await AppServices.AuthorRepo.GetAllAsync();
        var categories = await AppServices.CategoryRepo.GetAllAsync();
        using var dlg  = new BookEditDialog(null, authors, categories);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        await AppServices.Books.AddBookAsync(dlg.Result!);
        await RefreshBooksAsync();
    }

    private async void BtnBookEdit_Click(object? sender, EventArgs e)
    {
        if (_dgvBooks.SelectedRows.Count == 0) return;
        if (_dgvBooks.DataSource is not List<Book> books) return;
        var book = books[_dgvBooks.SelectedRows[0].Index];
        var authors    = await AppServices.AuthorRepo.GetAllAsync();
        var categories = await AppServices.CategoryRepo.GetAllAsync();
        using var dlg  = new BookEditDialog(book, authors, categories);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        await AppServices.Books.UpdateBookAsync(dlg.Result!);
        await RefreshBooksAsync();
    }

    private async void BtnBookDelete_Click(object? sender, EventArgs e)
    {
        if (_dgvBooks.SelectedRows.Count == 0) return;
        if (_dgvBooks.DataSource is not List<Book> books) return;
        var book = books[_dgvBooks.SelectedRows[0].Index];
        if (MessageBox.Show($"Delete \"{book.Title}\"?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        var (ok, err) = await AppServices.Books.DeleteBookAsync(book.Id);
        if (!ok) ShowError(err!);
        else { HideError(); await RefreshBooksAsync(); }
    }

    private async System.Threading.Tasks.Task RefreshBooksAsync()
    {
        var books = await AppServices.Books.GetAllBooksAsync();
        _dgvBooks.DataSource = null;
        _dgvBooks.DataSource = books;
    }

    // ────────────────────────────────────────────────────────
    // AUTHORS TAB
    // ────────────────────────────────────────────────────────
    private void BuildAuthorsTab(TabPage tab)
    {
        var pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.White };

        var btnAdd = new Button { Text = "Add Author", Size = new Size(110, Theme.ButtonHeight), Location = new Point(8, 8) };
        UIHelper.StyleSuccess(btnAdd);
        btnAdd.Click += BtnAuthorAdd_Click;

        var btnEdit = new Button { Text = "Edit", Size = new Size(80, Theme.ButtonHeight), Location = new Point(126, 8) };
        UIHelper.StylePrimary(btnEdit);
        btnEdit.Click += BtnAuthorEdit_Click;

        var btnDel = new Button { Text = "Delete", Size = new Size(80, Theme.ButtonHeight), Location = new Point(214, 8) };
        UIHelper.StyleDestructive(btnDel);
        btnDel.Click += BtnAuthorDelete_Click;

        pnlToolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDel });

        _dgvAuthors = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGrid(_dgvAuthors);
        _dgvAuthors.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID",   DataPropertyName = "Id",   Name = "Id"   });
        _dgvAuthors.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Name = "Name" });
        _dgvAuthors.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Bio",  DataPropertyName = "Bio",  Name = "Bio"  });
        _dgvAuthors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        tab.Controls.AddRange(new Control[] { _dgvAuthors, pnlToolbar });
    }

    private async void BtnAuthorAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new AuthorEditDialog(null);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        await AppServices.AuthorRepo.AddAsync(dlg.Result!);
        await RefreshAuthorsAsync();
    }

    private async void BtnAuthorEdit_Click(object? sender, EventArgs e)
    {
        if (_dgvAuthors.SelectedRows.Count == 0) return;
        if (_dgvAuthors.DataSource is not List<Author> authors) return;
        var author = authors[_dgvAuthors.SelectedRows[0].Index];
        using var dlg = new AuthorEditDialog(author);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        await AppServices.AuthorRepo.UpdateAsync(dlg.Result!);
        await RefreshAuthorsAsync();
    }

    private async void BtnAuthorDelete_Click(object? sender, EventArgs e)
    {
        if (_dgvAuthors.SelectedRows.Count == 0) return;
        if (_dgvAuthors.DataSource is not List<Author> authors) return;
        var author = authors[_dgvAuthors.SelectedRows[0].Index];
        if (MessageBox.Show($"Delete author \"{author.Name}\"?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        await AppServices.AuthorRepo.DeleteAsync(author.Id);
        await RefreshAuthorsAsync();
    }

    private async System.Threading.Tasks.Task RefreshAuthorsAsync()
    {
        var authors = await AppServices.AuthorRepo.GetAllAsync();
        _dgvAuthors.DataSource = null;
        _dgvAuthors.DataSource = authors;
    }

    // ────────────────────────────────────────────────────────
    // CATEGORIES TAB
    // ────────────────────────────────────────────────────────
    private void BuildCategoriesTab(TabPage tab)
    {
        var pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.White };

        var btnAdd = new Button { Text = "Add Category", Size = new Size(120, Theme.ButtonHeight), Location = new Point(8, 8) };
        UIHelper.StyleSuccess(btnAdd);
        btnAdd.Click += BtnCatAdd_Click;

        var btnEdit = new Button { Text = "Edit", Size = new Size(80, Theme.ButtonHeight), Location = new Point(136, 8) };
        UIHelper.StylePrimary(btnEdit);
        btnEdit.Click += BtnCatEdit_Click;

        var btnDel = new Button { Text = "Delete", Size = new Size(80, Theme.ButtonHeight), Location = new Point(224, 8) };
        UIHelper.StyleDestructive(btnDel);
        btnDel.Click += BtnCatDelete_Click;

        pnlToolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDel });

        _dgvCats = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGrid(_dgvCats);
        _dgvCats.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID",   DataPropertyName = "Id",   Name = "Id"   });
        _dgvCats.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Name = "Name" });
        _dgvCats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        tab.Controls.AddRange(new Control[] { _dgvCats, pnlToolbar });
    }

    private async void BtnCatAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new InputDialog("Category name:", "Add Category");
        if (dlg.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.Value)) return;
        await AppServices.CategoryRepo.AddAsync(new Category { Name = dlg.Value.Trim() });
        await RefreshCatsAsync();
    }

    private async void BtnCatEdit_Click(object? sender, EventArgs e)
    {
        if (_dgvCats.SelectedRows.Count == 0) return;
        if (_dgvCats.DataSource is not List<Category> cats) return;
        var cat = cats[_dgvCats.SelectedRows[0].Index];
        using var dlg = new InputDialog("Category name:", "Edit Category", cat.Name);
        if (dlg.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.Value)) return;
        cat.Name = dlg.Value.Trim();
        await AppServices.CategoryRepo.UpdateAsync(cat);
        await RefreshCatsAsync();
    }

    private async void BtnCatDelete_Click(object? sender, EventArgs e)
    {
        if (_dgvCats.SelectedRows.Count == 0) return;
        if (_dgvCats.DataSource is not List<Category> cats) return;
        var cat = cats[_dgvCats.SelectedRows[0].Index];
        if (MessageBox.Show($"Delete category \"{cat.Name}\"?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        await AppServices.CategoryRepo.DeleteAsync(cat.Id);
        await RefreshCatsAsync();
    }

    private async System.Threading.Tasks.Task RefreshCatsAsync()
    {
        var cats = await AppServices.CategoryRepo.GetAllAsync();
        _dgvCats.DataSource = null;
        _dgvCats.DataSource = cats;
    }

    // ────────────────────────────────────────────────────────
    // ORDERS TAB
    // ────────────────────────────────────────────────────────
    private void BuildOrdersTab(TabPage tab)
    {
        var pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.White };

        var lblStatus = new Label { Text = "Status:", AutoSize = true, Font = Theme.FontLabel, ForeColor = Theme.TextSubtle, Location = new Point(8, 18) };

        _cbStatus = new ComboBox
        {
            Location      = new Point(60, 12),
            Size          = new Size(140, Theme.ButtonHeight),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = Theme.FontBase,
        };
        foreach (var s in Constants.OrderStatus.All) _cbStatus.Items.Add(s);
        _cbStatus.SelectedIndex = 0;

        var btnUpdate = new Button { Text = "Update Status", Size = new Size(120, Theme.ButtonHeight), Location = new Point(208, 12) };
        UIHelper.StylePrimary(btnUpdate);
        btnUpdate.Click += BtnOrderUpdate_Click;

        pnlToolbar.Controls.AddRange(new Control[] { lblStatus, _cbStatus, btnUpdate });

        // Orders grid (top half)
        _dgvOrders = new DataGridView { Dock = DockStyle.Top, Height = 240 };
        UIHelper.StyleDataGrid(_dgvOrders);
        _dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Order #",  DataPropertyName = "Id",          Name = "OrderId" });
        _dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "User ID",  DataPropertyName = "UserId",      Name = "UserId"  });
        _dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date",     DataPropertyName = "OrderDate",   Name = "Date"    });
        _dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status",   DataPropertyName = "Status",      Name = "Status"  });
        _dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total",    DataPropertyName = "TotalAmount", Name = "Total"   });
        _dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;

        // Items grid (bottom half)
        var lblItems = new Label { Text = "Order Items", Font = Theme.FontBold, ForeColor = Theme.TextSubtle, Dock = DockStyle.Top, Height = 24, TextAlign = ContentAlignment.BottomLeft, Padding = new Padding(4, 0, 0, 0) };

        _dgvOrderItems = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGrid(_dgvOrderItems);
        _dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Book",       DataPropertyName = "BookTitle", Name = "Book"  });
        _dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Qty",        DataPropertyName = "Quantity",  Name = "Qty"   });
        _dgvOrderItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Unit Price", DataPropertyName = "UnitPrice", Name = "Price" });
        _dgvOrderItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        tab.Controls.AddRange(new Control[] { _dgvOrderItems, lblItems, _dgvOrders, pnlToolbar });
    }

    private async void BtnOrderUpdate_Click(object? sender, EventArgs e)
    {
        if (_dgvOrders.SelectedRows.Count == 0) return;
        if (_dgvOrders.DataSource is not List<Order> orders) return;
        var order     = orders[_dgvOrders.SelectedRows[0].Index];
        var newStatus = _cbStatus.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(newStatus)) return;
        await AppServices.Orders.UpdateOrderStatusAsync(order.Id, newStatus);
        await RefreshOrdersAsync();
    }

    private void DgvOrders_SelectionChanged(object? sender, EventArgs e)
    {
        if (_dgvOrders.SelectedRows.Count == 0) return;
        if (_dgvOrders.DataSource is not List<Order> orders) return;
        int idx = _dgvOrders.SelectedRows[0].Index;
        if (idx >= 0 && idx < orders.Count)
        {
            _dgvOrderItems.DataSource = null;
            _dgvOrderItems.DataSource = orders[idx].Items;
        }
    }

    private async System.Threading.Tasks.Task RefreshOrdersAsync()
    {
        var orders = await AppServices.Orders.GetAllOrdersAsync();
        _dgvOrders.DataSource = null;
        _dgvOrders.DataSource = orders;
    }

    // ────────────────────────────────────────────────────────
    // Error helpers
    // ────────────────────────────────────────────────────────
    private void ShowError(string msg)
    {
        _lblError.Text    = msg;
        _pnlError.Visible = true;
    }

    private void HideError() => _pnlError.Visible = false;
}
