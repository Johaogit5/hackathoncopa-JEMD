# Transcripción Oficial — Workshop #4: Observabilidad, Application Insights, Logs y KQL

> **Video:** https://www.youtube.com/watch?v=4uqM6LG0rro · Transcripción oficial entregada por la organización. Los subtítulos automáticos (para buscar frases textuales) están en [`subtitulos/w4.txt`](subtitulos/w4.txt).

## ⚡ Puntos clave para el reto

- Tablas de App Insights: **`requests`**, **`exceptions`**, **`dependencies`** y **`traces`**. **`operation_Id`** enlaza una misma transacción entre ellas.
- KQL: `where`, `summarize ... by bin(timestamp, 5m)`, `order by`, `project`, `take`.
- Herramientas del portal: **Live Metrics** (tiempo real), **Metrics** (cuidado con Local vs. UTC), **Application Map** (rojo = errores) y **Logs** (KQL).
- Monitoreo = umbrales conocidos; observabilidad = responder preguntas no anticipadas.

---

**Evento:** Hackatón Copa 2026 — API, DevOps y Observabilidad  
**Expositor Principal:** Daniel Vacas (Business IT)  
**Moderadores y Presentadores:** Luis Martínez, Evelin Ceballos (Business IT), Hugo Aquino  

---

## 1. Bienvenida e Introducción al Workshop #4

**Luis Martínez:**

Muy buenas noches a todos, bienvenidos. Este es el cuarto workshop de la serie enfocada en **API, DevOps y Observabilidad**. Ya llevamos tres sesiones realizadas en semanas anteriores, las cuales pueden consultar directamente en el canal oficial de YouTube del evento (**Hakaton CM**). En el chat les estaremos compartiendo los enlaces a las grabaciones, redes sociales oficiales y el formulario de asistencia para registrar su participación.

El taller de la semana pasada fue guiado y altamente práctico (DevOps con Terraform); si no pudieron seguirlo en vivo, los invito a revisar la grabación para ponerse al día y replicar los ejercicios en sus máquinas. 

Esta noche nos acompañan Evelin Ceballos y Daniel Vacas por parte del equipo técnico de Business IT, así como el señor Hugo Aquino del comité organizador. Les cedo la palabra.

**Evelin Ceballos:**

Muchas gracias, Luis. Bienvenidos nuevamente a esta edición de la Hackatón. Les presento a Daniel Vacas, quien será nuestro instructor durante esta noche. Esperamos que los conceptos compartidos sean de máximo provecho para fortalecer sus habilidades técnicas de cara a la competencia. Adelante, Daniel.

**Daniel Vacas:**

Muchas gracias, Evelin y Luis. Muy buenas noches con todos, bienvenidos. 

Hoy daremos continuidad a lo abordado en sesiones anteriores, pero entraremos en una etapa crítica del ciclo de vida del software: **¿Qué pasa cuando todo falla?** ¿Cómo diagnosticamos, aislamos y entendemos un problema en producción? El objetivo de hoy es dominar la observabilidad moderna utilizando **Application Insights**, gestión estructurada de logs, métricas y consultas analíticas mediante **KQL (Kusto Query Language)**.

---

## 2. Los Dos Grandes Pilares de Monitoreo y Seguridad en Azure

**Daniel Vacas:**

Dentro del ecosistema de Microsoft Azure existen dos grandes áreas de supervisión y gobernanza de datos telemétricos:

1. **Seguridad y Auditoría Global (Microsoft Sentinel / Microsoft Defender for Cloud):**  
   Analizan el comportamiento de identidades, amenazas de red y la seguridad integral de todo el *tenant*. Procesa eventos de auditoría y registros centralizados.
2. **Observabilidad y Rendimiento de Aplicaciones (Azure Monitor):**  
   Gestiona la supervisión operativa del software, la disponibilidad, las métricas de rendimiento y la telemetría en tiempo de ejecución. 

