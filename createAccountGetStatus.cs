using Amazon.Lambda.Core;
using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.Organizations;
using Amazon.Organizations.Model;

namespace Account
{
    public class CreateGetStatus
    {
        public JObject FunctionHandler(JObject input)
        {
            LambdaLogger.Log(input.ToString());
            string requestId = input.SelectToken("CreateAccountStatus.CreateAccountStatus.Id").ToString();

            AmazonOrganizationsClient client = new AmazonOrganizationsClient();
            DescribeCreateAccountStatusRequest request = new DescribeCreateAccountStatusRequest() {
                CreateAccountRequestId = requestId
            };

            DescribeCreateAccountStatusResponse response = client.DescribeCreateAccountStatusAsync(request).Result;

            JObject outputObject = new JObject();
            outputObject.Add("CreateAccountStatus", JObject.FromObject(response));
            outputObject.Add("EventData", input.SelectToken("EventData"));

            return outputObject;
        }
    }
}