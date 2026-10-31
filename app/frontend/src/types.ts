export type FlightStatus = 'Scheduled' | 'Boarding' | 'Delayed' | 'Cancelled' | 'InFlight' | 'Landed';
export type AncillaryCategory = 'ExtraBaggage' | 'SeatSelection' | 'SpecialMeals';
export interface FlightStatusDto { id: string; flightNumber: string; origin: string; destination: string; status: FlightStatus; statusLabel: string; scheduledDeparture: string; estimatedDeparture: string | null; scheduledArrival: string; estimatedArrival: string | null; departureGate: string | null; arrivalGate: string | null; delayMinutes: number; }
export interface AncillaryServiceDto { serviceId: string; category: AncillaryCategory; categoryLabel: string; name: string; description: string; priceUsd: number; isAvailable: boolean; isAdded: boolean; }
export interface ReservationSummaryDto { pnr: string; passengerName: string; ancillaryServices: AncillaryServiceDto[]; }
export interface ApiResponse<T> { success: boolean; data: T | null; error: string | null; }
