# LAB 8 - AMBITNE

## Co framework daje „za darmo”
W projekcie UniDesk framework ASP.NET Core automatycznie generuje dokumentację Swagger/OpenAPI na podstawie kontrolerów, tras, modeli DTO oraz metadata odpowiedzi. Dzięki temu po dodaniu Swaggera i atrybutów `ProducesResponseType(...)` można od razu zobaczyć endpointy `GET /api/tickets`, `POST /api/tickets`, `GET /api/tickets/{id}` oraz `PATCH /api/tickets/{id}/status` w czytelnym interfejsie Swagger UI.

## Co nadal wymaga świadomej decyzji programisty
Framework nie decyduje sam, jakie odpowiedzi HTTP powinny być częścią kontraktu API. W UniDesk to programista musiał świadomie określić, że utworzenie zgłoszenia powinno zwracać `201 Created`, błąd walidacji `400 Bad Request` jako `ValidationProblemDetails`, a brak zasobu `404 Not Found` jako `ProblemDetails`. Bez tych decyzji Swagger pokazywałby kontrakt w sposób mniej precyzyjny.

## Gdzie automatyzacja pomaga, ale może też usypiać czujność
Automatyzacja pomaga, ponieważ Swagger szybko pokazuje aktualny kontrakt API i ułatwia ręczne testowanie endpointów. Jednocześnie może usypiać czujność inżyniera, jeśli programista uzna, że samo pojawienie się endpointu w dokumentacji oznacza już poprawny kontrakt. W praktyce nadal trzeba sprawdzić, czy kody odpowiedzi, modele błędów i semantyka endpointów rzeczywiście odpowiadają temu, co system powinien wystawiać.