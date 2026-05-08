using System;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Forms;

public class WishlistForm : Form
{
    private DataGridView _dgvWishlist = null!;
    private Panel        _pnlMessage  = null!;
    private Label        _lblMessage  = null!;

    public WishlistForm()
    {
        Text            = "My Wishlist";
        ClientSize      = new Size(720, 560);
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

        // ── Header ──────────────────────────────────────────
        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Theme.Background };
        var lblTitle  = new Label
        {
            Text      = "My Wishlist",
            Font      = Theme.FontHeading,
            ForeColor = Theme.TextMain,
            AutoSize  = true,
            Location  = new Point(16, 14),
        };
        pnlHeader.Controls.Add(lblTitle);

        // ── Bottom bar ──────────────────────────────────────
        var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Theme.Background, Padding = new Padding(12) };

        var btnOrder = new Button { Text = "Place Order", Size = new Size(120, Theme.ButtonHeight), Location = new Point(12, 12) };
        UIHelper.StyleSuccess(btnOrder);
        btnOrder.Click += BtnOrder_Click;

        var btnBack = new Button { Text = "Back", Size = new Size(80, Theme.ButtonHeight), Location = new Point(140, 12) };
        UIHelper.StyleSecondary(btnBack);
        btnBack.Click += (s, e) => Close();

        _pnlMessage = new Panel
        {
            Location  = new Point(232, 12),
            Size      = new Size(460, 36),
            BackColor = Theme.SuccessBg,
            Visible   = false,
        };
        _lblMessage = new Label { Dock = DockStyle.Fill, Font = Theme.FontBase, ForeColor = Theme.Success, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) };
        _pnlMessage.Controls.Add(_lblMessage);

        pnlBottom.Controls.AddRange(new Control[] { btnOrder, btnBack, _pnlMessage });

        // ── DataGridView ─────────────────────────────────────
        _dgvWishlist = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGrid(_dgvWishlist);
        _dgvWishlist.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title",    HeaderText = "Title",      DataPropertyName = "BookTitle" });
        _dgvWishlist.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price",    HeaderText = "Price",      DataPropertyName = "BookPrice" });
        _dgvWishlist.Columns.Add(new DataGridViewTextBoxColumn { Name = "AddedAt",  HeaderText = "Added",      DataPropertyName = "AddedAt"   });

        var btnRemoveCol = new DataGridViewButtonColumn
        {
            Name       = "Remove",
            HeaderText = "Action",
            Text       = "Remove",
            UseColumnTextForButtonValue = true,
            Width      = 90,
            FlatStyle  = FlatStyle.Flat,
        };
        _dgvWishlist.Columns.Add(btnRemoveCol);
        _dgvWishlist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _dgvWishlist.CellClick += DgvWishlist_CellClick;

        Controls.AddRange(new Control[] { _dgvWishlist, pnlBottom, pnlHeader });
        ResumeLayout();

        Load += async (s, e) => await RefreshAsync();
    }

    private async System.Threading.Tasks.Task RefreshAsync()
    {
        if (!SessionManager.IsLoggedIn) return;
        var items = await AppServices.Wishlist.GetWishlistAsync(SessionManager.CurrentUser!.Id);
        _dgvWishlist.DataSource = null;
        _dgvWishlist.DataSource = items;
    }

    private async void DgvWishlist_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != _dgvWishlist.Columns["Remove"]!.Index) return;
        if (_dgvWishlist.DataSource is not System.Collections.Generic.List<WishlistItem> items) return;

        var item = items[e.RowIndex];
        await AppServices.Wishlist.RemoveFromWishlistAsync(SessionManager.CurrentUser!.Id, item.BookId);
        await RefreshAsync();
    }

    private async void BtnOrder_Click(object? sender, EventArgs e)
    {
        if (!SessionManager.IsLoggedIn) return;

        var items = await AppServices.Wishlist.GetWishlistAsync(SessionManager.CurrentUser!.Id);
        if (items.Count == 0)
        {
            ShowMessage("Wishlist is empty.", success: false);
            return;
        }

        var order = await AppServices.Orders.PlaceOrderAsync(SessionManager.CurrentUser!.Id, items);
        ShowMessage($"Order #{order.Id} placed! Total: ${order.TotalAmount:F2}", success: true);
        await RefreshAsync();
    }

    private void ShowMessage(string msg, bool success)
    {
        _lblMessage.Text      = msg;
        _pnlMessage.BackColor = success ? Theme.SuccessBg : Theme.ErrorBg;
        _lblMessage.ForeColor = success ? Theme.Success   : Theme.Destructive;
        _pnlMessage.Visible   = true;
    }
}
