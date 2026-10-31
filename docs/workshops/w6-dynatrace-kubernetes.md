# Transcripción Oficial — Workshop #6: Observabilidad con Dynatrace, Kubernetes y DQL

> **Video:** https://www.youtube.com/watch?v=WV9QXQs9-Ts · Transcripción oficial entregada por la organización. Los subtítulos automáticos (para buscar frases textuales) están en [`subtitulos/w6.txt`](subtitulos/w6.txt).

## ⚡ Puntos clave para el reto

- **Habrá 2 retos separados:** Azure (contenido de W1–W5) y Dynatrace (este workshop).
- El reto de Dynatrace corre en un **tenant privado** con una app que genera los problemas. No es el Playground.
- **Davis AI** estará activo; **Dynatrace Assist (chat) estará apagado**. **No se permite IA generativa externa.**
- Las preguntas pedirán **escribir DQL** y **encontrar datos en la plataforma** (nombres de servicio, infraestructura, causa raíz, métricas).
- Flujo para un problema: **Problems** → causa raíz marcada por Davis → mapa (Smartscape) → trazas → logs/excepciones.
- Arquitectura: OneAgent → OpenPipeline → **Grail** → DQL / Dashboards / Notebooks / Davis.

---

**Evento:** Hackatón Copa 2026 — API, DevOps y Observabilidad

**Expositores Principales (Equipo Dynatrace):**

* **Carlos Fernández:** Ingeniero en Computación (Universidad Simón Bolívar), consultor con más de 20 años de experiencia tecnológica en Latinoamérica y el Caribe.

* **Ángel Alemán:** Especialista en observabilidad, dashboards y pipelines de datos con más de 6 años de experiencia en México, Centroamérica y el Caribe.

**Moderadores y Representantes del Comité Organizador:**

* **Luis Martínez** (Host y voluntario del Hackatón)

* **Evelin Ceballos** (Learning Leader en Business IT)

* **Larisa Aguilar** y **Hugo Aquino** (Comité Organizador Hackatón Copa)

## 1. Bienvenida e Introducción al Workshop #6

**Luis Martínez:**

Muy buenas noches y buenas tardes a todos. Les damos la más cordial bienvenida a este workshop enfocado en la plataforma de **Dynatrace Playground**, uno de los insumos fundamentales que estarán utilizando durante los retos del evento.

En la noche de hoy nos acompañan nuestros compañeros de Business IT, en este caso Evelin Ceballos, y los compañeros especialistas de Dynatrace: Ángel Alemán y Carlos Fernández. Por parte del comité organizador contamos con la presencia de Larisa Aguilar y el señor Hugo Aquino.

Mi nombre es Luis Martínez, voluntario del Hackatón desde el año 2022. Les presento a quienes los estarán guiando hoy:

* **Carlos Fernández:** Ingeniero en computación egresado de la Universidad Simón Bolívar en Caracas, Venezuela; profesional con más de 20 años de experiencia en tecnología, ingeniería y consultoría representando fabricantes líderes en Latinoamérica y el Caribe.

* **Ángel Alemán:** Especialista con más de 6 años de experiencia en pipelines de datos, dashboards y observabilidad cubriendo México, Centroamérica y el Caribe, apasionado por enseñar y compartir conocimiento bajo la premisa de que en esta industria todos aprendemos de todos.

Les recordamos que todas las dudas pueden dejarlas en el chat para que los instructores las respondan en vivo. También les compartiremos el enlace de asistencia y los canales oficiales. Adelante, chicos.

**Ángel Alemán:**

Muchas gracias, Luis. Buenas noches y buenas tardes a todos. Durante esta hora y media les contaremos sobre observabilidad y cómo aplicarla en la práctica utilizando Dynatrace, preparándolos para los desafíos del hackatón.

Recorreremos qué es Kubernetes, por qué surge la necesidad de observar entornos modernos, los tres pilares de la observabilidad (Logs, Métricas y Trazas) y cómo Dynatrace unifica todo este ecosistema.

