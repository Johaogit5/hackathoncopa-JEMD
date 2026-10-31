# Despliegue del reto Azure, paso a paso

Código en `app/` (rama `reto-azure`). E3, E4, E5 y E6 ya están corregidos en el código. **E1 y E2 necesitan las URLs reales** que entrega `terraform output`.

## 1. Infraestructura (un solo integrante, Azure Cloud Shell → Bash)

Verificar antes: suscripción `Azure subscription 1`, directorio `marcoayalalitumaoutlook.onmicrosoft.com` y grupo `rg-team-<N>`.

```bash
git clone https://github.com/HackathonLabsNetworks/2026-Panama-Microsoft-IaC.git
cd 2026-Panama-Microsoft-IaC
chmod +x deploy.ps1
./deploy.ps1 -TeamId <N>
terraform output
```

Anotar en la [bitácora](../BITACORA.md): `frontend_url`, `backend_url`, `frontend_app_name` y `backend_app_name`. No hay que modificar los `.tf`.

## 2. Corregir E1 y E2 con las URLs reales

- **E1:** en `app/frontend/src/environments/environment.prod.ts`, `apiBaseUrl` = `backend_url` (sin `/` final).
- **E2:** en `app/backend/CopaPassenger.API/appsettings.Production.json`, `Cors.AllowedOrigins` = `[ "<frontend_url>" ]` (https, sin `/` final). **Nunca `"*"`.**

Un commit por error.

## 3. Compilar (en la laptop)

```powershell
# Backend
dotnet test app/backend/CopaPassenger.slnx
dotnet publish app/backend/CopaPassenger.API -c Release -o out/api
Compress-Archive -Path out/api/* -DestinationPath out/api.zip -Force

# Frontend: dist/ incluye server.js y package.json (vienen de public/)
cd app/frontend; npm ci; npm run build; cd ../..
Compress-Archive -Path app/frontend/dist/* -DestinationPath out/front.zip -Force
```

Para comprobar que la URL quedó incrustada: `Select-String -Path app/frontend/dist/assets/*.js -Pattern "<host del backend>"`.

## 4. Desplegar

Requiere `az login` hecho por el integrante (desde una IP registrada), o subir los ZIP a Cloud Shell.

```powershell
az webapp deploy -g rg-team-<N> -n hackathon-copa-panama-2026-<N>-api      --src-path out/api.zip   --type zip
az webapp deploy -g rg-team-<N> -n hackathon-copa-panama-2026-<N>-frontend --src-path out/front.zip --type zip
```

Si alguna app no arranca (revisar Log stream), configurar su comando de inicio:

```powershell
az webapp config set -g rg-team-<N> -n hackathon-copa-panama-2026-<N>-api      --startup-file "dotnet CopaPassenger.API.dll"
az webapp config set -g rg-team-<N> -n hackathon-copa-panama-2026-<N>-frontend --startup-file "node server.js"
```

## 5. Validar (en orden: E1–E3 → E4 → E5 → E6)

```bash
curl -i <backend_url>/health/deep          # 200, "status":"Healthy"     (E4)
curl -i <backend_url>/api/status           # 200, "success":true        (E5)
curl -i "<backend_url>/api/flights/list?origin=PTY&destination=BOG&date=<AAAA-MM-DD de hoy>"
curl -i -X POST <backend_url>/api/flights/create -H "Content-Type: application/json" \
  -d '{"flightNumber":"CM900","origin":"PTY","destination":"BOG","scheduledDeparture":"2026-11-01T10:00:00Z","scheduledArrival":"2026-11-01T12:00:00Z","departureGate":"G1","arrivalGate":"A1"}'   # 200 (E6)
curl -i -X OPTIONS <backend_url>/api/flights/list -H "Origin: <frontend_url>" -H "Access-Control-Request-Method: GET"   # Access-Control-Allow-Origin = frontend (E2)
```

En el navegador, abrir `<frontend_url>` con DevTools: Network debe apuntar a `backend_url` (E1), Console sin errores de CORS (E2) y los servicios de los PNR `ABC123`, `DEF456` y `GHI789` deben cargar (E3).

Application Insights → Logs (la telemetría tarda de 1 a 3 minutos):

```kql
exceptions
| where timestamp > ago(30m)
| summarize count() by ErrorCode = tostring(customDimensions.ErrorCode), type
```

```kql
requests
| where timestamp > ago(30m)
| summarize count() by name, resultCode
| order by count_ desc
```

Notas: el backend escucha en el puerto 8080 (`Urls` en `appsettings.Production.json`), que es el que App Service usa por defecto en Linux. La base de datos es en memoria, así que los vuelos creados se pierden al reiniciar la app.
