namespace Trojan.Models
{
    public sealed class NoteContentPayload
    {
        public int Version { get; set; } = 1;
        public string Rtf { get; set; } = string.Empty;
    }
}