### ¿Qué es Dynatrace?

Dynatrace es una plataforma de observabilidad integral nacida hace aproximadamente 17 años en Linz, Austria. Cubre todas las capas tecnológicas de una empresa:

* **Infraestructura física y virtual** (servidores, redes, contenedores).

* **Rendimiento de aplicaciones** y microservicios.

* **Experiencia de usuario digital** (monitoreo de sesiones web y móviles).

* **Seguridad en tiempo de ejecución** (detección de vulnerabilidades activas).

* **Inteligencia Artificial y analítica de datos** (Davis AI).

Imaginen la aplicación móvil de una aerolínea como Copa Airlines para comprar un boleto, o una aplicación de banca en línea: detrás de cada clic intervienen decenas de capas tecnológicas interconectadas. La misión de Dynatrace es monitorear cada eslabón para que, ante cualquier degradación, se identifique y resuelva la falla antes de impactar a los usuarios.

## 2. Introducción a Kubernetes y la Necesidad de la Observabilidad

**Ángel Alemán:**

Cuando una aplicación atiende a miles o millones de usuarios, no puede residir en un único servidor; requiere múltiples máquinas operando coordinadamente como un clúster resiliente y elástico. Aquí es donde entra **Kubernetes**.

### La Analogía de la Cocina del Restaurante

> * **Los Nodos (Nodes):** Son las máquinas físicas o virtuales, equivalentes a las **estaciones de cocina**.
>
> * **Los Pods (y Contenedores):** Son las unidades mínimas donde residen las aplicaciones, equivalentes a los **platillos que se preparan** en cada estación. Varios contenedores pueden compartir red, almacenamiento y tareas dentro del mismo pod.
>
> * **Kubernetes:** Es el **Chef Ejecutivo**. Decide en qué estación se cocina cada platillo, balancea la carga si una estación se satura, aprovisiona nuevas estaciones según la demanda y reemplaza de inmediato cualquier componente que falle.

### Componentes Clave en Kubernetes:

1. **Pods:** Unidad básica de cómputo que contiene uno o más contenedores compartiendo recursos y contexto de red.

2. **Deployments:** Controladores que definen cuántas réplicas idénticas de un pod deben mantenerse activas en todo momento, escalando hacia arriba o hacia abajo.

3. **Services:** Direcciones IP virtuales estables y balanceadores de carga que enrutan el tráfico hacia los pods correspondientes.

4. **Namespaces:** Segmentaciones lógicas dentro de un mismo clúster para aislar servicios, entornos o proyectos (por ejemplo: desarrollo, producción o pasarela de pagos).

5. **Estrategias de Actualización (Rollouts y Rollbacks):** Despliegues graduales sin caída del servicio y reversión automática ante fallos inesperados.

### El Desafío de la Complejidad

En un entorno con miles de microservicios efímeros que suben y bajan dinámicamente: *¿Cómo sabemos con precisión qué falló sin perder horas adivinando o revisando registros aislados?* Si no podemos ver lo que ocurre en tiempo real, no lo podemos arreglar.

## 3. Demostración Visual del Clúster de Kubernetes en Dynatrace

**Carlos Fernández:**

A continuación comparto pantalla para que observen cómo Dynatrace representa visualmente la topología de un clúster de Kubernetes.

*(Carlos proyecta la consola de Dynatrace mostrando el mapa topológico del clúster)*

Lo que ven aquí es la topología interactiva (coloquialmente llamada "la araña"):

* Desde el **clúster principal** se desglosan los **Namespaces**, y dentro de ellos se agrupan los diferentes **servicios** y **cargas de trabajo (workloads)**.

* Dynatrace mapea automáticamente las relaciones de dependencia: qué servicio consume a cuál y sobre qué nodos físicos o virtuales reside.

* **Identificación visual de anomalías:** Aquellos nodos o pods que presentan problemas de saturación o fallas se marcan automáticamente en **color rojo**, permitiendo que los operadores identifiquen cuellos de botella de un vistazo sin revisar línea por línea.

