# Transcripción Oficial — Workshop #5: Troubleshooting, Health Checks, CORS, Variables de Configuración y Diagnóstico

> **Video:** https://www.youtube.com/watch?v=siqZ_bTbmk8 · Transcripción oficial entregada por la organización. Los subtítulos automáticos (para buscar frases textuales) están en [`subtitulos/w5.txt`](subtitulos/w5.txt).

## ⚡ Puntos clave para el reto

- Método: **Síntoma → Evidencia → Hipótesis → Validación → Corrección**. Si la validación falla, volver a Evidencia.
- Variables en 3 niveles: App Settings, pipeline CI/CD y archivos locales (`appsettings.json`/`.env`). Las que quedan **quemadas en el build** no cambian al editar App Settings.
- Key Vault: `@Microsoft.KeyVault(SecretUri=...)` en App Settings.
- CORS: origen = protocolo + dominio + puerto. El navegador hace un preflight `OPTIONS`. Nunca usar `*` en producción.
- `/health` (liveness → reinicio) vs. `/health/deep` (readiness → sale del balanceador). Estados: Healthy, Degraded y Unhealthy.
- Kudu: `https://<app>.scm.azurewebsites.net` → `/home/site/wwwroot`. DevTools: Network, **Preserve log** y exportar HAR.

---

**Evento:** Hackatón Copa 2026 — API, DevOps y Observabilidad

**Expositor Principal:** Daniel Vacas (Business IT)

**Moderadores y Presentadores:** Luis Martínez, Larisa Aguilar, Hugo Aquino

## 1. Bienvenida e Introducción al Workshop #5

**Daniel Vacas:**

Perfecto. Muchas gracias a todos, bienvenidos nuevamente. En sesiones previas habíamos visto conceptos sobre la nube, desarrollo, aplicaciones modernas, observabilidad e infraestructura como código con DevOps y Terraform.

El día de hoy vamos a ver cómo razonamos para llegar a solucionar aquellos inconvenientes que a veces son evidentes, pero en otros casos requieren mayor intuición técnica dependiendo de cómo levantamos un proyecto.

La sesión de hoy se titula **Troubleshooting, Health Checks, CORS y Configuraciones**. Abordaremos cinco pilares esenciales:

1. **Diagnóstico estructurado de errores:** Síntomas, evidencia, hipótesis y validación.
2. **Variables de configuración:** Gestión en Application Settings, código y baúles de secretos.
3. **CORS y seguridad:** Bloqueos por origen, métodos permitidos y capas intermedias.
4. **Health Checks:** Monitoreo de liveness y readiness para asegurar la continuidad operativa.
5. **Errores HTTP y causa raíz:** Interpretación de códigos de estado y análisis de trazas de pila (*stack traces*).

## 2. Diagnóstico Estructurado de Errores: El Método de los Cinco Pasos

**Daniel Vacas:**

Para resolver incidentes de forma profesional no debemos actuar a ciegas. Seguimos una metodología rigurosa basada en cinco pasos:

```
┌─────────────┐     ┌──────────────┐     ┌───────────────┐     ┌────────────────┐     ┌───────────────┐
│ 1. Síntoma  │ ──> │ 2. Evidencia │ ──> │ 3. Hipótesis  │ ──> │ 4. Validación  │ ──> │ 5. Corrección │
└─────────────┘     └──────────────┘     └───────────────┘     └────────────────┘     └───────────────┘
```

### Paso 1: Síntoma (Las 5 Ws)
Identificar qué ocurre exactamente:
* **¿Qué** mensaje de error aparece?
* **¿Cuándo** comenzó a fallar? (La marca temporal es crucial).
* **¿A quién** afecta? (¿A todos los usuarios, a un solo cliente o a una región geográfica?).
* **¿Dónde** se manifiesta? (¿En el frontend, en el API gateway o en el backend?).
* **¿Por dónde** viaja la solicitud?

> *Nota:* Muchas veces el problema parece ser del software, pero termina siendo de "capa 8" (usuario final) o de conectividad local del cliente.

### Paso 2: Evidencia
Recopilación de hechos concretos mediante:
* Registros históricos en **Logs** (Log Analytics / Application Insights).
* Salida en tiempo real desde la **Consola**.
* Códigos de estado **HTTP/HTTPS**.
* Tablas y registros de red (**Network Tables** / DevTools).

> *Diferencia clave:* La consola proporciona información volátil en tiempo real sobre el estado del proceso; los logs ofrecen el registro histórico persistente del evento.

