import { environment } from './environments/environment';
import { trackErrorCode } from './telemetry/app-insights';
import type { ApiResponse, AncillaryServiceDto, FlightStatusDto, ReservationSummaryDto } from './types';

// Tipo que Application Insights muestra como "Exception type" para cada error
export const ERROR_TYPES: Record<string, string> = {
  E1: 'Connection Refused',
  E2: 'strict-origin-when-cross-origin',
  E3: 'BadRequest'
};

/**
 * Resuelve la ruta de un modulo desde la configuracion.
 * Si la clave no existe, reporta E3 y devuelve null.
 */
export function resolveEndpoint(key: string): string | null {
  const url = environment.services[key];
  if (url) return url;

  trackErrorCode('E3', `No hay ruta configurada para ${key}`, ERROR_TYPES['E3']);
  return null;
}

// Probe mode:cors: distingue backend inalcanzable de respuesta bloqueada por el navegador
async function classifyNetworkError(): Promise<never> {
  let code = 'E1';
  try {
    const probe = await fetch(
      `${environment.apiBaseUrl}/api/diagnostics/probe?_t=${Date.now()}`,
      { mode: 'cors', cache: 'no-store', signal: AbortSignal.timeout(2000) }
    );
    if (probe.ok) code = 'E2';
  } catch {
    code = 'E1';
  }

  trackErrorCode(
    code,
    code === 'E2'
      ? 'El backend responde pero el navegador bloqueo la respuesta (CORS)'
      : 'No se pudo alcanzar el backend en ' + environment.apiBaseUrl,
    ERROR_TYPES[code]
  );

  throw new Error(`Error ${code}`);
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response;
  try {
    response = await fetch(`${environment.apiBaseUrl}${path}`, init);
  } catch {
    return classifyNetworkError();
  }

  if (!response.ok) {
    const body = await response.json().catch(() => null) as (ApiResponse<unknown> & { errorCode?: string }) | null;

    // El backend reporta el codigo del reto en el cuerpo ("Error E4", "Error E5", ...)
    const code = body?.errorCode ?? body?.error?.match(/\bE[1-5]\b/)?.[0];
    if (code) {
      trackErrorCode(code, `${path} respondio HTTP ${response.status}`, ERROR_TYPES[code]);
      throw new Error(`Error ${code}`);
    }

    if (response.status === 404) {
      trackErrorCode('E3', `La ruta ${path} no existe en el backend`, ERROR_TYPES['E3']);
      throw new Error('Error E3');
    }

    throw new Error(body?.error ?? `HTTP ${response.status}`);
  }

  const body = await response.json() as ApiResponse<T>;
  if (!body.success || body.data === null) throw new Error(body.error ?? 'Error de servicio');
  return body.data;
}

function flightsPath(): string {
  const url = resolveEndpoint('FLIGHTS_ENDPOINT_URL');
  if (!url) throw new Error('Error E3');
  return url;
}

function reservationsPath(): string {
  const url = resolveEndpoint('RESERVATIONS_ENDPOINT_URL');
  if (!url) throw new Error('Error E3');
  return url;
}

export const api = {
  flightByNumber: (number: string) =>
    request<FlightStatusDto>(`${flightsPath()}/${number}`),

  flightsByRoute: (origin: string, destination: string, date: string) =>
    request<FlightStatusDto[]>(`${flightsPath()}/list?origin=${origin}&destination=${destination}&date=${date}`),

  services: (pnr: string) =>
    request<ReservationSummaryDto>(`${reservationsPath()}/${pnr}/ancillary-services`),

  addService: (pnr: string, id: string) =>
    request<AncillaryServiceDto>(`${reservationsPath()}/${pnr}/ancillary-services/${id}`, { method: 'POST' })
};
