# Bibliotekssystem – UserService API

Detta är UserService för bibliotekssystemet.  
API:t hanterar användare och roller (Admin/User) och används av MVC-klienten.

## Funktioner
- Hämta alla användare
- Hämta användare via id
- Skapa användare
- Uppdatera användare
- Ta bort användare
- Login
- Filtrera användare på roll

## Teknik
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- DTO-validering
- API-Key middleware

## Starta projektet lokalt

1. Klona repo
2. Öppna projektet i Visual Studio
3. Kör:
dotnet run

Swagger öppnas automatiskt i development-läge.

## Exempel endpoints
GET /api/users
GET /api/users/{id}
POST /api/users
PUT /api/users/{id}
DELETE /api/users/{id}
POST /api/users/login
GET /api/users/role/{role}

## Säkerhet
Skrivande endpoints kräver API-Key via header:
X-API-KEY


