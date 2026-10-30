# Conceptos Clave de Observabilidad en Dynatrace

> Fuente: `Conceptos Clave de Observabilidad en Dynatrace.pdf` (Dynatrace · Hackathon COPA).

---

## ¿Qué es la Observabilidad?

Es la capacidad de entender el **estado interno** de un sistema a partir de los datos que genera hacia el exterior. En Dynatrace va más allá del monitoreo tradicional: no solo dice **qué** está fallando, sino **por qué** y **dónde** está el origen.

Dynatrace recolecta automáticamente las tres señales fundamentales:

- **Métricas:** rendimiento y salud del sistema (CPU, latencia, tasa de errores).
- **Logs:** registros de eventos y errores de aplicaciones e infraestructura.
- **Trazas (Traces):** recorrido de cada transacción a través de los servicios.

Todo potenciado por **Dynatrace Intelligence**, el motor de IA de Dynatrace, que correlaciona automáticamente estos datos y detecta la **causa raíz**.

## ¿Qué es DQL? (Dynatrace Query Language)

Lenguaje de consulta nativo de Dynatrace para explorar, analizar y visualizar en tiempo real todos los datos de observabilidad.

Con DQL puedes:
- Consultar métricas, logs, trazas y eventos desde una única interfaz.
- Construir dashboards y análisis personalizados.
- Correlacionar datos de distintas fuentes con una sintaxis sencilla.

DQL forma parte de **Grail**, la plataforma de datos de Dynatrace que unifica toda la información en un *lakehouse* escalable.

> Sintaxis y ejemplos de DQL en [04-dynatrace-sesion1-observabilidad-inteligente.md](04-dynatrace-sesion1-observabilidad-inteligente.md#bloque-4--logs-y-dql).

## ¿Qué es la Observabilidad End-to-End?

Visibilidad completa del recorrido de una transacción o experiencia de usuario, **desde el primer clic en el navegador hasta la base de datos**, pasando por cada microservicio, API, contenedor y capa de infraestructura.

| Capa | Qué monitorea |
|---|---|
| **Frontend (RUM)** | Experiencia real del usuario en browser/móvil |
| **Aplicación (APM)** | Rendimiento de servicios, APIs y microservicios |
| **Infraestructura** | Hosts, Kubernetes, contenedores, nubes |
| **Red y Base de Datos** | Consultas lentas, cuellos de botella de red |

El resultado es un **mapa de servicio automático (Smartscape)** que muestra cómo se relacionan todos los componentes y facilita ver el impacto real de cualquier incidente.

## ¿Por qué es importante para las empresas hoy?

Las arquitecturas son cada vez más complejas (microservicios, multicloud, Kubernetes, APIs de terceros). En ese contexto:

1. **Reducción del tiempo de resolución (MTTR):** resolver incidentes en minutos, no horas, minimiza el impacto económico y reputacional.
2. **Experiencia del cliente:** un segundo de latencia extra puede significar ventas perdidas. La observabilidad conecta la tecnología con el negocio.
3. **Confiabilidad y SLAs:** cumplir acuerdos de nivel de servicio con datos precisos, de forma proactiva.
4. **Eficiencia operativa:** DevOps y SRE dejan de "apagar incendios" y se enfocan en innovar.
5. **Transformación digital segura:** cada despliegue se valida en tiempo real, acelerando la entrega continua sin perder estabilidad.

> **En resumen:** la observabilidad no es solo una herramienta técnica, es una ventaja competitiva.

---

## Cápsula informativa: tres funcionalidades clave

> ℹ️ Las tres funcionan juntas sobre el **mismo agente de Dynatrace (OneAgent)**: no hay que instalar nada extra.

### LiveDebugger: depuración en vivo, sin interrupciones

Permite inspeccionar qué pasa dentro de una aplicación **en tiempo real**, con usuarios reales dentro, **sin apagarla** y sin que nadie lo note.

> 💡 Como un médico que te hace una ecografía sin necesidad de operarte.

### AppSec: seguridad de aplicaciones, protección activa

Detecta amenazas y vulnerabilidades **mientras la aplicación está funcionando**. No solo revisa el código antes de producción: **vigila en tiempo real** si alguien intenta atacar el sistema.

> 💡 Como un guardia que no solo revisa la puerta, sino que patrulla todo el edificio constantemente.

### Business Events: eventos de negocio, impacto real

Conecta lo técnico con lo que le importa al negocio. Rastrea eventos como **compras, registros o pagos** y los cruza con datos de rendimiento, para saber si un problema técnico **está afectando las ventas o a los usuarios**.

> 💡 Como ver en el mismo tablero cuándo se cae el servidor y cuántas ventas se perdieron en ese momento.
