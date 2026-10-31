# Transcripción Oficial — Workshop #1: Fundamentos de Azure y Arquitectura en la Nube

> **Video:** https://www.youtube.com/watch?v=vzvzVZH1Bw4 · Transcripción oficial entregada por la organización. No hay subtítulos automáticos de este video.

## ⚡ Puntos clave para el reto

- **Modelos de servicio:** IaaS (tú administras SO, parches, red virtual y apps), PaaS (Microsoft administra hardware, red física, virtualización y SO; tú solo código) y SaaS (solo identidades, usuarios, dispositivos y datos).
- **Nubes:** privada, pública e híbrida (el *core* aislado y lo expuesto en la pública).
- **Jerarquía:** Management Group → Suscripción (facturación) → Resource Group (ciclo de vida, RBAC, etiquetas) → Recurso.
- **Infraestructura global:** región = centros de datos con red de baja latencia; Availability Zones = centros de datos separados dentro de la región; Region Pairs separados al menos ~500 km.
- **Cómputo:** VMs (IaaS), Bastion (acceso por 443 sin abrir RDP 3389 ni SSH 22), ACI / ACA / AKS (contenedores), Functions (serverless por eventos) y App Service (PaaS para web y APIs).
- **Storage:** LRS (3 copias en un datacenter), ZRS (3 zonas), GRS (+ región secundaria), GZRS; Blob, Files, Queues (64 KB por mensaje) y Tables; niveles Hot, Cool (30 d), Cold (90 d) y Archive (180 d).
- **Identidad:** Entra ID. Autenticación = quién eres; autorización = qué puedes hacer. MFA y Acceso Condicional.
- **Escalado:** vertical (scale up, más CPU/RAM) vs. horizontal (scale out, más instancias). Load Balancer (capa 4) vs. Application Gateway (capa 7, con WAF).
- **Terraform vs. Azure SDK:** Terraform para aprovisionar infraestructura; SDK para operar recursos desde el código de la app.
- Consejo de Daniel para el reto: repasar Microsoft Learn (AZ-900).

---
**Evento:** Hackatón Copa 2026 — API, DevOps y Observabilidad  
**Expositor:** Daniel Vacas (Business IT)  
**Moderadores / Anfitriones:** Luis Martínez, Larisa Aguilar, Hugo Aquino  

---

## 1. Bienvenida e Introducción

**Luis Martínez:**  
Buenas noches a todos, bienvenidos. En la noche de hoy nos acompaña Daniel Vacas de la empresa Business IT, quien compartirá su experiencia y su conocimiento en Azure con lo que está enfocado al tema de nuestro workshop: **Fundamentos de Azure y Arquitectura en la Nube**.

Sin más preámbulos, los invito a que dejen todas sus consultas en el chat; todas serán contestadas a medida que se vaya desarrollando la actividad. De igual forma, estaremos teniendo algunos recesos durante la presentación. Adelante, Daniel.

**Daniel Vacas:**  
¿Qué tal, Luis? Muchas gracias. Les agradezco a todos y muy bienvenidos. Permítanme un momento, les comparto la pantalla mientras verificamos el contenido.

Como lo mencionó Luis, vamos a observar un poco más sobre el contenido relacionado a fundamentos en la nube y parte de lo que sería la arquitectura. Por favor, cualquier duda o punto de desconocimiento pueden dejarnos un mensaje en el chat, y abriremos un espacio para dar solución a sus inquietudes.

---

## 2. Del Entorno Tradicional (On-Premise) a los Modelos de Nube

**Daniel Vacas:**  
Primero vamos a empezar desde el punto de origen tradicional. Normalmente, desde los orígenes teníamos todo ubicado en un solo lugar, considerado *on-premise*. Teníamos la información en un data center físico, un local o una oficina donde teníamos el 100% del acceso y control.

Sin embargo, la tecnología ha evolucionado y nos ha permitido llevar la información a la nube. Pero, ¿qué implica tener la información en la nube? La información no solo requiere almacenamiento aislado; requiere procesamiento y, sobre todo, redes para moverse. Todo esto congregado es lo que nos permite operar en la nube: no solo almacenamiento, sino interconexiones de red, procesamiento de cómputo y servicios avanzados que hoy en día llegan hasta la Inteligencia Artificial.

