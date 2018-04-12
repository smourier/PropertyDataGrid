using System;
using System.Globalization;
using System.Text;

namespace PropertyDataGrid.Utilities
{
    public static class Decamelizer
    {
        public static string Decamelize(this string text, DecamelizeOptions options = DecamelizeOptions.Default)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var lastCategory = CharUnicodeInfo.GetUnicodeCategory(text[0]);
            var prevCategory = lastCategory;
            if (lastCategory == UnicodeCategory.UppercaseLetter)
            {
                lastCategory = UnicodeCategory.LowercaseLetter;
            }

            int i = 0;
            var sb = new StringBuilder(text.Length);
            bool firstIsStillUnderscore = (text[0] == '_');
            if (((options & DecamelizeOptions.UnescapeUnicode) == DecamelizeOptions.UnescapeUnicode) && CanUnicodeEscape(text, 0))
            {
                sb.Append(GetUnicodeEscape(text, ref i));
            }
            else if (((options & DecamelizeOptions.UnescapeHexadecimal) == DecamelizeOptions.UnescapeHexadecimal) && CanHexEscape(text, 0))
            {
                sb.Append(GetHexEscape(text, ref i));
            }
            else
            {
                if ((options & DecamelizeOptions.ForceFirstUpper) == DecamelizeOptions.ForceFirstUpper)
                {
                    sb.Append(Char.ToUpper(text[0]));
                }
                else
                {
                    sb.Append(text[0]);
                }
            }

            bool separated = false;
            bool keepFormat = (options & DecamelizeOptions.KeepFormattingIndices) == DecamelizeOptions.KeepFormattingIndices;

