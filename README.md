# UniDesk

UniDesk to prosty system obslugi zgloszen przygotowany na laboratoria ASP.NET Core MVC.
Kod aplikacji znajduje sie w folderze `src/UniDesk.Web`.

## Uruchomienie

### Visual Studio

1. Otworz solution `src/UniDesk/UniDesk.sln`.
2. Ustaw projekt `UniDesk.Web` jako startowy.
3. Uruchom aplikacje przez F5.
4. W przegladarce otworz adres pokazany przez Visual Studio.

### Terminal

1. Przejdz do folderu `src/UniDesk.Web`.
2. Wykonaj `dotnet restore`.
3. Wykonaj `dotnet build`.
4. Wykonaj `dotnet run --urls http://localhost:5099`.
5. Otworz `http://localhost:5099`.

## Baza danych

Aplikacja korzysta z SQLite. Connection string znajduje sie w pliku:

```text
src/UniDesk.Web/appsettings.json
```

Domyslna baza:

```text
Data Source=UniDesk.db
```

Migracje EF Core sa w folderze `src/UniDesk.Web/Migrations`.
Przy starcie aplikacja wykonuje migracje i tworzy dane testowe, jezeli baza jest pusta.
Konfiguracja seedowania znajduje sie w sekcji `SeedData` w `appsettings.json`.

## Struktura projektu

- `Controllers` - kontrolery MVC i API, np. `TicketsController` oraz `TicketsApiController`.
- `Views` - widoki Razor dla interfejsu MVC, np. lista, szczegoly i edycja zgloszen.
- `wwwroot` - pliki statyczne aplikacji: CSS, JavaScript oraz biblioteki frontendowe.
- `Models` - encje domenowe, konfiguracje EF Core i `UniDeskDbContext`.
- `DTOs` - modele wejscia i wyjscia dla API, aby encje bazy nie wychodzily bezposrednio do HTTP.
- `Services` - logika aplikacyjna, m.in. `TicketService` i serwis komentarzy.
- `Data` - seeding uzytkownikow, rol, zgloszen i komentarzy.
- `Middleware` - przekrojowe elementy potoku, np. correlation id i obsluga 404.
- `Endpoints` - Minimal API dla wybranych operacji.
- `HealthChecks` - wlasne sprawdzanie stanu aplikacji i zaleznosci.

## Punkty wejscia

- `/` - strona glowna z napisem Hello UniDesk.
- `/About` - strona informacyjna.
- `/Tickets` - lista zgloszen, filtrowanie, stronicowanie i formularz dodawania.
- `/Tickets/Details/{id}` - szczegoly zgloszenia, dyskusja i formularz komentarza.
- `/Tickets/Edit/{id}` - edycja zgloszenia.
- `/api/tickets` - glowne API zgloszen.
- `/api/tickets/{id}` - pobranie pojedynczego zgloszenia przez API.
- `/api/tickets/{id}/status` - zmiana statusu przez PATCH.
- `/api/v2/tickets` - Minimal API.
- `/api/ambitne/login` - logowanie API i pobranie tokena.
- `/health/live` - sprawdzenie, czy proces aplikacji dziala.
- `/health/ready` - sprawdzenie gotowosci aplikacji, bazy SQLite i zaleznosci.
- `/swagger` - dokumentacja API dostepna tylko w srodowisku Development.

## Konta testowe

- Admin: `admin@unidesk.local` / `Admin123!`
- Uzytkownik: `employee@top-uni.edu.pl` / `Employee123!`
- Uzytkownik bez dostepu do cudzych spraw: `outsider@unidesk.local` / `Outsider123!`

`employee` jest wlascicielem przykladowych zgloszen. `admin` ma role `Admin`.
`outsider` jest zalogowany, ale nie ma dostepu do dyskusji cudzych spraw.

## Najwazniejsze funkcje

- Lista zgloszen korzysta z filtrowania, sortowania i stronicowania.
- API zwraca DTO, a nie encje EF Core.
- Formularze uzywaja walidacji ModelState i Anti-Forgery Token.
- Zgloszenia i komentarze sa zapisywane w SQLite.
- Komentarze sa powiazane ze zgloszeniem relacja 1:N.
- Dyskusja jest dostepna tylko dla autora sprawy albo administratora.
- Komentarze obsluguja prosty Markdown: pogrubienie, kod inline i bloki kodu.
- HTML w komentarzach jest kodowany, aby ograniczyc ryzyko XSS.
- Dodawanie komentarzy ma rate limiting i po przekroczeniu limitu zwraca `429 Too Many Requests`.
- Logi sa zapisywane strukturalnie w JSON i zawieraja `CorrelationId`.
- Projekt ma wlaczone `TreatWarningsAsErrors`.

## Testy i weryfikacja

Budowanie aplikacji:

```bash
dotnet build src/UniDesk.Web/UniDesk.Web.csproj
```

Uruchomienie testow:

```bash
dotnet test src/UniDesk/UniDesk.sln
```

Przy poprawnym stanie projektu kompilacja konczy sie bez ostrzezen, a testy jednostkowe i integracyjne przechodza.
