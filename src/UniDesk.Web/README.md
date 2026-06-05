# UniDesk.Web

Prosty system obsługi zgłoszeń dla laboratorium ASP.NET.

## Uruchomienie

1. Otwórz terminal w folderze `src/UniDesk.Web`.
2. Uruchom `dotnet restore`.
3. Uruchom `dotnet build`.
4. Uruchom `dotnet run`.
5. Wejdź w adres pokazany w terminalu, najczęściej `https://localhost:5001` albo `http://localhost:5000`.

## Baza danych

Aplikacja używa SQLite. Connection string znajduje się w `appsettings.json`:

`Data Source=UniDesk.db`

Przy starcie aplikacja wykonuje migracje EF Core i tworzy dane testowe, jeśli baza jest pusta.

## Konta testowe

- Admin: `admin@unidesk.local` / `Admin123!`
- Użytkownik: `employee@top-uni.edu.pl` / `Employee123!`
- Uzytkownik bez dostepu do cudzych spraw: `outsider@unidesk.local` / `Outsider123!`

Wartości seedowania są ustawione przez sekcję `SeedData` w `appsettings.json`.

## Punkty wejścia

- `/Tickets` - lista zgłoszeń i formularz dodawania
- `/Tickets/Details/{id}` - szczegóły zgłoszenia, komentarze i formularz dyskusji
- `/api/tickets` - kontroler API dla zgłoszeń
- `/api/v2/tickets` - Minimal API dla zgłoszeń
- `/health/live` i `/health/ready` - endpointy diagnostyczne
- `/swagger` - dokumentacja API tylko w środowisku Development

## Zakres Stretch

- Komentarze obsluguja prosty Markdown: `**pogrubienie**`, `kod inline` i bloki kodu w ``` .
- Markdown jest renderowany po stronie serwera przez bezpieczny formatter, ktory najpierw koduje HTML.
- Dyskusja jest widoczna tylko dla autora sprawy albo administratora.
- Obcy uzytkownik widzi zablokowany interfejs dyskusji.
- Dodawanie komentarzy ma rate limiting: po przekroczeniu limitu aplikacja zwraca HTTP `429 Too Many Requests`.
