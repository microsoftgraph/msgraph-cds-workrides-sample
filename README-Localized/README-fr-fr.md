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

# Créez des solutions complètes à l’aide de Microsoft Graph et de Common Data Service

## Vue d’ensemble
> **REMARQUE** il s’agit d’une solution intégrale publiée dans le cadre de la session Microsoft Build 2017 [P4136](https://channel9.msdn.com/Events/Build/2017/P4136). Elle contient plusieurs projets utilisant Microsoft Graph et Common Data Service, et des technologies Xamarin.

Microsoft offre une plateforme pour créer des solutions complètes. Cet exemple indique comment les développeurs professionnels peuvent créer des applications riches en données et orientées vers les utilisateurs, qui exploitent les données métier et de productivité à partir de [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") et [Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service"). Et la façon dont les utilisateurs avancés peuvent exploiter encore plus ces données pour créer des applications, des flux et des tableaux de bord.

[Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") est l’API unifiée pour les services Microsoft. Elle comprend Office 365, Azure Active Directory et bien d’autres.

[Microsoft Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service") ou CDS est un point focal pour les données d’une entreprise. Les développeurs professionnels peuvent créer des applications qui interagissent avec ces données dans des CDS et les utilisateurs chevronné peuvent tirer parti des applications PowerApps, Flow et PowerBI pour créer des applications, concevoir des flux de travail et effectuer des analyses approfondies sur ces données sans écriture de codes.

Microsoft Graph vous permet d’accéder à des données enrichies des services Microsoft. Après avoir acheminé ces données sur CDS, vous pouvez les combiner avec d’autres données professionnelles dont votre entreprise a besoin. Vous pouvez ensuite créer des applications et des flux de travail enrichis avec des blocs de constructions d'utilisation simple (par exemple, [PowerApps](https://powerapps.microsoft.com/en-us/), [Flow](https://flow.microsoft.com/en-us/), [PowerBI](https://powerbi.microsoft.com/en-us/)), que vous sachiez écrire un code ou pas. L'utilisation de Microsoft Graph et de Common Data Service permet ainsi à des développeurs de créer des applications axées sur les personnes et les applications enrichies, et l'application peut également être développée par des utilisateurs avancés qui ne sont pas des codeurs classiques pour qu'ils bénéficient d'expériences supplémentaires.

## Quels sont les scénarios couverts dans cet exemple ?
Les exemples publiés dans le cadre de ce projet sont créés selon le scénario suivant :

### Scénario pour développeur d’applications professionnelles
De nombreuses entreprises sont confrontées aux problèmes de déplacements et de stationnement de leurs employés. Ils cherchent une solution. Un développeur d’applications professionnel œuvre pour résoudre ce problème. Il élabore une application mobile permettant aux utilisateurs de trouver des personnes pouvant les conduire depuis et vers leur lieu de travail. Cette application s’adapte parfaitement à la façon dont les personnes travaillent concrètement et elle peut être déployée dans de nombreuses sociétés.

<img src="./media/prodevScenario.jpg" Height="80%" Width="80%" />

### Scénario d'entreprise pour utilisateurs chevronnés
Contoso est l’un des premiers clients à avoir fait l'acquisition de cette application et à la rendre disponible auprès de tous ses employés. Les utilisateurs expérimentés de Contoso (des codeurs non professionnels en général, tels que des personnes aux Ressources Humaines) déploient facilement les données créées par cette application pour la création d'expériences supplémentaires.

<img src="./media/poweruserScenario.jpg" Height="80%" Width="80%" />

## Ce que comprend ce projet :

Veuillez suivre les liens fournis ci-dessous pour découvrir comment différents éléments ont été créés et recréez une solution complète pour vous.

### Développeur d’applications professionnelles : créer une application à l’aide de codes
- [Application mobile](./mobileapp/README.md) : les applications Xamarin Forms qui utilisent les API Microsoft Graph pour obtenir des données de personnes et de magasins dans Common Data Service via l’API web.

### Utilisateurs qualifiés d’entreprise : tirent parti des données et développent une solution sans utilisation de codes
- [CDS](./cds/README.md) : Common Data Service est utilisé comme référentiel de données pour l’Application mobile et les données sont exploitées de manière plus approfondie par les applications PowerApps, Flow et PowerBI.
- [Application PowerApps](./powerapps/README.md) : l'application PowerApps créée par un utilisateur professionnel expérimenté qui utilise Excel comme langage d’expression pour créer un cœur de métier (LOB) mobile, sans avoir de connaissance particulière en matière d'écriture de code.
- [Flow](./flow/README.md) : un flux de travail d’approbation des dépenses créé par un utilisateur avancé de l’entreprise qui utilise Microsoft Flow, lequel envoie des messages d’approbation au responsable et à d'autres services.
- [Tableau de bord PowerBI](./powerbi/README.md) : un tableau de bord créé par un utilisateur professionnel chevronné qui permet d’analyser les données partagées de parcours.

### Présentation technique

<img src="./media/workridesTech.jpg" Height="80%" Width="80%" />

## En savoir plus
- [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph")
- [Microsoft Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service")
