# Transcripción Oficial — Workshop #2: Aplicaciones Modernas: API, Frontend y Backend

> **Video:** https://www.youtube.com/watch?v=fy6m-dhzGhM · Transcripción oficial entregada por la organización. Los subtítulos automáticos (para buscar frases textuales) están en [`subtitulos/w2.txt`](subtitulos/w2.txt).

## ⚡ Puntos clave para el reto

- **App Service Plan** = el cómputo (CPU/RAM); **Web App** = la app que corre encima. Un plan aloja varias apps.
- **Static Web Apps** para frontend (CDN, CI/CD con GitHub); **Web App** para backend/API.
- SKUs: Free/Shared (sin escalado), Basic (solo vertical), **Standard** (autoescalado + deployment slots), Premium/Isolated.
- Verbos: GET leer, POST crear, PUT/PATCH actualizar, DELETE borrar. Códigos: 200/201, 400, 401 (sin credenciales), 403 (sin permiso), 404, 500.
- Endpoints de salud de la demo: `/api/status`, `/health` (liveness) y `/health/deep` (dependencias).
- Patrón CQRS: Queries (lectura) separadas de Commands (escritura).

---

**Evento:** Hackatón Copa 2026 — API, DevOps y Observabilidad  
**Expositor Principal:** Daniel Vacas (Business IT)  
**Moderadores y Colaboradores:** Luis Martínez, Evelin Ceballos (Learning Leader en Business IT), Larisa Aguilar  

---

## 1. Bienvenida e Introducción al Workshop #2

**Luis Martínez:**  
Quería comentarles que en la noche de hoy estaremos viendo el tema de este workshop: **Aplicaciones Modernas: API, Frontend y Backend**. Los dejo en estos momentos con Evelin, quien nos va a compartir parte de la información importante que necesitan.

**Evelin Ceballos:**  
Buenas noches con todos, mi nombre es Evelin Ceballos, me presento nuevamente. Soy Learning Leader en Business IT. Nosotros somos una empresa de tecnología y justamente estamos hoy con Dani Vacas, quien será el instructor de esta noche.

De mi parte me gustaría indicarles cómo acceder al material oficial de entrenamiento por parte de Microsoft. Les dejaré un mensaje en el chat para que lo puedan revisar. Tenemos dos pasos a seguir:
1. Ingresar al entrenamiento de la certificación **AZ-900**, sobre el cual están basados los fundamentos que vemos en estos workshops.
2. Un enlace para canjear códigos de logro.

En este momento procedo a compartir mi pantalla para que puedan visualizar cómo acceder a Microsoft Learn.

---

## 2. Recursos Oficiales en Microsoft Learn y Certificación AZ-900

**Evelin Ceballos:**  
¿Qué es Microsoft Learn? Es la plataforma oficial donde Microsoft aloja toda la información de nuestros entrenamientos.

En el chat disponen de dos enlaces:
* **Guía de estudio:** Al copiarla y pegarla en su navegador, los llevará de referencia al entrenamiento **AZ-900**. Dentro de cada una de las rutas de aprendizaje encontrarán descripciones, módulos interactivos, evaluaciones, resúmenes y videos de apoyo para estudiar a su propio ritmo.
* **Código de logro:** Al ingresar a este enlace podrán canjear el código que habilita los entrenamientos y los certificados de los cursos que vamos desarrollando. Hacen clic en "Canjear" y se agregarán a su perfil. Es importante que verifiquen que sus nombres y apellidos en su perfil de Microsoft Learn estén correctos para que los certificados se generen adecuadamente.

Eso es todo de mi parte. Les presento formalmente a nuestro instructor Daniel Vacas. Daniel es nuestro consultor de desarrollo de software especializado en tecnologías Microsoft, con más de 3 años de experiencia en Microsoft Azure, automatización de procesos, ciberseguridad, implementación de soluciones Cloud, Microsoft Sentinel, integraciones empresariales y gestión de proyectos tecnológicos. Además participa activamente como capacitador e instructor seleccionado en inteligencia artificial. Bienvenido, Dani.

---

## 3. Presentación y Objetivos de la Sesión