## 4. Los Tres Pilares de la Observabilidad

**Ángel Alemán:**

La observabilidad de cualquier sistema en la industria se basa en tres señales esenciales que no se sustituyen entre sí, sino que se complementan:

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
   Bitácora textual del       Valores agregados en       Viaje transaccional
   evento (Timestamp,         el tiempo (Counters,       de punta a punta
   Severidad, Mensaje)        Gauges, Histogramas)       (Cascada distribuida)

```

### 1. Logs (Registros Textuales)

Es la bitácora histórica de lo ocurrido. Cada línea contiene comúnmente:

* **Timestamp:** Marca temporal exacta del evento.

* **Nivel de severidad:** `INFO` (informativo), `WARN` (advertencia) o `ERROR` (fallo de ejecución o error HTTP).

* **Origen:** Identificador del host, máquina virtual, namespace, pod o servicio emisor.

* **Mensaje:** Descripción detallada (por ejemplo: `Error 500 al conectarse a la base de datos a las 14:02:11`).

### 2. Métricas (Valores Numéricos Agregados en el Tiempo)

Muestran el comportamiento cuantitativo del sistema (uso de CPU, consumo de memoria, throughput de red, peticiones por segundo). Se dividen en tres tipos:

* **Counter (Contadores):** Valores siempre incrementales (como el total de personas conectadas a una sesión o el total de compras procesadas).

* **Gauge (Medidores):** Valores que fluctúan hacia arriba y hacia abajo en tiempo real (similar a la aguja del velocímetro o acelerador de un automóvil).

* **Histogramas:** Distribuciones estadísticas agrupadas por percentiles o medias (por ejemplo: percentil 95 o 99 del tiempo de respuesta cada 10 minutos).

### 3. Trazas (Traces - Transacciones Distribuidas de Punta a Punta)

Es el mapa de ruta completo que recorre una solicitud a través de todos los componentes del sistema.

*Ejemplo de compra:* Cuando un usuario hace clic en el botón *"Comprar y Pagar"* en una tienda online, la solicitud pasa secuencialmente por:

1. Validación de usuario y sesión en el API Gateway.

2. Verificación de saldo y tarjeta en la pasarela de pagos bancaria.

3. Descuento de inventario en la base de datos de stock.

4. Servicio de facturación y despacho de correo de confirmación.

La traza mide el tiempo exacto que tardó cada una de esas llamadas e identifica inmediatamente cuál de los componentes falló o generó latencia excesiva.

## 5. Análisis Práctico de Señales en Dynatrace

**Carlos Fernández:**

Volviendo a la consola práctica, analizamos cómo interactúan estas tres señales en un entorno real:

### 1. Métricas del Clúster

Dynatrace presenta paneles con el consumo global de CPU, saturación de memoria RAM y el número exacto de pods y unidades de cómputo operativas dentro del clúster.

### 2. Seguridad en Tiempo de Ejecución (Application Security)

La plataforma analiza las cargas de trabajo e identifica **vulnerabilidades activas**:

* Nivel de criticidad (CVSS).

* Componente y librería de software afectada.

* Versión específica de la librería vulnerable.

* Si la vulnerabilidad está expuesta al internet público o tiene vector de explotación activo.

### 3. Monitoreo de Servicios (Ejemplo: Reserva de Vuelos)

Aislamos un servicio específico (como la reserva de vuelos dentro de una aplicación móvil):

* Visualización gráfica de la **tasa de fallos (*failure rate*)**: la gráfica muestra un incremento repentino de errores donde antes operaba con normalidad.

* Desglose de solicitudes afectadas y tiempos de respuesta degradados.

### 4. Inspección de Trazas Distribuidas

Al abrir la traza de una transacción fallida, observamos la cadena completa de llamadas. Si la transacción no pudo completarse, Dynatrace resalta el componente exacto que arrojó la excepción, permitiendo abrir directamente los logs asociados a ese servicio, la clase de código y la causa raíz del error sin tener que buscar manualmente entre millones de registros.

## 6. La Arquitectura Subyacente de Dynatrace: OneAgent, OpenPipeline y Grail

**Ángel Alemán:**

Dynatrace no requiere instrumentar manualmente el código fuente de cada microservicio. Su arquitectura se fundamenta en tres pilares tecnológicos:

```
┌─────────────────┐       ┌────────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│    OneAgent     │ ───>  │    OpenPipeline    │ ───>  │      Grail      │ ───>  │  DQL / Dashboards
│ (Host / Nodos)  │       │ (Enriquecimiento)  │       │  (Data Lake)    │       │   / Davis AI    │
└─────────────────┘       └────────────────────┘       └─────────────────┘       └─────────────────┘

