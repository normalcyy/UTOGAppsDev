using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OnlineBookstoreWinForms.Helpers;

public static class UIHelper
{
    public static void RoundCorners(Control control, int radius = Theme.CornerRadius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(0, 0, d, d, 180, 90);
        path.AddArc(control.Width - d, 0, d, d, 270, 90);
        path.AddArc(control.Width - d, control.Height - d, d, d, 0, 90);
        path.AddArc(0, control.Height - d, d, d, 90, 90);
        path.CloseFigure();
        control.Region = new Region(path);
    }

    public static void StylePrimary(Button btn)
    {
        btn.FlatStyle  = FlatStyle.Flat;
        btn.BackColor  = Theme.Primary;
        btn.ForeColor  = Color.White;
        btn.Font       = Theme.FontBold;
        btn.Height     = Theme.ButtonHeight;
        btn.Cursor     = Cursors.Hand;
        btn.FlatAppearance.BorderSize          = 0;
        btn.FlatAppearance.MouseOverBackColor  = Theme.PrimaryHover;
        btn.FlatAppearance.MouseDownBackColor  = Theme.PrimaryActive;
        RoundCorners(btn);
    }

    public static void StyleSecondary(Button btn)
    {
        btn.FlatStyle  = FlatStyle.Flat;
        btn.BackColor  = Theme.SecondaryBg;
        btn.ForeColor  = Theme.TextSubtle;
        btn.Font       = Theme.FontBold;
        btn.Height     = Theme.ButtonHeight;
        btn.Cursor     = Cursors.Hand;
        btn.FlatAppearance.BorderSize          = 0;
        btn.FlatAppearance.MouseOverBackColor  = Theme.SecondaryHover;
        RoundCorners(btn);
    }

    public static void StyleDestructive(Button btn)
    {
        btn.FlatStyle  = FlatStyle.Flat;
        btn.BackColor  = Theme.Destructive;
        btn.ForeColor  = Color.White;
        btn.Font       = Theme.FontBold;
        btn.Height     = Theme.ButtonHeight;
        btn.Cursor     = Cursors.Hand;
        btn.FlatAppearance.BorderSize          = 0;
        btn.FlatAppearance.MouseOverBackColor  = Theme.DestructiveHov;
        RoundCorners(btn);
    }

    public static void StyleSuccess(Button btn)
    {
        btn.FlatStyle  = FlatStyle.Flat;
        btn.BackColor  = Theme.Success;
        btn.ForeColor  = Color.White;
        btn.Font       = Theme.FontBold;
        btn.Height     = Theme.ButtonHeight;
        btn.Cursor     = Cursors.Hand;
        btn.FlatAppearance.BorderSize          = 0;
        btn.FlatAppearance.MouseOverBackColor  = Theme.SuccessHover;
        RoundCorners(btn);
    }

    public static void StyleTextBox(TextBox txt)
    {
        txt.BorderStyle = BorderStyle.FixedSingle;
        txt.BackColor   = Color.White;
        txt.ForeColor   = Theme.TextMain;
        txt.Font        = Theme.FontBase;
        txt.Height      = Theme.InputHeight;
    }

    public static void StyleDataGrid(DataGridView dgv)
    {
        dgv.BorderStyle              = BorderStyle.FixedSingle;
        dgv.BackgroundColor          = Color.White;
        dgv.GridColor                = Theme.Border;
        dgv.RowHeadersVisible        = false;
        dgv.AllowUserToAddRows       = false;
        dgv.AllowUserToDeleteRows    = false;
        dgv.ReadOnly                 = true;
        dgv.SelectionMode            = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect              = false;
        dgv.AutoSizeColumnsMode      = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.Font                     = Theme.FontBase;
        dgv.DefaultCellStyle.BackColor            = Color.White;
        dgv.DefaultCellStyle.ForeColor            = Theme.TextMain;
        dgv.DefaultCellStyle.SelectionBackColor   = Theme.SelectedRow;
        dgv.DefaultCellStyle.SelectionForeColor   = Theme.TextMain;
        dgv.DefaultCellStyle.Padding              = new Padding(6, 0, 6, 0);
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Theme.Background;
        dgv.ColumnHeadersDefaultCellStyle.BackColor   = Theme.SecondaryBg;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor   = Theme.TextSubtle;
        dgv.ColumnHeadersDefaultCellStyle.Font        = Theme.FontBold;
        dgv.ColumnHeadersDefaultCellStyle.Padding     = new Padding(6, 0, 6, 0);
        dgv.ColumnHeadersHeight      = 40;
        dgv.RowTemplate.Height       = 44;
        dgv.EnableHeadersVisualStyles = false;

        dgv.CellMouseEnter += (s, e) =>
        {
            if (e.RowIndex >= 0)
                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Theme.HoverRow;
        };
        dgv.CellMouseLeave += (s, e) =>
        {
            if (e.RowIndex >= 0)
                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    e.RowIndex % 2 == 0 ? Color.White : Theme.Background;
        };
    }

    public static void PaintCard(object sender, PaintEventArgs e)
    {
        var panel = (Panel)sender;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using (var pen = new Pen(Theme.Border, 1))
        {
            var rect = new Rectangle(1, 1, panel.Width - 2, panel.Height - 2);
            int r = Theme.CornerRadius * 2;
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            g.FillPath(new SolidBrush(Color.White), path);
            g.DrawPath(pen, path);
        }
    }

    public static void StyleTabControl(TabControl tabControl)
    {
        tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabControl.DrawItem += (s, e) =>
        {
            var tab  = tabControl.TabPages[e.Index];
            bool sel = e.Index == tabControl.SelectedIndex;
            e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
            if (sel)
            {
                var underline = new Rectangle(e.Bounds.X, e.Bounds.Bottom - 2, e.Bounds.Width, 2);
                e.Graphics.FillRectangle(new SolidBrush(Theme.Primary), underline);
            }
            var font  = sel ? Theme.FontBold : Theme.FontBase;
            var color = sel ? Theme.Primary  : Theme.TextMuted;
            var sf    = new StringFormat
            {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            e.Graphics.DrawString(tab.Text, font, new SolidBrush(color), e.Bounds, sf);
        };
    }
}
