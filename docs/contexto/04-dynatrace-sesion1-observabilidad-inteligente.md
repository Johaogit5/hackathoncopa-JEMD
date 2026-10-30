# Sesión 1 de 2: Observabilidad Inteligente con Dynatrace

> Fuente: `Sesion 1 - Observabilidad Inteligente con Dynatrace.pdf` (Dynatrace).
> De los pilares fundamentales a la infraestructura, los servicios y los logs: cómo ver, entender y consultar todo lo que pasa en un sistema.
>
> La **Sesión 2** (aún no recibida) cubrirá: Application Security, User Experience, Business Observability y Dynatrace Intelligence.

## Agenda

1. **Pilares de la observabilidad:** qué es observar un sistema y las 3 señales que lo hacen posible.
2. **Infraestructura (VMs y Kubernetes):** de un servidor físico a un clúster dinámico de contenedores.
3. **Servicios y trazas:** cómo Dynatrace descubre servicios y sigue una petición punta a punta.
4. **Logs y DQL:** el registro fino del sistema y el lenguaje para preguntarle cualquier cosa.

---

## Bloque 1 — Pilares de la observabilidad

### Monitorear no es lo mismo que observar

| Monitorear | Observar |
|---|---|
| Vigilar métricas y umbrales que **ya conoces de antemano**: "avísame si el CPU pasa de 80%". | Poder hacer **preguntas nuevas** sobre un sistema sin haberlas previsto, usando los datos que ya genera. |

En sistemas modernos (cientos de microservicios, contenedores que nacen y mueren) es imposible anticipar cada pregunta. Por eso se necesita observabilidad y no solo monitoreo.

### Las 3 señales que hacen posible observar

| Señal | Qué es | Ejemplo |
|---|---|---|
| **Logs** | Eventos discretos con todo el detalle: qué pasó exactamente y cuándo | "Error 500 al conectar con la base de datos a las 14:32:07" |
| **Métricas** | Números agregados en el tiempo que describen la salud general | CPU al 87% en el host web-03 durante los últimos 5 minutos |
| **Trazas** | El recorrido completo de una solicitud por varios servicios | Un checkout pasa por 6 microservicios en 1.8 s |

### En Dynatrace, los 3 pilares viven juntos: Grail

**Grail** es el *lakehouse* de datos de Dynatrace: logs, métricas, trazas y eventos de seguridad se guardan **con el mismo contexto**, listos para consultarse juntos con un solo lenguaje: **DQL**.

```
Logs     ─┐                         ┌─► Dashboards & Notebooks
Métricas ─┼─► Grail (consultado ────┼─► Dynatrace Intelligence (causa raíz)
Trazas   ─┘    con DQL)             └─► Alertas y Workflows
```

---

## Bloque 2 — Infraestructura: VMs y Kubernetes

### Todo empieza con un servidor

- **Servidor físico:** máquina real con su propio CPU, memoria y disco. Cara y difícil de escalar rápido.
- **Máquina Virtual (VM):** software que simula un servidor completo dentro de otro servidor físico, con su propio sistema operativo. Permite correr varias "máquinas" en el mismo hardware.

### Contenedores: más ligeros que una VM

```
Máquina Virtual                    Contenedores
─────────────────                  ─────────────────────────────
Hardware físico                    Hardware físico
Hipervisor                         Sistema operativo (compartido)
S.O. invitado 1 (completo)         Motor de contenedores
  App A                            App A | App B | App C | App D
S.O. invitado 2 (completo)
  App B
```

Los contenedores comparten el mismo sistema operativo: **arrancan en segundos y pesan una fracción de una VM**.

### Kubernetes: quién ordena a los contenedores

Con cientos de contenedores en producción, alguien tiene que decidir dónde corre cada uno, reiniciarlos si fallan y escalarlos según demanda. Ese "alguien" es Kubernetes.

- **Auto-reparación:** reemplaza contenedores caídos sin intervención humana.
- **Auto-escalado:** agrega o quita réplicas según CPU, memoria o tráfico.
- **Scheduling:** decide en qué nodo se coloca cada contenedor.
- **Rolling updates:** despliega nuevas versiones de forma gradual, sin cortes.

### Tanta automatización es difícil de vigilar a mano

Los Pods nacen y mueren, las IPs cambian, el clúster puede tener miles de contenedores en decenas de nodos. **¿Cómo sabes si un nodo se está quedando sin memoria ahora mismo?** Sin métricas de infraestructura no lo sabrías hasta que fuera tarde.

### La app de Kubernetes: salud del clúster de un vistazo

Ejemplo de vista (`Dynatrace › Kubernetes › prod-cluster-01`):

| Indicador | Valor |
|---|---|
| Nodos | 18 / 18 saludables |
| Pods | 342 en ejecución |
| CPU del clúster | 63% usado |
| Pods con errores | 3 en *crashloop* |

Namespaces por consumo de CPU: checkout 82% · payments 61% · inventory 44% · notifications 22% · auth 15%.

La app incluye una vista *Overview* (clusters, nodes, namespaces, workloads, services, pods, containers), un *Explorer*, *Recommendations* y dashboards ya listos (cluster, node-pods, namespace-workloads, namespace-pods, persistent volumes).

### Smartscape: el mapa vivo de la infraestructura

**Smartscape** es la topología automática de Dynatrace: descubre y conecta en tiempo real hosts, VMs, contenedores, procesos y servicios, **sin que nadie la dibuje a mano**.

---

## Bloque 3 — Servicios y trazas

