# Transcripción Oficial — Workshop #3: DevOps, Git, GitHub y Terraform

> **Video:** https://www.youtube.com/watch?v=MBCEiNpzfa0 · Transcripción oficial entregada por la organización. Los subtítulos automáticos (para buscar frases textuales) están en [`subtitulos/w3.txt`](subtitulos/w3.txt).

## ⚡ Puntos clave para el reto

- Ciclo: **Desplegar → Observar → Corregir (en el `.tf`, nunca con clics en el portal) → Validar**.
- Terraform: `init` → `fmt` → `validate` → **`plan` (leerlo siempre)** → `apply` → `output`/`show` → `destroy`.
- `validate` solo revisa sintaxis: no detecta cuotas, políticas ni nombres duplicados en Azure.
- `.tf` = lo que queremos; `.tfstate` = lo que Terraform sabe; Azure = la realidad. El tfstate **nunca** va a Git.
- Azure CLI: `az login`, `az account show --output table`, `az account set --subscription ...`; los providers `Microsoft.Web` y `Microsoft.Storage` deben estar *Registered*.

---

**Evento:** Hackatón Copa 2026 — API, DevOps y Observabilidad  
**Instructora:** Emily Chimbo (Business IT)  
**Moderadores y Presentadores:** Luis Martínez, Dominic, Carelis, Hugo Aquino  

---

## 1. Bienvenida e Introducción al Workshop #3

**Luis Martínez:**

Muy buenas noches a todos, muchas gracias por participar en este tercer workshop del Hackatón Copa 2026 con el tema central de este año: **API, DevOps y Observabilidad**. En la noche de hoy llevaremos a cabo la tercera sesión de nuestra serie de talleres.

Los dos workshops anteriores ya se encuentran disponibles en nuestro canal de YouTube (**Hakaton CM**) para que puedan verlos, repasarlos y no perderse ningún detalle de lo compartido, ya que les será de enorme utilidad para la resolución de los retos tanto en la sede de Santiago como en la de Ciudad de Panamá.

Esta noche nos acompaña Hugo Aquino por parte del comité organizador, junto con nuestros moderadores y voluntarios de Business IT: Emily, Dominic y Carelis. El tema de hoy es **DevOps, Git, GitHub y Terraform**, facilitado por el equipo técnico de Business IT.

Para consultas sobre las bases del evento pueden escribir directamente a **jacatoncopa@copair.com** o ingresar al portal oficial **hacatoncopa.com**, donde están publicadas las bases legales. También pueden sumarse al canal de difusión de WhatsApp del Hackatón. Sin más preámbulos, le cedo la palabra a Dominic para iniciar.

**Dominic:**

Muy buenas noches a todos. Bienvenidos a la sesión número tres. Les presento a su instructora del día de hoy, **Emily Chimbo**, quien compartirá todo su conocimiento práctico en DevOps, Git, GitHub y Terraform. Estaremos muy atentos al chat para canalizar sus preguntas. Emily, bienvenida, adelante.

---

## 2. Objetivos y Filosofía de la Cultura DevOps

**Emily Chimbo:**

Muchas gracias, Dominic. Buenas noches con todos. Mi nombre es Emily Chimbo y los acompañaré durante esta sesión.

El objetivo de esta clase no es memorizar herramientas ni comandos aislados; buscamos conectar todos los conceptos para que puedan aplicarlos tanto en sus proyectos cotidianos como en los desafíos de la hackatón. Abordaremos la metodología de trabajo DevOps, el control de versiones, la infraestructura como código y Terraform. Al finalizar, comprenderán con claridad qué infraestructura se construye, qué pasos se ejecutan y cómo diagnosticar un despliegue cuando ocurre algún fallo.

### El ciclo operativo

1. **Desplegar:** Publicar una versión declarada de la infraestructura.
2. **Observar:** Revisar los resultados del plan, los mensajes de error y el estado de los recursos provistos.
3. **Corregir:** Frente a un fallo, nunca recurrir a clics improvisados en el portal; se regresa al código fuente (`.tf`), se corrige la causa y se vuelve a ejecutar.
4. **Validar:** Comprobar que el estado real en la nube coincida con lo declarado.

### DevOps como cultura, no como herramienta

Históricamente existía una desconexión entre el equipo de **Desarrollo** (que escribía el software) y el de **Operaciones** (que lo recibía al final para desplegarlo). Ante un problema en producción, cada equipo se deslindaba de responsabilidades.

DevOps surge para establecer una responsabilidad compartida a lo largo de todo el ciclo de vida:
* El desarrollo no termina con el `git commit`.
* Operaciones interviene desde el inicio de la arquitectura.
* Se comparten objetivos de automatización, observabilidad, retroalimentación continua y acuerdos de nivel de servicio.

