# Hackathon COPA 2026: equipo JEMD

Repositorio del equipo (4 integrantes) para la Hackathon COPA 2026. Tracks: Microsoft (Azure App Service, Terraform, Azure Monitor / Application Insights, KQL) y Dynatrace (observabilidad, DQL).

## Antes de trabajar en el reto

1. Leer `docs/README.md` y, sobre todo, `docs/contexto/02-charla2-operacion-bajo-presion.md`: describe el tipo de fallas que plantea el reto (API 500, variable de entorno, URL mal configurada, error de integración, CORS, errores en cascada en `/health` → `/health/deep` → `/api/status`).
2. Guías destiladas: `docs/guias/reto-azure.md`, `docs/guias/reto-dynatrace.md` y `docs/guias/reglas-y-logistica.md`.
3. Detalle: `docs/workshops/` (transcripciones W2–W6 con puntos clave; en `subtitulos/` están los subtítulos para buscar con grep) y `docs/contexto/` (PDFs oficiales convertidos).

**Importante:** según W6, durante la competencia no se permite IA generativa externa. Claude sirve para preparar y estudiar, no para resolver el reto en vivo, salvo que la organización indique otra cosa.

## Cómo trabajar

- Seguir el framework: **Detectar → Observar → Analizar → Hipótesis → Corregir → Validar**. Si la validación falla, volver a Observar.
- Toda hipótesis se apoya en evidencia concreta (log, código HTTP, excepción, línea de `main.tf`). Nada de cambios "a ver si funciona".
- **Un cambio por commit**, con un mensaje que diga síntoma, causa y corrección.
- Registrar cada hipótesis en `docs/BITACORA.md` (también las refutadas) y anotar ahí las respuestas para CTFd.
- No relajar la seguridad para salir del paso: nada de CORS `"*"` ni de desactivar validaciones.
- La URL del API en el frontend se incrusta al compilar: si cambia, hay que reconstruir y redesplegar el frontend.
- Proponer el reparto de tareas entre los 4 integrantes para que no investiguen lo mismo (roles sugeridos en `docs/README.md`).
