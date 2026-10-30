# Cápsulas informativas — Track Microsoft (Business IT)

> Fuente: `Capsulas Informativas - Microsoft.pdf` (Business IT, Hackathon COPA 2026).
> Objetivo del material: dar el conocimiento base, de forma progresiva, de los temas que se tratarán en los retos.

**Ruta de aprendizaje:**
Cloud → Aplicaciones → DevOps → IaC → Observabilidad → Logs → KQL → Diagnóstico → Corrección

## Índice

1. [¿Qué es Microsoft Azure?](#cápsula-1--qué-es-microsoft-azure)
2. [¿Qué es un Resource Group?](#cápsula-2--qué-es-un-resource-group)
3. [¿Qué es Azure App Service?](#cápsula-3--qué-es-azure-app-service)
4. [¿Qué es una API?](#cápsula-4--qué-es-una-api)
5. [¿Qué es una Arquitectura Frontend y Backend?](#cápsula-5--qué-es-una-arquitectura-frontend-y-backend)
6. [¿Qué es DevOps?](#cápsula-6--qué-es-devops)
7. [¿Qué es Infraestructura como Código (IaC)?](#cápsula-7--qué-es-infraestructura-como-código-iac)
8. [¿Qué es Terraform?](#cápsula-8--qué-es-terraform)
9. [¿Qué es la Observabilidad?](#cápsula-9--qué-es-la-observabilidad)
10. [¿Qué son los Logs?](#cápsula-10--qué-son-los-logs)
11. [¿Qué es KQL?](#cápsula-11--qué-es-kql-kusto-query-language)
12. [¿Qué es Application Insights?](#cápsula-12--qué-es-application-insights)
13. [¿Qué son los Logs, las Excepciones y los Errores 500?](#cápsula-13--qué-son-los-logs-las-excepciones-y-los-errores-500)
14. [¿Qué es CORS y por qué existe?](#cápsula-14--qué-es-cors-y-por-qué-existe)
15. [¿Qué es un Health Check?](#cápsula-15--qué-es-un-health-check)
16. [¿Qué son las Variables de Configuración?](#cápsula-16--qué-son-las-variables-de-configuración)
17. [Pensamiento de Diagnóstico Tecnológico](#cápsula-17--pensamiento-de-diagnóstico-tecnológico)

---

## Cápsula 1 — ¿Qué es Microsoft Azure?

**Concepto.** Azure es la plataforma de nube de Microsoft. Permite crear, desplegar y administrar aplicaciones sin comprar ni mantener servidores físicos. Ofrece cómputo, almacenamiento, redes, IA, analítica y seguridad a través de Internet.

**¿Por qué importa?** Miles de organizaciones lo usan para aplicaciones críticas, datos y servicios digitales. Las soluciones crecen o se reducen según la necesidad del negocio.

**Recordar**
- Azure es una plataforma cloud.
- Los recursos se consumen bajo demanda.
- Permite innovar sin administrar infraestructura física.

## Cápsula 2 — ¿Qué es un Resource Group?

**Concepto.** Contenedor lógico que organiza y administra recursos relacionados en Azure. Todos los componentes de una solución pueden vivir en el mismo Resource Group.

**¿Por qué importa?** Permite organizar recursos, aplicar permisos, controlar costos y simplificar el mantenimiento.

**Recordar**
- Agrupa recursos relacionados.
- Facilita la administración.
- Mantiene ordenados los entornos.

## Cápsula 3 — ¿Qué es Azure App Service?

**Concepto.** Plataforma administrada para ejecutar aplicaciones web, sitios y APIs sin gestionar servidores ni sistemas operativos.

**¿Por qué importa?** Publica aplicaciones de forma rápida, segura y escalable; Azure se encarga de la infraestructura.

**Recordar**
- Publica aplicaciones web.
- Soporta múltiples lenguajes.
- Reduce la complejidad operativa.

## Cápsula 4 — ¿Qué es una API?

**Concepto.** *Application Programming Interface*: conjunto de reglas que permite que distintas aplicaciones intercambien información y trabajen juntas.

**¿Por qué importa?** Son la base de las aplicaciones modernas: conectan sistemas, comparten datos y automatizan procesos (apps de vuelos, banca digital, e-commerce, redes sociales).

**Recordar**
- Las APIs conectan sistemas.
- Comparten datos de forma controlada.
- Son fundamentales en arquitecturas modernas.

## Cápsula 5 — ¿Qué es una Arquitectura Frontend y Backend?

**Frontend:** la interfaz con la que interactúa el usuario (web, móvil, portal). Muestra información y captura acciones.

**Backend:** procesa la información y ejecuta la lógica de negocio. Recibe solicitudes, consulta datos, aplica reglas y responde.

**¿Cómo trabajan juntos?** El frontend envía una solicitud al backend a través de una API:

```
Usuario
  │
  ▼
Frontend
  │
  ▼
API
  │
  ▼
Backend
  │
  ▼
Respuesta
```

**¿Por qué importa?** Cada parte se desarrolla, mantiene y escala de forma independiente.

**Recordar**
- El frontend es lo que ve el usuario.
- El backend procesa la información.
- Se comunican mediante APIs.
- **Si uno falla, toda la experiencia puede verse afectada.**

## Cápsula 6 — ¿Qué es DevOps?

**Concepto.** Cultura de trabajo que integra desarrollo y operaciones para entregar soluciones de calidad más rápido.

**¿Por qué importa?** Reduce errores, mejora la colaboración y automatiza despliegue y monitoreo.

**Ciclo típico:** Planificar → Desarrollar → Probar → Desplegar → Monitorear → Mejorar

**Recordar**
- DevOps es una metodología.
- Promueve automatización y colaboración.
- Busca entregas rápidas y confiables.

## Cápsula 7 — ¿Qué es Infraestructura como Código (IaC)?

**Concepto.** Definir la infraestructura mediante archivos de configuración escritos en código.

**¿Por qué importa?** Despliegues repetibles, menos errores humanos, entornos completos en minutos.

**Recordar**
- La infraestructura se crea automáticamente.
- Los cambios quedan documentados.
- Mejora la consistencia entre entornos.

## Cápsula 8 — ¿Qué es Terraform?

**Concepto.** Herramienta para implementar IaC. Crea y administra recursos mediante archivos declarativos (p. ej. `main.tf`).

**¿Por qué importa?** Muy usado para automatizar despliegues cloud con consistencia y repetibilidad.

**Recordar**
- Automatiza la creación de infraestructura.
- Reduce tareas manuales.
- Facilita la gestión de entornos complejos.

## Cápsula 9 — ¿Qué es la Observabilidad?

**Concepto.** Capacidad de comprender el comportamiento interno de un sistema analizando la información que genera.

**Los tres pilares**
- **Logs:** registros de eventos y actividades.
- **Métricas:** indicadores numéricos de desempeño.
- **Trazas:** seguimiento completo del recorrido de una solicitud dentro del sistema.

**¿Por qué importa?** Detectar problemas, entender comportamientos inesperados y decidir con evidencia.

**Recordar**
- No se puede mejorar lo que no se puede medir.
- La observabilidad ayuda a encontrar causas raíz.

## Cápsula 10 — ¿Qué son los Logs?

**Concepto.** Registro automático generado por una aplicación, servicio o sistema mientras funciona. Dice **qué** ocurrió, **cuándo** y con **qué resultado**.

**Pueden contener:** solicitudes recibidas, usuarios autenticados, procesos ejecutados, errores, eventos de seguridad, integraciones con otros sistemas.

**Ejemplo**

```
10:15:01 - Solicitud recibida
10:15:02 - Consulta procesada
10:15:02 - Respuesta enviada
```

**¿Por qué importan?** Son la principal fuente de evidencia al investigar un problema; los equipos suelen empezar por ahí.

**Recordar**
- Los logs cuentan la historia de lo que ocurrió.
- Permiten investigar incidentes.
- Son de las herramientas más usadas en operaciones y soporte.
- Una aplicación sin logs es difícil de diagnosticar.

## Cápsula 11 — ¿Qué es KQL (Kusto Query Language)?

**Concepto.** Lenguaje de consultas de Microsoft para analizar datos en **Azure Monitor, Log Analytics y Application Insights**. Sirve para encontrar información específica en grandes volúmenes de datos de monitoreo.

**KQL permite:** buscar errores, filtrar, identificar tendencias, analizar excepciones, revisar solicitudes e investigar incidentes.

**¿Cómo funciona?** Consulta tablas con los datos recopilados por Azure: `requests`, `exceptions`, `traces`, `dependencies`, `metrics`.

**Ejemplos básicos**

```kql
// Últimas solicitudes registradas
requests
| take 10
```

```kql
// Excepciones registradas
exceptions
| take 10
```

**Conceptos básicos**

| Concepto | Para qué sirve | Ejemplo |
|---|---|---|
| Tabla | Donde se almacenan los datos | `requests`, `exceptions`, `traces`, `dependencies` |
| Filtrar | Mostrar solo lo que interesa | `exceptions \| where severityLevel >= 3` |
| Ordenar | Ver primero lo más reciente | `exceptions \| order by timestamp desc` |
| Contar | Saber cuántos eventos ocurrieron | `requests \| count` |

**Recordar**
- KQL es el lenguaje para consultar datos de monitoreo en Azure.
- Analiza logs, excepciones y métricas.
- Es fundamental para observabilidad y diagnóstico.
- **No modifica datos; solo los consulta.**

> Ver consultas más avanzadas en [02-charla2-operacion-bajo-presion.md](02-charla2-operacion-bajo-presion.md#kql-de-bolsillo-4-consultas-que-resuelven-el-80-de-los-casos).

## Cápsula 12 — ¿Qué es Application Insights?

**Concepto.** Funcionalidad de Azure Monitor para supervisar aplicaciones y analizar su comportamiento en tiempo real.

**Proporciona:** rendimiento, excepciones, disponibilidad, solicitudes, dependencias, trazas.

**Recordar**
- Convierte datos técnicos en información útil.
- Facilita el diagnóstico.
- Da visibilidad sobre el estado de las aplicaciones.

## Cápsula 13 — ¿Qué son los Logs, las Excepciones y los Errores 500?

**Log:** registro de eventos (inicio de procesos, acciones, resultados, advertencias, errores). Ayuda a reconstruir lo que pasó.

```
Usuario autenticado correctamente.
Solicitud recibida.
Proceso completado.
```

**Excepción:** situación inesperada que impide continuar normalmente. Por ejemplo, un dato requerido no existe, una configuración es incorrecta o un servicio externo no responde. Suelen registrarse automáticamente.

```
Archivo no encontrado.
Variable de configuración inexistente.
Conexión no disponible.
```

**Error 500 (`Internal Server Error`):** ocurrió un problema dentro del servidor al procesar la solicitud.
- **No** significa necesariamente que el servidor esté apagado.
- Con frecuencia indica una **excepción** o una **configuración incorrecta**.

**Recordar**
- Los logs cuentan la historia.
- Las excepciones indican problemas específicos.
- Un 500 señala un fallo interno en la aplicación.
- Analizar evidencias es una habilidad clave.

## Cápsula 14 — ¿Qué es CORS y por qué existe?

**Concepto.** *Cross-Origin Resource Sharing*: mecanismo de seguridad de los **navegadores** que controla qué aplicaciones pueden comunicarse entre sí.

**Origen (Origin) = protocolo + dominio + puerto.**
`https://miapp.com` es un origen **distinto** de `https://api.miapp.com`, aunque sean de la misma organización.

**¿Por qué existe?** Sin él, cualquier sitio podría acceder a datos de otras aplicaciones sin autorización. Antes de responder, el navegador verifica si la comunicación está permitida; si no, la bloquea.

**Recordar**
- CORS es una medida de seguridad.
- Controla quién accede a una aplicación **desde el navegador**.
- **Un sistema puede funcionar bien y aun así ser bloqueado por CORS.**
- Es común encontrar configuraciones CORS en APIs web.

## Cápsula 15 — ¿Qué es un Health Check?

**Concepto.** Mecanismo para verificar automáticamente si una aplicación funciona correctamente (p. ej. un endpoint `/health`).

**Recordar**
- **Una aplicación puede estar encendida y no funcionar correctamente.**
- Los Health Checks detectan problemas rápido.
- Son práctica común en sistemas modernos.

## Cápsula 16 — ¿Qué son las Variables de Configuración?

**Concepto.** Parámetros que definen el comportamiento de una app sin modificar el código: URLs de servicios, credenciales, parámetros de integración, configuraciones de seguridad.

**¿Por qué importa?** Adaptan la app a distintos entornos: desarrollo, pruebas, producción.

**Recordar**
- Separan la configuración del código.
- Facilitan los despliegues.
- Son esenciales para operar aplicaciones modernas.

## Cápsula 17 — Pensamiento de Diagnóstico Tecnológico

**Concepto.** Resolver problemas con evidencia y datos, siguiendo un proceso estructurado:

```
Observar → Investigar → Analizar → Corregir → Validar
```

**¿Por qué importa?** Encuentra la causa real y evita cambios innecesarios o incorrectos.

**Recordar**
- Los datos ayudan a tomar mejores decisiones.
- La observación es tan importante como la corrección.
- Comprender el problema es el primer paso para resolverlo.
