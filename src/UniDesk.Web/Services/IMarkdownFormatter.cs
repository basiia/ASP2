namespace UniDesk.Web.Services
{
    public interface IMarkdownFormatter
    {
        string ToSafeHtml(string markdown);
    }
}
