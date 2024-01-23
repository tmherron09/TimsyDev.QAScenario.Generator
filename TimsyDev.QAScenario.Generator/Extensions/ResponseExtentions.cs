using Amazon.Lambda.APIGatewayEvents;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TimsyDev.QAScenario.Generator.Extensions
{
    public static class ResponseExtentions
    {
        public static APIGatewayProxyResponse Ok(this APIGatewayProxyResponse response)
        {
            try
            {
                response.Headers = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Headers", "application/json" },
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS, GET" },
                };
                response.Body = "";
                response.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public static APIGatewayProxyResponse Ok<T>(this APIGatewayProxyResponse response, T responseItem)
        {
            try
            {
                response.Headers = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Headers", "application/json" },
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "OPTIONS, GET" },
                };
                response.Body = JsonSerializer.Serialize(responseItem);
                response.StatusCode = 200;
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

    }
}
