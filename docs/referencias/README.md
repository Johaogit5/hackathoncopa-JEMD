# Referencias externas para consultar durante el reto

Índice comentado de repositorios, laboratorios y documentación que complementan el material oficial de [`../contexto/`](../contexto/). Aquí solo hay enlaces y resúmenes propios: no se copió contenido de terceros (cada repo tiene su propia licencia).

**Cómo usarlo:** busca primero por el síntoma en [la charla 2](../contexto/02-charla2-operacion-bajo-presion.md); si necesitas una consulta, un ejemplo de Terraform o un procedimiento que no esté ahí, viene a esta página.

> Recopilado el 2026-10-31. Las entradas marcadas con ✅ se comprobaron en una búsqueda ese día; las marcadas con ⚠️ son direcciones previsibles que no se verificaron, así que confirma que existen antes de depender de ellas.

## 1. Retos del mismo estilo (romper y diagnosticar)

| Recurso | Qué tiene | Por qué sirve |
|---|---|---|
| ✅ [What The Hack: Azure Monitoring (007)](https://microsoft.github.io/WhatTheHack/007-AzureMonitoring/) | Hack de Microsoft por retos: Azure Monitor, Log Analytics, Application Insights y Workbooks | Es lo más parecido al track Microsoft: practicar el ciclo de detectar y observar con App Insights |
| ✅ [Frontier SRE Agent: reto 4](https://microsoft.github.io/frontier-sre-agent-rvas/Student/Challenge-04.md) | Lab de Microsoft con infraestructura en Terraform y una app rota por una **variable de entorno mal puesta** | Reproduce casi exacto el caso "variable de entorno" de la charla 2 (sobre Container Apps, no App Service) |
| ✅ [TWL300: troubleshoot en runtime](https://microsoft.github.io/TWL300-GitHub-Copilot-and-platform/docs/03_github_actions_pipeline/03_03.html) y [GitHub Actions](https://microsoft.github.io/TWL300-GitHub-Copilot-and-platform/docs/03_github_actions_pipeline/03_02.html) | Diagnóstico de App Service con errores 504 y "Application Error"; una de las causas es el puerto mal enlazado | Casos reales de App Service y de pipelines que fallan |
| ✅ [What The Hack: Chaos Studio en AKS (055)](https://microsoft.github.io/WhatTheHack/055-ChaosStudio4AKS/) | Inyección de fallas con Azure Chaos Studio en una app de ejemplo | Para entender cómo se "rompe" una app a propósito |
| ✅ [Azure Chaos Studio](https://learn.microsoft.com/azure/chaos-studio) | Documentación oficial; menciona los *Game Days* | Marco oficial para este tipo de ejercicios |
| ✅ [KodeKloud AZ-204: Application Map](https://notes.kodekloud.com/docs/AZ-204-Developing-Solutions-for-Microsoft-Azure/Monitoring-App-Performance/Troubleshooting-App-Performance-by-Using-Application-Map) | Demo "Broken App" que genera excepciones típicas y una base de datos que no conecta | Ver cómo se ve una falla de dependencia en Application Map |

## 2. KQL y Application Insights

| Recurso | Qué tiene | Por qué sirve |
|---|---|---|
| ✅ [microsoft/AzureMonitorCommunity](https://github.com/microsoft/AzureMonitorCommunity) ([wiki](https://github.com/microsoft/AzureMonitorCommunity/wiki)) | Repo oficial y comunitario con consultas KQL, workbooks y alertas, ordenado por servicio de Azure y por escenario | **La primera fuente de consultas ya hechas.** Usar el de `microsoft/`, no los forks |
| ✅ [rod-trent/MustLearnKQL](https://github.com/rod-trent/MustLearnKQL) | Curso completo de KQL: serie de posts, eBook en PDF, workshop por módulos | Para aprender o repasar operadores (`where`, `summarize`, `join`, `bin`) |
| ✅ [App Insights y KQL en la práctica (dev.to)](https://dev.to/manoharij/azure-monitoring-in-practice-application-insights-and-the-kql-queries-4jbi) | Filtra `requests` con `success == false` y sigue las `dependencies` que fallaron | Patrón directo para "endpoint da 500" |
| ✅ [Guía de App Insights sin rodeos (dev.to)](https://dev.to/cristiansifuentes/azure-application-insights-the-no-bs-guide-for-pro-teams-apm-kql-cost-and-gotchas-23l5) | Tablas `requests`, `dependencies`, `exceptions`; p95 de latencia por dependencia | Consultas de arranque y trampas comunes |
| ✅ [KQL para analizar logs (open-exam-prep)](https://open-exam-prep.com/study-guides/ai-200/observability/kql-logs) | Explica la diferencia de nombres de tabla | Ver la advertencia de abajo |

**Dos detalles que ahorran tiempo:**

- **Nombres de tabla:** en Application Insights basado en workspace, desde Log Analytics las tablas se llaman `AppRequests`, `AppExceptions`, `AppDependencies`, `AppTraces`. Desde el recurso de Application Insights se siguen usando `requests`, `exceptions`, `dependencies`, `traces`. Si una consulta "no encuentra la tabla", es esto.
- **Correlación:** `requests`, `dependencies`, `exceptions` y `traces` de una misma petición comparten `operation_Id`. Unirlas por ese campo reconstruye la cascada `/health` → `/health/deep` → `/api/status`.

## 3. Dynatrace y DQL

| Recurso | Qué tiene | Por qué sirve |
|---|---|---|
| ✅ [Usar consultas DQL (docs)](https://docs.dynatrace.com/docs/shortlink/dql-use-queries) | Cómo funciona `fetch` y los rangos de tiempo, por ejemplo `fetch logs, from:now() - 2h` | Referencia base de sintaxis |
| ✅ [Buenas prácticas de DQL](https://docs.dynatrace.com/docs/shortlink/dql-best-practices) | Filtrar pronto con `filter` o `search`, dejar `sort` al final | Consultas más rápidas cuando hay mucho volumen |
| ✅ [Advanced Tracing Analytics](https://docs.dynatrace.com/docs/shortlink/advanced-tracing-analytics) | `fetch spans`, filtrar spans raíz y peticiones fallidas | El equivalente DQL de `requests` con `success == false` |
| ✅ [Dynatrace-Asad-Ali/DQL-Examples](https://github.com/Dynatrace-Asad-Ali/DQL-Examples) | Ejemplos para parsear, filtrar y resumir logs | Comunitario y "en construcción": útil como inspiración, no como referencia oficial |
| ✅ [dynatrace-perfclinics/dynatrace-getting-started](https://github.com/dynatrace-perfclinics/dynatrace-getting-started) | Cómo sacar una prueba gratis, explorar el **Playground** y notebooks guiados (trazas distribuidas, bases de datos, infraestructura) | Practicar DQL con datos reales antes del reto |
| ✅ [DQL: fetch, filter, fields (open-exam-prep)](https://open-exam-prep.com/study-guides/dynatrace-associate/log-analytics-dql/dql-syntax-pipelines) | Guía de estudio de la certificación Associate | Repaso rápido de comandos y funciones de texto |

## 4. Terraform y App Service

| Recurso | Qué tiene | Por qué sirve |
|---|---|---|
| ⚠️ [Azure Verified Modules: `avm-res-web-site`](https://registry.terraform.io/modules/Azure/avm-res-web-site/azurerm/latest) ([repo](https://github.com/Azure/terraform-azurerm-avm-res-web-site), [sitio AVM](https://azure.github.io/Azure-Verified-Modules)) | Módulo oficial de Microsoft para App Service | Ejemplos de cómo se declaran `app_settings`, CORS y Application Insights "bien hechos" |
| ✅ [nventive/terraform-azurerm-linux-docker-app-service](https://github.com/nventive/terraform-azurerm-linux-docker-app-service) | Módulo comunitario con `azurerm_linux_web_app`, mapa `app_settings` y `https_only` | Ejemplo simple para comparar contra nuestro `main.tf` |
| ✅ [Prowler: CORS que permite todo](https://docs.prowler.com/checks/azure/azure-general-policies/ensure-that-cors-disallows-every-resource-to-access-app-services) y [regla de Semgrep](https://registry.semgrep.dev/rule/terraform.azure.security.appservice.azure-appservice-disallowed-cors.azure-appservice-disallowed-cors) | Por qué `allowed_origins = ["*"]` es una mala configuración | Argumento para el CTFd y la bitácora: la corrección de CORS es el origen exacto, nunca `"*"` |

**Dónde mirar en `main.tf`** (sintaxis del proveedor `azurerm` 3.x/4.x; confirmar contra la versión del reto):

```hcl
resource "azurerm_linux_web_app" "api" {
  app_settings = {
    # Un nombre mal escrito aquí = "la app arranca pero un servicio no inicia"
  }
  site_config {
    cors {
      allowed_origins = ["https://<frontend>.azurewebsites.net"] # nunca "*"
    }
  }
}
```

## 5. Diagnóstico en App Service

| Recurso | Qué tiene |
|---|---|
| ✅ [Errores 502 y 503 en App Service (Learn)](https://learn.microsoft.com/bs-latn-ba/azure/app-service/troubleshoot-http-502-http-503) | Guía oficial. Explica **Kudu** (`https://<app>.scm.azurewebsites.net`): variables de entorno, log stream y volcados |
| ✅ [Error HTTP 500 en Web App (Q&A de Learn)](https://learn.microsoft.com/en-us/answers/questions/1316181/web-app-http-error-500) | Activar App Service logs y revisar el log stream |
| ✅ [500 en los endpoints del API (Q&A de Learn)](https://learn.microsoft.com/en-gb/answers/questions/1302937/web-app-service-returns-500-internal-server-error) | Caso en el que abrir CORS no arregló el 500: el error venía de la app. **Un 500 no es un problema de CORS** |
| ✅ [Activar logs y diagnóstico en App Service (OneUptime)](https://oneuptime.com/blog/post/2026-02-16-how-to-enable-application-logging-and-diagnostics-on-azure-app-service/markdown) | Comandos `az webapp log tail` y `az webapp log download` |

Atajos que conviene tener a mano:

| Para qué | Dónde |
|---|---|
| Ver logs en vivo tras un redeploy | `az webapp log tail -n <app> -g <rg>` o Kudu → `/api/logstream` |
| Ver las variables de entorno que **de verdad** recibe la app | Kudu → `/Env` (compararlas con `app_settings` de `main.tf`) |
| Diagnóstico automático | Portal → App Service → *Diagnose and solve problems* |

## 6. Pendiente de completar

- Requisitos nuevos que anuncien los organizadores.
- Integración Dynatrace ↔ Azure App Service, si el reto la usa (OneAgent o extensión del sitio).
- Pipelines (GitHub Actions o Azure DevOps), si el reto incluye despliegue continuo.
