using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenMediaID.Models
{
    public record MediaEntry
    {
        [JsonPropertyName("filename")]
        public string Filename { get; init; }

        [JsonPropertyName("hash")]
        public string Hash { get; init; }

        [JsonPropertyName("length")]
        public long LengthInBytes { get; init; }

        [JsonPropertyName("metadata")]
        public MediaMetadata Metadata { get; init; }

        [JsonPropertyName("mimeType")]
        public string MimeType { get; init; } //

        // ➕ Neue Properties:
        public string? ThumbnailPath { get; init; }        // z. B. "thumbnails/video1.jpg"
        public string? PreviewMediaPath { get; init; }     // z. B. "media/video1_preview.mp4"
    }
}
