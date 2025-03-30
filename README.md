# F-Chat-Log-Converter

The HTML exporter included in F-Chat's desktop client is somewhat lacking, as the generated files are annoying to browse and use and it is rather slow and tends to crash on character with hundreds of logs.

This custom HTML exporter runs significantly faster and converts all logs for all characters within a matter of seconds, even with thousands of logs.

Other key features:
- Autodetects f-chat client.
- Converts logs for all characters in one go.
- Ignores channels and system log, focusing on character logs instead.
- No configuration needed.

More importantly: it generates much nicer HTML documents which are easy to read and use:

- Generates one HTML document per character you interacted with, keeping all conversations with that character in one, convenient spot.
- Autodetects chat sessions with that character and sorts the document into those sessions, keeping everything coherent and easy to browse.
- Sessions show the start time, number of messages, emotes and the length of the session, making it easy to recognize relevant sessions.
- Nice and easy to read formatting, taking emotes into account.

## How to use

- For the moment, only Windows is officially supported.
- Make sure you have [.NET 8.0]([url](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-8.0.14-windows-x64-installer)) installed.
- Download the release version, unzip and run the executable. It should automatically finish within a couple of seconds.
- The HTML documents will be created in a HTML directory within the appdata/roaming/f-chat/<character>/logs directory. Just open them with your browser and enjoy!
- If you have new or changed logs, just run the application again and it will re-generate the documents accordingly.
