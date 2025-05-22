# Projet .NET - ERP

Ce projet est une application .NET Core pour la gestion RH. Suivez les étapes ci-dessous pour configurer et exécuter le projet.

## Prérequis

Avant de commencer, vous devez installer les packages NuGet listés ci-dessous, avec les versions spécifiées.

### Installation des packages

Exécutez les commandes suivantes dans votre terminal à la racine du projet pour installer les packages requis :

```bash
dotnet add package Microsoft.AspNetCore.Identity --version 2.1.39
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 8.0.7
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 7.0.5
dotnet add package Swashbuckle.AspNetCore
```
### Configuration de la base de données
Une fois les packages installés, appliquez les migrations pour créer la base de données en exécutant la commande suivante :
```bash
dotnet ef database update
```
### Connexion à la base de données
Maintenant pour visualiser les données dans la base il faut se connecter sur SQL Server soit à travers SQL Server de Visual Studio 
ou bien à travers l'application SQL Server Management Studio et voila les données de connxion figurant dans appsettings.json

![image](https://github.com/user-attachments/assets/281e5924-1a38-4504-9bd2-7f3f14f71b77)

### Accès à la documentation Swagger
Une fois le projet lancé, vous pouvez accéder à l'interface Swagger pour tester les endpoints de l'API via l’URL suivante :
```bash
http://localhost:xxxx/swagger
```
Remplace xxxx par le port utilisé par ton application (visible dans la console au démarrage ou dans launchSettings.json).
