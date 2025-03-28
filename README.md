# VirtualZoo Designdocument
C# opdracht: Webapp en API voor een virtuele dierentuin

---

### 1. **Titelpagina**

Projectnaam: VirtualZoo

Auteur: Kento Bergsma

Datum: 28/03/2025

Versie: 1.0

---

### 2. **Inleiding**

#### Korte beschrijving van de applicatie:
VirtualZoo is een webapplicatie waarmee gebruikers een virtuele dierentuin kunnen beheren. De applicatie is gericht op studenten, ontwikkelaars en testers die inzicht willen krijgen in hoe een dierentuin op basis van logica, categorieën en verblijven functioneert. Gebruikers kunnen dieren aanmaken, beheren, koppelen aan verblijven en categorieën, en automatisch een indeling laten maken van dieren over verblijven op basis van vooraf ingestelde regels.

De applicatie is ontwikkeld in het kader van een schoolopdracht voor het vak C#. De opdracht is verstrekt door de opleiding zelf en bevatte concrete eisen zoals CRUD-functionaliteit, een duidelijke projectstructuur met een API en frontend, automatische acties zoals "Feeding Time", en het naleven van best practices binnen ASP.NET Core.

#### Korte uitleg van gekozen technologieën:
Voor de ontwikkeling van VirtualZoo is gekozen voor de volgende technologieën:
* ASP.NET Core (MVC en API)

De applicatie is gestructureerd in twee onderdelen:
* Een ASP.NET Core Web API, waarin alle logica, services, repositories en data operations zijn ondergebracht.
* Een ASP.NET Core MVC WebApp, die fungeert als frontend en via HTTP-aanroepen de API gebruikt om data op te halen en weer te geven.

Entity Framework Core
EF Core is gebruikt voor de communicatie met de SQL Server database. Met behulp van code-first benadering worden de modellen automatisch vertaald naar tabellen, en kunnen relaties zoals one-to-many of many-to-many eenvoudig worden beheerd.

Projectstructuur
Het project is opgesplitst in duidelijke lagen:

* VirtualZooAPI: bevat de controllers, services en repositories.

* VirtualZoo: de frontend met controllers en views.

* VirtualZooShared: gedeelde modellen en enums.

* VirtualZooTests: unittests en integratietests.



---

### 3. **Wireframes**

maak wireframes


![Wireframe van Zoo Dashboard](docs/wireframes/zoo_dashboard.png)

---

### 4. **Entity-Relationship Diagram (ERD)**

![ERD van de VirtualZoo](Docs/ERD.svg)

---

### 5. **API Documentatie**

Beschrijf de belangrijkste API-endpoints:

Bijvoorbeeld:


#### GET /api/animal
Retourneert een lijst van alle dieren

#### POST /api/enclosure
Voegt een nieuw verblijf toe  
Body:
```json
{
  "name": "Savannah",
  "size": 100,
  "securityLevel": "High"
}
POST /api/zoo/autoassign?resetExisting=true
Genereert automatisch verblijven voor dieren

```

---

### 6. **Reflectie**
- Wat ging goed tijdens het project?
- Wat waren lastige punten?
- Wat heb je geleerd?
- Wat zou je verbeteren als je meer tijd had?

---

### 7. **Installatie-instructies**
Als je project lokaal opstartbaar moet zijn:
```md
### Installatie
1. Clone deze repo
2. Zorg voor .NET 8 SDK
3. Voer `dotnet ef database update` uit
4. Start de app via `dotnet run`

```


