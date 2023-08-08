// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Resources.Models;
using Azure.ResourceManager.Samples.Common;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager;

namespace DeployUsingARMTemplateWithProgress
{
    public class Program
    {
        /**
         * Azure Resource sample for deploying resources using an ARM template and
         * showing progress.
         */
        public static async Task RunSample(ArmClient client)
        {
            // Get default subscription
            SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync();

            // Create a resource group in the EastUS region
            string rgName = Utilities.CreateRandomName("ARMTemplateRG");
            Utilities.Log($"created resource group with name:{rgName}");
            ArmOperation<ResourceGroupResource> rgLro = await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, rgName, new ResourceGroupData(AzureLocation.EastUS));
            ResourceGroupResource resourceGroup = rgLro.Value;
            Utilities.Log("Created a resource group with name: " + resourceGroup.Data.Name);

            try
            {
                //=============================================================
                // Create a deployment for an Azure App Service via an ARM template.

                string deploymentName = Utilities.CreateRandomName("myDeployment");

                Utilities.Log("Starting a deployment for an Azure App Service: " + deploymentName);

                // Get the deployment collection from the resource group
                ArmDeploymentCollection armDeploymentCollection = resourceGroup.GetArmDeployments();
                Utilities.Log("Load a template JSON, which can originate from local or network sources...");
                ArmDeploymentContent input = new ArmDeploymentContent(new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
                {
                    //Template = BinaryData.FromString(File.ReadAllText("appservice-template.json")),
                    TemplateLink = new ArmDeploymentTemplateLink()
                    {
                        Uri = new Uri("https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/quickstarts/microsoft.web/app-service-docs-linux/azuredeploy.json")
                    },
                });
                ArmOperation<ArmDeploymentResource> lro = await armDeploymentCollection.CreateOrUpdateAsync(WaitUntil.Completed, deploymentName, input);
                ArmDeploymentResource deployment = lro.Value;

                Utilities.Log("Started a deployment for an Azure App Service: " + deploymentName);
                Utilities.Log("Current deployment status : " + deployment.Data.Properties.ProvisioningState);
            }
            finally
            {
                try
                {
                    Utilities.Log("Deleting Resource Group: " + rgName);
                    await resourceGroup.DeleteAsync(WaitUntil.Completed);
                    Utilities.Log("Deleted Resource Group: " + rgName);
                }
                catch (NullReferenceException)
                {
                    Utilities.Log("Did not create any resources in Azure. No clean up is necessary");
                }
                catch (Exception ex)
                {
                    Utilities.Log(ex);
                }
            }
        }

        public static async Task Main(string[] args)
        {
            try
            {
                //=================================================================
                // Authenticate
                var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
                var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
                var subscription = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID");
                ClientSecretCredential credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                ArmClient client = new ArmClient(credential, subscription);

                await RunSample(client);
            }
            catch (Exception ex)
            {
                Utilities.Log(ex);
            }
        }
    }
}