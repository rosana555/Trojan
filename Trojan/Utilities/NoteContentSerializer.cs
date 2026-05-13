using System;
using System.Text.Json;
using Trojan.Models;

namespace Trojan.Utilities
{
    public static class NoteContentSerializer
    {
        public static string SerializeRtf(string rtf)
        {
            var payload = new NoteContentPayload
            {
                Rtf = rtf ?? string.Empty
            };

            return JsonSerializer.Serialize(payload);
        }

        public static (string Content, bool IsRtf) DeserializeForEditor(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return (string.Empty, true);
            }

            try
            {
                var payload = JsonSerializer.Deserialize<NoteContentPayload>(content);
                if (!string.IsNullOrWhiteSpace(payload?.Rtf))
                {
                    return (payload.Rtf, true);
                }
            }
            catch (JsonException)
            {
                return (content, false);
            }

            return (string.Empty, true);
        }
    }
}
