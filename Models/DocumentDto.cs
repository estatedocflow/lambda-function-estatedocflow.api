using Amazon.DynamoDBv2.DataModel;

namespace Models;

[DynamoDBTable("documentapi")]
public class DocumentDto
{
    [DynamoDBHashKey("Pk")]
    public Guid Pk { get; set; }

    [DynamoDBRangeKey(AttributeName = "Sk")]
    public Guid Sk { get; set; }

    [DynamoDBProperty("Id")]
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public DateTime CreationDate { get; set; }

    public string? Notes { get; set; }

    public bool? Cancelled { get; set; }
}