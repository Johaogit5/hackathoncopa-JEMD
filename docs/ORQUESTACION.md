# Orquestación de agentes

Política para decidir qué agente lanzar, con qué modo y cómo dejar rastro. Adaptada del skill `enrutar-agentes` del proyecto gestion-escolar-saas (ADR-0008 a 0011 de ese proyecto no se tuvieron a la vista: los valores por defecto de modelo y esfuerzo se heredaron del skill y se verificaron el 2026-10-31 con `agy models` y `~/.codex/config.toml`; no se midieron con tareas reales del reto).

Se aplica junto con el framework de `CLAUDE.md`: Detectar → Observar → Analizar → Hipótesis → Corregir → Validar.

## Principios

- Un agente **investiga o propone**; nada entra al repo sin evidencia (log, código HTTP, excepción, línea de `main.tf`) y sin pasar por Validar.
- Lo que devuelve un agente externo es un **borrador**. Se comprueba con el endpoint, el log o la query antes de registrarlo como hecho.
- Nada de `--dangerously-skip-permissions`, `--yolo` ni `--dangerously-bypass-approvals-and-sandbox`.
- No se relaja la seguridad para que un agente "avance" (nada de CORS `"*"` ni validaciones desactivadas).

## Preguntas, en orden (la primera que aplica gana)

1. **¿Toca secretos, connection strings, claves de Application Insights, `terraform.tfstate`, credenciales de Azure/Dynatrace o decide arquitectura?** → La hace **Claude** (sesión principal). Ningún agente externo lo recibe.
2. **¿La herramienta no está disponible o agotó su cuota?** → Reasignar a Claude y anotar el fallo en el registro.
3. **¿Depende de otra tarea en marcha o toca los mismos archivos?** → Un solo agente. Las fallas en cascada (`/health` → `/health/deep` → `/api/status`) se analizan en orden, no en paralelo.
4. **Según el tipo de tarea:**

| Tipo | Agente | Modo |
|---|---|---|
| Búsqueda de solo lectura en el repo | Claude, subagente `Explore` | solo lectura |
| Investigación externa (docs de Azure, KQL, DQL) | `agy` | `--mode plan` |
| Revisión de archivos nombrados, sin secretos | `agy` | `--mode plan` |
| Implementación acotada con comprobación clara | `codex` o subagente de Claude | worktree propio, escritura |
| Verificación del resultado | Claude | solo lectura |
| Diagnóstico y decisión de causa raíz | Claude, sesión principal | — |

5. **Modelo y esfuerzo.** Valores iniciales heredados (subir solo si falla y anotarlo):

| Herramienta | Por defecto | Subir cuando |
|---|---|---|
| `codex` | `gpt-6.1-sol` en `medium` (config local en `~/.codex/config.toml`, sin flags) | tareas simples: `-c model_reasoning_effort=low`; si falla el primer intento, `high` solo con aprobación del equipo |
| `agy` | `gemini-3.8-flash-medium` | síntesis difícil o falla: `gemini-4-argon-medium`; segunda opinión de otra familia: `claude-opus-5-5-medium` |

Modelos disponibles en `agy models`: `gemini-4-argon-{low,medium,high}`, `gemini-3.8-flash-{low,medium,high}`, `claude-opus-5-5-{low,medium,high}`, `claude-sonnet-5-5-{low,medium,high}`, `gpt-oss-120b-medium`. Haiku no está en la lista, así que la exploración de solo lectura la hace el subagente `Explore` de Claude Code.

## Salida: una línea de decisión

`agente · modelo · esfuerzo · modo · razón (pregunta que decidió)`

## Plantilla de encargo

Un agente externo solo sabe lo que está en el encargo. Incluye: **objetivo**, **entradas** (archivos o fuentes), **salida esperada**, **restricciones** y **cómo se comprobará**. Nunca incluye secretos.

`agy` (solo lectura):
```
agy -p "<TAREA>. <ENTRADAS>. Responde en español, máximo <N> líneas y cita la fuente de cada dato. No ejecutes comandos de terminal: solo lee archivos o busca en la web si hace falta. No modifiques archivos." --mode plan --model gemini-3.8-flash-medium --output-format json
```
La frase «No ejecutes comandos de terminal» es obligatoria (sin ella la respuesta puede venir vacía).

`codex` (implementación acotada):
```
codex exec -s workspace-write -C <worktree> --ephemeral --json -o <archivo> "<TAREA>. <ESPECIFICACION>. Comprueba con <comando> al terminar. No toques otros archivos."
```

## Topes y aislamiento

- Máximo 2 a 3 agentes de escritura a la vez; los de solo lectura no tienen tope.
- Quien escribe trabaja en su propio worktree, nunca en `main` ni en los archivos de otro agente.
- **Un cambio por commit**, con mensaje de síntoma, causa y corrección.

## Después de cada delegación

1. Añadir una fila a `docs/registro-delegaciones.md`.
2. Si produjo una hipótesis, registrarla en `docs/BITACORA.md` (también si se refuta).
3. Validar contra el endpoint o el log antes de dar por bueno el resultado.

## Reparto sugerido entre los 4 integrantes

Cada integrante dirige uno de estos frentes y sus agentes; así no se investiga lo mismo.

| Frente | Qué mira | Evidencia típica |
|---|---|---|
| Infra (Terraform/Azure) | `main.tf`, variables de entorno, URLs | línea de `main.tf`, App Settings |
| Backend | `/health`, `/health/deep`, `/api/status`, excepciones | código HTTP, stack trace |
| Frontend y CORS | URL del API incrustada al compilar, CORS | Console, cabeceras de respuesta |
| Observabilidad | KQL (App Insights) y DQL (Dynatrace) | query y resultado |

## Anti-patrones → PARAR

| Si estás a punto de… | Realidad |
|---|---|
| Mandar a un externo algo con secretos "porque es un borrador" | Pregunta 1: no sale de Claude |
| Lanzar agentes en paralelo sobre la cascada de `/health` | Pregunta 3: uno solo, en orden |
| Dar por bueno lo que devolvió `codex` o `agy` | Es un borrador: validar primero |
| Aplicar un cambio sin evidencia | Se registra la hipótesis y se observa antes |
| Saltarte el registro porque la tarea fue pequeña | Sin filas no hay datos para ajustar esta política |
| Seguir lanzando agentes tras un error de límite | Pregunta 2: reasignar y anotar |
