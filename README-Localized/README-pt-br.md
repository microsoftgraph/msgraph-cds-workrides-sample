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

# Criar soluções completas usando o Microsoft Graph e o serviço de dados comuns

## Visão Geral
> **Observação** esta é uma solução completa publicada como parte da sessão do Microsoft Build 2017 [P4136](https://channel9.msdn.com/Events/Build/2017/P4136). Inclui vários projetos que usam o Microsoft Graph e o serviço de dados comuns, tecnologias Xamarin.

A Microsoft fornece uma plataforma para criar soluções de ponta a ponta. Este exemplo mostra casos de apresentação de como os profissionais desenvolvedores podem compilar pessoas no centro, aplicativos de dados avançados que aproveitam produtividade e dados comerciais do [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") e [o serviço de dados comuns](https://aka.ms/CommonDataService "serviço de dados comuns da Microsoft"). E como os usuários avançados podem utilizar esses dados para criar aplicativos, fluxos e painéis.

[Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") é a API unificada para os serviços da Microsoft. Isso inclui o Office 365, Azure Active Directory e muitos outros.

[serviço de dados comuns da Microsoft](https://aka.ms/CommonDataService "o serviço de dados comuns da Microsoft") ou CDS é um ponto focal para os dados da empresa. Os desenvolvedores profissionais podem criar aplicativos que interagem com esses dados em CDS e os usuários avançados podem tirar proveito do PowerApps, do Flow e do PowerBI para criar aplicativos, criar fluxos de trabalho e executar análises detalhadas sobre os dados sem escrever códigos.

O Microsoft Graph oferece acesso a dados ricos de serviços da Microsoft. Depois de colocar esses dados em CDS, você pode combiná-los com outros dados corporativos dos quais a sua empresa depende. Em seguida, você pode criar aplicativos ricos e fluxos de trabalho com blocos de construção fáceis de usar (por exemplo, [PowerApps](https://powerapps.microsoft.com/en-us/), [Fluxo](https://flow.microsoft.com/en-us/)[PowerBI](https://powerbi.microsoft.com/en-us/)) se souber como escrever códigos ou não. Usar o Microsoft Graph e os desenvolvedores de serviços de dados comuns não só podem criar pessoas centralizadas, aplicativos de dados ricos, mas o aplicativo pode ser estendido por usuários avançados que não são codificadores normais de ter experiências adicionais.

## Quais são as situações abordadas neste exemplo?
Os exemplos publicados como parte deste projeto são criados com base no seguinte cenário:

### Cenário de desenvolvedor de aplicativos profissionais
Muitas empresas enfrentam problemas para facilitar a distância e o estacionamento dos funcionários. Estão procurando soluções. Um desenvolvedor de aplicativos profissionais está trabalhando para resolver esse problema. Ela cria um aplicativo móvel que ajuda as pessoas a encontrar caronas de e para o trabalho. Esse aplicativo se encaixa bem em como as pessoas realmente funcionam. e como esse aplicativo pode ser implantado em várias empresas.

<img src="./media/prodevScenario.jpg" Height="80%" Width="80%" />

### Cenário de usuários avançados corporativos
A contoso é um dos primeiros clientes a adquirir esse aplicativo e disponibilizá-lo para todos os seus funcionários. Os usuários avançados da Contoso (geralmente codificadores não profissionais, como um profissional de RH) ampliam facilmente os dados criados por esse aplicativo para criar experiências adicionais.

<img src="./media/poweruserScenario.jpg" Height="80%" Width="80%" />

## O que está incluso neste projeto:

Siga os links abaixo para saber como as partes individuais foram criadas e recriadas a experiência ponta a ponta para você.

### Desenvolvedor de aplicativos profissionais: criar um aplicativo usando código
- [aplicativo móvel](./mobileapp/README.md)-Xamarin Forms aplicativos que usam as APIs do Microsoft Graph para obter dados e lojas no serviço de dados comuns por meio da API da Web.

### Usuários avançados corporativos – Aproveite os dados e amplie uma solução usando nenhum código
- [CDS](./cds/README.md) – o serviço de dados comuns é usado como repositório de dados para o aplicativo móvel e esses dados são aproveitados ainda mais pelo PowerApps, pelo fluxo e pelo PowerBI.
- [aplicativo PowerApps](./powerapps/README.md)-aplicativo PowerApps criado por um usuário avançado da empresa que usa o Excel como a linguagem de expressão para criar um aplicativo móvel de LOB com o conhecimento para escrever códigos.
- [fluxo](./flow/README.md) o fluxo de trabalho de aprovação de despesas criado por um usuário avançado da empresa que usa o Microsoft Flow que envia emails de aprovação para o gerente e para outros departamentos.
- [Painel do PowerBI](./powerbi/README.md)-um painel criado por um usuário avançado da empresa que ajuda a analisar os dados de compartilhamento de percurso.

### Visão geral técnica

<img src="./media/workridesTech.jpg" Height="80%" Width="80%" />

## Saiba mais
- [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph")
- [Serviços de dados comuns da Microsoft](https://aka.ms/CommonDataService "Serviços de dados comuns da Microsoft")
