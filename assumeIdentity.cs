using Amazon.Lambda.Core;
using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace Identity
{
    public class AssumeIdentity
    {
       public static AssumeRoleResponse AssumeRole(string accountId)
       {
            AmazonSecurityTokenServiceClient client = new AmazonSecurityTokenServiceClient();

            AssumeRoleRequest request = new AssumeRoleRequest() {
                DurationSeconds = 3600,
                ExternalId = "AccountFactory",
                RoleArn = string.Format("arn:aws:iam::{0}:role/AWSAccountAdmin", accountId),
                RoleSessionName = "Provisioner"
            };

            AssumeRoleResponse response = client.AssumeRoleAsync(request).Result;

            return response;
       }
    }
}