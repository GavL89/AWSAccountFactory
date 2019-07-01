using Amazon.Lambda.Core;
using System;
using System.Net;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.Organizations;
using Amazon.Organizations.Model;

using Identity;

namespace Permissions
{
    public class CreateSamlProvider
    {
        
        public JObject FunctionHandler(JObject input)
        {
            LambdaLogger.Log(JObject.FromObject(input).ToString());
            string accountId = input.SelectToken("CreateAccountStatus.CreateAccountStatus.AccountId").ToString();

            var credentials = AssumeIdentity.AssumeRole(accountId).Credentials;

            string accessKey = credentials.AccessKeyId;
            string secretkey = credentials.SecretAccessKey;
            string sessionToken = credentials.SessionToken;

            AmazonIdentityManagementServiceClient client = new AmazonIdentityManagementServiceClient(accessKey, secretkey, sessionToken);

            CreateSAMLProviderRequest request = new CreateSAMLProviderRequest() {
                Name = "ADFS",
                SAMLMetadataDocument = MetadataXML()
            };

            CreateSAMLProviderResponse response = client.CreateSAMLProviderAsync(request).Result;

            JObject outputObject = new JObject();
            outputObject.Add("CreateSAMLProviderResponse", JObject.FromObject(response));
            outputObject.Add("CreateAccountResponse", input.SelectToken("CreateAccountStatus"));
            outputObject.Add("EventData", input.SelectToken("EventData"));

            return outputObject;
        }

        public string MetadataXML() {
            var webRequest = WebRequest.Create(Environment.GetEnvironmentVariable("idpMetadata"));

            using (var response = webRequest.GetResponse())
            using(var content = response.GetResponseStream())
            using(var reader = new StreamReader(content)){
                var strContent = reader.ReadToEnd();
                return strContent;
            }
        }
    }
}