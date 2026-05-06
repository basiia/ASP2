# Checklista bezpieczeństwa LAB 09

## Checklista

- Pola krytyczne zgłoszenia, czyli Title oraz Description, posiadają walidację po stronie serwera.
- Pola Title oraz Description posiadają reguły Required oraz ograniczenie długości StringLength.
- Formularz MVC wysyłający dane metodą POST posiada ochronę tokenem anty-CSRF.
- Żądanie formularza MVC bez poprawnego tokenu anty-CSRF nie przechodzi jako poprawna operacja.
- Aplikacja dodaje nagłówek bezpieczeństwa X-Content-Type-Options: nosniff.
- Aplikacja dodaje nagłówek bezpieczeństwa X-Frame-Options: DENY.
- Błędne dane wejściowe wysłane do endpointu API są odrzucane kodem 400 Bad Request.
- Scenariusz błędnego wejścia został sprawdzony ręcznie dla formularza MVC i endpointu API.

## Ryzyko formularzowe

Ryzykiem formularzowym jest możliwość wysłania żądania POST z obcej strony bez świadomej akcji użytkownika. Takie ryzyko ogranicza token anty-CSRF, ponieważ żądanie bez poprawnego tokenu nie powinno zostać potraktowane jako poprawna operacja.
