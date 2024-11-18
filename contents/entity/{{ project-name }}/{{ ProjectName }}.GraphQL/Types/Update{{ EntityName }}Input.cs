namespace {{ ProjectName }}.GraphQL.Types;

public class Update{{ EntityName }}Input
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}