**Daniel Vacas:**  
Muchas gracias por esa introducción. Bienvenidos a todos. En la primera sesión revisamos conceptos básicos sobre la distribución de servicios en Azure, todo lo necesario para iniciar en el desarrollo en la nube. Hoy vamos a profundizar en servicios orientados a:
* **Azure App Service** y **App Service Plans**.
* Diferencias de arquitectura entre **Frontend** y **Backend**.
* Comunicación mediante **APIs REST** y consumo entre capas.
* Endpoints, protocolos y verbos **HTTP**, códigos de respuesta y errores comunes en despliegues.

---

## 4. Fundamentos de Azure App Service y el Modelo PaaS

**Daniel Vacas:**  
En la sesión anterior revisamos los modelos IaaS, PaaS y SaaS. **PaaS (Plataforma como Servicio)** aloja nuestras aplicaciones web en la nube de Azure con grandes ventajas:

* Azure se encarga de administrar el sistema operativo, los parches de seguridad, las actualizaciones, el balanceo de carga y la alta disponibilidad.
* No tenemos que desgastarnos configurando la infraestructura base; nos concentramos 100% en el código y en la lógica de negocio.

### La Analogía Inmobiliaria
> *Hacer un símil:* Utilizar un servicio PaaS es equivalente a **rentar un departamento completamente amoblado**. No es lo mismo comprar un terreno baldío y construir desde cero (IaaS), lo cual demanda tiempo, licencias y mantenimiento. Rentar un departamento amoblado reduce costos iniciales, ofrece disponibilidad inmediata y permite enfocarse en habitarlo (desplegar la aplicación).

### Azure App Service vs. Azure App Service Plan
Es crucial distinguir estos dos conceptos:
* **Azure App Service Plan:** Representa el servidor o la capacidad de cómputo físico/virtual subyacente asignada (vCPUs, RAM, almacenamiento, red y características de escalado).
* **Azure Web App:** Es la aplicación en sí misma que corre sobre ese plan de cómputo. Un solo App Service Plan puede alojar múltiples Web Apps.

---

## 5. Arquitecturas Frontend vs. Backend en Azure

**Daniel Vacas:**  
El **Frontend** es la cara visible al usuario, donde ocurren las interacciones, clics y renderizado en pantalla. El **Backend** contiene la lógica de negocio, reglas de cálculo, validaciones y acceso a datos.

Azure ofrece servicios especializados para cada capa:

| Tipo de Servicio | Azure Static Web Apps | Azure App Service (Web Apps) |
| :--- | :--- | :--- |
| **Enfoque Principal** | Frontend y sitios estáticos / Jamstack | Backend, APIs REST y monolitos dinámicos |
| **Frameworks / Lenguajes** | Angular, React, Vue, Svelte, Blazor, Next.js, HTML/JS | .NET, Node.js, Python, Java, PHP |
| **Integración CI/CD** | Nativa con GitHub y Azure DevOps con flujos automáticos | Git, ZIP push, FTP, pipelines de CI/CD y Visual Studio |
| **Cómputo** | Distribución global basada en CDN sin servidor dedicado | Corre sobre un App Service Plan con cómputo dedicado |

### Despliegue de un Frontend con Azure Static Web Apps
Al crear una Static Web App seleccionamos la suscripción, el grupo de recursos y el nombre del recurso. Elegimos el plan (*Free* para proyectos personales o demos; *Standard* para entornos empresariales). Conectamos directamente el repositorio de GitHub o Azure DevOps, seleccionando la rama y el framework utilizado (Angular, React, etc.), especificando la carpeta de salida del build (por ejemplo `/dist` o la raíz).

### Despliegue de un Backend con Azure Web Apps
Para el Backend seleccionamos una Web App convencional. Al configurarla debemos definir:
1. **Nombre del recurso:** Determina el subdominio `.azurewebsites.net` (debe ser único a nivel global y cumplir reglas de nomenclatura sin caracteres especiales inválidos).
2. **Método de publicación:** Código fuente (*Code*) o Contenedor (*Docker Container*).
3. **Runtime Stack:** La tecnología y versión (por ejemplo, .NET 10, Node.js, Python 3.x, Java).
4. **Sistema Operativo (Linux vs. Windows):**
   * Linux suele ser más económico y eficiente para la mayoría de stacks modernos y entornos de microservicios.
   * Ciertas librerías o proyectos legados de ASP.NET clásico requieren Windows. Es fundamental revisar cómo gestiona el proyecto los *paths* del sistema de archivos.
