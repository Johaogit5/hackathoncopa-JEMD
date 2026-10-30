# Hackathon COPA 2026: documentación del equipo JEMD

Material de preparación entregado por los organizadores, convertido a Markdown para consultarlo rápido durante el reto.

**Equipo:** 4 integrantes.
**Tracks:** Microsoft (Azure Monitor + Application Insights) y Dynatrace (observabilidad).

## Contexto (material oficial)

| # | Documento | De qué trata | Cuándo consultarlo |
|---|---|---|---|
| 01 | [Cápsulas informativas (Microsoft)](contexto/01-capsulas-informativas-microsoft.md) | 17 conceptos base: Azure, App Service, APIs, IaC/Terraform, logs, KQL, CORS, health checks, variables | Para repasar un concepto |
| 02 | [Charla 2: Operación Bajo Presión](contexto/02-charla2-operacion-bajo-presion.md) | Metodología de troubleshooting, mapa de síntomas, 5 casos típicos, KQL de bolsillo | **Durante el reto: es el mapa principal** |
| 03 | [Conceptos clave de Dynatrace](contexto/03-dynatrace-conceptos-clave.md) | Observabilidad, DQL, end-to-end, LiveDebugger, AppSec, Business Events | Para repasar Dynatrace |
| 04 | [Dynatrace Sesión 1](contexto/04-dynatrace-sesion1-observabilidad-inteligente.md) | Pilares, Kubernetes, servicios, PurePath, DQL con ejemplos, equivalencias KQL↔DQL | Para escribir consultas DQL |

Herramienta de trabajo: **[BITACORA.md](BITACORA.md)**, la bitácora del incidente que llenaremos durante el reto.

## Lo esencial en 30 segundos

1. **Framework de 6 pasos:** Detectar → Observar → Analizar → Hipótesis → Corregir → Validar. Si la validación falla, volver a **Observar**.
2. **Un cambio a la vez**, siempre con una hipótesis respaldada por evidencia.
3. **Mapa de síntomas** (ver doc 02):

   | Síntoma | Sospechoso | Dónde mirar |
   |---|---|---|
   | Endpoint da 500 | Excepción en backend | App Insights → Failures / `exceptions` |
   | App arranca pero un servicio no inicia | Variable de entorno mal nombrada o faltante | `exceptions` + App settings + `main.tf` |
   | Frontend carga pero sin datos, backend sin requests | URL del API mal configurada (se incrusta al compilar) | DevTools → Network |
   | 404 / 405 / 415 / 200 vacío | Contrato frontend ↔ backend roto | Network vs. OpenAPI |
   | Postman sí, navegador no | CORS | DevTools → Console + requests `OPTIONS` |

4. **Los errores vienen en cascada:** `/health` → `/health/deep` → `/api/status`. Arreglar uno puede destapar el siguiente.
5. **La telemetría tarda 1–3 min** en aparecer en Logs; para validar un redeploy usar Live Metrics / Log stream.
6. **Nunca** "arreglar" CORS con `"*"`, ni redesplegar sin saber qué cambió.

## Reparto sugerido para 4 integrantes

La charla 2 lo advierte: *"que los 4 del equipo investiguen lo mismo"* es un error. Propuesta inicial (ajustar cuando conozcamos el reto):

| Rol | Responsable | Se enfoca en |
|---|---|---|
| **Coordinador / bitácora** | _(nombre)_ | Lleva la [BITACORA.md](BITACORA.md), reparte hipótesis, valida (`/health`, `/health/deep`, `/api/status`) y prepara las respuestas para CTFd |
| **Backend / App Insights** | _(nombre)_ | Failures, `exceptions`, `traces`, stack traces, código del API |
| **Infra / IaC** | _(nombre)_ | `main.tf`, App settings, variables de entorno, CORS de plataforma, Activity Log, redeploys |
| **Frontend / Red** | _(nombre)_ | DevTools (Console + Network), URL del API, contrato OpenAPI, rebuild del frontend |

Los problemas independientes se atacan en paralelo; los que dependen de otro se resuelven en orden.

## Cómo usar esto con Claude

Cuando llegue el enunciado del reto, pásenselo a Claude junto con el repositorio. Las instrucciones de [`../CLAUDE.md`](../CLAUDE.md) le indican leer esta carpeta primero y trabajar con la metodología de la charla 2.
