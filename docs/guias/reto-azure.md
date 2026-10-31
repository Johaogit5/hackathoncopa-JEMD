# Reto Azure: guía rápida (App Service, Terraform, App Insights, KQL y troubleshooting)

Fuentes: [W2](../workshops/w2-aplicaciones-modernas.md), [W3](../workshops/w3-devops-git-terraform.md), [W4](../workshops/w4-observabilidad-kql.md), [W5](../workshops/w5-troubleshooting.md), [Cápsulas](../contexto/01-capsulas-informativas-microsoft.md) y [Charla 2](../contexto/02-charla2-operacion-bajo-presion.md). El W1 (fundamentos de Azure) no tiene transcripción todavía.

## 1. Método de diagnóstico

W5 y la Charla 2 describen el mismo método con nombres distintos:

| W5 (5 pasos) | Charla 2 (6 pasos) | Qué hacer |
|---|---|---|
| Síntoma | Detectar | Qué error, cuándo empezó, a quién afecta y en qué capa (frontend, API o backend) |
| Evidencia | Observar | Logs, consola, código HTTP, DevTools → Network |
| — | Analizar | Correlacionar (`operation_Id`) y separar la causa del síntoma |
| Hipótesis | Hipótesis | Una causa concreta que se pueda comprobar |
| Validación | Validar | Si la hipótesis se refuta, **volver a Evidencia**. No improvisar |
| Corrección | Corregir | Un cambio a la vez, redeploy y volver a probar |

Los errores vienen en cascada: `/health` → `/health/deep` → `/api/status`. Arreglar uno puede destapar el siguiente. La telemetría tarda de 1 a 3 minutos en aparecer en Logs; para ver un redeploy en el momento, usar **Live Metrics**.

## 2. Mapa de síntomas

| Síntoma | Sospechoso | Dónde mirar |
|---|---|---|
| Endpoint da 500 | Excepción no controlada | App Insights → Failures / `exceptions` |
| Servicio no inicia | Variable de entorno faltante o mal nombrada | `exceptions` + App Settings + `main.tf` |
| Frontend sin datos y backend sin requests | URL del API mal configurada (**se incrusta al compilar**) | DevTools → Network |
| 404 / 405 / 415 / 200 vacío | Contrato frontend ↔ backend roto | Network vs. Swagger/OpenAPI |
| Funciona en Postman y falla en el navegador | **CORS** | DevTools → Console + requests `OPTIONS` |
| 401 | Falta token o credencial | Headers de la petición |
| 403 | Autenticado pero sin permiso | Roles |
| 502 / 503 | Servicio intermedio caído o saturado | `dependencies`, health checks |

## 3. KQL (Application Insights / Log Analytics)

Tablas: `requests` (HTTP entrantes), `exceptions` (errores), `dependencies` (llamadas salientes a BD/APIs) y `traces` (logs de la app). **`operation_Id`** une las cuatro.

```kql
// Fallos de la última hora cada 5 min (W4)
requests
| where timestamp > ago(1h)
| where success == false
| summarize Fallos = count() by bin(timestamp, 5m), resultCode
| order by timestamp desc
```

```kql
// Top 10 excepciones de las últimas 24 h (W4)
exceptions
| where timestamp > ago(24h)
| summarize Total = count() by type, outerMessage
| order by Total desc
| take 10
```

```kql
// ¿Qué está fallando? (Charla 2)
requests
| where timestamp > ago(30m) and success == false
| summarize total = count() by name, resultCode
| order by total desc
```

```kql
// ¿Qué dijo la app? (Charla 2)
traces
| where timestamp > ago(30m) and severityLevel >= 2
| project timestamp, severityLevel, message
| order by timestamp desc
```

```kql
// ¿Falla o tarda algo externo? (Charla 2)
dependencies
| where timestamp > ago(30m)
| summarize fallos = countif(success == false),
            p95 = percentile(duration, 95) by target
```

```kql
// Todo lo que pasó en una operación (Charla 2)
union requests, dependencies, exceptions, traces
| where timestamp > ago(30m)
| where operation_Id == "4bf92f35..."
| project timestamp, itemType, name, resultCode, type, message
| order by timestamp asc
```

Herramientas del portal: **Live Metrics**, **Metrics** (revisar si la hora está en Local o en UTC), **Application Map** (rojo = errores), **Failures** y **Logs**.

## 4. Configuración, CORS y health checks

- **Variables:** viven en App Settings, en el pipeline CI/CD o en archivos (`appsettings.json`/`.env`). Si quedaron **quemadas en el build**, cambiar App Settings no sirve: hay que recompilar.
- **Key Vault:** en App Settings va `@Microsoft.KeyVault(SecretUri=https://mi-vault.vault.azure.net/secrets/MiSecreto/version)`.
- **CORS:** origen = protocolo + dominio + puerto. El navegador primero envía `OPTIONS` (preflight) y el backend responde `Access-Control-Allow-Origin`. **Nunca usar `*`**: agregar el origen exacto del frontend.
- **`/health`** (liveness): ¿el proceso vive? Si falla, se reinicia. **`/health/deep`** (readiness): ¿responden BD y APIs? Si falla, la instancia sale del balanceador. Estados: Healthy, Degraded y Unhealthy.
- **Kudu:** `https://<app>.scm.azurewebsites.net` → `/home/site/wwwroot` para ver qué se desplegó realmente (por ejemplo, un `appsettings.json` congelado).
- **DevTools:** Network con **Preserve log**, Console para CORS y exportar `.har`.

## 5. App Service

- **App Service Plan** = el cómputo; **Web App** = la app. Un plan aloja varias apps.
- **Static Web Apps** para frontend; **Web App** para API/backend. El nombre define `<nombre>.azurewebsites.net`.
- SKUs: Free/Shared (sin escalado) · Basic B1–B3 (solo vertical) · **Standard S1–S3** (autoescalado + deployment slots) · Premium/Isolated.
- Formas de desplegar: Deployment Center (GitHub Actions), Publish desde el IDE, Publish Profile, deployment slots + swap.
- Error `QuotaExceeded` al crear: cambiar de región (en W5 se usó East US).

## 6. Terraform y Azure CLI

```bash
az login
az account show --output table
az account set --subscription "<ID-o-Nombre>"

terraform init       # descarga providers y crea .terraform.lock.hcl
terraform fmt        # formatea
terraform validate   # solo sintaxis: no detecta cuotas, políticas ni nombres duplicados
terraform plan       # + crear  ~ modificar  - destruir  → LEER SIEMPRE
terraform apply      # pide escribir "yes"
terraform output     # URLs y nombres creados
terraform show       # estado detallado
terraform destroy
```

- `.tf` = lo deseado · `.tfstate` = lo que Terraform sabe · Azure = la realidad. Corregir **en el `.tf`**, no a mano en el portal.
- `.gitignore`: `.terraform/`, `*.tfstate`, `*.tfstate.backup`, `crash.log` y `*.tfvars` con secretos.
- Los providers `Microsoft.Web` y `Microsoft.Storage` deben estar en estado *Registered*.