### Tipos de Nube:
1. **Nube Privada:**  
   Muchas empresas consideran que mantener una intranet es seguro y razonable. Todo el contenido y las operaciones están aislados exclusivamente para los colaboradores de la empresa. Sin embargo, surge la necesidad de exponer servicios a clientes y proveedores.
2. **Nube Pública:**  
   Consiste en desplegar servicios en la infraestructura de proveedores globales (como Microsoft Azure), exponiendo la información de forma controlada hacia diversas ubicaciones públicas.
3. **Nube Híbrida:**  
   Combina lo mejor de ambos mundos. Todo el núcleo (*core*) del negocio puede mantenerse 100% aislado en la nube privada u on-premise, mientras que los servicios y capas de consumo que se desean exponer hacia el exterior se despliegan en la nube pública bajo políticas estrictas de seguridad.

---

## 3. Modelos de Servicio y Responsabilidad Compartida

**Daniel Vacas:**  
En el pasado se alquilaban computadoras o servidores físicos. En la nube esto se traduce en modelos de servicio:

* **IaaS (Infraestructura como Servicio):**  
  Es la base primordial de la arquitectura en la nube. Microsoft se encarga del centro de datos físico, la energía, el cableado y el hardware del host físico. El cliente es responsable de configurar y administrar el sistema operativo, los parches, la red virtual, las aplicaciones, el firewall y los datos.
* **PaaS (Plataforma como Servicio):**  
  Microsoft asume la administración del hardware, la red física, la capa de virtualización y el sistema operativo (actualizaciones, parches y balanceo de carga automático). El cliente se concentra exclusivamente en desarrollar el software, la lógica de negocio y desplegar su código.
* **SaaS (Software como Servicio):**  
  Es una solución completa lista para consumir (por ejemplo, Microsoft 365). El proveedor gestiona toda la infraestructura, el sistema operativo y el software. El cliente solo administra sus identidades, usuarios, dispositivos y datos.

---

## 4. Infraestructura Global de Azure: Regiones y Zonas de Disponibilidad

**Daniel Vacas:**  
Una duda frecuente es: *¿Dónde está físicamente mi información?*  
Azure tiene presencia en más de 140 países y aproximadamente 60 regiones.

* **Regiones:**  
  Son conjuntos de centros de datos ubicados dentro de un perímetro definido y conectados mediante una red dedicada de baja latencia.
* **Zonas de Disponibilidad (Availability Zones):**  
  Son centros de datos físicamente separados dentro de la misma región. Cada zona cuenta con alimentación, refrigeración y conectividad independientes. Azure maneja redundancia zonal bajo una regla de "trilogía" (distribución en 3 zonas dentro de la región primaria) para garantizar que si una zona sufre un incidente, las otras sigan operando.
* **Pares de Regiones (Region Pairs):**  
  Cada región de Azure está emparejada con otra dentro de la misma geografía, separadas por una distancia mínima recomendada de 500 km (aproximadamente 300 millas) para protegerse de desastres naturales o eventos geopolíticos mayores.
* **Azure Government:**  
  Nube dedicada y aislada construida exclusivamente para cumplir con los estándares de cumplimiento y seguridad de entidades gubernamentales de EE. UU.

---

## 5. Jerarquía de Gobierno y Organización en Azure

**Daniel Vacas:**  
Para administrar recursos de forma estructurada, Azure define cuatro niveles jerárquicos:

1. **Grupos de Administración (Management Groups):**  
   Permiten gestionar el acceso, las políticas y el cumplimiento normativo en múltiples suscripciones a la vez.
2. **Suscripciones (Subscriptions):**  
   Es el contenedor que delimita la facturación y los límites de pago.  
   *Analogía:* Si los recursos son juguetes, la suscripción es el anaquel donde caben los juguetes y la mesada que asignaban los padres para adquirirlos.
3. **Grupos de Recursos (Resource Groups):**  
   Contenedores lógicos donde se agrupan y gestionan los recursos relacionados (ciclo de vida, permisos RBAC y etiquetas de costos).
4. **Recursos:**  
   Las instancias individuales de servicios creados (máquinas virtuales, cuentas de almacenamiento, bases de datos, redes virtuales).

---

## 6. Servicios de Cómputo y Redes

**Daniel Vacas:**  
Dentro del catálogo de cómputo encontramos diferentes niveles de abstracción:

* **Máquinas Virtuales (Azure VMs - IaaS):**  
  Control total del sistema operativo (Windows o Linux), red, puertos y almacenamiento.
* **Azure Bastion:**  
  Servicio PaaS que permite conectarse a máquinas virtuales mediante un navegador web a través de SSL/TLS (puerto 443), eliminando la necesidad de exponer IPs públicas o abrir puertos RDP (3389) y SSH (22).
* **Contenedores:**
  * **ACI (Azure Container Instances):** Despliegue rápido e inmediato de contenedores individuales sin gestionar servidores.
  * **ACA (Azure Container Apps):** Entorno PaaS serverless para microservicios y arquitecturas basadas en contenedores con escalado dinámico (incluyendo soporte de GPU y escalado KEDA).
  * **AKS (Azure Kubernetes Service):** Clúster administrado para orquestación empresarial compleja de contenedores.
* **Azure Functions (Serverless):**  
  Servicio basado en eventos que ejecuta código bajo demanda. En el plan consumo (*Consumption Plan*), ofrece hasta 1 millón de ejecuciones gratuitas por mes y un tiempo máximo de ejecución por defecto. Soporta lenguajes como C# (.NET), Python, Node.js y Java.
* **Azure App Service (PaaS):**  
  Alojamiento administrado para aplicaciones web y APIs en entornos Windows o Linux (soporta .NET, Node.js, Java, Python y PHP).
* **Redes Virtuales y Conectividad:**
  * **Azure Virtual Network (VNet):** Segmentación y aislamiento de red en la nube.
  * **VNet Peering:** Interconexión directa y de baja latencia entre dos redes virtuales.
  * **VPN Gateway:** Conexión segura sitio a sitio entre redes locales y Azure.
  * **ExpressRoute:** Conexión privada, dedicada y de alta velocidad entre la infraestructura local y los centros de datos de Microsoft, sin pasar por el internet público.

---

## 7. Almacenamiento en Azure (Azure Storage)

**Daniel Vacas:**  
Clasificación de datos:
1. **Estructurada:** Tablas relacionales con esquemas estrictos.
2. **Semiestructurada:** Documentos JSON/NoSQL sin esquema rígido.
3. **No estructurada:** Archivos binarios, imágenes, videos, logs y copias de seguridad.

### Tipos de Redundancia de Almacenamiento:
* **LRS (Locally Redundant Storage):** 3 copias dentro de un único centro de datos en la región primaria.
* **ZRS (Zone-Redundant Storage):** 3 copias distribuidas sincrónicamente entre 3 zonas de disponibilidad de la región primaria.
* **GRS (Geo-Redundant Storage):** 3 copias en la región primaria (LRS) y replicación asincrónica a una región secundaria (3 copias adicionales).
* **GZRS (Geo-Zone-Redundant Storage):** Replicación zonal (ZRS) en la región primaria y replicación a un centro de datos en la región secundaria.

### Servicios dentro de Azure Storage Account:
* **Azure Blob Storage:** Almacenamiento masivo de objetos no estructurados (imágenes, documentos, backups, Data Lakes).
* **Azure Files:** Recursos compartidos de archivos totalmente administrados y accesibles mediante el protocolo SMB/NFS.
* **Azure Queues:** Almacenamiento de mensajes en cola (hasta 64 KB por mensaje) con procesamiento FIFO para desacoplar componentes.
* **Azure Tables:** Almacén NoSQL tipo clave-valor para conjuntos de datos estructurados no relacionales.

### Niveles de Acceso en Blob Storage y Ciclo de Vida:
* **Frecuente (Hot):** Optimizado para datos que se consultan constantemente; menor costo de acceso, mayor costo de almacenamiento.
* **Esporádico (Cool):** Para datos que no se acceden por al menos 30 días; menor costo de almacenamiento, mayor costo de lectura.
* **Frío (Cold):** Para datos con poco acceso por al menos 90 días.
* **Archivado (Archive):** Para datos históricos que rara vez se consultan (al menos 180 días fuera de línea); costo de almacenamiento mínimo, pero requiere varias horas para rehidratar los datos.
* **Políticas de Ciclo de Vida (Lifecycle Management):** Reglas automatizadas para mover blobs entre niveles (Hot → Cool → Cold → Archive) o eliminarlos según su antigüedad.

---

## 8. Identidad, Seguridad y Costos