### Paso 3: Formulación de la Hipótesis
A partir del síntoma y la evidencia, planteamos una causa probable: *"La base de datos rechazó la conexión por timeout"* o *"Una variable de entorno apunta a un endpoint incorrecto"*.

### Paso 4: Validación
Contrastamos la hipótesis contra la realidad técnica:
* ¿El servicio dependiente está encendido?
* ¿Hay reglas de firewall o restricciones de IP bloqueando el tráfico?
* Si la validación refuta la hipótesis, no improvisamos: regresamos al paso 2 (revisar más logs y evidencia) y formulamos una nueva hipótesis.

### Paso 5: Corrección y Cierre
Aplicamos la solución en el código o en la configuración, redesplegamos y validamos el comportamiento.

> **La analogía médica:** Cuando un paciente acude a consulta, el médico primero escucha los **síntomas**, ordena exámenes clínicos para obtener **evidencia**, formula una **hipótesis** diagnóstica, la **valida** con los resultados de laboratorio y finalmente prescribe el tratamiento o realiza la intervención quirúrgica. En ingeniería de software seguimos exactamente el mismo rigor.

## 3. Variables de Configuración y Azure Key Vault

**Daniel Vacas:**

Las variables de entorno operan bajo el modelo **Clave-Valor** (`Key-Value`). Un solo valor erróneo o mal formateado puede romper por completo todo un entorno de ejecución.

### Ejemplos comunes de variables:
* `API_BASE_URL`: Dirección a la que se conecta el frontend o backend.
* `DEBUG`: Booleano (`true`/`false`) para activar logs detallados (¡nunca en producción!).
* `FEATURE_FLAGS`: Interruptores lógicos para habilitar o deshabilitar módulos sin redesplegar.

### Los tres niveles donde viven las variables en Azure:

1. **Application Settings (App Service):** Variables inyectadas por la plataforma en tiempo de ejecución.
2. **Pipelines de CI/CD (Azure DevOps / GitHub Actions):** Variables pasadas durante el build.
3. **Archivos locales en código (`appsettings.json` o `.env`):** Archivos embebidos en el proyecto.

### El riesgo de las variables "quemadas" en el Build
Cuando compilamos un frontend o empaquetamos un backend en un archivo `.zip`, muchas variables quedan grabadas de forma estática e inmutable dentro de los binarios. Si se necesita cambiar una credencial o URL, modificar el App Settings en el portal no surtirá efecto si el build sigue leyendo el archivo estático local.

### La solución empresarial: Azure Key Vault
En lugar de almacenar credenciales sensibles o cadenas de conexión en texto plano:
* Guardamos los secretos, certificados y tokens dentro de **Azure Key Vault**.
* En el App Service configuramos una referencia directa al Key Vault:
  ```
  @Microsoft.KeyVault(SecretUri=https://mi-vault.vault.azure.net/secrets/MiSecreto/version)
  ```
* **Ventaja:** Al actualizar el secreto en Key Vault, el cambio se refleja en producción de manera segura, auditable y sin necesidad de recompilar ni exponer credenciales en repositorios.

## 4. CORS (Cross-Origin Resource Sharing) y Seguridad

**Daniel Vacas:**

El mecanismo de CORS es una capa de seguridad implementada en los navegadores para restringir solicitudes HTTP entre diferentes orígenes.

### Definición de Origen
Un origen está compuesto estrictamente por tres elementos:
$$\text{Origen} = \text{Protocolo} + \text{Dominio} + \text{Puerto}$$
*Ejemplo:* `https://` (protocolo) + `miapp.com` (dominio) + `:443` (puerto). Si alguno de estos tres difiere entre el frontend y el backend, el navegador considera la solicitud como *Cross-Origin*.

### Flujo de Negociación CORS (Preflight Request):
1. **Petición Preflight (`OPTIONS`):** Antes de enviar una solicitud real (como `POST`, `PUT` o `DELETE`), el navegador consulta al backend: *"¿Aceptas peticiones provenientes de mi origen?"*.
2. **Validación del Backend:** El backend revisa si el origen entrante está en su lista blanca y qué métodos y cabeceras tiene permitidos.
3. **Respuesta de Cabeceras:** Si es válido, responde con la cabecera `Access-Control-Allow-Origin: <origen>`.
4. **Ejecución Real:** El navegador procede a enviar la petición transaccional de datos. Si el origen no está autorizado, el navegador bloquea la respuesta y muestra un error de CORS en la consola del cliente.

