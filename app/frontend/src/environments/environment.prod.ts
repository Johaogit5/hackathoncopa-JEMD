import type { Environment } from './environment';

export const environment: Environment = {
  production: true,
  apiBaseUrl: 'https://api.copa-passenger.example.com',
  services: {
    FLIGHTS_ENDPOINT_URL: '/api/flights',
    RESERVATIONS_ENDPOINT_URLX: '/api/reservations'
  }
};
