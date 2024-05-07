using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Document.Get;

public class Function
{
    public async Task<DocumentDto?> GetDocumentByIdAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        try
        {
            var client = new AmazonDynamoDBClient();
            var dbContext = new DynamoDBContext(client);
            var idFromPath = request.PathParameters["id"];
            var id = new Guid(idFromPath);
            var booking = await dbContext.LoadAsync<DocumentDto>(id, id);
               
            if (booking == null)
            {
                LambdaLogger.Log("No booking");
                return null;
            }

            return booking;
        }
        catch (Exception ex)
        {
            LambdaLogger.Log(ex.Message);
            return null;
        }
    }
}