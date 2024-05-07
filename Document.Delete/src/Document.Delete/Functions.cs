using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Document.Delete;

public class Function
{
    public async Task<APIGatewayHttpApiV2ProxyResponse> DeleteDocumentAsync(APIGatewayHttpApiV2ProxyRequest? request,
        ILambdaContext context)
    {
        try
        {
            if (request == null)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = "",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            var idFromPath = request.PathParameters["id"];

            if (idFromPath == null)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = "",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            var client = new AmazonDynamoDBClient();
            var dbContext = new DynamoDBContext(client);

            var id = new Guid(idFromPath);
            var existingBooking = await dbContext.LoadAsync<DocumentDto>(id, id);

            if (existingBooking is null)
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = "No booking to delete",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };

            await dbContext.DeleteAsync<DocumentDto>(existingBooking);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = $"Booking with Id {idFromPath} Deleted",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = ex.Message,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}