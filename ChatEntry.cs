namespace KrazenLabs.LogConverter
{
    public class ChatEntry
    {
        public DateTime Timestamp { get; set; }
        public required string Speaker { get; set; }
        public required string Message { get; set; }
        public bool IsEmote { get; set; }
    }

}
