# Deliveries.Application

Ce projet contient la couche application pour le système de gestion des livraisons. Il implémente les patterns CQRS avec MediatR, validation avec FluentValidation, et les services applicatifs.

## Structure

### Commands (`Commands/`)
- **CreateDeliveryCommand** : Commande pour créer une nouvelle livraison
- **CreateDeliveryResult** : Résultat de création avec gestion d'erreurs

### Queries (`Queries/`)
- **GetDeliveryByIdQuery** : Récupérer une livraison par ID
- **GetAllDeliveriesQuery** : Récupérer toutes les livraisons
- **GetDeliveriesByStatusQuery** : Récupérer les livraisons par statut

### Handlers (`Handlers/`)
#### Commands
- **CreateDeliveryCommandHandler** : Gère la création de livraisons

#### Queries
- **GetDeliveryByIdQueryHandler** : Gère la récupération par ID
- **GetAllDeliveriesQueryHandler** : Gère la récupération de toutes les livraisons
- **GetDeliveriesByStatusQueryHandler** : Gère la récupération par statut

### Services (`Services/`)
- **DeliveryApplicationService** : Service applicatif principal
- **PricingService** : Implémentation du service de tarification

### Validators (`Validators/`)
- **CreateDeliveryCommandValidator** : Validation FluentValidation pour la création
- **CreateAddressDtoValidator** : Validation des adresses

### Mappers (`Mappers/`)
- **DeliveryMapper** : Conversion entre entités du domaine et DTOs

### Behaviors (`Behaviors/`)
- **ValidationBehavior** : Comportement MediatR pour validation automatique
- **LoggingBehavior** : Comportement pour le logging des performances

### Interfaces (`Interfaces/`)
- **IDeliveryApplicationService** : Interface du service applicatif

### Extensions (`Extensions/`)
- **ServiceCollectionExtensions** : Configuration de l'injection de dépendance

## Patterns Implémentés

### CQRS (Command Query Responsibility Segregation)
- Séparation claire entre les commandes (écriture) et les requêtes (lecture)
- Utilisation de MediatR pour le routing des commandes et requêtes

### Validation
- Validation automatique via FluentValidation
- Pipeline behavior pour validation avant exécution des handlers
- Validation métier au niveau du domaine

### Architecture en Couches
- Couche application indépendante du domaine et de l'infrastructure
- Utilisation d'interfaces pour l'inversion de dépendance
- Services applicatifs orchestrant les opérations

## Règles de Validation (FluentValidation)

### CreateDeliveryCommand
- **Nom du client** : 2-100 caractères, obligatoire
- **Poids** : 0.1-1000 kg
- **Distance** : 0.1-10,000 km
- **Adresses** : validation complète avec tous les champs obligatoires
- **Logique métier** : adresses origine et destination différentes

### Messages d'Erreur
- Messages en français, descriptifs
- Validation spécifique par champ
- Gestion des erreurs de validation groupées

## Services Applicatifs

### IDeliveryApplicationService
```csharp
// Création d'une livraison
var delivery = await _deliveryService.CreateDeliveryAsync(createDto);

// Récupération par ID
var delivery = await _deliveryService.GetDeliveryByIdAsync(id);

// Récupération de toutes les livraisons
var deliveries = await _deliveryService.GetAllDeliveriesAsync();

// Récupération par statut
var deliveries = await _deliveryService.GetDeliveriesByStatusAsync("Pending");
```

## Configuration de l'Injection de Dépendance

### Dans Startup.cs ou Program.cs
```csharp
using Deliveries.Application.Extensions;

// Ajouter tous les services de l'application
services.AddApplicationServices();

// Ou configuration manuelle
services.AddMediatR(typeof(CreateDeliveryCommand));
services.AddValidatorsFromAssembly(typeof(CreateDeliveryCommandValidator));
services.AddScoped<IDeliveryApplicationService, DeliveryApplicationService>();
```

## Pipeline MediatR

### Behaviors Configurés
1. **LoggingBehavior** : Log des performances et erreurs
2. **ValidationBehavior** : Validation automatique avant exécution

### Ordre d'Exécution
```
Request → ValidationBehavior → LoggingBehavior → Handler → Response
```

## Gestion des Erreurs

### Types d'Exceptions
- **ValidationApplicationException** : Erreurs de validation
- **NotFoundException** : Ressource introuvable
- **BusinessConflictException** : Conflit métier

### Logging
- Logs structurés avec contexte
- Mesure des performances
- Traçabilité complète des opérations

## Utilisation

### Création d'une Livraison
```csharp
var createDto = new CreateDeliveryDto
{
    ClientName = "Jean Dupont",
    OriginAddress = new CreateAddressDto
    {
        Street = "123 Rue de la Paix",
        City = "Paris",
        PostalCode = "75001",
        Country = "France"
    },
    DestinationAddress = new CreateAddressDto
    {
        Street = "456 Avenue des Champs",
        City = "Lyon",
        PostalCode = "69000",
        Country = "France"
    },
    Weight = 5.0,
    Distance = 450.0
};

var delivery = await _deliveryService.CreateDeliveryAsync(createDto);
```

### Récupération d'une Livraison
```csharp
var delivery = await _deliveryService.GetDeliveryByIdAsync(deliveryId);
if (delivery == null)
{
    // Livraison non trouvée
}
```

## Tarification

### Service de Tarification
- **Règle** : Prix de base + (Distance × Tarif/km) + (Poids × Tarif/kg)
- **Tarifs configurables** par injection de dépendance
- **Prix minimum** appliqué automatiquement

### Configuration des Tarifs
```csharp
services.AddScoped<IPricingService>(provider => 
    new PricingService(
        basePrice: 5.0m,    // Prix de base
        pricePerKm: 0.5m,   // Prix par km
        pricePerKg: 1.0m    // Prix par kg
    ));
```

## Extensibilité

La couche application est conçue pour être facilement extensible :
- Nouveaux commands/queries via MediatR
- Nouveaux behaviors pour cross-cutting concerns
- Validation personnalisée via FluentValidation
- Services applicatifs additionnels
- Mappers personnalisés

## Dépendances

- **MediatR** : Pattern mediator pour CQRS
- **FluentValidation** : Validation fluide et expressive
- **Microsoft.Extensions.Logging** : Logging standardisé
- **Microsoft.Extensions.DependencyInjection** : Injection de dépendance