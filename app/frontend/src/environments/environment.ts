export interface Environment {
  production: boolean;
  apiBaseUrl: string;
  services: Record<string, string | undefined>;
}

export const environment: Environment = {
  production: false,
  apiBaseUrl: 'http://localhost:5300',
  services: {
    FLIGHTS_ENDPOINT_URL: '/api/flights',
    RESERVATIONS_ENDPOINT_URLX: '/api/reservations'
  }
};
