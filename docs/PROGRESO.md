# Progreso del equipo: punto de retorno para una sesión nueva

> Última actualización: 2026-10-31. Si eres una sesión nueva de Claude, lee este archivo y después `CLAUDE.md`, `docs/BITACORA.md` y `docs/guias/despliegue-azure.md`.

## Datos del equipo

- **TeamId: 35.** Resource Group `rg-team-35`, App Service Plan `plan-back-18`, suscripción `Azure subscription 1`.
- La organización confirmó que **se permite usar IA** (esto reemplaza la nota de W6 en `CLAUDE.md`). Aun así, el uso de la extensión de Chrome debe ser discreto, y **las respuestas del CTF las envía el usuario**, no Claude.
- CTF: https://ctf.panama.hackathonlabs.net. Cada pregunta tiene **un solo intento**.
- Dynatrace del reto: https://daa00609.apps.dynatrace.com.
- Rama de trabajo: `reto-azure`. `main` todavía no tiene estos cambios.

## Reto Microsoft / Azure

| Error | Estado | Dónde |
|---|---|---|
| E1 `apiBaseUrl` apunta a `api.copa-passenger.example.com` | 🟡 falta la `backend_url` real | `app/frontend/src/environments/environment.prod.ts` |
| E2 CORS solo permite `http://localhost:4300` | 🟡 falta la `frontend_url` real | `app/backend/CopaPassenger.API/appsettings.Production.json` |
| E3 clave `RESERVATIONS_ENDPOINT_URLX` | ✅ commit `c708207` | `environment*.ts` |
| E4 `HealthCheck.Mode = basic` | ✅ commit `66fce26` | `appsettings.Production.json` |
| E5 `EnableFinalValidation = false` | ✅ commit `1f11007` | `appsettings.Production.json` |
| E6 falta `POST /api/flights/create` | ✅ commit `5e82400` (+ 3 tests) | `CreateFlightCommand`, validador, repositorio, `FlightsController` |

Verificado en local:

- `dotnet test`: 11/11 pasan.
- El API corre con `ASPNETCORE_ENVIRONMENT=Production`.
- `/health/deep` responde 200 `Healthy` y `/api/status` responde 200.
- `create` responde 200; un duplicado o un dato inválido responden 400.
- `npm run build` compila.

**Siguiente paso:**

1. Conseguir `frontend_url` y `backend_url` del equipo 35, con `terraform output` en Cloud Shell o desde el portal en App Services.
2. Corregir E1 y E2 con un commit por error.
3. Recompilar el frontend y desplegar con `az webapp deploy`. Los pasos están en `docs/guias/despliegue-azure.md`.
4. Validar con curl y KQL.

Claude no puede iniciar sesión en Azure. El portal lo usa el integrante desde su ventana de incógnito, que la extensión de Chrome no alcanza.

Preguntas del CTF de Microsoft que siguen abiertas porque necesitan datos del portal o de App Insights: 1301, 1303, 1308, 1503, 1206, 1602, 1603, 1604 y 1605.

## Reto Dynatrace: cómo se consultaron los datos

Se usó DQL de solo lectura sobre Grail, desde la sesión del usuario en Chrome. Las llamadas fueron `POST /platform/storage/query/v1/query:execute` y luego `query:poll`, con la cookie de sesión.

Fuentes de datos encontradas:

| Dato | Filtro DQL | Notas |
|---|---|---|
| Netflow | `fetch logs \| filter log.source == "netflow"` | ~5959 registros |
| Nginx | `filter log.type == "nginx"` | ~1000 registros |
| Apache | `filter log.type == "apache"` | ~1388 registros, **con más de 30 días**: hay que ampliar el timeframe (por ejemplo `from: now()-400d`) |
| Order service | `filter log.type == "order"` | |

- Lookups (`dt.system.files`):
  - `/lookups/apache-ip-geo-locations`
  - `/lookups/netflow-ip-enriched-table`
  - `/lookups/netflow-tls-cipher-suites-table`
  - `/lookups/order-service-user-lookup`
  - `/lookups/order-service-error-codes`
- Notebooks plantilla:
  - "COPA- DQL Use Case - Networks Logs"
  - "Nginx Access Logs"
  - "PaymentService Logs"
  - "Network Logs"
- Kubernetes: `smartscapeNodes` (nodos, deployments, nodeInfo) y `fetch dt.davis.problems`.

Trampas de DQL que ya resolvimos:

- Un `lookup` entre IPADDR y string da `INCOMPATIBLE_DATA_TYPES`. Se resuelve con `fieldsAdd ips = toString(ip)`.
- Al ordenar promedios, los nulos salen primero. Hay que filtrar con `isNotNull(...)`.
- Los campos con `:` (`tags:k8s.labels`) dan `PARSE_ERROR`. Hay que procesarlos fuera de DQL.
- El timeframe por defecto es de 30 días y no alcanza para los logs de Apache.

Preguntas de Dynatrace todavía sin respuesta segura: 2107 y 2207. Ya se gastaron las de un solo intento 2101, 2102, 2105, 2204 y 2205.

## Respuestas del CTF

**No están en el repo** porque el repo es público. Están solo en la laptop, en `docs/CTF-RESPUESTAS.local.md`, que git ignora. Si una sesión nueva corre en esta misma carpeta, puede leerlo.

## Pendientes

- [ ] E1 y E2 con las URLs reales del equipo 35, y redeploy.
- [ ] Preguntas abiertas de Microsoft (ver arriba).
- [ ] Confirmar 2107 y 2207 de Dynatrace.
- [ ] El usuario anota en `docs/CTF-RESPUESTAS.local.md` qué respuestas resultaron correctas o incorrectas.
- [ ] Abrir un PR de `reto-azure` a `main` cuando el equipo lo decida.
