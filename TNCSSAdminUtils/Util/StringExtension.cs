using System.Globalization;
using System.Text;

namespace TNCSSAdminUtils.Util;

public static class StringExtension
{
    public static int GetDisplayWidth(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        int width = 0;
        foreach (char c in text)
        {
            bool isWideChar = CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.OtherLetter 
                              || char.GetUnicodeCategory(c) == UnicodeCategory.OtherSymbol
                              || (c >= '\u3000' && c <= '\u9FFF')  // CJK Unified Ideographs, Hiragana, Katakana, etc.
                              || (c >= '\uFF00' && c <= '\uFFEF'); // Fullwidth forms, symbols, etc.

            width += isWideChar ? 2 : 1;
        }
        return width;
    }

    public static string PadRightByWidth(this string text, int totalWidth, char paddingChar = ' ')
    {
        if (string.IsNullOrEmpty(text))
            return new string(paddingChar, totalWidth);

        int currentWidth = text.GetDisplayWidth();
        int paddingNeeded = totalWidth - currentWidth;

        if (paddingNeeded <= 0)
            return text;

        return text + new string(paddingChar, paddingNeeded);
    }
    
    public static string TruncateByWidth(this string text, int maxWidth, bool addEllipsis = true)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        
        // If text already fits, return it as is
        int displayWidth = text.GetDisplayWidth();
        if (displayWidth <= maxWidth)
            return text;
        
        // Calculate the width of ellipsis if needed
        int ellipsisWidth = addEllipsis ? "...".GetDisplayWidth() : 0;
        int availableWidth = maxWidth - ellipsisWidth;
    
        if (availableWidth <= 0)
            return addEllipsis ? "..." : string.Empty;
    
        // Truncate the string
        StringBuilder result = new StringBuilder();
        int currentWidth = 0;
    
        foreach (char c in text)
        {
            bool isWideChar = CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.OtherLetter 
                              || char.GetUnicodeCategory(c) == UnicodeCategory.OtherSymbol
                              || (c >= '\u3000' && c <= '\u9FFF')
                              || (c >= '\uFF00' && c <= '\uFFEF');
            
            int charWidth = isWideChar ? 2 : 1;
        
            // Check if adding this character would exceed available width
            if (currentWidth + charWidth > availableWidth)
                break;
            
            result.Append(c);
            currentWidth += charWidth;
        }
    
        // Add ellipsis if requested
        if (addEllipsis)
            result.Append("...");
        
        return result.ToString();
    }
}