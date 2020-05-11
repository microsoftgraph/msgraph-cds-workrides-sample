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

# Microsoft Graph と Common Data Service を使用したエンドツーエンド ソリューションの構築

## 概要
> **注** これは、Microsoft Build 2017 セッション [P4136](https://channel9.msdn.com/Events/Build/2017/P4136) の一部として公開されたエンドツーエンド ソリューションです。これには、Microsoft Graph と Common Data Service、Xamarin テクノロジを使用する複数のプロジェクトが含まれます。

Microsoft は、エンドツーエンド ソリューションを構築するためのプラットフォームを提供しています。このサンプルでは、プロの開発者が [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") や [Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service") からの生産性データとビジネス データを活用して、使いやすくデータ豊富なアプリを構築する方法を紹介します。そして、パワー ユーザーがそのデータをさらに活用して、アプリ、フロー、およびダッシュボードを作成する方法も紹介します。

[Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") は、Microsoft サービス用の統合 API です。これには、Office 365、Azure Active Directory などが含まれます。

[Microsoft Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service") または CDS は、ビジネス データの焦点です。プロの開発者は CDS 内のデータを操作するアプリケーションを作成でき、パワー ユーザーは PowerApps、Flow、および PowerBI を活用してアプリを作成し、ワークフローを設計し、コードを記述することなくそのデータに対して詳細な分析を実行することができます。

Microsoft Graph を使用すると、Microsoft サービスからの豊富なデータにアクセスできます。そのデータを CDS に取り込めば、ビジネスが依存する他のビジネス データと組み合わせることができます。そして、コードの記述方法を知っているかどうかに関係なく、使いやすい構成要素 ([PowerApps](https://powerapps.microsoft.com/en-us/)、[Flow](https://flow.microsoft.com/en-us/)、[PowerBI](https://powerbi.microsoft.com/en-us/) など) を使用して多機能なアプリケーションおよびワークフローを構築できます。そのため、Microsoft Graph と Common Data Service を使用すれば、開発者がユーザー中心のデータ豊富なアプリを構築できるだけでなく、通常のコーダーではないパワー ユーザーがアプリを拡張して追加のエクスペリエンスを提供することもできます。

## このサンプルでカバーされているシナリオ
このプロジェクトの一部として公開されたサンプルは、次のシナリオに基づいて構築されています。

### プロフェッショナルのアプリ開発者シナリオ
多くの企業は、従業員の通勤や駐車の円滑化に関する問題に直面しています。彼らは解決策を探しています。プロのアプリ開発者がこの問題の解決に取り組んでいます。彼女は、人々が仕事の行き帰りに必要な乗り物を見つけるのに役立つモバイル アプリを構築します。このアプリは、人々の実際の仕事方法に適合します。また、このアプリは多くの企業に展開可能なのです。

<img src="./media/prodevScenario.jpg" Height="80%" Width="80%" />

### エンタープライズ パワーユーザー シナリオ
Contoso は、このアプリを導入し全従業員が利用できるようにした最初の顧客の 1 つです。Contoso のパワー ユーザー (通常、人事担当者などの非プロフェッショナルなコーダー) は、このアプリで作成されたデータを簡単に拡張して、追加のエクスペリエンスを構築します。

<img src="./media/poweruserScenario.jpg" Height="80%" Width="80%" />

## プロジェクトの内容

以下のリンクをたどって、個々の要素がどのように構築されたかを学び、エンドツーエンド エクスペリエンスを自分で再現してください。

### プロのアプリ開発者 - コードを使用してアプリケーションを構築する
- [モバイル アプリ](./mobileapp/README.md) \- Microsoft Graph API を使用して、Web API を介してユーザー データを取得して Common Data Service に保存する Xamarin Forms アプリ。

### エンタープライズ パワー ユーザー - データを活用し、コードを使用せずにソリューションを拡張します
- [CDS](./cds/README.md) \- Common Data Service はモバイル アプリのデータ リポジトリとして使用され、そのデータは PowerApps、Flow、および PowerBI によってさらに活用されます。
- [PowerApps アプリ](./powerapps/README.md) \- Excel のような式言語を使用して、コードを書くことなく LOB モバイル アプリを構築するエンタープライズ パワー ユーザーによって構築された PowerApps アプリ。
- [Flow](./flow/README.md) \- Microsoft Flow を使用しているエンタープライズ パワー ユーザーによって作成された経費承認のワークフローです。これは、上司および他部門に承認メールを送信します。
- [PowerBI ダッシュボード](./powerbi/README.md) \- ライド シェア データの分析に役立つエンタープライズ パワー ユーザーによって作成されたダッシュボード。

### 技術的な概要

<img src="./media/workridesTech.jpg" Height="80%" Width="80%" />

## 詳細情報
- [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph")
- [Microsoft Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service")
