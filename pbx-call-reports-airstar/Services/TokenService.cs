using pbx_call_reports.Models;
using RestSharp;

namespace pbx_call_reports.Services
{
    public static class TokenService
    {
        /// <summary>
        /// Gets jwt token to be used as bearer for all other queries
        /// </summary>
        public static TokenResponse GetResponse(ApplicationConfiguration Configuration)
        {
            var client = new RestClient($"{Configuration.Api.Url}oauth2/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");

            request.AddParameter($"multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW",
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"client_id\"\r\n\r\n{Configuration.Api.ClientId}\r\n" +
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"client_secret\"\r\n\r\n{Configuration.Api.ClientSecret}\r\n" +
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"username\"\r\n\r\n{Configuration.Api.Username}\r\n------" +
                $"WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"password\"\r\n\r\n{Configuration.Api.Password}\r\n" +
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"grant_type\"\r\n\r\npassword\r\n" +
                $"------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            var tokenResponse = client.Execute<TokenResponse>(request);

            return tokenResponse.Data;
        }
    }
}