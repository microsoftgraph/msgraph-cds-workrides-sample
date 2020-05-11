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

# Cree soluciones de un extremo a otro con Microsoft Graph y Common Data Service

## Información general
> **Nota** esta es una solución de un extremo a otro publicada como parte de la sesión de Microsoft Build 2017 [P4136](https://channel9.msdn.com/Events/Build/2017/P4136). Esta contiene varios proyectos que usan Microsoft Graph y Common Data Service, las tecnologías de Xamarin.

Microsoft proporciona una plataforma para crear soluciones de un extremo a otro. Este ejemplo muestra cómo los desarrolladores profesionales pueden crear aplicaciones ricas en datos personas y centradas en los usuarios, que aprovechan la productividad y los datos profesionales de [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") y [Common Data Service](https://aka.ms/CommonDataService "Microsoft Common Data Service"). Y el modo en que los usuarios avanzados pueden usar los datos aún más para crear aplicaciones, flujos y paneles.

[Microsoft Graph](https://graph.microsoft.com "Microsoft Graph") es la API unificada de servicios de Microsoft. Incluye Office 365, Azure Active Directory y muchos otros.

El [servicio de datos comunes de Microsoft](https://aka.ms/CommonDataService "servicio de datos comunes de Microsoft") o CDS es un punto focal para los datos de una empresa. Los desarrolladores profesionales pueden escribir aplicaciones que interactúen con esos datos en CDS y los usuarios avanzados podrán aprovechar PowerApps, Flow y Power BI para crear aplicaciones, diseñar flujos de trabajo y realizar análisis en profundidad sobre esos datos sin tener que escribir ningún código.

Microsoft Graph permite tener acceso a los datos enriquecidos de los servicios Microsoft. Una vez que haya incorporado esos datos a CDS, puede combinarlos con otros datos profesionales de los que dependa su empresa. Después, puede crear aplicaciones enriquecidas y flujos de trabajo con bloques de creación fáciles de usar (por ejemplo, [PowerApps](https://powerapps.microsoft.com/en-us/), [Flow](https://flow.microsoft.com/en-us/) y [PowerBI](https://powerbi.microsoft.com/en-us/)), independientemente de si ya sabes cómo escribir código. Por lo tanto, el uso de Microsoft Graph y los programadores de servicio de datos comunes no solo puede crear aplicaciones centradas en personas y con datos enriquecidos, sino que la aplicación puede ser ampliada por usuarios avanzados que no sean codificadores regulares para tener experiencias adicionales.

## ¿Qué escenarios cubre este ejemplo?
Las muestras publicadas como parte de este proyecto se basan en el escenario siguiente:

### Escenario de desarrollador de aplicaciones profesional
Muchas empresas se enfrentan a problemas para facilitar el transporte y el estacionamiento para sus empleados. Están buscando soluciones. Un desarrollador de aplicaciones profesional está trabajando para resolver este problema. Crea una aplicación móvil que ayuda a los usuarios a encontrar a alguien que les lleve desde y al trabajo. Esta aplicación se adapta perfectamente al modo en que los usuarios trabajan en la práctica, y puede implementarse en muchas empresas.

<img src="./media/prodevScenario.jpg" Height="80%" Width="80%" />

### Escenario de usuarios avanzados de empresas
Contoso es uno de los primeros clientes en adquirir esta aplicación y hacer que esté disponible para todos los empleados. Los usuarios avanzados de Contoso (por lo general, que no son codificadores profesionales, como una persona de RRHH) pueden ampliar fácilmente los datos creados por esta aplicación para crear experiencias adicionales.

<img src="./media/poweruserScenario.jpg" Height="80%" Width="80%" />

## ¿Qué incluye este proyecto?

Siga los vínculos que se muestran a continuación para obtener información sobre cómo se han creado las distintas partes individuales y recrear la experiencia de un extremo a otro.

### Desarrollador de aplicaciones profesional: crear una aplicación mediante código
- [Aplicación móvil](./mobileapp/README.md): las aplicaciones de Xamarin Forms que usan las API de Microsoft Graph para obtener datos de personas y los almacena en el servicio de datos comunes mediante la API Web.

### Usuarios avanzados de empresas: usan datos y amplían las soluciones sin código.
- [CDS](./cds/README.md): el servicio de datos comunes se usa como repositorio de datos de la aplicación móvil y esos datos son aprovechados más tarde por PowerApps, Flow y PowerBI.
- [PowerApps App](./powerapps/README.md): PowerApps creado por un usuario avanzado de empresa que usa Excel como el lenguaje de expresión para crear una aplicación de LOB para móviles sin saber escribir código.
- [Flow](./flow/README.md): flujos de trabajo de aprobación de gastos creado por un usuario avanzado de empresa que usa Microsoft Flow que envía correos electrónicos de aprobación al administrador y a otros departamentos.
- [Panel de PowerBI](./powerbi/README.md): un panel creado por un usuario avanzado de empresa que ayuda a analizar los datos de viajes.

### Información general técnica

<img src="./media/workridesTech.jpg" Height="80%" Width="80%" />

## Más información
- [Microsoft Graph](https://graph.microsoft.com "Microsoft Graph")
- [Servicio de fatos comunes de Microsoft](https://aka.ms/CommonDataService "Servicio de fatos comunes de Microsoft")