### ¿Qué es un "servicio" para Dynatrace?

Cualquier proceso que atiende solicitudes: una API, un microservicio, una función. Dynatrace los detecta **automáticamente al instalar OneAgent**, sin configurar nada a mano.

- Se agrupan automáticamente por tecnología: Java, .NET, Node.js, Go…
- Cada servicio recibe una **entidad propia** con su historial de rendimiento.
- Se conectan entre sí solos y forman el **mapa de dependencias real**.

Ejemplo de mapa de dependencias (checkout):

```
api-gateway ──► cart-service ──► payments-service ──► payments-db
     │
auth-service
```

### Las 4 métricas que definen la salud de un servicio

| Métrica | Qué mide |
|---|---|
| **Tiempo de respuesta** | Cuánto tarda en responder (p50, p90, p99) |
| **Tasa de fallos** | % de solicitudes que terminan en error |
| **Throughput** | Solicitudes procesadas por minuto/segundo |
| **Saturación** | Qué tan cerca está de su capacidad máxima (CPU, memoria, hilos) |

Dynatrace las calcula automáticamente para cada servicio detectado. Son la base del **Service flow** y de cualquier alerta de rendimiento.

### PurePath: la traza que sigue cada petición

**PurePath** es la tecnología de trazado de Dynatrace: captura cada solicitud individual de punta a punta, **con precisión de código y sin muestreo** (cada transacción, siempre).

En la app *Services* se ve, por servicio: *Failure rate*, *Response time*, *Throughput* y pestañas de Logs, Metrics, Database queries, Messaging, Outbound calls, Infrastructure, Events, Alerts y Analyze, además del botón **View traces**.

### Caso práctico: ¿quién ralentizó el checkout?

1. **Alerta:** el tiempo de respuesta de "checkout" sube de 400 ms a 2.1 s.
2. **Service flow:** se ve que `payments-service` concentra el salto de latencia.
3. **PurePath:** en una traza puntual, la llamada a `payments-db` tarda 1.6 s.
4. **Causa raíz:** una consulta SQL sin índice, identificada sin adivinar y en minutos.

---

## Bloque 4 — Logs y DQL

### Logs: el detalle que las métricas no dan

Cuando una métrica dice que algo está mal, **el log dice exactamente qué pasó**.

### DQL: una sola forma de preguntar por todo

DQL consulta logs, métricas, trazas y eventos de Grail **encadenando comandos con el operador pipe (`|`)**, igual que en la terminal de Linux.

- No necesitas saber en qué tabla vive el dato.
- Funciona igual sobre logs, métricas, trazas o infraestructura.
- Se usa en **Notebooks, Dashboards y Workflows**.

```dql
fetch logs
| filter k8s.namespace.name == "payments"
| filter loglevel == "ERROR"
| sort timestamp desc
| limit 50
```

### Anatomía de una query DQL

| Comando | Qué hace |
|---|---|
| `fetch` | Elige la fuente: `logs`, `spans`, `dt.entity.host`… |
| `filter` | Reduce el conjunto según condiciones |
| `fields` | Elige o calcula columnas a mostrar |
| `summarize` | Agrega: `count()`, `avg()`, `by:{...}` |
| `sort` / `limit` | Ordena y acota el resultado final |

```dql
fetch logs
| filter k8s.namespace.name == "payments"
| summarize errores = count(), by:{k8s.pod.name}
| sort errores desc
```

### Ejemplo: correlacionar un nodo con sus errores

Detectaste en el dashboard de Kubernetes que el nodo `aks-prod-node-07` tiene memoria alta. Esta query filtra solo los logs de error de los Pods de ese nodo, agrupados para ver cuál concentra más fallos:

```dql
fetch logs, from: -1h
| filter k8s.node.name == "aks-prod-node-07"
| filter loglevel == "ERROR"
| summarize errores = count(), by:{k8s.pod.name}
| sort errores desc
```

**Así se cierra el círculo:**
Infraestructura (**dónde**) → Servicio (**qué proceso**) → Traza (**qué tan lento**) → Log (**por qué exactamente**), con DQL como el hilo que conecta las cuatro.

---

## Lo que se llevan hoy

- **Observar > monitorear:** logs, métricas y trazas responden preguntas que no anticipaste.
- **K8s automatiza la infraestructura**, pero la vuelve dinámica y difícil de vigilar sin herramientas.
- **Los servicios se descubren solos:** PurePath sigue cada transacción punta a punta, sin muestreo.
- **DQL conecta todo:** un lenguaje simple para preguntar sobre infraestructura, servicios y logs.

---

## Equivalencias Azure ↔ Dynatrace (referencia rápida, elaboración propia)

| Concepto | Azure (Track Microsoft) | Dynatrace |
|---|---|---|
| Lenguaje de consulta | **KQL** | **DQL** |
| Almacén de telemetría | Log Analytics | Grail |
| Encadenar pasos | `\|` | `\|` |
| Filtrar | `where` | `filter` |
| Agrupar/contar | `summarize count() by x` | `summarize count(), by:{x}` |
| Ordenar | `order by x desc` | `sort x desc` |
| Limitar | `take N` | `limit N` |
| Rango de tiempo | `where timestamp > ago(1h)` | `fetch logs, from: -1h` |
| APM / trazas | Application Insights (`operation_Id`) | PurePath |
| Mapa de componentes | Application Map | Smartscape / Service flow |
| Tiempo real | Live Metrics / Log stream | LiveDebugger / Live data |