            for (i++; i < text.Length; i++)
            {
                char c = text[i];
                if (((options & DecamelizeOptions.UnescapeUnicode) == DecamelizeOptions.UnescapeUnicode) && CanUnicodeEscape(text, i))
                {
                    sb.Append(GetUnicodeEscape(text, ref i));
                    separated = true;
                }
                else if (((options & DecamelizeOptions.UnescapeHexadecimal) == DecamelizeOptions.UnescapeHexadecimal) && CanHexEscape(text, i))
                {
                    sb.Append(GetHexEscape(text, ref i));
                    separated = true;
                }
                else if (c == '_')
                {
                    if (!firstIsStillUnderscore || (options & DecamelizeOptions.KeepFirstUnderscores) != DecamelizeOptions.KeepFirstUnderscores)
                    {
                        sb.Append(' ');
                        separated = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                    switch (category)
                    {
                        case UnicodeCategory.ClosePunctuation:
                        case UnicodeCategory.ConnectorPunctuation:
                        case UnicodeCategory.DashPunctuation:
                        case UnicodeCategory.EnclosingMark:
                        case UnicodeCategory.FinalQuotePunctuation:
                        case UnicodeCategory.Format:
                        case UnicodeCategory.InitialQuotePunctuation:
                        case UnicodeCategory.LineSeparator:
                        case UnicodeCategory.OpenPunctuation:
                        case UnicodeCategory.OtherPunctuation:
                        case UnicodeCategory.ParagraphSeparator:
                        case UnicodeCategory.SpaceSeparator:
                        case UnicodeCategory.SpacingCombiningMark:
                            if (keepFormat && c == '{')
                            {
                                while (c != '}')
                                {
                                    c = text[i++];
                                    sb.Append(c);
                                }
                                i--;
                                separated = true;
                                break;
                            }

                            if ((options & DecamelizeOptions.ForceRestLower) == DecamelizeOptions.ForceRestLower)
                            {
                                sb.Append(Char.ToLower(c));
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            sb.Append(' ');
                            separated = true;
                            break;

                        case UnicodeCategory.LetterNumber:
                        case UnicodeCategory.DecimalDigitNumber:
                        case UnicodeCategory.OtherNumber:
                        case UnicodeCategory.CurrencySymbol:
                        case UnicodeCategory.LowercaseLetter:
                        case UnicodeCategory.MathSymbol:
                        case UnicodeCategory.ModifierLetter:
                        case UnicodeCategory.ModifierSymbol:
                        case UnicodeCategory.NonSpacingMark:
                        case UnicodeCategory.OtherLetter:
                        case UnicodeCategory.OtherNotAssigned:
                        case UnicodeCategory.Control:
                        case UnicodeCategory.OtherSymbol:
                        case UnicodeCategory.Surrogate:
                        case UnicodeCategory.PrivateUse:
                        case UnicodeCategory.TitlecaseLetter:
                        case UnicodeCategory.UppercaseLetter:
                            if ((category != lastCategory && c != ' ') && IsNewCategory(category, options))
                            {
                                if (!separated && prevCategory != UnicodeCategory.UppercaseLetter &&
                                    (!firstIsStillUnderscore || (options & DecamelizeOptions.KeepFirstUnderscores) != DecamelizeOptions.KeepFirstUnderscores))
                                {
                                    sb.Append(' ');
                                }

                                if ((options & DecamelizeOptions.ForceRestLower) == DecamelizeOptions.ForceRestLower)
                                {
                                    sb.Append(Char.ToLower(c));
                                }
                                else
                                {
                                    sb.Append(Char.ToUpper(c));
                                }

                                char upper = Char.ToUpper(c);
                                category = CharUnicodeInfo.GetUnicodeCategory(upper);
                                lastCategory = category == UnicodeCategory.UppercaseLetter ? UnicodeCategory.LowercaseLetter : category;
                            }
                            else
                            {
                                if ((options & DecamelizeOptions.ForceRestLower) == DecamelizeOptions.ForceRestLower)
                                {
                                    sb.Append(Char.ToLower(c));
                                }
                                else
                                {
                                    sb.Append(c);
                                }
                            }
                            separated = false;
                            break;
                    }

                    firstIsStillUnderscore = firstIsStillUnderscore && (c == '_');
                    prevCategory = category;
                }
            }

            if ((options & DecamelizeOptions.ReplaceSpacesByUnderscore) == DecamelizeOptions.ReplaceSpacesByUnderscore)
                return sb.Replace(' ', '_').ToString();

            if ((options & DecamelizeOptions.ReplaceSpacesByMinus) == DecamelizeOptions.ReplaceSpacesByMinus)
                return sb.Replace(' ', '-').ToString();

            if ((options & DecamelizeOptions.ReplaceSpacesByDot) == DecamelizeOptions.ReplaceSpacesByDot)
                return sb.Replace(' ', '.').ToString();

            return sb.ToString();
        }

        private static bool CanHexEscape(string text, int i) => (i + 6) < text.Length && text[i] == '_' && text[i + 1] == 'x' && text[i + 6] == '_' &&
                IsHexNumber(text[i + 2]) &&
                IsHexNumber(text[i + 3]) &&
                IsHexNumber(text[i + 4]) &&
                IsHexNumber(text[i + 5]);

        private static char GetHexEscape(string text, ref int i)
        {
            string s = text[i + 2].ToString(CultureInfo.InvariantCulture);
            s += text[i + 3].ToString(CultureInfo.InvariantCulture);
            s += text[i + 4].ToString(CultureInfo.InvariantCulture);
            s += text[i + 5].ToString(CultureInfo.InvariantCulture);
            i += 6;
            return (char)int.Parse(s, NumberStyles.HexNumber);
        }

        private static bool CanUnicodeEscape(string text, int i) => (i + 5) < text.Length &&
                text[i] == '\\' &&
                text[i + 1] == 'u' &&
                IsAsciiNumber(text[i + 2]) &&
                IsAsciiNumber(text[i + 3]) &&
                IsAsciiNumber(text[i + 4]) &&
                IsAsciiNumber(text[i + 5]);

        private static char GetUnicodeEscape(string text, ref int i)
        {
            string s = text[i + 2].ToString(CultureInfo.InvariantCulture);
            s += text[i + 3].ToString(CultureInfo.InvariantCulture);
            s += text[i + 4].ToString(CultureInfo.InvariantCulture);
            s += text[i + 5].ToString(CultureInfo.InvariantCulture);
            i += 5;
            return (char)int.Parse(s);
        }

        private static bool IsHexNumber(char c) => IsAsciiNumber(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        private static bool IsAsciiNumber(char c) => c >= '0' && c <= '9';

        private static bool IsNewCategory(UnicodeCategory category, DecamelizeOptions options)
        {
            if ((options & DecamelizeOptions.DontDecamelizeNumbers) == DecamelizeOptions.DontDecamelizeNumbers)
            {
                if (category == UnicodeCategory.LetterNumber ||
                    category == UnicodeCategory.DecimalDigitNumber ||
                    category == UnicodeCategory.OtherNumber)
                    return false;
            }
            return true;
        }
    }
}
