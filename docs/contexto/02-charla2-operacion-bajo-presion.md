# Charla 2 — Operación Bajo Presión (Track Microsoft)

> Fuente: `Charla 2 - Operacion Bajo Presion - Microsoft.pdf` (Business IT · Partner Microsoft · Azure Expert MSP). Duración: 60 minutos.
> **Tema:** observabilidad y troubleshooting en aplicaciones cloud. Cómo diagnosticar, analizar y resolver incidentes en producción con **Azure Monitor** y **Application Insights**.
>
> ⚠️ **Es el material más cercano a lo que pedirá el reto.** Describe el tipo de fallas que tendremos que encontrar y corregir.
>
> Nota: el PDF no incluye las diapositivas 17 (Caso 2), 22 y 24. El Caso 2 (variable de entorno) se reconstruyó con las otras diapositivas que lo mencionan.

---

## Escenario: "Son las 10:07. El reloj ya está corriendo"

| Hora | Qué pasa |
|---|---|
| 10:00 | `terraform apply` termina sin errores. Todo "verde". |
| 10:03 | El frontend carga, pero la lista de productos aparece vacía. |
| 10:05 | `curl -i https://.../health` → `HTTP/1.1 500 Internal Server Error` |
| 10:07 | Cuatro personas, cuatro teorías distintas… y ninguna evidencia. |

**Objetivo:** pasar de "¿qué hacemos?" a "encontramos la causa raíz" con una metodología basada en evidencia.

## Agenda

| # | Bloque | Contenido | Tiempo |
|---|---|---|---|
| 01 | La aplicación falla… ¿por dónde empiezo? | Mentalidad SRE y diagnóstico por evidencia | 10 min |
| 02 | Observabilidad moderna | Logs, métricas, trazas, correlación, Azure Monitor | 15 min |
| 03 | Anatomía de una incidencia real | 500, variables, URL, integración, CORS | 15 min |
| 04 | Framework de Troubleshooting | Detectar → Observar → … → Validar | 15 min |

---

## Bloque 1 — Mentalidad SRE (Site Reliability Engineering)

### Pensar como un SRE

- **Evidencia > intuición.** Toda hipótesis se apoya en un dato observable: un log, un código HTTP, una traza.
- **El síntoma no es la causa.** "No carga la lista" es lo que se ve; la causa está más abajo.
- **Un cambio a la vez.** Si cambias tres cosas y funciona, no sabes cuál lo arregló. Si empeora, tampoco.
- **Restaurar primero, perfeccionar después.** El objetivo es reducir el tiempo de recuperación (**MTTR**), no escribir el código ideal.
- **Cultura blameless:** se analiza el sistema, no se culpa a las personas.

### Qué hacer y qué NO hacer cuando algo falla

| ✅ Hacer | ❌ NO hacer |
|---|---|
| Reproducir el error y anotar la hora exacta | Cambiar código "a ver si funciona" |
| Leer el mensaje de error completo, hasta el final | Redesplegar sin saber qué cambiaste |
| Revisar Application Insights **antes** de tocar código | Desactivar seguridad (CORS `"*"`) para salir del paso |
| Comparar lo que la app espera vs. lo que recibe | Asumir que "en mi máquina funciona" aplica en la nube |
| Registrar cada hipótesis y su resultado | **Que los 4 del equipo investiguen lo mismo** |
| Validar con `/health` y la funcionalidad real | Declarar victoria sin validar el resultado |

### Dónde está la evidencia, capa por capa

| Capa | Herramienta / dónde mirar |
|---|---|
| **Navegador** | DevTools → Console y Network: URL llamada, código HTTP, errores CORS |
| **HTTP** | `curl -i` / Postman: código, headers y cuerpo de la respuesta |
| **Aplicación** | Application Insights: requests, exceptions, traces, dependencies |
| **Plataforma** | App Service: Log stream, App settings, CORS, *Diagnose and solve problems* |
| **Código e IaC** | Repositorio: archivos de configuración, `main.tf`, `git diff` |

**Regla práctica:** empezar por donde está el síntoma y bajar capa por capa hasta el **primer punto donde la realidad no coincide con lo esperado**.

> **La ausencia de evidencia también es evidencia:** si el backend no registra ningún request, el problema ocurre antes de llegar a él.

---

## Bloque 2 — Observabilidad moderna

### Tres señales, tres preguntas

