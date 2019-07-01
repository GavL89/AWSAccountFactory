using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace ActiveDirectory
{
    public class CreateGroupAssignUser
    {
       public JObject FunctionHandler(JObject input)
       {
            Amazon.Runtime.BasicAWSCredentials credentials = new Amazon.Runtime.BasicAWSCredentials(Environment.GetEnvironmentVariable("AD_AccessKey"), Environment.GetEnvironmentVariable("AD_SecretKey"));
            AmazonSimpleSystemsManagementClient client = new AmazonSimpleSystemsManagementClient(credentials, Amazon.RegionEndpoint.APSoutheast2);

            string username = input.SelectToken("EventData.username").ToString();
            string accountId = input.SelectToken("CreateAccountResponse.CreateAccountStatus.AccountId").ToString();
            string roleName = input.SelectToken("EventData.roleName").ToString();
            string groupName = roleName.Split("-")[0] + "-" + accountId + "-" + roleName.Split("-")[1];
            string groupPath = Environment.GetEnvironmentVariable("AD_SandboxOU");
            string type = input.SelectToken("EventData.type").ToString();

            if (type == "production") {
                groupPath = Environment.GetEnvironmentVariable("AD_ProdOU");
            }

            Dictionary<string, List<string>> ParametersData = new Dictionary<string, List<string>>();
            ParametersData.Add("commands", new List<string>{
                string.Format("New-ADGroup -Name \"{0}\" -SamAccountName {0} -GroupCategory Security -GroupScope Global -DisplayName \"{0}\" -Path \"{1}\" -Description \"AWS Account Group\"", groupName, groupPath),
                string.Format("Add-ADGroupMember -Identity {0} -Members {1}", groupName, username)
            });

            SendCommandRequest request = new SendCommandRequest() {
                DocumentName = "AWS-RunPowerShellScript",
                InstanceIds = new List<string> {
                    Environment.GetEnvironmentVariable("AD_InstanceId")
                },
                Parameters = ParametersData
            };

            SendCommandResponse response = client.SendCommandAsync(request).Result;
            
            return input;
       }
    }
}