# Notatka techniczna lab6: Problemy z testowalnością

## Problemy z testowalnością
### 1. Zależność od czasu
Wiele klas w projekcie używa `DateTime.Now` do ustawiania bieżącej daty i godziny. To utrudnia testowanie, ponieważ testy mogą nie być deterministyczne (np. niektóre testy mogą działać w różnych momentach czasu).

Rozwiązanie:
- Wprowadzenie abstrakcji czasu za pomocą interfejsu `ISystemClock` rozwiązuje ten problem, pozwalając na kontrolowanie czasu w testach i eliminując błędy związane z różnicami czasowymi.

### 2. Zależność od zewnętrznych usług
Komponenty takie jak serwisy komunikujące się z bazą danych lub zewnętrznymi API utrudniają testowanie. Na przykład, serwisy, które komunikują się z rzeczywistą bazą danych, mogą wymagać istnienia bazy testowej, co powoduje, że testy są zależne od zewnętrznego środowiska.

Rozwiązanie:
- Zastosowanie moków (np. `Moq`) do symulowania pracy z bazą danych i zewnętrznymi usługami pomaga rozwiązać ten problem. Można także wykorzystać bazy danych w pamięci (in-memory) do testowania.

### 3. Złożone zależności
Niektóre klasy w projekcie mają skomplikowane zależności, co utrudnia ich testowanie. Na przykład serwisy z wieloma repozytoriami, które muszą być zamockowane, aby umożliwić testowanie.

Rozwiązanie:
- Wykorzystanie wstrzykiwania zależności (Dependency Injection) i moków do izolacji zależności ułatwia testowanie tych komponentów.

## Podsumowanie
Główne problemy z testowalnością w projekcie dotyczą zależności od czasu i zewnętrznych usług. Wprowadzenie abstrakcji czasu i zastosowanie moków dla usług zewnętrznych rozwiązuje te problemy, poprawiając testowalność systemu.