### ¿Dónde se almacenan estos registros?
Toda la información (logs, eventos, auditorías) se almacena y clasifica en un recurso central llamado **Log Analytics Workspace**. 
* Organiza los datos en tablas estructuradas optimizadas para consultas analíticas de alta velocidad.
* **Gobierno y retención:** Por defecto, los datos tienen un periodo de retención configurable (comúnmente 30 o 60 días para entornos estándar); es fundamental planificar la retención para optimizar costos de almacenamiento.

---

## 3. Azure Monitor, Application Insights y OpenTelemetry

**Daniel Vacas:**

Dentro de Azure Monitor destacan dos componentes clave:
* **Métricas:** Comportamiento numérico y agregado de los recursos en función del tiempo.
* **Application Insights:** Servicio especializado en APM (*Application Performance Monitoring*) que proporciona seguimiento detallado de eventos, solicitudes, trazas y excepciones en el código.

### La relación con OpenTelemetry
Application Insights está fundamentado en el estándar abierto **OpenTelemetry**:
* **Ventaja nativa en Azure:** Al crear un servicio (como Azure Functions o App Service), basta con habilitar Application Insights mediante un interruptor (*toggle*); Azure inyecta la telemetría de forma desatendida sin necesidad de modificar el código fuente.
* **Flexibilidad híbrida y multi-nube:** Si una aplicación reside on-premise, en otra nube (AWS, GCP) o en local, se puede instrumentar con el SDK abierto de OpenTelemetry y dirigir la telemetría hacia Application Insights o Log Analytics Workspace.

---

## 4. Los Tres Pilares de la Observabilidad

**Daniel Vacas:**

Para diagnosticar cualquier sistema distribuido nos apoyamos en tres señales fundamentales:

```
                      ┌──────────────────────────┐
                      │      OBSERVABILIDAD      │
                      └─────────────┬────────────┘
         ┌──────────────────────────┼──────────────────────────┐
         ▼                          ▼                          ▼
   ┌───────────┐              ┌───────────┐              ┌───────────┐
   │   LOGS    │              │  MÉTRICAS │              │  TRAZAS   │
   └─────┬─────┘              └─────┬─────┘              └─────┬─────┘
         │                          │                          │
   ¿Qué pasó exactamente?     ¿Cómo se comporta el       ¿Por dónde pasó la
   (Registros estructurados,  sistema en el tiempo?      petición? (Cascada,
   contexto del evento)       (CPU, RAM, latencia)       stack trace, llamadas)
```

1. **Logs (¿Qué pasó exactamente?):**  
   Instantáneas detalladas del evento. Permiten determinar cuándo ocurrió una falla, qué usuario se vio afectado y cuál fue el mensaje de error.
2. **Métricas (¿Cómo se comporta en el tiempo?):**  
   Valores agregados en tiempo real (porcentaje de uso de CPU, consumo de memoria, tasa de peticiones por segundo, latencias promedio). Permiten identificar tendencias, saturación y disparar reglas de autoescalado horizontal o vertical.
3. **Trazas / Traces (¿Por dónde pasó la solución?):**  
   Mapeo de la ruta completa de una solicitud (el *stack trace* en código o la cascada de llamadas entre servicios, microservicios y bases de datos). Permiten identificar con precisión quirúrgica el eslabón donde se degradó el flujo.

### Monitoreo Tradicional vs. Observabilidad Moderna
* **Monitoreo:** Enfoque preventivo/sintomático basado en umbrales conocidos ("la CPU superó el 85%", "el endpoint arrojó un error 500").
* **Observabilidad:** Enfoque diagnóstico posterior al fallo que correlaciona señales para responder preguntas complejas sobre estados no anticipados ("¿Por qué este usuario específico en esta región recibió un error 500 tras invocar una transacción mixta?").
* **Mitigación:** La fase de remediación que cierra el ciclo.

---

## 5. Arquitectura de Telemetría y Tablas Clave en Application Insights

**Daniel Vacas:**

