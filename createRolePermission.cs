using Amazon.Lambda.Core;
using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.Organizations;
using Amazon.Organizations.Model;

using Identity;

namespace Permissions
{
    public class CreateRole
    {
       public JObject FunctionHandler(JObject input)
       {
            JObject createAccountResponseObject = JObject.FromObject(input.SelectToken("CreateAccountResponse"));
            string accountId = createAccountResponseObject.SelectToken("CreateAccountStatus.AccountId").ToString();

            var credentials = AssumeIdentity.AssumeRole(accountId).Credentials;

            string accessKey = credentials.AccessKeyId;
            string secretkey = credentials.SecretAccessKey;
            string sessionToken = credentials.SessionToken;

            AmazonIdentityManagementServiceClient client = new AmazonIdentityManagementServiceClient(accessKey, secretkey, sessionToken);

            CreateRoleRequest request = new CreateRoleRequest() {
                RoleName = input.SelectToken("EventData.roleName").ToString(),
                MaxSessionDuration = 43200,
                AssumeRolePolicyDocument = "{ \"Version\": \"2012-10-17\", \"Statement\": { \"Effect\": \"Allow\", \"Action\": \"sts:AssumeRoleWithSAML\", \"Principal\": {\"Federated\": \"arn:aws:iam::" + accountId + ":saml-provider/ADFS\"}, \"Condition\": {\"StringEquals\": {\"SAML:aud\": \"https://signin.aws.amazon.com/saml\"}} } }"
            };

            CreateRoleResponse response = client.CreateRoleAsync(request).Result;

            JObject outputObject = new JObject();
            outputObject.Add("CreateAccountResponse", createAccountResponseObject);
            outputObject.Add("CreateRoleResponse", JObject.FromObject(response));
            outputObject.Add("EventData", input.SelectToken("EventData"));

            return outputObject;
       }
    }
}