> **Importante:** DevOps no es una herramienta, no es Terraform y no es un puesto de trabajo aislado; es la unión armónica de **cultura, prácticas y herramientas**.

* **Git** aporta la trazabilidad local.
* **Terraform** provee la repetibilidad y consistencia del entorno.
* **Azure** ofrece la plataforma para el aprovisionamiento de los recursos.

---

## 3. Control de Versiones: Git vs. GitHub

**Emily Chimbo:**

Aunque suelen mencionarse juntos, cumplen funciones completamente distintas:

* **Git:** Es el sistema de control de versiones distribuido que opera de manera local en el computador. Registra el historial de cambios, quién los hizo, qué archivos se modificaron y en qué momento.
* **GitHub:** Es la plataforma remota y colaborativa en la nube. Permite alojar repositorios, coordinar revisiones de código mediante *Pull Requests*, controlar ramas de trabajo y orquestar flujos de integración continua (CI/CD).

### Manejo de archivos y el archivo `.gitignore`

Al trabajar con infraestructura como código existen archivos que **nunca** deben versionarse ni compartirse en repositorios remotos:
* Directorios temporales (`.terraform/`).
* Archivos de bloqueo y registros de fallo (`crash.log`).
* Archivos de estado local (`terraform.tfstate` y `terraform.tfstate.backup`), ya que pueden contener información sensible, contraseñas o cadenas de conexión en texto plano.
* Variables locales con secretos (`*.tfvars` que contengan credenciales).

El archivo `.gitignore` debe contener estas exclusiones antes de realizar el primer envío al repositorio.

---

## 4. Práctica de Flujo de Trabajo en Git y GitHub

**Emily Chimbo:**

En la práctica, organizamos los cambios a través de Visual Studio Code o la terminal:

1. **Estados del código:**
   * **Changes (Working Directory):** Modificaciones locales aún no preparadas.
   * **Staged Changes (Área de preparación):** Archivos revisados y listos para ser empaquetados mediante `git add`.
2. **Confirmación (Commit):** Empaquetado lógico con un mensaje descriptivo de la acción (`git commit -m "cambios iniciales"`).
3. **Publicación (Push):** Envío de los commits locales hacia la rama remota configurada en GitHub (`git push origin main`).

---

## 5. Preparación del Entorno en Azure CLI

**Emily Chimbo:**

Antes de ejecutar Terraform contra una nube, es necesario autenticar el entorno y validar que se tengan los roles y proveedores requeridos:

1. **Autenticación en la nube:**
   ```bash
   az login
   ```
   Abre el navegador web para iniciar sesión con las credenciales corporativas o personales.

2. **Verificación y selección de suscripción:**
   ```bash
   az account show --output table
   ```
   Si se tiene acceso a varias suscripciones, se debe definir explícitamente en cuál se trabajará:
   ```bash
   az account set --subscription "<ID-o-Nombre-de-la-Suscripcion>"
   ```

3. **Verificación de proveedores de recursos (*Resource Providers*):**
   Para aprovisionar servicios como cuentas de almacenamiento o Azure Functions, los proveedores `Microsoft.Storage` y `Microsoft.Web` deben encontrarse en estado `Registered`. Si aparecen como `Unregistered`, se deben registrar o solicitar al administrador los permisos necesarios (típicamente el rol de *Colaborador / Contributor*).

---

## 6. Flujo de Trabajo e Infraestructura como Código con Terraform

**Emily Chimbo:**

Terraform funciona mediante un ciclo de comandos bien estructurado:

### Paso 1: `terraform init`
Inicializa el directorio de trabajo. Lee los bloques de configuración, descarga los plugins de los proveedores requeridos (como `azurerm` de HashiCorp) dentro de la carpeta `.terraform` y genera el archivo de bloqueo `terraform.lock.hcl` para garantizar que todos los miembros del equipo utilicen exactamente las mismas versiones de las dependencias.

### Paso 2: `terraform fmt`
Normaliza y aplica formato estandarizado a todos los archivos de configuración (`.tf`), asegurando legibilidad, alineación y sangría uniforme.

### Paso 3: `terraform validate`
Revisa de forma estática la sintaxis, tipos de datos y referencias internas de los archivos de Terraform.  
*Nota:* Una validación exitosa garantiza coherencia sintáctica, pero no comprueba restricciones externas en Azure (como cuotas regionales, políticas de gobierno corporativo o nombres duplicados globalmente).

### Paso 4: `terraform plan`
Compara el código escrito con el estado conocido (`terraform.tfstate`) y la realidad en la nube. Presenta una vista previa exacta de lo que sucederá sin realizar cambios aún:
* Recursos que se van a agregar (`+`).
* Recursos que se van a modificar (`~`).
* Recursos que se van a destruir (`-`).

> **Regla de oro:** Nunca se debe ejecutar un `apply` directo sin haber inspeccionado detenidamente el resultado de `plan`. Si el plan sugiere eliminar un recurso de manera inesperada, se debe detener el proceso e investigar.