| Señal | Pregunta | Qué es | Tablas en App Insights (KQL) |
|---|---|---|---|
| **Métricas** | ¿**QUÉ** está mal? | Valores numéricos en el tiempo: tasa de error, latencia, req/seg, CPU, memoria | `requests`, `performanceCounters`, `customMetrics` |
| **Trazas** | ¿**DÓNDE** está el problema? | Recorrido de una operación entre frontend, backend y dependencias, con tiempos por salto | `requests`, `dependencies`, `operation_Id` |
| **Logs** | ¿**POR QUÉ** ocurrió? | Eventos discretos con contexto: mensajes estructurados, excepciones, stack traces | `traces`, `exceptions` |

### `operation_Id`: el hilo que une todo

Una sola operación deja rastro en cuatro tablas, todas con el mismo `operation_Id`:

| Tabla | Ejemplo |
|---|---|
| `requests` | `GET /api/products` → 500 · 212 ms |
| `dependencies` | HTTP GET catálogo interno → falló · 180 ms |
| `exceptions` | `InvalidOperationException` — "Setting no encontrado" |
| `traces` | "Error al inicializar servicio" · severityLevel 3 |

```kql
// Todo lo que pasó en una operación
union requests, dependencies, exceptions, traces
| where timestamp > ago(30m)
| where operation_Id == "4bf92f35..."
| project timestamp, itemType, name, resultCode, type, message
| order by timestamp asc
```

En el portal: **Failures → seleccionar request → "End-to-end transaction details"** muestra esta misma cadena de forma visual.

### Azure Monitor y Application Insights

**Azure Monitor** (plataforma unificada de monitoreo):
- **Métricas de plataforma:** CPU, memoria, HTTP 5xx del App Service.
- **Activity Log:** quién cambió qué recurso y cuándo.
- **Log Analytics:** almacén consultable con KQL.
- **Alertas y Dashboards:** notificar y visualizar.
- **Application Insights (APM):** telemetría de la aplicación (requests, dependencias, excepciones, trazas, Application Map, Live Metrics).

**¿Cuál uso?**

| Pregunta | Herramienta |
|---|---|
| ¿Por qué falla este endpoint? | App Insights · **Failures** |
| ¿Qué excepción y en qué línea? | App Insights · **Exceptions** |
| ¿Está saturado el servidor? | **Métricas** de App Service |
| ¿Quién cambió la config? | **Activity Log** |
| ¿Qué pasa ahora mismo? | **Live Metrics** / **Log stream** |

### Caja de herramientas (lo que tendremos abierto durante el reto)

- **Failures:** requests fallidos agrupados por código y operación. Punto de partida para un 500.
- **Transaction search:** buscar un request puntual y abrir su detalle end-to-end.
- **Live Metrics:** telemetría segundo a segundo. Ideal para validar un redeploy.
- **Logs (KQL):** consultas libres sobre requests, exceptions, traces y dependencies.
- **Application Map:** mapa de componentes y llamadas fallidas entre ellos.
- **Azure Insights:** vista consolidada del equipo: errores, latencia, disponibilidad.

> ⏱️ **La telemetría puede tardar 1–3 minutos en aparecer en Logs.** Live Metrics y Log stream son casi inmediatos.

### KQL de bolsillo: 4 consultas que resuelven el 80% de los casos

```kql
// 1. ¿Qué está fallando?
requests
| where timestamp > ago(30m) and success == false
| summarize total = count() by name, resultCode
| order by total desc
```

```kql
// 2. ¿Qué excepción y con qué mensaje?
exceptions
| where timestamp > ago(30m)
| summarize total = count() by type, outerMessage
| order by total desc
```

```kql
// 3. ¿Qué dijo la app? (logs estructurados)
traces
| where timestamp > ago(30m) and severityLevel >= 2
| project timestamp, severityLevel, message
| order by timestamp desc
```

```kql
// 4. ¿Falla o tarda algo externo?
dependencies
| where timestamp > ago(30m)
| summarize fallos = countif(success == false),
            p95 = percentile(duration, 95) by target
```

---

## Bloque 3 — Anatomía de una incidencia real

### Mapa de síntomas: del síntoma a la primera evidencia

> **"Guarden esta tabla: es su mapa de navegación durante el reto."**

