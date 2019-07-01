using Amazon.Lambda.Core;
using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.Organizations;
using Amazon.Organizations.Model;

namespace Account
{
    public class Create
    {
        // public CreateAccountResponse FunctionHandler(JObject input)
        public JObject FunctionHandler(JObject input)
        {
            LambdaLogger.Log(input.ToString());
            // string tempJson = "{  \"CreateAccountStatus\": {    \"AccountId\": null,    \"AccountName\": \"TEST - Test 2\",    \"CompletedTimestamp\": \"0001-01-01T00:00:00\",    \"FailureReason\": null,    \"Id\": \"car-c7464020363411e9aa4b50d5029d06f1\",    \"RequestedTimestamp\": \"2019-02-21T23:59:52.345Z\",    \"State\": {      \"Value\": \"IN_PROGRESS\"    }  },  \"ResponseMetadata\": {    \"RequestId\": \"c7108b16-3634-11e9-bafa-ad7d8c47e1d6\",    \"Metadata\": {}  },  \"ContentLength\": 159,  \"HttpStatusCode\": 200}";
            // JObject response = JObject.FromObject(JsonConvert.DeserializeObject(tempJson));

            AmazonOrganizationsClient client = new AmazonOrganizationsClient();
            
            CreateAccountRequest request = new CreateAccountRequest() {
                AccountName = input.SelectToken("accountName").ToString(),
                Email = input.SelectToken("emailAddress").ToString(),
                RoleName = "AWSAccountAdmin"
            };

            CreateAccountResponse response = client.CreateAccountAsync(request).Result;

            JObject outputObject = new JObject();
            outputObject.Add("CreateAccountStatus", JObject.FromObject(response));
            outputObject.Add("EventData", input);

            return outputObject;
        }
    }
}