### Paso 5: `terraform apply`
Calcula el gráfico de dependencias (construyendo primero los elementos base y desplegando en paralelo aquellos que sean independientes), solicita confirmación explícita escribiendo `yes` y comienza el aprovisionamiento real de los recursos en Azure.

---

## 7. Demostración Práctica: Aprovisionamiento de Azure Functions

**Emily Chimbo:**

Durante la práctica desplegamos una arquitectura compuesta por:
1. Un **Random String** para asegurar nombres globalmente únicos.
2. Un **Resource Group** para agrupar los componentes.
3. Una **Storage Account** requerida para el funcionamiento interno del entorno serverless.
4. Un **App Service Plan** en modalidad consumo.
5. Una instancia de **Azure Function App**.

### Comandos de inspección posteriores al despliegue:
* **`terraform output`:** Muestra las variables de salida definidas en la configuración (nombres de los recursos creados, grupos de recursos y la URL pública calculada para el servicio).
* **`terraform show`:** Expone el estado interno detallado que Terraform tiene registrado tras la ejecución.

---

## 8. La Importancia Crítica del Archivo de Estado (`terraform.tfstate`)

**Emily Chimbo:**

Terraform no almacena la arquitectura en memoria volátil; mantiene una copia exacta de la correspondencia entre los recursos declarados y los IDs reales de Azure dentro del archivo `terraform.tfstate`.

* El archivo `.tf` describe lo que queremos.
* El archivo `.tfstate` describe lo que Terraform sabe que existe.
* Azure contiene la realidad de la infraestructura.

### Riesgos asociados al archivo de estado:
1. **Borrado accidental:** Si se elimina el archivo `.tfstate`, Terraform pierde la trazabilidad de los recursos y en la siguiente ejecución intentará crearlos de nuevo, produciendo errores por colisión de nombres.
2. **Seguridad:** El estado contiene valores en texto plano. Nunca debe subirse a Git. En equipos colaborativos de producción, el estado se almacena en un *remote backend* (como un contenedor seguro en Azure Blob Storage) habilitando bloqueo de estado (*state locking*) para evitar colisiones entre despliegues concurrentes.

---

## 9. Limpieza de Infraestructura y Gobierno de Costos

**Emily Chimbo:**

El ciclo operativo responsable de DevOps no culmina con el despliegue, sino con la retirada adecuada de la infraestructura cuando ya no se utiliza:

```bash
terraform destroy
```

Este comando evalúa el archivo de estado, identifica los recursos gestionados y solicita confirmación antes de eliminarlos en el orden inverso a su creación para respetar las dependencias. Una vez concluido el proceso, se verifica en el Portal de Azure que el grupo de recursos haya desaparecido por completo.

Gestionar los costos forma parte del gobierno técnico: crear infraestructura es sencillo, pero mantener recursos ociosos encendidos genera consumos innecesarios.

---

## 10. Sesión de Preguntas y Respuestas (Q&A)

**Participante:**  
*¿Existe alguna documentación oficial que detalle qué propiedades acepta cada recurso en Terraform para agregarlas o modificarlas?*

**Emily Chimbo:**  
Sí, la fuente primaria es el **Terraform Registry** oficial (`registry.terraform.io`). Dentro de la sección del proveedor de Azure (`azurerm`), cada recurso cuenta con su documentación completa, listando argumentos obligatorios, parámetros opcionales y atributos exportados como salidas.

**Participante:**  
*¿Cuál sería el resumen fundamental del propósito de Terraform?*  
**Emily Chimbo:**  
Terraform es una herramienta de infraestructura como código que permite definir, mediante archivos declarativos legibles y versionables, los servicios que se necesitan en la nube. Reemplaza las configuraciones manuales en portales web, garantiza que los entornos puedan reproducirse de manera idéntica y facilita el control de cambios en equipo.

---

## 11. Cierre de la Sesión

**Emily Chimbo:**  
Con esto completamos las bases esenciales de la sesión de hoy. Espero que estos fundamentos les sirvan como punto de partida para adentrarse en la cultura DevOps y utilizarlos eficazmente en los retos del evento. Muchas gracias por su tiempo y atención.

**Luis Martínez:**  
Muchísimas gracias, Emily, por tu excelente explicación y por guiarnos paso a paso a través de la terminal y los conceptos.

A todos los participantes:
1. Les recordamos llenar el **formulario de asistencia** compartido en el chat.
2. Esta grabación estará disponible en nuestro canal de YouTube (**Hakaton CM**) para que puedan pausarla, repasar los comandos y replicar el laboratorio en sus propios entornos.
3. El próximo taller continuará con temas clave de observabilidad, Application Insights y monitoreo.

¡Muchas gracias a todos por su participación y que tengan una excelente noche!