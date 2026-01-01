# Deliveries.Infrastructure

Ce projet contient la couche infrastructure pour le système de gestion des livraisons. Il implémente la persistance avec Entity Framework Core et PostgreSQL.

## Structure

### Persistence (`Persistence/`)
- **DeliveryDbContext** : Contexte Entity Framework Core
- **Configurations/** : Configuration des entités pour EF Core
- **Seeders/** : Initialisation des données de test
- **Factories/** : Factory pour les migrations

### Repositories (`Repositories/`)
- **DeliveryRepository** : Implémentation du repository avec EF Core

### Data (`Data/`)
- **PostgreSQLConnectionFactory** : Factory de connexions PostgreSQL
- **DatabaseConfiguration** : Configuration de la base de données

### Services (`Services/`)
- **DatabaseInitializationService** : Service d'initialisation de la DB

### Health Checks (`HealthChecks/`)
- **DatabaseHealthCheck** : Vérification de la connexion DB
- **DatabaseMigrationHealthCheck** : Vérification des migrations

### Extensions (`Extensions/`)
- **ServiceCollectionExtensions** : Configuration de l'injection de dépendance

## Configuration de la Base de Données

### PostgreSQL
- **Base de données** : `deliveries_db`
- **Utilisateur** : `admin`
- **Mot de passe** : `admin`
- **Host** : `localhost` (par défaut)

### Chaîne de Connexion
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=deliveries_db;Username=admin;Password=admin;"
  }
}
```

## Schéma de Base de Données

### Table `deliveries`
```sql
CREATE TABLE deliveries (
    id UUID PRIMARY KEY,
    client_name VARCHAR(100) NOT NULL,
    weight DECIMAL(8,2) NOT NULL,
    distance DECIMAL(10,2) NOT NULL,
    status VARCHAR(50) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL,
    updated_at TIMESTAMPTZ,
    
    -- Adresse d'origine
    origin_street VARCHAR(200) NOT NULL,
    origin_city VARCHAR(200) NOT NULL,
    origin_postal_code VARCHAR(20) NOT NULL,
    origin_country VARCHAR(100) NOT NULL,
    
    -- Adresse de destination
    destination_street VARCHAR(200) NOT NULL,
    destination_city VARCHAR(200) NOT NULL,
    destination_postal_code VARCHAR(20) NOT NULL,
    destination_country VARCHAR(100) NOT NULL,
    
    -- Prix calculé
    price_base DECIMAL(10,2),
    price_weight DECIMAL(10,2),
    price_distance DECIMAL(10,2),
    price_currency VARCHAR(3) DEFAULT 'EUR'
);

-- Index pour les performances
CREATE INDEX ix_deliveries_client_name ON deliveries(client_name);
CREATE INDEX ix_deliveries_status ON deliveries(status);
CREATE INDEX ix_deliveries_created_at ON deliveries(created_at);
```

## Configuration Entity Framework

### Value Objects
Les value objects (`Address`, `Price`) sont mappés comme owned entities :
- **Address** : Mappé inline dans la table deliveries
- **Price** : Mappé inline avec séparation des composants

### Enums
- **DeliveryStatus** : Stocké comme string pour lisibilité

### Conventions de Nommage
- **Tables** : snake_case (PostgreSQL standard)
- **Colonnes** : snake_case
- **Index** : Préfixe `ix_`

## Repository Pattern

### IDeliveryRepository
```csharp
// Récupération
var delivery = await _repository.GetByIdAsync(id);
var allDeliveries = await _repository.GetAllAsync();

// Création
var newDelivery = await _repository.AddAsync(delivery);

// Mise à jour
var updated = await _repository.UpdateAsync(delivery);

// Suppression
var deleted = await _repository.DeleteAsync(id);

// Vérification d'existence
var exists = await _repository.ExistsAsync(id);
```

### Fonctionnalités
- **Logging** : Logs détaillés pour toutes les opérations
- **Gestion d'erreurs** : Exceptions appropriées
- **Timestamps automatiques** : Gestion via SaveChangesAsync
- **Performance** : Requêtes optimisées avec index

## Initialisation et Seeding

### DatabaseInitializationService
Service hébergé qui :
- Crée la base de données si nécessaire
- Applique les migrations automatiquement
- Seed les données de test

### Données de Test
5 livraisons de test créées automatiquement :
- Paris → Lyon (Jean Dupont)
- Marseille → Nice (Marie Martin)
- Toulouse → Bordeaux (Pierre Durand)
- Lille → Strasbourg (Sophie Petit)
- Nantes → Rennes (Michel Blanc)

## Health Checks

### DatabaseHealthCheck
- Vérifie la connexion PostgreSQL
- Test les requêtes sur la table deliveries
- Retourne des métadonnées utiles

### DatabaseMigrationHealthCheck
- Vérifie l'état des migrations
- Liste les migrations en attente
- Status dégradé si migrations pending

### Endpoints Health Check
```
GET /health/ready - Statut global
GET /health/live - Liveness check
```

## Configuration de l'Injection de Dépendance

### Utilisation Simple
```csharp
using Deliveries.Infrastructure.Extensions;

// Dans Program.cs ou Startup.cs
services.AddInfrastructureServices(configuration);
services.AddPostgreSQLConfiguration();
```

### Services Enregistrés
- `DeliveryDbContext` avec PostgreSQL
- `IDeliveryRepository → DeliveryRepository`
- `IDbConnectionFactory → PostgreSQLConnectionFactory`
- `DeliveryDbSeeder`
- `DatabaseInitializationService`
- Health checks personnalisés

## Migrations Entity Framework

### Commandes Utiles
```bash
# Créer une nouvelle migration
dotnet ef migrations add InitialCreate --project src/Deliveries.Infrastructure

# Appliquer les migrations
dotnet ef database update --project src/Deliveries.Infrastructure

# Générer le script SQL
dotnet ef migrations script --project src/Deliveries.Infrastructure

# Supprimer la dernière migration
dotnet ef migrations remove --project src/Deliveries.Infrastructure
```

### Configuration des Migrations
- **Assembly** : Deliveries.Infrastructure
- **Factory** : DeliveryDbContextFactory pour design-time
- **Nom des tables** : snake_case pour PostgreSQL

## Configuration PostgreSQL

### Docker Compose (Optionnel)
```yaml
version: '3.8'
services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: deliveries_db
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: admin
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

### Configuration Avancée
```csharp
services.AddDbContext<DeliveryDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5));
    });
    
    // Développement uniquement
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});
```

## Gestion des Erreurs

### Types d'Erreurs Gérés
- **Connexion PostgreSQL** : Retry automatique
- **Contraintes de base** : Validation métier
- **Timeouts** : Configuration du timeout des commandes
- **Transactions** : Gestion automatique par EF Core

### Logging
- **Debug** : Requêtes SQL et paramètres
- **Information** : Opérations CRUD réussies
- **Error** : Exceptions avec contexte complet

## Performance

### Optimisations
- **Index stratégiques** sur les colonnes fréquemment requêtées
- **Connection pooling** via Npgsql
- **Retry policy** pour la résilience
- **Async/await** pour toutes les opérations DB

### Métriques
- Health checks pour monitoring
- Logs structurés pour observabilité
- Timeout configurables

## Dépendances

- **Microsoft.EntityFrameworkCore** : ORM principal
- **Npgsql.EntityFrameworkCore.PostgreSQL** : Provider PostgreSQL
- **Microsoft.Extensions.Hosting** : Services hébergés
- **Microsoft.Extensions.HealthChecks** : Monitoring santé

## Utilisation en Développement

### Première Installation
1. Installer PostgreSQL ou utiliser Docker
2. Créer la base `deliveries_db`
3. Configurer la chaîne de connexion
4. Lancer l'application (migrations automatiques)

### Reset de la Base
```bash
# Supprimer toutes les migrations
rm -rf Migrations/

# Créer une nouvelle migration
dotnet ef migrations add InitialCreate

# Appliquer
dotnet ef database update
```

L'infrastructure est maintenant prête pour supporter les endpoints API avec une persistance complète PostgreSQL.