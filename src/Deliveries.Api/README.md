# API Deliveries - Documentation

## Endpoints Implémentés

### 1. POST /api/deliveries - Création de livraison

Crée une nouvelle livraison avec calcul automatique du prix selon la règle : **prix_de_base + distance + poids**

#### Requête
```http
POST /api/deliveries
Content-Type: application/json

{
  "clientName": "Jean Dupont",
  "originAddress": {
    "street": "123 Avenue des Champs-Élysées",
    "city": "Paris",
    "postalCode": "75008",
    "country": "France"
  },
  "destinationAddress": {
    "street": "45 Place Bellecour",
    "city": "Lyon",
    "postalCode": "69002",
    "country": "France"
  },
  "weight": 5.0,
  "distance": 465.0
}
```

#### Réponse (201 Created)
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "clientName": "Jean Dupont",
  "originAddress": {
    "street": "123 Avenue des Champs-Élysées",
    "city": "Paris",
    "postalCode": "75008",
    "country": "France",
    "fullAddress": "123 Avenue des Champs-Élysées, Paris 75008, France"
  },
  "destinationAddress": {
    "street": "45 Place Bellecour",
    "city": "Lyon",
    "postalCode": "69002",
    "country": "France",
    "fullAddress": "45 Place Bellecour, Lyon 69002, France"
  },
  "weight": 5.0,
  "distance": 465.0,
  "status": "Pending",
  "price": {
    "basePrice": 5.00,
    "weightPrice": 5.00,
    "distancePrice": 232.50,
    "totalPrice": 242.50,
    "currency": "EUR",
    "formattedPrice": "242,50 EUR"
  },
  "createdAt": "2026-01-01T10:30:00.000Z",
  "updatedAt": null
}
```

#### Validation et Règles Métier
- **Nom du client** : 2-100 caractères, obligatoire
- **Poids** : 0.1-1000 kg
- **Distance** : 0.1-10,000 km
- **Adresses** : Tous les champs obligatoires, longueurs limitées
- **Logique** : Adresses origine et destination différentes

#### Calcul du Prix
- **Prix de base** : 5,00 €
- **Prix par kg** : 1,00 € (5 kg × 1,00 € = 5,00 €)
- **Prix par km** : 0,50 € (465 km × 0,50 € = 232,50 €)
- **Total** : 5,00 € + 5,00 € + 232,50 € = **242,50 €**

### 2. GET /api/deliveries/{id} - Récupération par ID

Récupère une livraison spécifique par son identifiant.

#### Requête
```http
GET /api/deliveries/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

#### Réponse (200 OK)
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "clientName": "Jean Dupont",
  "originAddress": {
    "street": "123 Avenue des Champs-Élysées",
    "city": "Paris",
    "postalCode": "75008",
    "country": "France",
    "fullAddress": "123 Avenue des Champs-Élysées, Paris 75008, France"
  },
  "destinationAddress": {
    "street": "45 Place Bellecour",
    "city": "Lyon",
    "postalCode": "69002",
    "country": "France",
    "fullAddress": "45 Place Bellecour, Lyon 69002, France"
  },
  "weight": 5.0,
  "distance": 465.0,
  "status": "Pending",
  "price": {
    "basePrice": 5.00,
    "weightPrice": 5.00,
    "distancePrice": 232.50,
    "totalPrice": 242.50,
    "currency": "EUR",
    "formattedPrice": "242,50 EUR"
  },
  "createdAt": "2026-01-01T10:30:00.000Z",
  "updatedAt": null
}
```

#### Réponse (404 Not Found)
```json
{
  "statusCode": 404,
  "message": "Aucune livraison trouvée avec l'ID: a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "timestamp": "2026-01-01T10:35:00.000Z"
}
```

## Endpoints Bonus

### 3. GET /api/deliveries - Liste de toutes les livraisons

Récupère toutes les livraisons du système.

#### Requête
```http
GET /api/deliveries
```

#### Réponse (200 OK)
```json
[
  {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "clientName": "Jean Dupont",
    // ... détails complets
  },
  {
    "id": "b2c3d4e5-f6g7-8901-bcde-f23456789012",
    "clientName": "Marie Martin",
    // ... détails complets
  }
]
```

## Gestion des Erreurs

### Validation (400 Bad Request)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "clientName": ["Le nom du client est obligatoire"],
    "weight": ["Le poids doit être supérieur à 0.1 kg"],
    "distance": ["La distance doit être supérieure à 0.1 km"]
  }
}
```

### Erreur Interne (500 Internal Server Error)
```json
{
  "statusCode": 500,
  "message": "Une erreur interne s'est produite",
  "timestamp": "2026-01-01T10:35:00.000Z"
}
```

## Health Checks

### GET /health - Statut global
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "database": {
      "status": "Healthy",
      "duration": "00:00:00.0098765"
    }
  }
}
```

## Service de Tarification

### Règle Implémentée
**Total = Prix de base + (Distance × Tarif/km) + (Poids × Tarif/kg)**

### Tarifs par Défaut
- **Prix de base** : 5,00 €
- **Prix par kilomètre** : 0,50 €
- **Prix par kilogramme** : 1,00 €
- **Prix minimum** : 1,00 €
- **Devise** : EUR

### Exemples de Calcul

#### Exemple 1 : Livraison courte
- Poids : 2 kg
- Distance : 50 km
- **Calcul** : 5,00 € + (50 × 0,50 €) + (2 × 1,00 €) = **32,00 €**

#### Exemple 2 : Livraison lourde
- Poids : 25 kg
- Distance : 100 km
- **Calcul** : 5,00 € + (100 × 0,50 €) + (25 × 1,00 €) = **80,00 €**

#### Exemple 3 : Livraison longue distance
- Poids : 1 kg
- Distance : 1000 km
- **Calcul** : 5,00 € + (1000 × 0,50 €) + (1 × 1,00 €) = **506,00 €**

## URLs de Test

### Développement Local
- **API Base** : http://localhost:5000
- **Swagger UI** : http://localhost:5000/swagger
- **Health Check** : http://localhost:5000/health

### Docker
- **API Base** : http://localhost:5000
- **Swagger UI** : http://localhost:5000/swagger
- **pgAdmin** : http://localhost:8080