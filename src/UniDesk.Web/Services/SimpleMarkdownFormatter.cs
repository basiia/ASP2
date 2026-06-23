using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace UniDesk.Web.Services;
public class SimpleMarkdownFormatter : IMarkdownFormatter
{
    public string ToSafeHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return string.Empty;
        }

        var result = new StringBuilder();
        var codeBlock = new StringBuilder();
        var inCodeBlock = false;

        foreach (var rawLine in markdown.Replace("\r\n", "\n").Split('\n'))
        {
            if (rawLine.Trim() == "```")
            {
                if (inCodeBlock)
                {
                    result.Append("<pre><code>");
                    result.Append(WebUtility.HtmlEncode(codeBlock.ToString()));
                    result.Append("</code></pre>");
                    codeBlock.Clear();
                    inCodeBlock = false;
                }
                else
                {
                    inCodeBlock = true;
                }

                continue;
            }

            if (inCodeBlock)
            {
                codeBlock.AppendLine(rawLine);
                continue;
            }

            if (string.IsNullOrWhiteSpace(rawLine))
            {
                continue;
            }

            result.Append("<p>");
            result.Append(FormatInline(rawLine));
            result.Append("</p>");
        }

        if (inCodeBlock)
        {
            result.Append("<pre><code>");
            result.Append(WebUtility.HtmlEncode(codeBlock.ToString()));
            result.Append("</code></pre>");
        }

        return result.ToString();
    }

    private static string FormatInline(string line)
    {
        var encoded = WebUtility.HtmlEncode(line);

        encoded = Regex.Replace(encoded, "`([^`]+)`", "<code>$1</code>");
        encoded = Regex.Replace(encoded, "\\*\\*([^*]+)\\*\\*", "<strong>$1</strong>");

        return encoded;
    }
}