5. **Región geográfica:** Afecta directamente la latencia del usuario final y los costos. Elegir regiones cercanas (por ejemplo, East US o Brasil) reduce el tiempo de respuesta.

---

## 6. Planes de App Service y Capacidades de Escalado

**Daniel Vacas:**  
Al configurar el App Service Plan encontramos diferentes niveles de servicio (SKU):

* **Free / Shared:** Nivel introductorio compartido (1 GB de memoria, cómputo compartido, sin escalado ni dominios personalizados). Útil para pruebas básicas.
* **Basic (B1, B2, B3):** Cómputo dedicado para cargas de desarrollo. Permite escalado manual vertical (cambiar tamaño de máquina), pero **no** soporta autoescalado horizontal ni ranuras de implementación (*deployment slots*).
* **Standard (S1, S2, S3):** Nivel recomendado para producción básica. Incluye:
  * **Autoescalado horizontal (*Scale Out*):** Aumento automático del número de instancias según métricas de CPU o memoria.
  * **Deployment Slots (Ranuras de implementación):** Permite tener entornos staging/dev sobre el mismo plan.
  * Soporte de balanceo de carga y backups integrados.
* **Premium (P1v3, P2v3, P3v3) e Isolated:** Alto rendimiento, hardware dedicado, soporte de redes virtuales avanzadas y aislamiento físico.

---

## 7. Desacoplamiento y Comunicación entre Frontend y Backend

**Daniel Vacas:**  
¿Por qué desacoplamos el Frontend del Backend? Porque cada uno experimenta patrones de carga y ciclos de vida diferentes. El Frontend puede atender miles de solicitudes estáticas vía CDN, mientras que el Backend procesa transacciones pesadas.

### La Analogía del Mesero en el Restaurante
> El cliente en la mesa representa al **Frontend**.  
> La cocina donde se preparan los platillos representa al **Backend**.  
> El **mesero** representa a la **API REST**.  
> El mesero lleva el pedido del cliente a la cocina y trae el platillo terminado a la mesa, pero el mesero no cocina y la cocina no interactúa directamente con los comensales.

### Protocolo HTTP, Verbos y Operaciones CRUD
* **GET:** Obtención y lectura de datos.
* **POST:** Creación y registro de un nuevo recurso.
* **PUT / PATCH:** Actualización total (PUT) o parcial (PATCH) de un recurso existente.
* **DELETE:** Eliminación de un recurso.

### Códigos de Estado HTTP
* **200 OK / 201 Created:** Petición exitosa o recurso creado satisfactoriamente.
* **400 Bad Request:** Petición mal formateada o datos inválidos.
* **401 Unauthorized:** Falta de credenciales o token no provisto.
* **403 Forbidden:** Autenticado, pero sin permisos suficientes.
* **404 Not Found:** La ruta o recurso solicitado no existe.
* **500 Internal Server Error:** Fallo no controlado en la lógica del servidor.

### De XML a JSON y Formatos Binarios
Antes predominaban las APIs SOAP basadas en XML pesado. Hoy el estándar ubicuo es **JSON (JavaScript Object Notation)**, y en arquitecturas de alto rendimiento se migra hacia formatos compactos y serializaciones eficientes (como Protocol Buffers o esquemas binarios tipados).

---

## 8. Patrón CQRS y Endpoints de Salud (Health Checks)

**Daniel Vacas:**  
En la demostración práctica con un proyecto en .NET estructuramos el código dividiendo responsabilidades en:
* **Infraestructura:** Conexiones y almacenamiento en memoria.
* **Dominio:** Entidades y modelos de datos.
* **Controladores:** Endpoints expuestos al público.
* **Aplicación:** Lógica dividida en **Queries** (consultas de lectura) y **Commands** (acciones de escritura y mutación).

### El Patrón CQRS (Command Query Responsibility Segregation)
Este patrón desacopla las operaciones de lectura de las de escritura:
* **Queries (GET):** Optimizadas para velocidad y lectura sin alterar el estado.
* **Commands (POST/PUT/DELETE):** Procesan validaciones, reglas de negocio y mutaciones de datos.

