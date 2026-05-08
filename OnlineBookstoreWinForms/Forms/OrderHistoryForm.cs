using System;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Forms;

public class OrderHistoryForm : Form
{
    private DataGridView _dgvOrders = null!;
    private DataGridView _dgvItems  = null!;
    private Panel        _pnlItems  = null!;

    public OrderHistoryForm()
    {
        Text            = "Order History";
        ClientSize      = new Size(820, 640);
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
        var lblTitle  = new Label { Text = "Order History", Font = Theme.FontHeading, ForeColor = Theme.TextMain, AutoSize = true, Location = new Point(16, 14) };
        pnlHeader.Controls.Add(lblTitle);

        // ── Bottom bar ──────────────────────────────────────
        var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 56, BackColor = Theme.Background, Padding = new Padding(12) };
        var btnBack   = new Button { Text = "Back", Size = new Size(80, Theme.ButtonHeight), Location = new Point(12, 10) };
        UIHelper.StyleSecondary(btnBack);
        btnBack.Click += (s, e) => Close();
        pnlBottom.Controls.Add(btnBack);

        // ── Orders grid ─────────────────────────────────────
        _dgvOrders = new DataGridView { Dock = DockStyle.Top, Height = 260 };
        UIHelper.StyleDataGrid(_dgvOrders);
        _dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Order #",  DataPropertyName = "Id",          Name = "OrderId"  });
        _dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date",     DataPropertyName = "OrderDate",   Name = "Date"     });
        _dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status",   DataPropertyName = "Status",      Name = "Status"   });
        _dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total",    DataPropertyName = "TotalAmount", Name = "Total"    });
        _dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;

        // ── Items sub-panel ──────────────────────────────────
        _pnlItems = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background, Padding = new Padding(0, 8, 0, 0) };

        var lblItems = new Label { Text = "Order Items", Font = Theme.FontBold, ForeColor = Theme.TextSubtle, AutoSize = true, Location = new Point(0, 0) };

        _dgvItems = new DataGridView { Location = new Point(0, 22), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
        _dgvItems.Size = new Size(820, 240);
        UIHelper.StyleDataGrid(_dgvItems);
        _dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Book Title", DataPropertyName = "BookTitle", Name = "BookTitle" });
        _dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Qty",        DataPropertyName = "Quantity",  Name = "Qty"       });
        _dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Unit Price", DataPropertyName = "UnitPrice", Name = "UnitPrice" });
        _dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _pnlItems.Controls.AddRange(new Control[] { lblItems, _dgvItems });

        Controls.AddRange(new Control[] { _pnlItems, _dgvOrders, pnlHeader, pnlBottom });
        ResumeLayout();

        Load += async (s, e) =>
        {
            if (!SessionManager.IsLoggedIn) return;
            var orders = await AppServices.Orders.GetOrderHistoryAsync(SessionManager.CurrentUser!.Id);
            _dgvOrders.DataSource = null;
            _dgvOrders.DataSource = orders;
        };
    }

    private void DgvOrders_SelectionChanged(object? sender, EventArgs e)
    {
        if (_dgvOrders.SelectedRows.Count == 0) return;
        if (_dgvOrders.DataSource is not System.Collections.Generic.List<Order> orders) return;

        int idx = _dgvOrders.SelectedRows[0].Index;
        if (idx >= 0 && idx < orders.Count)
        {
            _dgvItems.DataSource = null;
            _dgvItems.DataSource = orders[idx].Items;
        }
    }
}
