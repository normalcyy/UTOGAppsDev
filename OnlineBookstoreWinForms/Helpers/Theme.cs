using System.Drawing;

namespace OnlineBookstoreWinForms.Helpers;

public static class Theme
{
    // === COLORS (from primitive-tokens.md) ===
    public static readonly Color Primary        = ColorTranslator.FromHtml("#2563EB");
    public static readonly Color PrimaryHover   = ColorTranslator.FromHtml("#1D4ED8");
    public static readonly Color PrimaryActive  = ColorTranslator.FromHtml("#1E40AF");
    public static readonly Color NavBar         = ColorTranslator.FromHtml("#1E3A8A");
    public static readonly Color Background     = ColorTranslator.FromHtml("#F9FAFB");
    public static readonly Color Card           = Color.White;
    public static readonly Color Border         = ColorTranslator.FromHtml("#E5E7EB");
    public static readonly Color MutedBorder    = ColorTranslator.FromHtml("#D1D5DB");
    public static readonly Color TextMain       = ColorTranslator.FromHtml("#111827");
    public static readonly Color TextSubtle     = ColorTranslator.FromHtml("#374151");
    public static readonly Color TextMuted      = ColorTranslator.FromHtml("#6B7280");
    public static readonly Color SecondaryBg    = ColorTranslator.FromHtml("#F3F4F6");
    public static readonly Color SecondaryHover = ColorTranslator.FromHtml("#E5E7EB");
    public static readonly Color Destructive    = ColorTranslator.FromHtml("#DC2626");
    public static readonly Color DestructiveHov = ColorTranslator.FromHtml("#B91C1C");
    public static readonly Color Success        = ColorTranslator.FromHtml("#16A34A");
    public static readonly Color SuccessHover   = ColorTranslator.FromHtml("#15803D");
    public static readonly Color ErrorBg        = ColorTranslator.FromHtml("#FEF2F2");
    public static readonly Color ErrorBorder    = ColorTranslator.FromHtml("#FECACA");
    public static readonly Color SuccessBg      = ColorTranslator.FromHtml("#F0FDF4");
    public static readonly Color SuccessBorder  = ColorTranslator.FromHtml("#BBF7D0");
    public static readonly Color HoverRow       = ColorTranslator.FromHtml("#EFF6FF");
    public static readonly Color SelectedRow    = ColorTranslator.FromHtml("#DBEAFE");

    // === FONTS ===
    public static readonly Font FontBase    = new Font("Segoe UI", 9.5f,  FontStyle.Regular);
    public static readonly Font FontSm      = new Font("Segoe UI", 8.5f,  FontStyle.Regular);
    public static readonly Font FontLg      = new Font("Segoe UI", 11f,   FontStyle.Regular);
    public static readonly Font FontBold    = new Font("Segoe UI", 9.5f,  FontStyle.Bold);
    public static readonly Font FontHeading = new Font("Segoe UI", 14f,   FontStyle.Bold);
    public static readonly Font FontSubhead = new Font("Segoe UI", 11f,   FontStyle.Bold);
    public static readonly Font FontLabel   = new Font("Segoe UI", 9f,    FontStyle.Regular);
    public static readonly Font FontMono    = new Font("Consolas",  9f,   FontStyle.Regular);

    // === SIZES (from component-specs.md) ===
    public const int ButtonHeight  = 36;
    public const int InputHeight   = 36;
    public const int NavHeight     = 52;
    public const int CornerRadius  = 6;
    public const int CardPadding   = 20;
}