> *Relevancia en Inteligencia Artificial:* Al integrar modelos de IA o agentes, es una excelente práctica separar los canales: las IAs consumen información masiva mediante endpoints de solo lectura (Queries), mientras que las acciones que ejecutan sobre el sistema se canalizan por comandos controlados (Commands).

### Endpoints de Monitoreo de Salud
Implementamos tres endpoints esenciales documentados mediante Swagger:
1. `/api/status`: Devuelve el estado general del sistema, ambiente, versión y marca de tiempo.
2. `/health`: Liveness check rápido para verificar si el proceso del servidor está vivo y respondiendo.
3. `/health/deep`: Verificación profunda que comprueba conectividad con bases de datos y APIs externas dependientes.

---

## 9. Despliegue hacia Azure App Service

**Daniel Vacas:**  
Existen múltiples mecanismos para desplegar código hacia Azure App Service:

1. **Integración Continua (CI/CD):** Mediante pipelines en GitHub Actions o Azure DevOps.
2. **Despliegue directo desde el IDE (Visual Studio / VS Code):**
   * Clic derecho en el proyecto $\rightarrow$ *Publish*.
   * Selección de la suscripción, grupo de recursos y App Service de destino.
3. **Perfil de Publicación (*Publish Profile / .publishsettings*):**
   * Archivo XML descargable desde Azure Portal que contiene las credenciales encriptadas de despliegue.
   * Permite que cualquier desarrollador publique directamente sin requerir acceso de lectura a nivel del portal de Azure ni permisos sobre la suscripción.
4. **Deployment Slots (Ranuras de Implementación):**
   * Permiten mantener entornos paralelos (por ejemplo `staging` y `production`) bajo el mismo plan.
   * Permite probar cambios en staging y realizar un intercambio instantáneo (*swap*) a producción con cero tiempo de inactividad (*zero-downtime deployment*). Requiere plan **Standard** o superior.

---

## 10. Métricas y Monitoreo en Azure Portal

**Daniel Vacas:**  
Al compartir la URL pública del backend desplegado, los participantes realizaron peticiones en vivo. Desde la sección de **Métricas** en el portal de Azure App Service se pudo observar:
* Conteo de solicitudes HTTP entrantes.
* Desglose por códigos de respuesta (solicitudes HTTP 200 vs. errores).
* Tiempos de respuesta y percentiles de latencia.
* Uso de CPU y memoria en tiempo real.

---

## 11. Sesión de Preguntas y Respuestas (Q&A)

**Orlando pregunta:**  
*¿Se pueden juntar el servicio de Web App y Static Web App? ¿No es uno para backend y el otro para frontend?*

**Daniel Vacas:**  
Sí se pueden juntar dependiendo de la arquitectura de la aplicación. Por ejemplo, en proyectos tradicionales tipo monolito con PHP (Laravel) o ASP.NET MVC, el backend y el frontend están embebidos en el mismo código y se despliegan juntos en una sola Web App.  
Sin embargo, desacoplarlos en una **Static Web App** (para el frontend) y una **Web App** (para la API backend) es la mejor práctica recomendada: permite escalar cada capa de forma independiente y optimizar costos de ancho de banda.

---

**Jasel pregunta:**  
*Sobre el uso de herramientas de Inteligencia Artificial en la hackatón: ¿estarán permitidas libremente?*

**Daniel Vacas:**  
Los lineamientos exactos los brindarán los coordinadores del evento. Sin embargo, técnicamente deben considerar que integrar IA no es solo llamar un endpoint:
* Hay restricciones de **límites de tokens** por minuto (TPM) y ventanas de contexto.
* Si conectan IA con bases de datos, deben planificar cómo manejar embeddings, indexaciones y capas de caché para no disparar los costos de consumo de tokens ni saturar la latencia.

---

**Pregunta del chat:**  
*¿En qué se diferencian estos servicios de Azure con otras opciones de despliegue como Vercel o Render?*

