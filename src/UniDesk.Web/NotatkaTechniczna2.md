# LAB 5 Notatka techniczna (AMBITNE)

## Zagrożenia przyjmowania surowego string jako pola sortowania

To może prowadzić do poważnych problemów związanych z bezpieczeństwem i stabilnością systemu.

### 1.Błędy zapytań (stabilność)

Jeśli użytkownik poda nieistniejące pole, np.: sortBy=ABC, zapytanie może zakończyć się błędem lub wyjątkiem, co może spowodować awarię aplikacji.

### 2.Ujawnienie struktury bazy danych

Użytkownik może próbować zgadywać nazwy pól, co może ujawnić strukturę tabeli (np. kolumny ukryte lub techniczne).

### 3.Niebezpieczne operacje dynamiczne

Dynamiczne sortowanie bez kontroli może prowadzić do nieprzewidywalnego zachowania zapytań oraz trudnych do debugowania błędów.

### 4.Potencjalne ryzyko ataków

Choć EF Core częściowo zabezpiecza przed SQL Injection, niekontrolowane dynamiczne operacje nadal mogą być wektorem ataku.

## Rozwiązanie

Zastosowano whitelistę dozwolonych pól sortowania:
- Title
- Status
- CreatedAt

Jeśli użytkownik poda inne pole, system ignoruje je i stosuje domyślne sortowanie po Id.

## Wniosek

Bezpieczniejsze jest ograniczenie możliwości systemu do zaufanych pól niż pozwolenie na pełną dowolność użytkownika.