# Bitácora del incidente

> "Lo que no se escribe, se repite." Basada en la Charla 2 (Operación Bajo Presión).
> Una fila por hipótesis. No borrar las hipótesis descartadas: también son progreso.

**Reto:** Microsoft / Azure: https://github.com/HackathonLabsNetworks/2026-Panama-Microsoft (IaC: https://github.com/HackathonLabsNetworks/2026-Panama-Microsoft-IaC)
**TeamId:** _(pendiente)_
**URLs del equipo** (copiar de `terraform output`): frontend `https://...` · backend `https://...`
**Recursos Azure:** Resource Group `rg-team-<N>` · App Insights `hackathon-copa-panama-2026-<N>-appi` · Apps `hackathon-copa-panama-2026-<N>-frontend` / `-api`

Código: `app/backend` (.NET 10) y `app/frontend` (React + Vite), rama `reto-azure`. Pasos de despliegue en [guias/despliegue-azure.md](guias/despliegue-azure.md).

## Estado de validación

| Endpoint | Esperado | Estado | Última verificación |
|---|---|---|---|
| `/health` | 200 | ⬜ | |
| `/health/deep` | 200 / `Healthy` (E4) | ⬜ | |
| `/api/status` | 200 `success: true` (E5) | ⬜ | |
| `POST /api/flights/create` | 200 (E6) | ⬜ | |
| UI funcional | datos visibles, Console sin errores (E1, E2, E3) | ⬜ | |

## Registro

| Hora | Quién | Síntoma | Evidencia | Hipótesis | Cambio (archivo / commit) | Resultado |
|---|---|---|---|---|---|---|
| | Claude | E1: el front llama a una URL ajena (connection refused / DNS) | `environment.prod.ts`: `apiBaseUrl: 'https://api.copa-passenger.example.com'` | La URL del API se incrusta al compilar y apunta a un dominio de ejemplo | `app/frontend/src/environments/environment.prod.ts` → `backend_url` real y recompilar | 🟡 falta la URL |
| | Claude | E2: `blocked by CORS policy`, preflight `OPTIONS` rechazado | `appsettings.Production.json`: `Cors:AllowedOrigins = ["http://localhost:4300"]` | El origen del front en Azure no está permitido | `appsettings.Production.json` → `frontend_url` exacta (sin `*`) | 🟡 falta la URL |
| | Claude | E3: `/api/reservations` "servicio interno no inicializado" (BadRequest) | `api.ts` busca `RESERVATIONS_ENDPOINT_URL`; los `environment*.ts` definen `RESERVATIONS_ENDPOINT_URLX` | Nombre de clave mal escrito | commit `c708207` | ✅ en código |
| | Claude | E4: `/health/deep` 503 `Degraded`, errorCode E4 | `DeepHealthCheck.cs` exige `HealthCheck:Mode = "deep"`; Production tenía `"basic"` | Configuración del modo de health check | commit `66fce26` | ✅ en código |
| | Claude | E5: `/api/status` 503 "Error E5" | `StatusController.cs` exige `FeatureFlags:EnableFinalValidation = true`; Production tenía `false` | Feature flag apagado | commit `1f11007` | ✅ en código |
| | Claude | E6: `POST /api/flights/create` 404/405 | `FlightsController.cs` solo tenía GET | Endpoint no implementado | `CreateFlightCommand` + validador + `FlightRepository.AddAsync` + acción `[HttpPost("create")]` | ✅ en código |

Resultado: ✅ Resuelto · ❌ Refutada · 🟡 En análisis

### Observaciones adicionales

- `main.tf` define `SCO_DO_BUILD_DURING_DEPLOYMENT` en lugar de `SCM_DO_BUILD_DURING_DEPLOYMENT`, así que la compilación de Oryx en App Service queda desactivada. No afecta porque desplegamos artefactos ya compilados (`dotnet publish` / `npm run build`). El reto pide no tocar el IaC.
- Los validadores de FluentValidation están registrados pero no hay un pipeline de MediatR que los ejecute. `CreateFlightCommandHandler` valida explícitamente y `ExceptionMiddleware` convierte el error en HTTP 400.
- El frontend lee la cadena de App Insights en tiempo de ejecución desde `/config.json` (`server.js`); el backend la lee de `APPLICATIONINSIGHTS_CONNECTION_STRING`. Ambas ya vienen configuradas por Terraform.

## Respuestas para CTFd

| Pregunta / flag | Respuesta | Evidencia (archivo, línea, query) |
|---|---|---|
| | | |
