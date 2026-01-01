# Deliveries.Domain

Ce projet contient la couche domaine pour le système de gestion des livraisons. Il implémente les concepts du Domain-Driven Design (DDD) avec des entités riches, des value objects, et des événements de domaine.

## Structure

### Entités (`Entities/`)
- **Delivery** : Entité principale représentant une livraison
- **EntityBase** : Classe de base pour la gestion des événements de domaine
- **AggregateRoot** : Racine d'agrégat avec gestion des timestamps

### Value Objects
- **Address** : Représente une adresse (origine et destination)
- **Price** : Représente un prix avec décomposition (base + poids + distance)

### Énumérations
- **DeliveryStatus** : Statuts possibles d'une livraison (Pending, Confirmed, InTransit, Delivered, Cancelled, Failed)

### Services
- **IPricingService** : Interface pour le calcul des prix
- **PricingService** : Implémentation avec la règle : prix_de_base + distance + poids

### Événements (`Events/`)
- **DeliveryCreatedEvent** : Événement levé lors de la création d'une livraison
- **DeliveryStatusChangedEvent** : Événement levé lors du changement de statut

### Exceptions (`Exceptions/`)
- **DeliveryDomainException** : Exception de base du domaine
- **DeliveryNotFoundException** : Livraison introuvable
- **DeliveryValidationException** : Erreur de validation
- **PricingCalculationException** : Erreur de calcul de prix

### Validateurs (`Validators/`)
- **DeliveryValidator** : Validation des règles métier pour les livraisons et adresses

### Factory (`Factories/`)
- **DeliveryFactory** : Factory pour créer des livraisons avec validation automatique

### DTOs (`DTOs/`)
- **CreateDeliveryDto** : DTO pour la création de livraison
- **DeliveryResponseDto** : DTO pour la réponse
- **CreateAddressDto** / **AddressResponseDto** : DTOs pour les adresses
- **PriceResponseDto** : DTO pour les prix

### Constantes (`Constants/`)
- **DeliveryConstants** : Limites métier (poids max, distance max, etc.)
- **PricingConstants** : Constantes de tarification par défaut

### Spécifications (`Specifications/`)
- **BaseSpecification<T>** : Pattern Specification pour les requêtes
- **DeliveriesByClientNameSpecification** : Filtrer par nom de client
- **DeliveriesByStatusSpecification** : Filtrer par statut
- **DeliveriesByDateRangeSpecification** : Filtrer par plage de dates
- **RecentDeliveriesSpecification** : Livraisons récentes

### Repository Interface
- **IDeliveryRepository** : Interface de repository avec méthodes async

## Règles Métier Implémentées

### Validation des Livraisons
- Nom du client : 2-100 caractères
- Poids : 0.1 kg - 1000 kg
- Distance : 0.1 km - 10 000 km
- Adresses : tous les champs obligatoires avec limites de taille
- Adresses origine et destination ne peuvent pas être identiques

### Calcul des Prix
- **Formule** : Prix de base + (Distance × Tarif/km) + (Poids × Tarif/kg)
- **Tarifs par défaut** :
  - Prix de base : 5,00 €
  - Prix par km : 0,50 €
  - Prix par kg : 1,00 €
- **Prix minimum** : 1,00 €

### Gestion des Statuts
- Statut initial : `Pending`
- Transitions possibles vers tous les autres statuts
- Événement généré à chaque changement de statut

## Utilisation

### Création d'une Livraison
```csharp
var pricingService = new PricingService();
var factory = new DeliveryFactory(pricingService);

var originAddress = new Address("123 Rue de la Paix", "Paris", "75001", "France");
var destinationAddress = new Address("456 Avenue des Champs", "Lyon", "69000", "France");

var delivery = factory.CreateDelivery(
    "Jean Dupont",
    originAddress,
    destinationAddress,
    5.0, // poids en kg
    450.0 // distance en km
);
```

### Repository Pattern
```csharp
// Récupération par ID
var delivery = await repository.GetByIdAsync(deliveryId);

// Sauvegarde
var newDelivery = await repository.AddAsync(delivery);

// Vérification d'existence
var exists = await repository.ExistsAsync(deliveryId);
```

### Spécifications
```csharp
// Livraisons d'un client
var spec = new DeliveriesByClientNameSpecification("Jean Dupont");

// Livraisons en transit
var spec = new DeliveriesByStatusSpecification(DeliveryStatus.InTransit);

// Livraisons récentes (24h)
var spec = new RecentDeliveriesSpecification(24);
```

## Événements de Domaine

Les événements sont automatiquement générés lors des actions importantes :
- Création d'une livraison
- Changement de statut
- Calcul de prix

Ces événements peuvent être utilisés pour :
- Notifications
- Audit trails
- Intégration avec d'autres bounded contexts
- Mise à jour de vues de lecture (CQRS)

## Extensibilité

Le domaine est conçu pour être facilement extensible :
- Nouveaux types d'adresses (avec coordonnées GPS)
- Règles de tarification personnalisées
- Nouveaux statuts de livraison
- Validation métier additionnelle
- Nouveaux événements de domaine