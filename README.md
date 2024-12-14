# Teranga.Core

Teranga.Core est une bibliothèque .NET qui fournit un accès aux données administratives du Sénégal ainsi qu'à d'autres fonctionnalités connexes.

## 📋 Fonctionnalités

- Accès aux données des régions, départements et communes du Sénégal
- Validation des données administratives
- Support de la sérialisation JSON
- Extensible pour d'autres types de données

## 🚀 Installation

Via NuGet Package Manager :
```bash
Install-Package Teranga.Core
```

Via .NET CLI :
```bash
dotnet add package Teranga.Core
```

## 🔧 Configuration

```csharp
using Teranga.Core;

// Dans Program.cs ou Startup.cs
services.AddTerangaCore();
```

## 📖 Utilisation

### Obtenir les données d'une région

```csharp
public class Example
{
    private readonly IAdministrativeService _adminService;

    public Example(IAdministrativeService adminService)
    {
        _adminService = adminService;
    }

    public async Task GetRegionExample()
    {
        var regions = await _adminService.GetAllRegionsAsync();
        var dakar = await _adminService.GetRegionByCodeAsync("DK");
    }
}
```

## 🛠️ Développement

### Prérequis

- .NET 8.0 SDK
- Visual Studio 2022 ou VS Code

### Build

```bash
dotnet build
```

### Tests

```bash
dotnet test
```

## 📝 Documentation

La documentation complète est disponible dans le dossier [/docs](/docs).

## 🤝 Contribution

Les contributions sont les bienvenues ! Voici comment vous pouvez contribuer :

1. Fork le projet
2. Créez votre branche (`git checkout -b feature/AmazingFeature`)
3. Committez vos changements (`git commit -m 'Add some AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrez une Pull Request

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](/LICENSE) pour plus de détails.

## ✨ Remerciements

- Contributeurs
- Communauté Open Source
- Utilisateurs du projet

## 📞 Contact

- Email : ibrahimadiaw1997@gmail.com
- X : [@IbrahimaDiaw](https://x.com/IbrahimaIbnOmar)
- GitHub : [@IbrahimaDiaw](https://github.com/IbrahimaDiaw)
- LinkedIn : [@ibrahimaDiaw](https://www.linkedin.com/in/ibrahima-diaw-0540a71b9/)
```