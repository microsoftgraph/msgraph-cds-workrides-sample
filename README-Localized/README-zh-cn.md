---
page_type: sample
products:
- office-365
- ms-graph
languages:
- csharp
extensions:
  contentType: samples 
  technologies:
  - Microsoft Graph
  services:
  - Office 365
  createdDate: 5/8/2017 5:02:44 PM
---

# 使用 Microsoft Graph 和 Common Data Service 构建端到端解决方案

## 概述
> **注意**：这是作为 Microsoft Build 2017 大会 [P4136](https://channel9.msdn.com/Events/Build/2017/P4136) 的一部分发布的端到端解决方案。其中包含多个使用 Microsoft Graph 和 Common Data Service 以及 Xamarin 技术的项目。

Microsoft 提供了用于构建端到端解决方案的平台。本示例展示了专业开发人员如何构建以人为中心且数据丰富的应用，以利用 [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") 和 [Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service") 的生产力和业务数据。它还演示了高级用户如何进一步利用这些数据来创建应用、流程和仪表板。

[Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") 是适用于 Microsoft 服务的统一 API。其中包括 Office 365、Azure Active Directory 等各种应用。

[Microsoft Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service") 或 CDS 的专注点是业务数据。专业开发人员可以在 CDS 中编写与该数据交互的应用程序，而高级用户则可以利用 PowerApps、Flow 和 PowerBI 创建应用、设计工作流并对这些数据执行深度分析，而无需编写任何代码。

Microsoft Graph 允许你访问来自 Microsoft 服务的丰富数据。将这些数据引入 CDS 后，你可以将其与企业所依赖的其他业务数据合并。然后，无论你是否知道如何编写代码，都可以利用易于使用的构建基块（例如 [PowerApps](https://powerapps.microsoft.com/en-us/)、[Flow](https://flow.microsoft.com/en-us/) 和 [PowerBI](https://powerbi.microsoft.com/en-us/)）来构建丰富的应用程序和工作流。因此，通过使用 Microsoft Graph 和 Common Data Service，开发人员不仅可以构建以人为中心且数据丰富的应用，而且不是常规编码人员的高级用户还可以扩展应用，以获得更多体验。

## 此示例涵盖了哪些方案？
作为此项目一部分发布的示例是基于以下方案构建的：

### 专业应用开发人员方案
许多公司都面临着为员工提供便利通勤和停车的问题。他们正在寻求解决方案。专业应用开发人员正致力于解决此问题。她构建了一款移动应用，旨在帮助人们寻找上下班的交通工具。此应用可极大满足人们的实际工作需要，因为它可以在许多公司内部署。

<img src="./media/prodevScenario.jpg" Height="80%" Width="80%" />

### 企业高级用户方案
Contoso 是首批获得此应用并将其提供给所有员工的客户之一。Contoso 的高级用户（通常是非专业编码人员，例如 HR 人员）可以轻松扩展此应用创建的数据，以构建更多体验。

<img src="./media/poweruserScenario.jpg" Height="80%" Width="80%" />

## 此项目中包含哪些内容？

请通过下面提供的链接来了解如何构建各个部分，并自行重新创建端到端体验。

### 专业应用开发人员 - 使用代码构建应用程序
- [移动应用](./mobileapp/README.md) \- Xamarin Forms 应用，它使用 Microsoft Graph API 通过 Web API 获取人员数据并存储在 Common Data Service 中。

### 企业高级用户 - 利用数据并扩展解决方案，而无需使用任何代码
- [CDS](./cds/README.md) \- Common Data Service 用作移动应用的数据存储库，并且 PowerApps、Flow 和 PowerBI 可进一步利用该数据。
- [PowerApps 应用](./powerapps/README.md) \- 由企业高级用户构建的 PowerApps 应用，该用户使用 Excel 之类的表达式语言来构建 LOB 移动应用，而不需要知道如何编写代码。
- [Flow](./flow/README.md) \- 由企业高级用户构建的费用审批工作流，它使用 Microsoft Flow 将审批电子邮件发送给经理和其他部门。
- [PowerBI 仪表板](./powerbi/README.md) \- 由企业高级用户构建的仪表板，有助于分析交通工具共享数据。

### 技术概述

<img src="./media/workridesTech.jpg" Height="80%" Width="80%" />

## 了解详细信息
- [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph")
- [Microsoft Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service")