```
Navegador (Frontend)                       Servidor (Backend API)
       │                                             │
       │ ── 1. OPTIONS (Preflight: Origen?) ───────> │
       │ <── 2. 200 OK (Access-Control-Allow-Origin) │
       │                                             │
       │ ── 3. POST /api/pedidos (Petición Real) ──> │
       │ <── 4. 200 OK (Datos JSON procesados) ───── │
```

### Buenas prácticas de CORS y Middlewares:
* **Nunca usar `*` (wildcard) en producción:** Permitir todos los orígenes expone las APIs a ataques de tipo CSRF y peticiones no autorizadas desde sitios maliciosos.
* **Restringir verbos HTTP:** Si un cliente solo requiere consultar datos, autorizar únicamente el método `GET`.
* **Uso de Middlewares:** Implementar capas intermedias para validar tokens JWT, verificar la expiración del payload y sanitizar cabeceras antes de llegar a los controladores de negocio.

## 5. Health Checks: Liveness vs. Readiness

**Daniel Vacas:**

Los endpoints de salud permiten supervisar si el sistema se encuentra en condiciones óptimas de operar:

| Tipo de Chequeo | Propósito | ¿Qué evalúa? | Acción ante fallo |
|---|---|---|---|
| **Liveness Check** (`/health`) | Confirma si el proceso de la aplicación sigue vivo. | Si el servidor web responde peticiones básicas en memoria. | Reinicio automático del contenedor o proceso. |
| **Readiness Check** (`/health/deep`) | Confirma si la aplicación está lista para recibir tráfico real. | Conectividad con dependencias críticas (Base de datos, colas, APIs de terceros). | El balanceador de carga retira la instancia del tráfico hasta que se recupere. |

### Diagnóstico de Estados:
* **Healthy (Saludable):** El proceso responde y todas las dependencias externas están operativas.
* **Degraded (Degradado):** El proceso está vivo, pero una dependencia secundaria presenta latencia elevada o reintentos.
* **Unhealthy (No Saludable):** Fallo crítico de conexión (por ejemplo, timeout hacia la base de datos por reglas de firewall o credenciales caducadas).

## 6. Errores HTTP y Análisis de Causa Raíz

**Daniel Vacas:**

En programación no hay "brujería"; cada falla deja un rastro claro en los registros:

* **Familia 200 (Éxito):** La solicitud fue procesada correctamente.
* **Familia 300 (Redirección):** El recurso solicitado cambió de ubicación temporal o permanente.
* **Familia 400 (Errores del Cliente):**
  * `400 Bad Request`: Petición mal formada o esquema JSON inválido.
  * `401 Unauthorized`: Falta de credenciales de autenticación (no ha iniciado sesión o no envió token).
  * `403 Forbidden`: Autenticado, pero sin permisos de rol suficientes para el recurso.
  * `404 Not Found`: La ruta o el endpoint solicitado no existe.
* **Familia 500 (Errores del Servidor):**
  * `500 Internal Server Error`: Excepción no controlada en el código.
  * `502 Bad Gateway` / `503 Service Unavailable`: Servicios intermedios caídos o saturados.

### Ejemplo Práctico de Diagnóstico de un Error 500:
* **Síntoma:** El cliente reporta error 500 al consultar `/api/pedidos/45`.
* **Evidencia en Log:** `TypeError: Cannot read property 'total' of undefined at calcularTotal()`.
* **Hipótesis:** La bandera de funcionalidad (*Feature Flag*) `validacion_final` espera que el campo `total` esté presente, pero no fue inicializado en el payload recibido.
* **Solución:** Validar la nulidad del campo antes del cálculo en el backend y emitir un código de validación `400` en lugar de dejar que el servidor colapse con un error `500`.

## 7. Demostración Práctica en el Portal de Azure

**Daniel Vacas:**

Durante la práctica en vivo se ejecutaron las siguientes configuraciones en el portal:

### 1. Despliegue de App Service y Manejo de Errores Regionales
* Se creó una Web App en Linux con runtime de **.NET 10**.
* Al intentar desplegar en una región con cuota saturada, Azure arrojó un error de tipo `Bad Request (QuotaExceeded)`.
* **Solución aplicada:** Se diagnosticó el mensaje de error del despliegue y se modificó la región geográfica hacia `East US`, completando el aprovisionamiento con éxito.

### 2. Creación y Configuración de Azure Key Vault
* Despliegue de un recurso Key Vault bajo el plan Standard.
* Explicación de los planes: el plan Standard cubre claves y secretos cifrados por software; el plan Premium habilita la gestión de llaves respaldadas por hardware físico (HSM).