```

1. **OneAgent:**
   Un único agente binario que se instala a nivel de host, máquina virtual o nodo de Kubernetes. Descubre automáticamente los procesos, lenguajes (Java, .NET, Node.js, Go, Python) y contenedores, inyectando la captura de logs, métricas y trazas de forma 100% desatendida.

2. **OpenPipeline:**
   Mecanismo de procesamiento que enriquece, transforma, extrae campos clave, sanitiza datos sensibles y enruta la información telemétrica en tiempo de ingestión.

3. **Grail:**
   Es el almacenamiento masivo tipo *Data Lakehouse* nativo de Dynatrace. Diseñado sin esquemas rígidos (*schema-on-read*), indexa y correlaciona instantáneamente peticiones, logs, dependencias topológicas y eventos dentro de un mismo contexto temporal.

## 7. Dominando DQL (Dynatrace Query Language)

**Ángel Alemán:**

Para consultar datos dentro de Grail, Dynatrace implementó **DQL (Dynatrace Query Language)**. Es un lenguaje analítico basado en tuberías (`|`), donde las instrucciones se encadenan de izquierda a derecha y de arriba hacia abajo.

### Operadores Fundamentales de DQL:

* **`fetch`:** Define la fuente de datos a consultar (`logs`, `events`, `dt.entity.host`, `bizevents`).

* **`filter`:** Restringe el conjunto de datos bajo condiciones booleanas (similar a `where` en SQL).

* **`fields`:** Proyecta, selecciona o renombra columnas específicas.

* **`summarize`:** Agrupa y realiza agregaciones matemáticas (`count()`, `avg()`, `sum()`, `percentiles()`).

* **`sort`:** Ordena los resultados de forma ascendente o descendente (`asc` / `desc`).

* **`limit`:** Restringe el número máximo de registros a devolver.

* **`makeTimeseries`:** Convierte registros de logs y datos discretos en series de tiempo gráficas continuas.

### Casos de Uso y Consultas Demostradas en Vivo

**Carlos Fernández:**

*(Carlos ejecuta las consultas en vivo sobre la consola de Dynatrace)*

#### 1. Filtrado de logs con error en un Namespace específico:

```
fetch logs
| filter matchesValue(k8s.namespace.name, "booking-prod")
| filter status == "ERROR"

```

*Resultado:* Dynatrace descarta los millones de logs del clúster y devuelve exclusivamente las líneas de fallo asociadas a ese microservicio.

#### 2. Agrupación y conteo de logs de Azure por nivel de severidad en los últimos 30 minutos:

```
fetch logs, from:now()-30m
| filter cloud.provider == "Azure"
| summarize TotalLogs = count(), by:{loglevel}

```

*Resultado:* Presenta una tabla con la distribución cuantitativa de logs categorizada por `INFO`, `WARN` y `ERROR`.

#### 3. Conversión de logs en gráfico temporal (`makeTimeseries`):

```
fetch logs, from:now()-2h
| filter status == "ERROR"
| makeTimeseries ConteoErrores = count(), interval:5m

