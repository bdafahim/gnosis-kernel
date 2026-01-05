public sealed record SearchHitDto(
    Guid ChunkId,
    Guid FileId,
    string Path,
    int StartLine,
    int EndLine,
    string Snippet,
    double Score
);

public sealed record SearchResponse(
    string Query,
    IReadOnlyList<SearchHitDto> Hits
);