### 3. Kudu y App Service Editor
* Para inspeccionar los archivos compilados del despliegue accedimos a **Kudu** (`https://<app-name>.scm.azurewebsites.net`).
* Desde la consola de Kudu navegamos hacia la ruta física de publicación:
  ```
  /home/site/wwwroot
  ```
* Se demostró cómo identificar si un archivo de variables (`appsettings.json`) quedó congelado o sobreescrito durante el proceso de empaquetado (.ZIP).

### 4. Herramientas de Diagnóstico en el Navegador (DevTools)
Al compartir la URL pública de la aplicación desplegada con los asistentes:
* **Pestaña Network:** Inspección de llamadas XHR/Fetch, cabeceras enviadas, cabeceras devueltas y códigos de respuesta.
* **Preserve Log:** Casilla fundamental para evitar que el historial de peticiones de red se borre cuando la página ejecuta una recarga automática.
* **Simulación de Condiciones de Red:** Uso del acelerador/limitador para probar el comportamiento de la aplicación en 3G, 4G lenta o en estado Offline.
* **Exportación de archivos HAR (.har):** Daniel demostró cómo descargar el archivo HTTP Archive (`.har`) completo con todas las trazas de red y solicitudes para entregarlo a equipos de soporte o introducirlo en herramientas de IA (como Claude o ChatGPT) para un análisis automatizado de cuellos de botella.

## 8. Sesión de Preguntas y Respuestas (Q&A)

**Ewell pregunta:**
*¿Cómo podría subir un sitio web que ya tengo en GitHub hacia el servicio Web App en Azure?*

**Daniel Vacas:**
Puedes ir directamente al recurso de la Web App en el portal de Azure y entrar a la sección **Deployment Center**. Allí seleccionas GitHub como origen, autorizas tu cuenta, eliges la organización, el repositorio y la rama de trabajo. Azure configurará automáticamente un flujo de trabajo de **GitHub Actions** con un pipeline YAML que compilará y desplegará tu aplicación ante cada nuevo push.

**Luis pregunta:**
*Si se despliega una aplicación containerizada, ¿cuál sería la mejor forma de manejar las variables de entorno?*

**Daniel Vacas:**
La mejor práctica es inyectarlas como variables de entorno del contenedor apuntando mediante referencias seguras a **Azure Key Vault**. Si utilizas un clúster de Kubernetes o Container Apps, puedes sincronizarlas mediante secretos nativos o montar volúmenes con el Secret Store CSI Driver.

**Pregunta del chat:**
*¿Por qué el primer despliegue de la Web App falló?*

**Daniel Vacas:**
El primer intento falló por una restricción de cuota y disponibilidad en esa región específica de Azure. Al leer el detalle del log de despliegue, identificamos que el SKU no tenía capacidad disponible en ese datacenter. Simplemente cambiamos la región de destino a East US y el aprovisionamiento concluyó sin contratiempos.

## 9. Cierre de la Sesión e Instrucciones para el Evento

**Daniel Vacas:**
Eso sería todo por mi parte el día de hoy. Recuerden siempre seguir la evidencia en los logs y no confiarse de suposiciones. En la programación la tecnología evoluciona a diario; manténganse estudiando y con hambre de aprender. Les deseo el mayor de los éxitos en los retos de la hackatón.

**Luis Martínez:**
Muchísimas gracias, Daniel. Una explicación impecable que complementa todos los talleres técnicos previos.

**Avisos cruciales para todos los participantes:**
1. **Grabaciones oficiales:** Todas las sesiones anteriores (Workshops #1 al #4) y la grabación de esta sesión están y estarán disponibles en el canal de YouTube: **Hakaton CM**.
2. **Formulario de asistencia:** Llenar el formulario compartido en el chat para registrar su participación.
3. **Próxima sesión (Lunes 14):** Se realizará una sesión especial previa a la competencia dedicada a la **Guía de Estudiantes de Dynatrace**, resolución de dudas generales y preparación técnica para el día del evento.
4. **Recursos y Guía Dynatrace:** En el sitio oficial **hacatoncopa.com** (sección *Recursos*) pueden descargar el documento PDF con la guía de registro para el **Dynatrace Playground** y el curso **Dynatrace Essentials** en Dynatrace University.
5. **Inscripciones Sede Santiago:** Larisa Aguilar confirmó que se abrieron los cupos para la sede del Hotel Gran David en Santiago de Veraguas, destinada a estudiantes de provincias centrales (Chiriquí, Bocas del Toro, Veraguas, Herrera, Los Santos y Coclé).

¡Muchas gracias a todos por su asistencia y compromiso, y que tengan una excelente noche!