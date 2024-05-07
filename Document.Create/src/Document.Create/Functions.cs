using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Models;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Document.Create;

public class Function
{
    public async Task<APIGatewayHttpApiV2ProxyResponse> CreateDocumentAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        try
        {
            var bookingRequest = JsonConvert.DeserializeObject<DocumentDto>(request.Body);

            if (bookingRequest == null)
            {
                return new APIGatewayHttpApiV2ProxyResponse
                {
                    Body = "",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            var guid = Guid.NewGuid();
            bookingRequest.Pk = guid;
            bookingRequest.Sk = guid;
            bookingRequest.Id = guid;

            var client = new AmazonDynamoDBClient();
            var dbContext = new DynamoDBContext(client);
            await dbContext.SaveAsync(bookingRequest);
            var message = $"Booking with Id {bookingRequest?.Id} Created";
            LambdaLogger.Log(message);
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = message,
                StatusCode = (int)HttpStatusCode.Created
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = ex.Message + " inner exception: " + ex.InnerException,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}