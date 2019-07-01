using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Mailer
{
    public class sendMail
    {
       public JObject FunctionHandler(JObject input)
       {
            LambdaLogger.Log(input.ToString());
            AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient();

            SendEmailRequest request = new SendEmailRequest() {
                Destination = new Destination {
                    ToAddresses = new List<string> {
                        input.SelectToken("EventData.emailAddress").ToString()
                    },
                    CcAddresses = new List<string> {
                        Environment.GetEnvironmentVariable("emailAdmin")
                    }
                },
                Message = new Message {
                    Body = new Body {
                        Html = new Content {
                            Charset = "UTF-8",
                            Data = string.Format("Hi,<br /><br />Your new AWS Account is in the process of being provisioned and should be available in the next 30 minutes.<br /><br />You'll be able to authenticate to your account by browsing to {0} and selecting the account {1}.<br /><br />Remember, you have any questions about building on the cloud please reach out!<br /><br />Cheers,<br />Gavin<br />Lead Cloud Architect<br /><br />-------------------------------<br /><br />Account Details<br />Account ID: {2}<br />Account Name: {3}<br />Role Name: {1}", Environment.GetEnvironmentVariable("awsLogin"), input.SelectToken("EventData.roleName"), input.SelectToken("CreateAccountResponse.CreateAccountStatus.AccountId"), input.SelectToken("CreateAccountResponse.CreateAccountStatus.AccountName"))
                        }
                    },
                    Subject = new Content {
                        Charset = "UTF-8",
                        Data = "Your Sandbox AWS Account is almost ready!"
                    }
                },
                Source = "Administrator <email@domain.com>"
            };

            SendEmailResponse response = client.SendEmailAsync(request).Result;
            LambdaLogger.Log(JObject.FromObject(response).ToString());

            return input;
       }
    }
}