**Daniel Vacas:**  
* **Microsoft Entra ID (anteriormente Azure Active Directory):** Servicio de gestión de identidades y accesos en la nube.
* **Autenticación vs. Autorización:**  
  * *Analogía de la discoteca:*
    * **Autenticación (Identificación):** Cuando llegas y muestras tu documento de identidad (DNI/cédula) para verificar quién eres.
    * **Autorización (Permisos):** Una vez adentro, intentar ingresar a una puerta que dice *"Solo Personal Autorizado"*. Si no tienes el rol ni la acreditación adecuada, no tienes permiso de entrar.
* **MFA (Multi-Factor Authentication):** Solicita dos o más elementos de verificación (contraseña + llamada/código por SMS o Microsoft Authenticator).
* **B2B y B2C:** Mecanismos de colaboración para identidades externas y clientes.
* **Acceso Condicional (Conditional Access):** Políticas inteligentes basadas en señales (ubicación geográfica por IP, dispositivo administrado, nivel de riesgo del usuario).
* **Seguridad Avanzada:** Integración de soluciones como Microsoft Sentinel (SIEM/SOAR) y Microsoft Defender for Cloud.
* **Factores de Costos:**
  1. Tipo y familia del recurso.
  2. Modelo de consumo (pago por uso vs. instancias reservadas).
  3. Ubicación geográfica y región.
  4. Tráfico de red saliente (egreso).
* **Herramientas de Costos:** Calculadora de Precios de Azure y Azure Policy para imponer restricciones de gobernanza presupuestaria y cumplimiento técnico.

---

## 9. Herramientas de Administración e Infraestructura como Código (IaC)

**Daniel Vacas:**  
Existen múltiples interfaces para gestionar la plataforma:
* **Azure Portal:** Interfaz gráfica web principal.
* **Azure PowerShell y Azure CLI:** Líneas de comandos para automatización en Windows, Linux y macOS.
* **Azure Cloud Shell:** Terminal interactiva accesible directamente desde el navegador (soporta Bash y PowerShell).
* **Azure Resource Manager (ARM Templates):** Plantillas declarativas en formato JSON.
* **Bicep:** Lenguaje de dominio específico (DSL) desarrollado por Microsoft más conciso y legible que JSON.
* **Terraform:** Herramienta agnóstica de HashiCorp para definir y versionar infraestructura como código.

---

## 10. Sesión de Preguntas y Respuestas (Q&A)

**Luis Martínez:**  
Daniel, por aquí tenemos varias preguntas de los participantes en el chat.

**Jorge pregunta:** *¿Azure usa la topología de Clos en sus servidores?*  
**Daniel Vacas:**  
Azure no utiliza una única topología en su red física. Utiliza varias topologías de red avanzadas (incluyendo Clos modificadas y arquitecturas *leaf-spine* de gran escala) dependiendo de los centros de datos, los servicios y los requerimientos de latencia de cada clúster.

**José pregunta:** *¿Para qué se utiliza Azure API Management?*  
**Daniel Vacas:**  
Azure API Management actúa como una puerta de enlace (*gateway*) centralizada frente a tus microservicios y APIs. Permite administrar políticas de seguridad, autenticación mediante tokens, limitación de tasa (*rate limiting* / *throttling*), transformación de peticiones, almacenamiento en caché y proveer un portal para desarrolladores.

**Pregunta del chat:** *¿Qué diferencia hay entre escalabilidad vertical y horizontal?*  
**Daniel Vacas:**  
* **Vertical (Scale Up):** Como estirar un chicle; significa darle más poder al mismo servidor (más vCPUs, más memoria RAM, mejor disco).  
* **Horizontal (Scale Out):** Añadir más instancias idénticas del servidor o máquina virtual para repartir la carga de trabajo detrás de un balanceador.

**Pregunta del chat:** *¿En qué situaciones no recomendarías usar Azure?*  
**Daniel Vacas:**  
En casos estrictamente necesarios donde la empresa o entidad regulatoria exija mantener todo 100% *on-premise* bajo un modelo CAPEX (compra de activos físicos propios), con aislamiento físico absoluto de internet y personal interno exclusivo para el mantenimiento de hardware (como ciertas bóvedas de datos de banca centralizada).

**Pregunta del chat:** *¿Qué ventajas tiene el uso de PowerShell para administración frente al Portal de Azure?*  
**Daniel Vacas:**  
La velocidad de ejecución, la automatización repetitiva mediante scripts y el acceso a configuraciones avanzadas, propiedades e IDs específicos que a veces no están visibles en la interfaz gráfica.

