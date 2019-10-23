---
page_type: sample
languages:
- csharp
products:
- dotnet
- azure
- azure-resource-manager
extensions:
- services: Resource-Manager
- platforms: dotnet
description: "Azure Resource sample for deploying resources using an ARM template and showing progress."
urlFragment: getting-started-on-deploying-using-an-arm-template-and-show-progress-in-c
---

# Get started deploying using an ARM template and show progress (C#)

Azure Resource sample for deploying resources using an ARM template and showing progress.

## Running this sample

To run this sample:

Set the environment variable `AZURE_AUTH_LOCATION` with the full path for an auth file. See [how to create an auth file](https://github.com/Azure/azure-libraries-for-net/blob/master/AUTH.md).

```bash
git clone https://github.com/Azure-Samples/resources-dotnet-deploy-using-arm-template-with-progress.git
cd resources-dotnet-deploy-using-arm-template-with-progress
dotnet build
bin\Debug\net452\DeployUsingARMTemplateWithProgress.exe
```

## More information

[Azure Management Libraries for C#](https://github.com/Azure/azure-sdk-for-net/tree/Fluent)
[Azure .Net Developer Center](https://azure.microsoft.com/en-us/develop/net/)
If you don't have a Microsoft Azure subscription you can get a FREE trial account [here](http://go.microsoft.com/fwlink/?LinkId=330212).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
