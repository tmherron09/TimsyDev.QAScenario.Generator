using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Collections.Generic;
using System.Text.Json;
using TimsyDev.QAScenario.Generator.Models;

namespace TimsyDev.QAScenario.Generator.Extensions
{
    public static class APIGatewayProxyResponseWrapper
    {

        private static string _accessControlAllowOrigin = string.Empty;
        private static string _accessControlAllowHeaders = string.Empty;

        private static string AccessControlAllowOrigin
        {
            get => _accessControlAllowOrigin;
            set => _accessControlAllowOrigin = !string.IsNullOrEmpty(value) ? value : "*";
        }

        private static string AccessControlAllowHeader
        {
            get => _accessControlAllowHeaders;
            set => _accessControlAllowHeaders = !string.IsNullOrEmpty(value) ? value : AccessControlAllowHeaders.ApplicationJson;
        }

        private static int StatusCode { get; set; }

        public static APIGatewayProxyResponse Build<T>(int statusCode, T responseItem, string allowHeaders = AccessControlAllowHeaders.ApplicationJson, string allowOrigin = "*")
        {
            try
            {
                AccessControlAllowOrigin = !string.IsNullOrEmpty(AccessControlAllowOrigin) ? AccessControlAllowOrigin : allowOrigin;
                AccessControlAllowHeader = !string.IsNullOrEmpty(AccessControlAllowHeader) ? AccessControlAllowHeader : allowHeaders;
                StatusCode = statusCode;

                if (responseItem is not null)
                {
                    return responseItem is string item ? BuildStringResponse(item) : BuildResponse(responseItem);
                }
                else
                {
                    return BuildEmptyResponse();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static APIGatewayProxyResponse Build(int statusCode) => Build(statusCode, null as object);

        private static APIGatewayProxyResponse BuildStringResponse(string responseItem) => new()
        {
            Headers = new Dictionary<string, string>
            {
                { "Access-Control-Allow-Headers", AccessControlAllowHeader },
                { "Access-Control-Allow-Origin", AccessControlAllowOrigin },
                { "Access-Control-Allow-Methods", "OPTIONS, GET" },
            },
            Body = responseItem,
            StatusCode = StatusCode
        };

        private static APIGatewayProxyResponse BuildEmptyResponse() => new()
        {
            Headers = new Dictionary<string, string>
            {
                { "Access-Control-Allow-Headers", AccessControlAllowHeader },
                { "Access-Control-Allow-Origin", AccessControlAllowOrigin },
                { "Access-Control-Allow-Methods", "OPTIONS, GET" },
            },
            StatusCode = StatusCode
        };

        private static APIGatewayProxyResponse BuildResponse<T>(T responseItem) => new()
        {
            Headers = new Dictionary<string, string>
            {
                { "Access-Control-Allow-Headers", AccessControlAllowHeader },
                { "Access-Control-Allow-Origin", AccessControlAllowOrigin },
                { "Access-Control-Allow-Methods", "OPTIONS, GET" },
            },
            Body = JsonSerializer.Serialize(responseItem),
            StatusCode = StatusCode
        };

        public static void SetAccessControlAllowHeader(string header)
        {
            AccessControlAllowHeader = header;
        }

        public static void SetAccessControlAllowOrigin(string origin)
        {
            AccessControlAllowOrigin = origin;
        }

    }
}
