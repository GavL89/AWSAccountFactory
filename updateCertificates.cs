using Amazon.Lambda.Core;
using System;
using System.Threading;
using System.Net;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.Organizations;
using Amazon.Organizations.Model;

using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;

using Identity;

namespace Account
{
    public class UpdateCertificates
    {
        public JObject FunctionHandler(JObject input)
        {
            foreach (string accountId in accountsWrapper()) {
                LambdaLogger.Log("Updating " + accountId + "... ");

                try {
                    updateSAMLProvider(accountId); 
                    LambdaLogger.Log("... success!" + Environment.NewLine);
                } catch (Exception ex) {
                    LambdaLogger.Log("... failed: " + ex.Message + Environment.NewLine);
                }
            }

            return null;
        }

        public JArray accountsWrapper() {
            JArray accountsArray = new JArray();

            ListAccountsResponse response = listAccounts();

            while (response.NextToken != null) {
                // LambdaLogger.Log("next set..." + Environment.NewLine);
                foreach (Amazon.Organizations.Model.Account awsAccount in response.Accounts) {
                    // LambdaLogger.Log(awsAccount.Id + Environment.NewLine);
                    if (awsAccount.Status == AccountStatus.ACTIVE) {
                        accountsArray.Add(awsAccount.Id);
                    }
                }

                Thread.Sleep(100);

                response = listAccounts(response.NextToken);
            }

            Thread.Sleep(100);

            // LambdaLogger.Log("One set only..." + Environment.NewLine);
            foreach (Amazon.Organizations.Model.Account awsAccount in response.Accounts) {
                // LambdaLogger.Log(awsAccount.Id + Environment.NewLine);
                if (awsAccount.Status == AccountStatus.ACTIVE) {
                    accountsArray.Add(awsAccount.Id);
                }
            }

            foreach (string accountId in Environment.GetEnvironmentVariable("AdditionalAccounts").Split(",")) {
                accountsArray.Add(accountId);
            }

            return accountsArray;
        }

        public ListAccountsResponse listAccounts(string nextToken = null) {
                AmazonOrganizationsClient client = new AmazonOrganizationsClient();

                ListAccountsRequest request = new ListAccountsRequest() {
                    NextToken = nextToken
                };
                ListAccountsResponse response = client.ListAccountsAsync(request).Result;

                return response;
        }

        public UpdateSAMLProviderResponse updateSAMLProvider(string accountId) {
            AmazonIdentityManagementServiceClient client = new AmazonIdentityManagementServiceClient();;

            if (accountId != "177654365656") {
                var credentials = AssumeIdentity.AssumeRole(accountId).Credentials;

                string accessKey = credentials.AccessKeyId;
                string secretkey = credentials.SecretAccessKey;
                string sessionToken = credentials.SessionToken;

                client = new AmazonIdentityManagementServiceClient(accessKey, secretkey, sessionToken);
            }

            UpdateSAMLProviderRequest request = new UpdateSAMLProviderRequest() {
                SAMLMetadataDocument = MetadataXML(),
                SAMLProviderArn = string.Format("arn:aws:iam::{0}:saml-provider/ADFS", accountId)
            };

            UpdateSAMLProviderResponse response = client.UpdateSAMLProviderAsync(request).Result;

            return response;
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