namespace KrazenLabs.LogConverter
{
    public static class LogIndexer
    {
        public static void ProcessLogs(string basePath)
        {
            List<string> fileList = GetFiles(basePath);
            foreach (var file in fileList)
            {
                LogWriter.ConvertToHtml(file);
            }
        }

        private static List<string> GetFiles(string basePath)
        {
            var fileArray = Directory.GetFiles(basePath, "*");
            return ArrayToList(fileArray);
        }

        private static List<string> ArrayToList(string[] fileArray)
        {
            List<string> fileList = [];
            foreach (var file in fileArray)
            {
                if (!file.Contains('.') && !file.Contains('_') && !Path.GetFileName(file).StartsWith('#'))
                {
                    fileList.Add(file);
                }
            }
            return fileList;
        }
    }
}