```

*Resultado:* Transforma líneas de texto plano en una gráfica temporal con intervalos de 5 minutos, permitiendo visualizar los picos donde se concentraron las fallas del sistema.

#### 4. Comparación de tendencias y deltas porcentuales:

Carlos mostró cómo DQL permite comparar periodos de tiempo contiguos: la plataforma detectó un incremento del $+2000\%$ en la tasa de errores frente al periodo anterior, indicando que un nuevo despliegue degradó la estabilidad de la aplicación.

## 8. Detección Inteligente de Causa Raíz: Davis AI y Dynatrace Assist

**Carlos Fernández:**

### Módulo de Problemas (*Problems*) y Motor Davis AI

Cuando ocurre una falla, Dynatrace no bombardea a los ingenieros con alertas inconexas; utiliza su motor de inteligencia artificial determinística y causal llamado **Davis AI**:

* Analiza automáticamente el grafo de dependencias de la arquitectura (topología Smartscape).

* Correlaciona eventos, métricas de infraestructura, tiempos de respuesta y errores de código.

* **Aislamiento de la Causa Raíz (*Root Cause*):** Davis marca directamente con una etiqueta roja cuál fue el servicio o la base de datos culpable que originó el problema en cascada (por ejemplo: un aumento de excepciones de JavaScript provocado por un fallo 500 en el backend de pagos).

### Demostración de Dynatrace Assist (Asistente Conversacional)

Carlos ejecutó una consulta en lenguaje natural dentro del asistente:

> *"Muéstrame una lista de todos los eventos, fallas de solicitud, problemas, entidades afectadas y dependencias entre las 6:00 AM y las 8:00 AM para investigar y determinar la causa raíz."*

En vivo, Dynatrace Assist orquestó múltiples agentes internos, evaluó las trazas y métricas del periodo especificado y devolvió un resumen estructurado indicando los problemas activos, la cronología del incidente y los componentes que sufrieron indisponibilidad.

## 9. Acceso al Dynatrace Playground y Certificación Dynatrace Essentials

**Carlos Fernández:**

Para que todos los estudiantes practiquen antes del hackatón, pueden seguir los pasos descritos en la guía oficial:

1. **Ingreso al Tenant de Playground:** Acceder al enlace del **Dynatrace Playground** compartido en los recursos oficiales para explorar entornos precargados con datos sintéticos y aplicaciones simuladas.

2. **Registro en Dynatrace University:** Al pie de la página del Playground encontrarán el acceso a **Dynatrace University**. Deben utilizar las mismas credenciales creadas para el playground.

3. **Curso y Certificación "Dynatrace Essentials":** Es un entrenamiento gratuito en video con evaluaciones por módulos. Al completarlo satisfactoriamente, el sistema les emitirá su **Certificación de Dynatrace Essentials**.

4. **Interactividad desde la Documentación:** En la documentación técnica oficial de DQL (por ejemplo, al consultar comandos como `fetch` o `fields`), encontrarán el botón interactivo **"Run in Playground"**, el cual abre la consulta directamente en el playground para ver los resultados en tiempo real.

## 10. Sesión de Preguntas y Respuestas (Q&A)

**Participante:**
*¿Durante el hackatón utilizaremos el entorno de Playground?*

**Ángel Alemán:**
No. Durante el hackatón no se utilizará el playground general. Se les invitará a un **tenant exclusivo y privado del hackatón**, donde estará corriendo una aplicación desarrollada específicamente por nuestro equipo para generar el tráfico, los problemas y las preguntas del reto.

**Participante:**
*¿Estará permitido el uso de herramientas de Inteligencia Artificial externas durante la competencia?*

**Ángel Alemán:**
Estará prohibido el uso de IAs generativas externas durante la competencia para preservar el valor de los desafíos. Además, técnicamente no les sería de utilidad: las preguntas evaluarán datos, métricas y problemas que ocurren en tiempo real dentro del tenant privado creado para el evento, por lo que una IA externa carecerá por completo de contexto.

**Adrián pregunta:**
*Si pudieran resumir la definición del uso de Dynatrace, ¿cómo la definirían?*

**Ángel Alemán:**
Dynatrace es la plataforma que permite observar todo lo que sucede dentro de tu stack tecnológico para minimizar el tiempo de afectación e indisponibilidad ante cualquier eventualidad.

**Carlos Fernández:**
Es el panel de control integral que ofrece visibilidad de punta a punta: desde la infraestructura y los servidores hasta la experiencia de los clientes finales, garantizando que los servicios de una empresa permanezcan operativos la mayor cantidad de tiempo posible.

**Participante:**
*¿Las trazas se utilizan exclusivamente para arquitecturas de microservicios?*

**Carlos Fernández:**
No. Cualquier aplicación que interactúe con otro componente (como una aplicación web tradicional conectándose a un servidor de base de datos) genera una traza transaccional. No está limitado a microservicios.

**Participante:**
*¿Dynatrace es una herramienta de pago? ¿Cómo se compara con Grafana?*

**Carlos Fernández:**
Sí, Dynatrace es una solución empresarial licenciada. Frente a Grafana: Grafana es una excelente herramienta para métricas específicas, pero Dynatrace ofrece una solución unificada nativa que integra logs, métricas, trazas, seguridad en runtime, experiencia de usuario y un motor causal de inteligencia artificial (Davis) sin requerir configuraciones de múltiples plugins.

**Participante:**
*Si una aplicación comienza a responder con lentitud, ¿cuál es el proceso recomendado para investigarlo en Dynatrace?*

**Carlos Fernández:**
Dynatrace detecta degradaciones de rendimiento antes de que la aplicación colapse. Se ingresa al módulo de **Problemas**, se revisa la alerta de lentitud identificada por Davis AI, se inspecciona el mapa topológico para determinar cuál componente introdujo la latencia y se profundiza en las trazas y excepciones asociadas.

**Participante:**
*¿Davis AI estará disponible durante el hackatón?*

**Carlos Fernández:**
El motor de inteligencia artificial Davis AI que analiza la topología y agrupa los problemas **sí estará activo** dentro de la plataforma. Lo que estará desactivado es el asistente conversacional de chat (Assist) para que los participantes realicen el análisis mediante su propia navegación y consultas DQL.

**Participante:**
*¿Se utiliza Dynatrace en etapas de desarrollo antes de llegar a producción?*

**Ángel Alemán:**
Absolutamente. Muchos equipos observan sus entornos de desarrollo, pruebas (QA) y preproducción para detectar regresiones de rendimiento, problemas de arquitectura o llamadas duplicadas antes de desplegar a producción.

**Participante:**
*¿La evaluación del hackatón requerirá el uso de DQL?*

**Ángel Alemán:**
Sí, el reto de Dynatrace contemplará secciones donde deberán escribir consultas en **DQL** para extraer registros específicos, así como identificar nombres de servicios, causas raíz y métricas de infraestructura dentro de la plataforma.

## 11. Cierre de la Sesión y Próximos Pasos

**Luis Martínez:**

Queremos agradecer a Carlos Fernández y a Ángel Alemán por esta excelente y detallada presentación.

**Avisos y recordatorios finales:**

1. **Grabaciones oficiales:** Esta grabación y los cinco workshops anteriores están disponibles en nuestro canal de YouTube oficial: **Hakaton CM**.

2. **Formulario de asistencia:** Les recordamos llenar el enlace compartido en el chat para certificar su presencia.

3. **Inicio Hackatón Sede Santiago:** Se llevará a cabo el **8 y 9 de octubre en el Hotel Gran David** (Santiago de Veraguas) para los estudiantes de provincias centrales.

4. **Documentación y enlaces:** Revisen la sección de recursos en **hacatoncopa.com** para descargar la guía de Dynatrace Playground y completar el curso *Dynatrace Essentials*.

**Ángel Alemán y Carlos Fernández:**
Muchísimas gracias a todos por su participación y entusiasmo. Les deseamos el mayor de los éxitos en la competencia. ¡Nos vemos en el Hackatón!

**Luis Martínez:**
¡Muchas gracias a todos y que tengan una excelente noche!