# AWSAccountFactory
An account factory used to automate the provisioning of AWS accounts

To use:
POST https://apigw/stage/create

{
    "accountName": "Sandbox-PersonName",
    "emailAddress": "email@domain",
    "username": "firstname.lastname",
    "roleName": "AWS-SandboxNAME",
    "type": "sandbox" OR "production"
}

To get status:
GET https://apigw/stage/status/<Request ID>

Will require configuration of serverless.yml and SES configured to work.