El flujo de ingestión opera de la siguiente manera:  
`Aplicación (con SDK / OpenTelemetry) ──> Ingesta de Telemetría ──> Log Analytics Workspace ──> KQL / Dashboards / Alertas`

Dentro de Log Analytics Workspace y Application Insights, la información no se guarda de forma dispersa; se organiza en tablas relacionales de telemetría:

| Tabla de Telemetría | Descripción | Atributos Principales |
| :--- | :--- | :--- |
| **`requests`** | Solicitudes HTTP entrantes recibidas por el servicio. | `name`, `url`, `resultCode`, `duration`, `success`, `timestamp` |
| **`exceptions`** | Errores no controlados durante la ejecución de la aplicación. | `problemId`, `type`, `outerMessage`, `details`, `innermostMessage` |
| **`dependencies`** | Llamadas salientes hacia sistemas externos (bases de datos, APIs de terceros, colas). | `target`, `data`, `resultCode`, `duration`, `type`, `success` |
| **`traces`** | Mensajes de log emitidos por la aplicación o el framework. | `message`, `severityLevel`, `operation_Id`, `customDimensions` |

### Correlación de Transacciones Distribuidas
El identificador **`operation_Id`** es crítico en arquitecturas modernas: permite enlazar un error originado en el frontend con la solicitud procesada en el backend y la posterior consulta fallida en la base de datos bajo un mismo hilo transaccional.

---

## 6. Introducción a KQL (Kusto Query Language)

**Daniel Vacas:**

KQL es el lenguaje optimizado de consulta para grandes volúmenes de telemetría y logs en Azure. Posee una sintaxis declarativa tabular basada en el operador tubería (`|`), muy similar conceptualmente a SQL pero encadenada secuencialmente de izquierda a derecha.

### Operadores Principales:
* **`where`:** Filtra registros según una condición booleana (equivalente a `WHERE` en SQL).
* **`summarize`:** Agrupa y realiza agregaciones matemáticas (`count()`, `avg()`, `sum()`, `percentiles()`).
* **`order by` / `sort by`:** Ordena los resultados de forma ascendente o descendente (`asc` / `desc`).
* **`project`:** Selecciona o renombra columnas específicas a proyectar en el resultado.
* **`take` / `limit`:** Restringe el número máximo de filas devueltas (similar a `TOP` o `LIMIT`).
* **`bin()`:** Agrupa marcas de tiempo (`timestamp`) en cubos o intervalos temporales específicos (por ejemplo, intervalos de 5 minutos o 1 hora), fundamental para construir series de tiempo.

### Ejemplos Prácticos de Consultas KQL:

#### 1. Detección de solicitudes fallidas en la última hora agrupadas cada 5 minutos:
```kql
requests
| where timestamp > ago(1h)
| where success == false
| summarize Fallos = count() by bin(timestamp, 5m), resultCode
| order by timestamp desc
```

#### 2. Las 10 excepciones más frecuentes de las últimas 24 horas:
```kql
exceptions
| where timestamp > ago(24h)
| summarize Total = count() by type, outerMessage
| order by Total desc
| take 10
```

---

## 7. Demostración Práctica: Inyección de Errores y Análisis de Telemetría

**Daniel Vacas:**

Para analizar el comportamiento de las herramientas de diagnóstico en tiempo real, desplegamos una aplicación **Azure Function** bajo un plan *Flex Consumption* en Linux con runtime de **.NET 10 LTS**.

### Simulación de Fallos Mediante Temporizador (*Timer Trigger*)
Se programó una Azure Function con una expresión CRON periódica (`0 */2 * * * *`) que ejecuta cuatro escenarios de prueba cada 2 minutos:
1. **Fallo de Dependencia en Base de Datos:** Simula una conexión fallida y tiempo de espera agotado (*timeout*) contra una base de datos ficticia.
2. **Fallo de Dependencia HTTP:** Consume un endpoint externo que responde intencionalmente códigos de estado 503 (*Service Unavailable*).
3. **Generación de Excepciones No Controladas:** Emite con una probabilidad superior al $60\%$ errores de tipo `System.ArgumentOutOfRangeException` con trazas de pila completas.
4. **Peticiones Exitosas:** Genera solicitudes HTTP con códigos 200 para contrastar la tasa de éxito vs. fallo.

