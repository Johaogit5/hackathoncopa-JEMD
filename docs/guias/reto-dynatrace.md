# Reto Dynatrace: guía rápida y cheat-sheet DQL

Fuentes: [W6](../workshops/w6-dynatrace-kubernetes.md), [Sesión 1](../contexto/04-dynatrace-sesion1-observabilidad-inteligente.md) y [Conceptos clave](../contexto/03-dynatrace-conceptos-clave.md). Todas las consultas están copiadas tal cual de esas fuentes.

## Cómo será el reto (lo dicho en W6)

- Es **un reto aparte** del de Azure: *"lo que vieron en las sesiones anteriores va a ser parte del reto de Azure y lo de esta sesión parte del reto de Dynatrace"*.
- Se trabaja en un **tenant privado del hackatón** (no en el Playground), con una aplicación que genera el tráfico y los problemas.
- **Davis AI está activo**: agrupa los problemas y marca la causa raíz. **Dynatrace Assist (el chat) está desactivado.**
- **No se permite IA generativa externa**, y además el acceso a los datos desde fuera estará restringido.
- Tipos de pregunta: **escribir DQL** en algunas secciones y **encontrar información en la plataforma** en otras (nombre de un servicio, infraestructura, causa raíz, métricas).

## Ruta para investigar un problema

1. **Problems**: abrir el problema que detectó Davis y leer la causa raíz que marca (en rojo).
2. **Mapa / Smartscape**: ver qué servicio llama a cuál y en qué nodo corre. Los componentes en rojo tienen problemas.
3. **Servicio**: revisar la *failure rate*, el tiempo de respuesta y las solicitudes afectadas.
4. **Trazas distribuidas (PurePath)**: abrir una traza fallida e identificar el componente que lanzó la excepción.
5. **Logs**: desde la traza o con DQL, obtener el mensaje exacto del error.

Así se cierra el círculo: Infraestructura (**dónde**) → Servicio (**qué proceso**) → Traza (**qué tan lento**) → Log (**por qué exactamente**).

## Kubernetes en 5 conceptos

| Concepto | Qué es | Analogía de la cocina (W6) |
|---|---|---|
| **Node** | Máquina física o virtual | Estación de cocina |
| **Pod** | Unidad mínima: uno o más contenedores que comparten red y almacenamiento | El platillo |
| **Deployment** | Cuántas réplicas de un pod deben estar vivas | — |
| **Service** | IP virtual estable y balanceador hacia los pods | — |
| **Namespace** | Separación lógica (dev, prod, pagos…) | — |
| Kubernetes | Decide dónde corre cada pod, balancea y reemplaza lo que falla | El chef ejecutivo |

## Tres pilares

| Pilar | Responde | Detalle |
|---|---|---|
| **Logs** | ¿Qué pasó exactamente? | timestamp, severidad (`INFO`/`WARN`/`ERROR`), origen y mensaje |
| **Métricas** | ¿Cómo se comporta en el tiempo? | Counter (solo sube), Gauge (sube y baja), Histograma (percentiles) |
| **Trazas** | ¿Por dónde pasó la petición? | Tiempo de cada llamada; señala el componente que falló |

Arquitectura: **OneAgent** (captura automática) → **OpenPipeline** (enriquece/sanitiza) → **Grail** (data lakehouse) → DQL / Dashboards / Notebooks / Davis.

## DQL: comandos

Los comandos se encadenan con `|` y se leen de izquierda a derecha y de arriba hacia abajo.

| Comando | Qué hace | Equivalente KQL |
|---|---|---|
| `fetch` | Fuente: `logs`, `events`, `spans`, `bizevents`, `dt.entity.host`… | nombre de la tabla |
| `filter` | Filtra por una condición | `where` |
| `fields` | Elige, calcula o renombra columnas | `project` |
| `summarize` | Agrega: `count()`, `avg()`, `sum()`, `percentiles()`, con `by:{campo}` | `summarize ... by` |
| `sort` | Ordena (`asc` / `desc`) | `order by` |
| `limit` | Limita el número de filas | `take` |
| `makeTimeseries` | Convierte registros en una serie de tiempo para graficar | `summarize ... by bin()` |

Rango de tiempo: `fetch logs, from:now()-30m` o `fetch logs, from: -1h`.

## DQL: consultas de ejemplo

```dql
// Errores de un namespace (W6)
fetch logs
| filter matchesValue(k8s.namespace.name, "booking-prod")
| filter status == "ERROR"
```

```dql
// Logs de Azure por severidad, últimos 30 min (W6)
fetch logs, from:now()-30m
| filter cloud.provider == "Azure"
| summarize TotalLogs = count(), by:{loglevel}
```

```dql
// Errores como gráfico cada 5 min (W6)
fetch logs, from:now()-2h
| filter status == "ERROR"
| makeTimeseries ConteoErrores = count(), interval:5m
```

```dql
// Últimos 50 errores de un namespace (Sesión 1)
fetch logs
| filter k8s.namespace.name == "payments"
| filter loglevel == "ERROR"
| sort timestamp desc
| limit 50
```

```dql
// ¿Qué pod concentra los logs? (Sesión 1)
fetch logs
| filter k8s.namespace.name == "payments"
| summarize errores = count(), by:{k8s.pod.name}
| sort errores desc
```

```dql
// Errores de los pods de un nodo, última hora (Sesión 1)
fetch logs, from: -1h
| filter k8s.node.name == "aks-prod-node-07"
| filter loglevel == "ERROR"
| summarize errores = count(), by:{k8s.pod.name}
| sort errores desc
```

> Ojo: en los ejemplos aparecen tanto `status == "ERROR"` como `loglevel == "ERROR"`. En el tenant, revisen primero qué campo trae el dato: ejecuten `fetch logs | limit 10` y miren las columnas.

## Recursos para practicar

- **Playground:** https://www.dynatrace.com/try-free/ → "Explore Playground". Registro paso a paso en la [Guía del estudiante](../contexto/05-guia-estudiante-dynatrace.md).
- **Dynatrace University → Dynatrace Essentials** (gratis, 9 lecciones, ~4 h 30 min, con examen de certificado). Se entra con las mismas credenciales del Playground.
- La documentación de DQL de Dynatrace tiene el botón **"Run in Playground"** para probar cada comando.
