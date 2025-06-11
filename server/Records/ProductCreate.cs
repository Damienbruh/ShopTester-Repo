namespace server.Records;

public record ProductCreate(string Name, int Price, int? CategoryId, string? Description, string? Image_url);