| Caso | Síntoma visible | Primera evidencia | Dónde mirar |
|---|---|---|---|
| **API responde 500** | El endpoint devuelve error genérico | Excepción con tipo, mensaje y stack | App Insights · Failures / exceptions |
| **Variable de entorno** | La app arranca pero un servicio no se inicializa | `"Required setting '...' not found"` | exceptions + App settings + `main.tf` |
| **URL mal configurada** | Frontend carga, datos vacíos | Network: host inexistente / error DNS | DevTools · Network; backend sin requests |
| **Error de integración** | 404, 415 o datos mal mapeados | Ruta o campo JSON distinto al contrato | Network + requests (resultCode) |
| **CORS** | Postman funciona, navegador no | `"blocked by CORS policy"` en Console | DevTools · Console + requests OPTIONS |

### Caso 1 — API responde 500: "un 500 avisa que algo falló, no dice qué"

**Paso 1: reproducir**

```
$ curl -i https://team-47-api.azurewebsites.net/api/products
HTTP/1.1 500 Internal Server Error
Content-Type: application/problem+json
x-ms-request-id: 4bf92f35-...
```

**Paso 2: leer la excepción** (Application Insights → Failures → Exceptions)

```
System.InvalidOperationException:
  Catalog service is not initialized.
    at GetProductsQueryHandler.Handle(...)
       in Application/Products/Queries/...:line 21
    at MediatR.Mediator.Send(...)
    at ProductsController.Get(...)
```

**Cómo leer un stack trace**
1. **Tipo:** qué clase de error (InvalidOperation, NullReference, Timeout…).
2. **Mensaje:** la pista humana; casi siempre nombra lo que falta.
3. **Primera línea propia:** el primer archivo de *nuestro* código. Ahí empieza la búsqueda.
4. **Contexto:** ¿qué necesita ese código para funcionar? Config, dependencia, dato.

> **El 500 es el síntoma. La excepción es la pista.**

### Caso 2 — Variable de entorno (reconstruido)

- **Síntoma:** la app arranca pero un servicio no se inicializa (y suele terminar en un 500).
- **Evidencia:** excepción del tipo `"Required setting '...' not found"` / "Setting no encontrado".
- **Dónde mirar:** `exceptions` + **App settings** del App Service + **`main.tf`** (Terraform).
- **Causa típica (de la bitácora de ejemplo):** el nombre de la variable en la IaC es distinto al que espera el código → **renombrar la clave en `main.tf`** y volver a desplegar.

### Caso 3 — URL mal configurada: "el frontend llama a un backend que no existe"

```
Frontend: fetch(apiBaseUrl + '/api/products')
   └──► api.ejemplo-generico.net   → ERR_NAME_NOT_RESOLVED (host inexistente)
Backend real: team-47-api…          → sin requests registrados
```

- **Evidencia:** en la pestaña Network la URL solicitada no es la del App Service del equipo. En App Insights del backend: silencio total.
- **Causa típica:** archivo de ambiente de producción con un host genérico o copiado de desarrollo.
- **Trampa:** ⚠️ en apps web **la URL se incrusta al compilar**. Cambiar un App Setting **no basta**: hay que editar, reconstruir (*rebuild*) y redesplegar el frontend.

### Caso 4 — Error de integración: "ambos lados funcionan… pero no se entienden"

| Desajuste | Ejemplo | Evidencia | Código típico |
|---|---|---|---|
| Ruta | `/api/product` vs `/api/products` | Network + requests | **404** |
| Verbo | POST donde se espera GET | requests (name) | **405** |
| Content-Type | Envía texto, espera JSON | Response body | **415 / 400** |
| Campo JSON | `productName` vs `name` | Response vs contrato | **200 con datos vacíos** |
| Protocolo | Frontend HTTPS llama a HTTP | Console: *mixed content* | Bloqueado |

- **El contrato es el juez:** comparar la petición real (Network → Headers / Payload) contra el contrato **OpenAPI**. El primer campo que no coincide es la causa raíz.
- **Un 200 con datos vacíos también es un error.**
- Los **4xx** apuntan al cliente (frontend); los **5xx** al servidor (backend).

### Caso 5 — CORS: "Postman funciona, el navegador no"

1. **Preflight:** el navegador envía `OPTIONS` preguntando si el origen está permitido.
2. **Respuesta:** el backend responde **sin** `Access-Control-Allow-Origin` para ese origen.
3. **Bloqueo:** el navegador descarta la respuesta y muestra el error en Console.

