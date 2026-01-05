public sealed record ChatRequest(
    string Message
);

public sealed record ChatResponse(
    string Answer,
    IReadOnlyList<string> Sources
);