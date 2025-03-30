namespace KrazenLabs.LogConverter
{
    public class MainRun
    {
        public static void Main(string[] args)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            basePath = Path.Combine(basePath, "fchat", "data");

            if (Directory.Exists(basePath))
            {
                FindCharacters(basePath);
            }
            else
            {
                Console.WriteLine("F-Chat installation was not found.");
            }
        }

        private static void FindCharacters(string basePath)
        {
            Console.WriteLine("Searching for characters...");
            string[] subfolders = Directory.GetDirectories(basePath);
            foreach (string subfolder in subfolders)
            {
                string lastFolderName = new DirectoryInfo(subfolder).Name;
                Console.WriteLine($"Processing character: {lastFolderName}");
                string characterPath = Path.Combine(basePath, subfolder, "logs");
                LogIndexer.ProcessLogs(characterPath);
            }
        }
    }
}