```
Access to fetch at 'https://team-47-api...' from origin
'https://team-47-frontend...' has been blocked by CORS policy
```

> **Origen = protocolo + host + puerto.** Debe coincidir exacto: **sin "/" final, con https**.

**Checklist CORS**
- Se corrige en el **backend**, no en el frontend.
- Permitir el **origen exacto** del frontend del equipo.
- **Nunca `"*"` con credenciales:** no es una solución.
- **No mezclar CORS del App Service (plataforma) con CORS del código:** el de plataforma toma el control.
- Validar en el **navegador**, no en Postman.
- En App Insights: buscar requests `OPTIONS`.

### Errores en cascada: "responde" no significa "está listo"

| Endpoint | Tipo | Pregunta |
|---|---|---|
| `/health` | **Liveness** | ¿El proceso está vivo? |
| `/health/deep` | **Readiness** | ¿Configuración y dependencias listas para producción? |
| `/api/status` | **Validación final** | ¿El negocio declara la release como lista (feature flags)? |

> Arreglar un error puede revelar el siguiente: **los errores de arriba enmascaran a los de abajo.**

**Qué esperar en el reto**
- Algunos problemas son **independientes** y se pueden atacar **en paralelo**.
- Otros solo aparecen cuando los anteriores están resueltos.
- Después de cada fix: **redeploy y volver a observar**.
- Leer los logs también cuando "todo parece bien": **los warnings dan pistas**.

---

## Bloque 4 — Framework de Troubleshooting

### Seis pasos, siempre en el mismo orden

| # | Paso | Qué se hace |
|---|---|---|
| 1 | **Detectar** | ¿Qué falla, desde cuándo y a quién afecta? |
| 2 | **Observar** | Recolectar evidencia **sin cambiar nada todavía**. |
| 3 | **Analizar** | Correlacionar señales y ubicar la capa que falla. |
| 4 | **Hipótesis** | Una causa concreta y verificable, con su evidencia. |
| 5 | **Corregir** | **Un solo cambio**, en el código versionado. |
| 6 | **Validar** | Redeploy y confirmar con datos que se resolvió. |

> 🔁 **Si la validación falla, se vuelve a Observar, no a Corregir.**
> Una hipótesis refutada también es progreso: descarta una causa y acota la búsqueda. **Lo que nunca es progreso es un cambio sin hipótesis.**

### Ejemplo aplicado: un caso de CORS en 6 pasos

1. **Detectar:** la lista de productos está vacía en el navegador.
2. **Observar:** Console dice `"blocked by CORS policy"`; `curl -i` a la API devuelve 200.
3. **Analizar:** el backend responde y el navegador bloquea. La capa es seguridad web.
4. **Hipótesis:** "El origen permitido no coincide con la URL del frontend, porque el error nombra ese origen."
5. **Corregir:** actualizar los orígenes permitidos en la config del backend → commit → redeploy.
6. **Validar:** la UI muestra productos, Console sin errores, `/health` = 200.

### Bitácora del incidente: "lo que no se escribe, se repite"

| Hora | Síntoma | Evidencia | Hipótesis | Cambio | Resultado |
|---|---|---|---|---|---|
| 10:12 | `/api/products` → 500 | Exception: setting no encontrado | Nombre de variable distinto en IaC | Renombrar clave en `main.tf` | ✅ Resuelto |
| 10:31 | Lista vacía en UI | Network: host inexistente | URL de producción incorrecta | Editar env + rebuild | ✅ Resuelto |
| 10:44 | `/health/deep` degradado | traces: chequeo interno no pasa | Configuración incompleta para producción | — | 🟡 En análisis |

**¿Para qué sirve?**
- **Evita repetir:** no se prueba dos veces la misma hipótesis descartada.
- **Coordina al equipo:** todos ven qué se investigó y qué falta.
- **Alimenta CTFd:** síntomas, causas y archivos ya están documentados para responder. *(El reto parece evaluarse con una plataforma **CTFd**: hay que poder responder qué falló, por qué y en qué archivo.)*

> Plantilla lista para usar: [../BITACORA.md](../BITACORA.md)

---

## Mensaje final

> "Los mejores equipos no son los que cometen menos errores. Son los que encuentran la causa raíz más rápido."

**Detectar · Observar · Analizar · Hipótesis · Corregir · Validar**