**Pregunta del chat:** *¿Azure SDK versus Terraform, cuál escoger y cuándo?*  
**Daniel Vacas:**  
Para aprovisionar y gestionar el ciclo de vida de la infraestructura de forma repetible y declarativa, la mejor práctica es **Terraform**. Si requieres interactuar programáticamente con los recursos de Azure desde el código de tu propia aplicación en tiempo de ejecución (por ejemplo, subir un blob o consultar una cola desde C# o Python), utilizas el **Azure SDK**.

**Jorge pregunta:** *Si realizo una escalabilidad horizontal, ¿cómo se llama el servicio o gateway que se utiliza en Azure?*  
**Daniel Vacas:**  
Se puede utilizar **Azure Load Balancer** para balanceo de nivel de red (Capa 4 - TCP/UDP) o **Azure Application Gateway** para nivel de aplicación (Capa 7 - HTTP/HTTPS con soporte de SSL offloading y Web Application Firewall).

**Mahatma pregunta:** *¿El sistema de contenedores de Azure se puede comparar con Docker o qué diferencia tiene?*  
**Daniel Vacas:**  
Está basado en los mismos estándares de Docker/OCI. Puedes usar tus imágenes creadas localmente y subirlas a Azure Container Registry (ACR), para luego ejecutarlas en ACI, ACA o AKS.

**Mike pregunta:** *¿Algún tip o hint para el hackatón?*  
**Daniel Vacas:**  
Revisen minuciosamente los módulos y rutas de aprendizaje en **Microsoft Learn** (especialmente los fundamentos de AZ-900). Hay laboratorios guiados y resúmenes que fortalecen exactamente los conceptos que se evalúan en los retos.

**Yacel pregunta:** *A nivel de cómputo y contenedores, ¿en qué escenarios reales es recomendable optar por Azure frente a GCP en costos?*  
**Daniel Vacas:**  
Depende fuertemente de los acuerdos corporativos y del licenciamiento previo (por ejemplo, Azure Hybrid Benefit para Windows Server y SQL Server). A nivel de gestión integral, monitoreo nativo y gobierno corporativo con Entra ID, Azure ofrece una suite sumamente robusta y centralizada.

**Pregunta del chat:** *¿Cuál es la diferencia entre Azure SQL Database y Azure Storage?*  
**Daniel Vacas:**  
Azure SQL Database es un motor de base de datos relacional transaccional (RDBMS) basado en SQL Server con soporte de transacciones ACID y consultas complejas. Azure Storage es una plataforma de almacenamiento de objetos no relacionales a gran escala (blobs, archivos, colas y tablas clave-valor).

**Juan pregunta:** *¿Cómo determina Azure cuántos recursos de cómputo dedicarle a una función?*  
**Daniel Vacas:**  
En el plan de consumo de Azure Functions, el controlador de escalado (*Scale Controller*) monitorea la tasa de eventos entrantes. Cuando la demanda o uso de memoria/cómputo sobrepasa los umbrales configurados (comúnmente alrededor del 80%), aprovisiona automáticamente nuevas instancias de ejecución en paralelo.

---

## 11. Cierre del Taller

**Daniel Vacas:**  
Eso sería todo por el día de hoy. Les agradezco mucho por su tiempo, participación y presencia. Mi nombre es Daniel Vacas, a sus órdenes. Te cedo el paso, Luis.

**Luis Martínez:**  
Excelente, Daniel. Muchísimas gracias por toda la información y la experiencia compartida. Se nota el dominio y la experticia en Azure.

Muchachos, les recordamos que en el chat dejamos el enlace del formulario de asistencia y el link del equipo de Business IT para registrarse y obtener la acreditación correspondiente. El próximo martes continuaremos con el Workshop #2 enfocado en **Aplicaciones Modernas: API, Frontend y Backend** a la misma hora (7:00 PM hora de Panamá).

**Larisa Aguilar:**  
Buenas noches a todos, gracias por conectarse. Estén muy pendientes de los canales oficiales y de los siguientes workshops todos los martes.

**Luis Martínez:**  
Muchas gracias a todos, al equipo de Business IT y a la organización del Hackatón Copa. ¡Que tengan una excelente noche!