# LAB 7 - AMBITNE  

## Podział testów: unit vs integration

W projekcie UniDesk testy jednostkowe (unit tests) powinny obejmować logikę biznesową zawartą w warstwie serwisów, np. w klasie `DbTicketService`.  
Do takich testów należy sprawdzanie:

- filtrowania i sortowania ticketów,
- walidacji danych,
- logiki zmiany statusu zgłoszenia,
- działania metod bez zależności od bazy danych.

Testy jednostkowe są szybkie, izolowane i pozwalają łatwo wykryć błędy w logice aplikacji.

Testy integracyjne natomiast powinny sprawdzać działanie całego systemu, w tym:

- kontrolerów (`TicketsApiController`),
- routingu HTTP,
- serializacji i deserializacji danych,
- komunikacji między warstwami aplikacji.

W projekcie UniDesk testy integracyjne obejmują endpointy API, takie jak:

- `GET /api/tickets`,
- `POST /api/tickets`,
- `PATCH /api/tickets/{id}/status`.

Ich celem jest weryfikacja poprawności kontraktu HTTP, czyli kodów odpowiedzi oraz struktury danych.

## Scenariusz, którego nie warto testować integracyjnie

Nie warto testować integracyjnie prostych operacji, takich jak mapowanie obiektu `Ticket` na `TicketReadDto`.

Tego typu logika:
- jest bardzo prosta,
- nie zależy od infrastruktury,
- może być szybko sprawdzona w testach jednostkowych.

Testy integracyjne w takim przypadku byłyby wolniejsze i trudniejsze w utrzymaniu, nie przynosząc dodatkowej wartości.
