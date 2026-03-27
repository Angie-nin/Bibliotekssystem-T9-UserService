# Bibliotekssystem – UserService API

## Beskrivning

Detta är UserService för bibliotekssystemet. API:t hanterar användare och roller (Admin/User) och används av en separat klient (MVC/React).

## Live API

* API: https://user-service-t9.azurewebsites.net
* API-dokumentation (Scalar): https://user-service-t9.azurewebsites.net/scalar

## Funktioner

* Hämta alla användare
* Hämta användare via id
* Skapa användare
* Uppdatera användare
* Ta bort användare
* Logga in användare
* Filtrera användare baserat på roll

## Teknologier

* ASP.NET Core Web API
* Entity Framework Core
* SQLite
* DTO-baserad datamodell
* API-Key middleware

## Starta projektet lokalt

1. Klona repositoryt
2. Öppna projektet i Visual Studio
3. Starta applikationen

API:t körs lokalt på:

```
https://localhost:7055
```

Dokumentation nås via:

```
https://localhost:7055/scalar/
```

## Testdata

Vid första uppstart skapas databasen automatiskt och testanvändare seedas.

Exempel:

* Anna Andersson (User)
* Erik Eriksson (Admin)
* Sara Svensson (User)
* Johan Johansson (User)
* Maria Karlsson (Admin)

Gemensamt lösenord:

```
Test123!
```

## Exempel på endpoints

* GET /api/users
* GET /api/users/{id}
* POST /api/users
* PUT /api/users/{id}
* DELETE /api/users/{id}
* POST /api/users/login
* GET /api/users/role/{role}

## Säkerhet

Skrivande endpoints kräver API-Key via header:

```
X-API-KEY
```

## Övrigt

API:t använder lösenordshashning via ASP.NET Identity PasswordHasher.
