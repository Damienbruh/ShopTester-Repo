namespace server.Records
{
    public record ProductPatch(string? Name, int? Price, int? CategoryId, string? Description, string? Image_url);
}