### Herramientas de Inspección en el Portal de Azure

1. **Live Metrics (Telemetría en Vivo):**  
   Muestra el pulso de la aplicación segundo a segundo: tasa de solicitudes entrantes, latencia de procesamiento, fallos en caliente y el consumo de CPU y memoria de las instancias activas.
2. **Explorador de Métricas (Azure Monitor Metrics):**  
   Permite contrastar métricas de sistema (CPU %, memoria utilizada) contra métricas de aplicación (conteo de invocaciones, errores). Daniel demostró cómo ajustar la escala temporal (visualización a 4 horas vs. 24 horas) y la zona horaria (Local vs. UTC) para evitar desajustes visuales al analizar incidentes.
3. **Application Map:**  
   Diagrama topológico generado automáticamente que ilustra la Azure Function, sus llamadas hacia APIs externas y las conexiones a bases de datos, resaltando en rojo los componentes que presentan tasas elevadas de error.
4. **Editor de Consultas de Logs (Log Analytics):**  
   Ejecución de consultas KQL sobre las tablas `traces`, `requests`, `exceptions` y `dependencies`. Desde la tabla `exceptions` se analizó el detalle del `problemId`, identificando exactamente la clase y línea de código donde falló la llamada.

---

## 8. Sesión de Preguntas y Respuestas (Q&A)

**Participante:**  
*¿Qué tan diferente es hacer esto mismo con Grafana o Prometheus?*

**Daniel Vacas:**  
La diferencia radica en el enfoque de integración y esfuerzo operativo:
* **Prometheus y Grafana:** Son herramientas de código abierto líderes y altamente potentes. Sin embargo, requieren instalar agentes recolectores, administrar servidores de base de datos de series de tiempo (TSDB), configurar almacenamiento y diseñar los paneles desde cero.
* **Application Insights en Azure:** Ofrece una experiencia completamente nativa y desatendida. Al activar un recurso, Azure enlaza automáticamente la recolección de logs, métricas y dependencias sin configuraciones manuales de red ni infraestructura de monitoreo dedicada. No obstante, mediante OpenTelemetry, Azure Monitor también permite exportar telemetría hacia Grafana si una empresa requiere tableros unificados multi-proveedor.

---

## 9. Cierre de la Sesión y Próximos Pasos

**Daniel Vacas:**

Dominar la observabilidad y el lenguaje KQL les permitirá detectar la causa raíz de cualquier problema en cuestión de minutos, en lugar de navegar a ciegas en archivos de texto plano. Les compartí en el chat los enlaces oficiales de **Microsoft Learn** para profundizar en Application Insights y OpenTelemetry. Les deseo el mayor de los éxitos en los retos. Muchas gracias a todos.

**Luis Martínez:**

Excelente explicación, Daniel. Toda esta información sobre métricas, logs y KQL será determinante para resolver los retos técnicos del Hackatón, tanto en la sede de Santiago como en la de Ciudad de Panamá.

**Recordatorios importantes para los participantes:**
1. Llenar el **formulario de asistencia** disponible en el chat.
2. Todas las grabaciones de los workshops están alojadas en nuestro canal de YouTube oficial: **Hakaton CM**.
3. El próximo **martes 8** continuaremos con el **Workshop #5**, enfocado en **Dynatrace, Kubernetes y Observabilidad Avanzada** (7:00 PM hora de Panamá).
4. El **lunes 14** tendremos una sesión abierta de **Preguntas y Respuestas (Q&A)** para resolver dudas generales previas a la competencia. Para consultas directas sobre las bases, pueden ingresar a **hacatoncopa.com** o escribir a **jacatoncopa@copaair.com**.

¡Muchísimas gracias a todos por su participación y que tengan una excelente noche!