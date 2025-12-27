namespace GnosisKernel.Api.Contracts;

public sealed record FileResponse(Guid Id, string Path, string? Language, long SizeBytes);

public sealed record GraphNodeDto(Guid Id, string Name, string Kind, Guid FileId, string FilePath);
public sealed record GraphEdgeDto(Guid FromSymbolId, Guid ToSymbolId, string Type);

public sealed record GraphResponse(
    IReadOnlyList<GraphNodeDto> Nodes,
    IReadOnlyList<GraphEdgeDto> Edges);

public sealed record ChunkResponse(Guid Id, Guid FileId, string FilePath, int StartLine, int EndLine, string Content);