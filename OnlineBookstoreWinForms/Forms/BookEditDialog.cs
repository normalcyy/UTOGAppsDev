using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Helpers;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Forms;

public class BookEditDialog : Form
{
    private readonly Book?         _existing;
    private readonly List<Author>  _authors;
    private readonly List<Category> _categories;

    private TextBox  _txtTitle       = null!;
    private TextBox  _txtISBN        = null!;
    private TextBox  _txtPrice       = null!;
    private TextBox  _txtStock       = null!;
    private TextBox  _txtDescription = null!;
    private ComboBox _cbAuthor       = null!;
    private ComboBox _cbCategory     = null!;

    public Book? Result { get; private set; }

    public BookEditDialog(Book? existing, List<Author> authors, List<Category> categories)
    {
        _existing   = existing;
        _authors    = authors;
        _categories = categories;

        Text            = existing == null ? "Add Book" : "Edit Book";
        ClientSize      = new Size(440, 480);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;
        BackColor       = Theme.Background;
        Font            = Theme.FontBase;

        BuildUI();
    }

    private void BuildUI()
    {
        int pad = 16, w = 408, y = 16;

        Label MakeLbl(string text)
        {
            var l = new Label { Text = text, AutoSize = true, Location = new Point(pad, y), Font = Theme.FontLabel, ForeColor = Theme.TextSubtle };
            Controls.Add(l);
            y += 20;
            return l;
        }

        TextBox MakeTxt(string? value = null)
        {
            var t = new TextBox { Location = new Point(pad, y), Size = new Size(w, Theme.InputHeight), Text = value ?? string.Empty };
            UIHelper.StyleTextBox(t);
            Controls.Add(t);
            y += Theme.InputHeight + 8;
            return t;
        }

        ComboBox MakeCb()
        {
            var cb = new ComboBox { Location = new Point(pad, y), Size = new Size(w, Theme.InputHeight), DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBase };
            Controls.Add(cb);
            y += Theme.InputHeight + 8;
            return cb;
        }

        MakeLbl("Title");       _txtTitle       = MakeTxt(_existing?.Title);
        MakeLbl("ISBN");        _txtISBN        = MakeTxt(_existing?.ISBN);
        MakeLbl("Price");       _txtPrice       = MakeTxt(_existing?.Price.ToString("F2"));
        MakeLbl("Stock");       _txtStock       = MakeTxt(_existing?.Stock.ToString());
        MakeLbl("Description"); _txtDescription = MakeTxt(_existing?.Description);

        MakeLbl("Author");
        _cbAuthor = MakeCb();
        _cbAuthor.DisplayMember = "Name";
        _cbAuthor.ValueMember   = "Id";
        foreach (var a in _authors) _cbAuthor.Items.Add(a);
        if (_existing != null)
            for (int i = 0; i < _cbAuthor.Items.Count; i++)
                if (((Author)_cbAuthor.Items[i]!).Id == _existing.AuthorId) { _cbAuthor.SelectedIndex = i; break; }
        if (_cbAuthor.SelectedIndex < 0 && _cbAuthor.Items.Count > 0) _cbAuthor.SelectedIndex = 0;

        MakeLbl("Category");
        _cbCategory = MakeCb();
        _cbCategory.DisplayMember = "Name";
        _cbCategory.ValueMember   = "Id";
        foreach (var c in _categories) _cbCategory.Items.Add(c);
        if (_existing != null)
            for (int i = 0; i < _cbCategory.Items.Count; i++)
                if (((Category)_cbCategory.Items[i]!).Id == _existing.CategoryId) { _cbCategory.SelectedIndex = i; break; }
        if (_cbCategory.SelectedIndex < 0 && _cbCategory.Items.Count > 0) _cbCategory.SelectedIndex = 0;

        y += 4;
        var btnSave   = new Button { Text = "Save", Location = new Point(pad, y), Size = new Size(190, Theme.ButtonHeight) };
        var btnCancel = new Button { Text = "Cancel", Location = new Point(pad + 198, y), Size = new Size(210, Theme.ButtonHeight) };
        UIHelper.StylePrimary(btnSave);
        UIHelper.StyleSecondary(btnCancel);
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.AddRange(new Control[] { btnSave, btnCancel });

        Load += (s, e) => { UIHelper.RoundCorners(btnSave); UIHelper.RoundCorners(btnCancel); };
        AcceptButton = btnSave;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!decimal.TryParse(_txtPrice.Text, out var price))  { MessageBox.Show("Invalid price.");  return; }
        if (!int.TryParse(_txtStock.Text, out var stock))      { MessageBox.Show("Invalid stock.");  return; }
        if (_cbAuthor.SelectedItem is not Author author)       { MessageBox.Show("Select author.");  return; }
        if (_cbCategory.SelectedItem is not Category category) { MessageBox.Show("Select category."); return; }

        Result = new Book
        {
            Id          = _existing?.Id ?? 0,
            Title       = _txtTitle.Text.Trim(),
            ISBN        = _txtISBN.Text.Trim(),
            Price       = price,
            Stock       = stock,
            Description = _txtDescription.Text.Trim(),
            AuthorId    = author.Id,
            CategoryId  = category.Id,
        };

        DialogResult = DialogResult.OK;
        Close();
    }
}
