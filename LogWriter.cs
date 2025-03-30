namespace KrazenLabs.LogConverter
{
    public static class LogWriter
    {
        public static void ConvertToHtml(string filePath)
        {
            List<ChatEntry> lines = ChatLogParser.Parse(filePath);
            string html = ChatHtmlFormatter.GenerateHtmlDocument(lines);
            string directoryName = Path.GetDirectoryName(filePath) ?? throw new ArgumentException("Invalid file path", nameof(filePath));
            string directoryPath = Path.Combine(directoryName, "html");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string htmlPath = Path.Combine(directoryPath, Path.GetFileName(filePath) + ".html");
            File.WriteAllText(htmlPath, html);
        }
    }
}