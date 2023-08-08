// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using Azure.ResourceManager.Samples.Common;
using Azure.Identity;
using Azure.ResourceManager;
using System.Threading.Tasks;
using Azure.Core;
using Azure.ResourceManager.Resources;
using Azure;
using Azure.ResourceManager.Resources.Models;

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

                string applicationDefinitionName = Utilities.CreateRandomName("sampleApplicationDefinition");

                Utilities.Log("Starting a deployment for an Azure App Service: " + applicationDefinitionName);

                // First we need to get the application definition collection from the resource group
                ArmApplicationDefinitionCollection applicationDefinitionCollection = resourceGroup.GetArmApplicationDefinitions();
                // Use the same location as the resource group
                var input = new ArmApplicationDefinitionData(resourceGroup.Data.Location, ArmApplicationLockLevel.None)
                {
                    DisplayName = applicationDefinitionName,
                    Description = $"{applicationDefinitionName} description",
                    PackageFileUri = new Uri("https://raw.githubusercontent.com/Azure/azure-managedapp-samples/master/Managed%20Application%20Sample%20Packages/201-managed-storage-account/managedstorage.zip")
                };
                ArmOperation<ArmApplicationDefinitionResource> lro = await applicationDefinitionCollection.CreateOrUpdateAsync(WaitUntil.Completed, applicationDefinitionName, input);
                ArmApplicationDefinitionResource applicationDefinition = lro.Value;

                Utilities.Log("Started a deployment for an Azure App Service: " + applicationDefinitionName);

                //Utilities.Log(applicationDefinition.Data);

                //var deployment = azure.Deployments.GetByResourceGroup(rgName, deploymentName);
                //Utilities.Log("Current deployment status : " + deployment.ProvisioningState);

                //while (!(StringComparer.OrdinalIgnoreCase.Equals(deployment.ProvisioningState, "Succeeded") ||
                //        StringComparer.OrdinalIgnoreCase.Equals(deployment.ProvisioningState, "Failed") ||
                //        StringComparer.OrdinalIgnoreCase.Equals(deployment.ProvisioningState, "Cancelled")))
                //{
                //    SdkContext.DelayProvider.Delay(10000);
                //    deployment = azure.Deployments.GetByResourceGroup(rgName, deploymentName);
                //    Utilities.Log("Current deployment status : " + deployment.ProvisioningState);
                //}
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