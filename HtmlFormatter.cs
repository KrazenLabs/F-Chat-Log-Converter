namespace KrazenLabs.LogConverter
{
    using System.Text;
    using System.Security.Cryptography;

    public class ChatHtmlFormatter
    {
        public static string GenerateHtmlDocument(List<ChatEntry> entries, string title = "Chat Log")
        {
            if (entries.Count == 0) return "<html><body><p>No chat data.</p></body></html>";

            var sb = new StringBuilder();
            var speakerColors = new Dictionary<string, string>();
            var sortedEntries = entries.OrderBy(e => e.Timestamp).ToList();

            var sessions = new List<List<ChatEntry>>();
            var currentSession = new List<ChatEntry> { sortedEntries[0] };

            for (int i = 1; i < sortedEntries.Count; i++)
            {
                var previous = sortedEntries[i - 1];
                var current = sortedEntries[i];

                if ((current.Timestamp - previous.Timestamp).TotalHours >= 6)
                {
                    sessions.Add(currentSession);
                    currentSession = new List<ChatEntry>();
                }
                currentSession.Add(current);
            }
            sessions.Add(currentSession);

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine($"    <title>{title}</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { margin: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #121212; color: #e0e0e0; }");
            sb.AppendLine("        main { margin-left: 250px; padding: 2em; }");
            sb.AppendLine("        h1 { color: #ffffff; border-bottom: 1px solid #333; padding-bottom: 0.5em; }");
            sb.AppendLine("        .sidebar { position: fixed; top: 0; left: 0; width: 230px; height: 100%; background-color: #1a1a1a; padding: 1em; overflow-y: auto; border-right: 1px solid #333; }");
            sb.AppendLine("        .sidebar h2 { font-size: 1.1em; color: #ccc; margin-top: 0; }");
            sb.AppendLine("        .toc { list-style: none; padding: 0; margin: 0; }");
            sb.AppendLine("        .toc-item { margin-bottom: 1em; padding: 0.5em; background-color: #222; border-radius: 5px; }");
            sb.AppendLine("        .toc-item a { display: block; color: inherit; text-decoration: none; }");
            sb.AppendLine("        .toc-item:hover { background-color: #333; cursor: pointer; }");
            sb.AppendLine("        .toc-item.active { background-color: #444; color: #fff; }");
            sb.AppendLine("        .toc-title { font-weight: bold; color: #fff; }");
            sb.AppendLine("        .toc-time { font-size: 0.85em; color: #ccc; }");
            sb.AppendLine("        .toc-meta { font-size: 0.85em; color: #80cbc4; margin-top: 0.2em; }");
            sb.AppendLine("        .chat-entry { margin-bottom: 1em; padding: 1em; background-color: #1e1e1e; border-radius: 6px; box-shadow: 0 0 4px rgba(0, 0, 0, 0.3); }");
            sb.AppendLine("        .timestamp { font-size: 0.75em; color: #888; margin-bottom: 0.25em; }");
            sb.AppendLine("        .speaker { font-weight: bold; margin-bottom: 0.2em; }");
            sb.AppendLine("        .message { white-space: pre-wrap; color: #e0e0e0; }");
            sb.AppendLine("        .session { margin-top: 2em; }");
            sb.AppendLine("        .session-header { cursor: pointer; background-color: #222; padding: 0.75em 1em; font-size: 1.1em; font-weight: bold; border-radius: 5px; }");
            sb.AppendLine("        .session-content { display: none; margin-top: 1em; }");
            sb.AppendLine("        .date-divider { font-size: 0.9em; margin: 1.5em 0 0.5em; padding: 0.3em 1em; background-color: #333; border-radius: 4px; color: #aaa; }");
            sb.AppendLine("        select { background-color: #1e1e1e; color: #e0e0e0; border: 1px solid #444; border-radius: 4px; }");
            sb.AppendLine("        @media (max-width: 700px) { .sidebar { position: relative; width: 100%; height: auto; border-right: none; } main { margin-left: 0; padding: 1em; } }");
            sb.AppendLine("    </style>");
            sb.AppendLine("    <script>");
            sb.AppendLine("        function scrollToSession(id) {");
            sb.AppendLine("            const content = document.getElementById('session-content' + id);");
            sb.AppendLine("            const allTocItems = document.querySelectorAll('.toc-item');");
            sb.AppendLine("            allTocItems.forEach(item => item.classList.remove('active'));"); // Remove active class
            sb.AppendLine("            const tocItem = document.querySelector(`.toc-item[data-index='${id}']`);");
            sb.AppendLine("            if (tocItem) tocItem.classList.add('active');"); // Add active class
            sb.AppendLine("            if (content.style.display === 'block') {");
            sb.AppendLine("                content.style.display = 'none';");
            sb.AppendLine("            } else {");
            sb.AppendLine("                const allContents = document.querySelectorAll('.session-content');");
            sb.AppendLine("                allContents.forEach(c => c.style.display = 'none');");
            sb.AppendLine("                content.style.display = 'block';");
            sb.AppendLine("                const target = document.getElementById('session' + id);");
            sb.AppendLine("                target.scrollIntoView({ behavior: 'smooth', block: 'start' });");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("        function toggle(id) {");
            sb.AppendLine("            const allContents = document.querySelectorAll('.session-content');");
            sb.AppendLine("            const allTocItems = document.querySelectorAll('.toc-item');");
            sb.AppendLine("            allContents.forEach(content => {");
            sb.AppendLine("                if (content.id !== id) {");
            sb.AppendLine("                    content.style.display = 'none';");
            sb.AppendLine("                }");
            sb.AppendLine("            });");
            sb.AppendLine("            allTocItems.forEach(item => item.classList.remove('active'));"); // Remove active class from all
            sb.AppendLine("            const el = document.getElementById(id);");
            sb.AppendLine("            const index = id.replace('session-content', '');"); // Extract session index
            sb.AppendLine("            const tocItem = document.querySelector(`.toc-item[data-index='${index}']`);");
            sb.AppendLine("            if (el.style.display === 'none') {");
            sb.AppendLine("                el.style.display = 'block';");
            sb.AppendLine("                if (tocItem) tocItem.classList.add('active');"); // Add active class to the corresponding sidebar item
            sb.AppendLine("            } else {");
            sb.AppendLine("                el.style.display = 'none';");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("        function sortSessions() {");
            sb.AppendLine("            const mode = document.getElementById('sortSelect').value;");
            sb.AppendLine("            const tocList = document.getElementById('tocList');");
            sb.AppendLine("            const items = Array.from(tocList.querySelectorAll('.toc-item'));");
            sb.AppendLine("            items.sort((a, b) => {");
            sb.AppendLine("                if (mode === 'time') return new Date(a.dataset.time) - new Date(b.dataset.time);");
            sb.AppendLine("                if (mode === 'emotes') return b.dataset.emotes - a.dataset.emotes;");
            sb.AppendLine("                if (mode === 'length') return b.dataset.length - a.dataset.length;");
            sb.AppendLine("                return 0;");
            sb.AppendLine("            });");
            sb.AppendLine("            items.forEach(item => tocList.appendChild(item));");
            sb.AppendLine("        }");
            sb.AppendLine("    </script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine("    <div class=\"sidebar\">");
            sb.AppendLine("        <h2>Sessions</h2>");
            sb.AppendLine("        <div style=\"margin-bottom: 1em;\">");
            sb.AppendLine("            <label for=\"sortSelect\" style=\"font-size: 0.85em; color: #aaa;\">Sort by:</label>");
            sb.AppendLine("            <select id=\"sortSelect\" onchange=\"sortSessions()\" style=\"width: 100%; padding: 0.3em; margin-top: 0.3em;\">");
            sb.AppendLine("                <option value=\"time\">\uD83D\uDD52 Time</option>");
            sb.AppendLine("                <option value=\"emotes\">\u2728 Emotes</option>");
            sb.AppendLine("                <option value=\"length\">\u23F1 Length</option>");
            sb.AppendLine("            </select>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <ul class=\"toc\" id=\"tocList\">");

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                var startTime = session.First().Timestamp;
                var endTime = session.Last().Timestamp;
                var duration = endTime - startTime;
                var label = $"Session {i + 1}";
                var timestamp = startTime.ToString("MMM dd, yyyy - HH:mm");
                var messageCount = session.Count;
                var emoteCount = session.Count(e => e.IsEmote);
                var durationStr = $"{(int)duration.TotalHours}h {duration.Minutes:D2}m";

                sb.AppendLine($"            <li class=\"toc-item\" data-index=\"{i}\" data-time=\"{startTime:O}\" data-emotes=\"{emoteCount}\" data-length=\"{duration.TotalSeconds}\">");
                sb.AppendLine($"                <a href=\"#\" onclick=\"scrollToSession({i}); return false;\">");
                sb.AppendLine($"                    <div class=\"toc-title\">{label}</div>");
                sb.AppendLine($"                    <div class=\"toc-time\">{timestamp}</div>");
                sb.AppendLine($"                    <div class=\"toc-meta\">\uD83D\uDCAC {messageCount}\u2003\u2728 {emoteCount}\u2003\u23F1 {durationStr}</div>");
                sb.AppendLine("                </a>");
                sb.AppendLine("            </li>");
            }

            sb.AppendLine("        </ul>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <main>");
            sb.AppendLine($"        <h1>{title}</h1>");

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                var header = $"Session {i + 1} - {session.First().Timestamp:MMM dd, yyyy - HH:mm}";
                sb.AppendLine($"        <div class=\"session\" id=\"session{i}\">");
                sb.AppendLine($"            <div class=\"session-header\" onclick=\"toggle('session-content{i}')\">{header}</div>");
                sb.AppendLine($"            <div class=\"session-content\" id=\"session-content{i}\" style=\"display: none;\">");

                DateTime? lastDate = null;
                foreach (var entry in session)
                {
                    var currentDate = entry.Timestamp.Date;
                    if (lastDate == null || currentDate != lastDate)
                    {
                        sb.AppendLine($"                <div class=\"date-divider\">{currentDate:MMMM dd, yyyy}</div>");
                        lastDate = currentDate;
                    }

                    if (!speakerColors.ContainsKey(entry.Speaker))
                        speakerColors[entry.Speaker] = GetColorFromName(entry.Speaker);

                    string color = speakerColors[entry.Speaker];

                    sb.AppendLine($"                <div class=\"chat-entry\" style=\"border-left: 4px solid {color};\">");
                    sb.AppendLine($"                    <div class=\"timestamp\">{entry.Timestamp:HH:mm}</div>");
                    if (entry.IsEmote)
                        sb.AppendLine($"                    <div class=\"message\" style=\"color: {color}; font-style: italic;\">{System.Net.WebUtility.HtmlEncode(entry.Speaker)}{FormatMessageWithBBCode(System.Net.WebUtility.HtmlEncode(entry.Message))}</div>");
                    else
                    {
                        sb.AppendLine($"                    <div class=\"speaker\" style=\"color: {color};\">{System.Net.WebUtility.HtmlEncode(entry.Speaker)}</div>");
                        sb.AppendLine($"                    <div class=\"message\">{FormatMessageWithBBCode(System.Net.WebUtility.HtmlEncode(entry.Message))}</div>");
                    }
                    sb.AppendLine("                </div>");
                }

                sb.AppendLine("            </div>");
                sb.AppendLine("        </div>");
            }

            sb.AppendLine("    </main>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private static string GetColorFromName(string name)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(name));
            int r = (hash[0] % 128) + 100;
            int g = (hash[1] % 128) + 100;
            int b = (hash[2] % 128) + 100;
            return $"rgb({r},{g},{b})";
        }

        private static string FormatMessageWithBBCode(string message)
        {
            return message
                .Replace("[b]", "<strong>").Replace("[/b]", "</strong>")
                .Replace("[i]", "<em>").Replace("[/i]", "</em>")
                .Replace("[sub]", "<sub>").Replace("[/sub]", "</sub>");
        }
    }
}
