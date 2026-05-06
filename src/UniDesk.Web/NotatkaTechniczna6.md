# Granice bezpieczeństwa kursu LAB 09 AMBITNE


## Co zostało wdrożone jako core

W ramach LAB 09 wdrożono podstawowe mechanizmy bezpieczeństwa wejścia i odpowiedzi HTTP:

- walidację danych wejściowych po stronie serwera,
- ochronę formularzy MVC tokenem anty-CSRF,
- podstawowe nagłówki bezpieczeństwa HTTP,
- weryfikację błędnych danych wejściowych dla API i MVC.

## Czego kurs świadomie nie wdraża jeszcze jako core

W tym etapie kurs nie wdraża jeszcze pełnego systemu uwierzytelniania i autoryzacji end-to-end. Oznacza to, że aplikacja nie posiada jeszcze kompletnego mechanizmu logowania użytkowników, ról, uprawnień oraz kontroli dostępu do konkretnych operacji.

Przykładowo, w prawdziwym systemie należałoby określić, kto może tworzyć zgłoszenia, kto może je edytować, kto może zmieniać ich status oraz kto może przeglądać wszystkie zgłoszenia.

## Element wymagający dalszego hardeningu w systemie produkcyjnym

W prawdziwym systemie produkcyjnym dalszego hardeningu wymagałaby przede wszystkim kontrola dostępu. Sama walidacja, CSRF i nagłówki bezpieczeństwa nie wystarczają, jeśli każdy użytkownik może wykonać każdą operację.

System produkcyjny powinien posiadać:

- logowanie użytkowników,
- role i uprawnienia,
- ograniczenie dostępu do wybranych endpointów,
- ochronę operacji administracyjnych,
- audyt zmian wykonanych przez użytkowników.

