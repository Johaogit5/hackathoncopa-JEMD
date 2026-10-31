# Hackathon COPA 2026: equipo JEMD

Repositorio del equipo (4 integrantes) para la Hackathon COPA 2026. Tracks: Microsoft (Azure App Service, Terraform, Azure Monitor / Application Insights, KQL) y Dynatrace (observabilidad, DQL).

## Antes de trabajar en el reto

0. **Sesión nueva: leer primero `docs/PROGRESO.md`.** Ahí está el estado actual (TeamId 35, errores E1–E6, consultas de Dynatrace y pendientes). Las respuestas del CTF están solo en local, en `docs/CTF-RESPUESTAS.local.md`.
1. Leer `docs/README.md` y, sobre todo, `docs/contexto/02-charla2-operacion-bajo-presion.md`: describe el tipo de fallas que plantea el reto (API 500, variable de entorno, URL mal configurada, error de integración, CORS, errores en cascada en `/health` → `/health/deep` → `/api/status`).
2. Guías destiladas: `docs/guias/reto-azure.md`, `docs/guias/reto-dynatrace.md` y `docs/guias/reglas-y-logistica.md`.
3. Detalle: `docs/workshops/` (transcripciones W2–W6 con puntos clave; en `subtitulos/` están los subtítulos para buscar con grep) y `docs/contexto/` (PDFs oficiales convertidos).

**Importante:** según W6, durante la competencia no se permite IA generativa externa. **Actualización: el equipo confirmó que la organización sí permite usar IA.** Aun así, las respuestas del CTF las envía siempre el usuario, y Claude nunca ingresa credenciales.

## Cómo trabajar

- Seguir el framework: **Detectar → Observar → Analizar → Hipótesis → Corregir → Validar**. Si la validación falla, volver a Observar.
- Toda hipótesis se apoya en evidencia concreta (log, código HTTP, excepción, línea de `main.tf`). Nada de cambios "a ver si funciona".
- **Un cambio por commit**, con un mensaje que diga síntoma, causa y corrección.
- Registrar cada hipótesis en `docs/BITACORA.md` (también las refutadas) y anotar ahí las respuestas para CTFd.
- No relajar la seguridad para salir del paso: nada de CORS `"*"` ni de desactivar validaciones.
- La URL del API en el frontend se incrusta al compilar: si cambia, hay que reconstruir y redesplegar el frontend.
- Proponer el reparto de tareas entre los 4 integrantes para que no investiguen lo mismo (roles sugeridos en `docs/README.md`).
