using Amazon.Lambda.Core;
using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;

namespace StateMachine
{
    public class StepFunctionStatus
    {
       public DescribeExecutionResponse FunctionHandler(JObject input)
       {
            LambdaLogger.Log(input.ToString());
            AmazonStepFunctionsClient client = new AmazonStepFunctionsClient();

            string executionArn = string.Format("{0}:{1}", Environment.GetEnvironmentVariable("stepFunctionsArn").Replace("stateMachine", "execution"), input.SelectToken("path.id"));
            LambdaLogger.Log(executionArn);

            DescribeExecutionRequest request = new DescribeExecutionRequest() {
                ExecutionArn = executionArn
            };

            DescribeExecutionResponse response = client.DescribeExecutionAsync(request).Result;

            return response;
       }
    }
}