**Daniel Vacas:**  
Vercel y Render son plataformas excelentes para despliegues rápidos. La ventaja de Azure radica en el **control empresarial y la integración nativa del ecosistema**:
* Disparadores basados en eventos con **Azure Functions**, **Logic Apps** y **Event Grid**.
* Monitoreo profundo con telemetría unificada a través de **Azure Monitor**, **Application Insights** y estándares **OpenTelemetry**.
* Integración con redes privadas virtuales (VNet), gobernanza y políticas empresariales centralizadas.

---

**José pregunta:**  
*¿La clase será subida a alguna plataforma o a YouTube?*

**Larisa Aguilar / Luis Martínez:**  
Todas las grabaciones de los workshops quedan guardadas y públicas en nuestro canal oficial de YouTube: **Hakaton CM**. De hecho, la grabación del Workshop #1 ya está disponible en el canal para que la puedan repasar.

---

**Manuel pregunta:**  
*Me quedo con una duda sobre los App Service Plans: si tengo un plan gratuito y necesito escalar la app, ¿se debe cambiar de plan para poder escalar?*

**Daniel Vacas:**  
Es correcto. En el plan gratuito (*Free*) no tienes disponible el escalado automático ni el escalado horizontal (*Scale Out*). Si tu aplicación demanda más recursos, debes migrar al menos a un plan **Basic** para escalado vertical (más CPU y RAM) o a un plan **Standard** para habilitar escalado automático horizontal y ranuras de implementación (*deployment slots*).

---

**Roger pregunta:**  
*A nivel de eficiencia, ¿es mejor desplegar la app web en un VPS o en un servicio autogestionado como este? Me imagino que depende de la etapa y madurez del proyecto.*

**Daniel Vacas:**  
Depende de la arquitectura y la madurez:
* Si manejas un monolito clásico o requieres control total del sistema operativo por temas de presupuesto estricto, un VPS puede servir inicialmente, pero requiere que tú administres parches, seguridad y respaldos.
* Para microservicios o arquitecturas limpias (como arquitectura hexagonal), migrar hacia **contenedores** o servicios PaaS autogestionados ahorra tiempo operativo crítico, aporta alta disponibilidad y permite escalar en base a métricas sin intervención manual.

---

**Pregunta del chat:**  
*¿El día de las competencias nos darán créditos o algún plan especial para uso de los servicios en Azure?*

**Luis Martínez:**  
Todos los accesos, suscripciones, créditos y herramientas que sean estrictamente necesarios para resolver los retos del evento les serán provistos directamente por la organización el día de la hackatón.

---

**Juan Castillo pregunta:**  
*¿Qué ventaja de rendimiento tenemos al usar contenedores frente al runtime nativo de App Service?*

**Daniel Vacas:**  
A nivel de hardware, el rendimiento puede ser muy similar dentro del mismo tamaño de App Service Plan. La ventaja decisiva de los **contenedores** está en el equipo de desarrollo:
* Si en el equipo unos programan en Windows, otros en macOS y otros en Linux, el contenedor garantiza que el código corra con idénticas dependencias en local y en producción, erradicando el clásico *"en mi máquina sí funcionaba"*.
* Si todo el equipo trabaja estandarizado en el mismo sistema operativo y stack, el runtime nativo de App Service funciona de manera impecable y simplifica los despliegues directos.

---

## 12. Cierre del Taller

**Daniel Vacas:**  
Con esto completamos los puntos de nuestra sesión. Les deseo el mayor de los éxitos en sus preparaciones para el evento. Manténganse curiosos y continúen explorando la plataforma. Muchas gracias a todos por su participación. Luis, te cedo la palabra.

**Luis Martínez:**  
Excelente, Daniel. Muchísimas gracias por compartir todo este conocimiento con los estudiantes. Les recordamos:
1. Llenar el formulario de registro y asistencia que dejamos en el chat.
2. Seguir las redes sociales oficiales y el canal de YouTube **Hakaton CM**.
3. Unirse al canal de difusión de WhatsApp del Hackatón.
4. Para dudas directas sobre las bases legales y el evento, escribir a **jacatoncopa@copaair.com**.

El próximo martes continuaremos con el **Workshop #3: DevOps, Git, GitHub y Terraform** a las 7:00 PM (hora de Panamá). ¡Que tengan una excelente noche!