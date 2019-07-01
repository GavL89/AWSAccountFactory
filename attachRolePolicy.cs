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
    public class AttachPermission
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

            AttachRolePolicyRequest request = new AttachRolePolicyRequest() {
                PolicyArn = "arn:aws:iam::aws:policy/AdministratorAccess",
                RoleName = input.SelectToken("EventData.roleName").ToString()
            };

            AttachRolePolicyResponse response = client.AttachRolePolicyAsync(request).Result;

            JObject outputObject = new JObject();
            outputObject.Add("AttachRolePolicyResponse", JObject.FromObject(response));
            outputObject.Add("CreateAccountResponse", input.SelectToken("CreateAccountResponse"));
            outputObject.Add("EventData", input.SelectToken("EventData"));

            return outputObject;
       }
    }
}