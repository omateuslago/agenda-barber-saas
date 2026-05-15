import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AppointmentResponse {
  id: number;
  customerName: string;
  customerPhone: string;
  startsAt: string;
  status: string;
  barberId: number;
  barberName: string;
  serviceId: number;
  serviceName: string;
  servicePrice: number;
  serviceDurationMinutes: number;
}

export interface CreateAppointmentRequest {
  customerName: string;
  customerPhone: string;
  startsAt: string;
  barberId: number;
  serviceId: number;
}

export interface UpdateAppointmentStatusRequest {
  status: string;
}

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  constructor(private http: HttpClient) {}

  getAll(date?: string, barberId?: number): Observable<AppointmentResponse[]> {
    let params = new HttpParams();

    if (date) {
      params = params.set('date', date);
    }

    if (barberId) {
      params = params.set('barberId', barberId);
    }

    return this.http.get<AppointmentResponse[]>(
      `${environment.apiUrl}/Appointments`,
      { params }
    );
  }

  create(data: CreateAppointmentRequest): Observable<AppointmentResponse> {
    return this.http.post<AppointmentResponse>(
      `${environment.apiUrl}/Appointments`,
      data
    );
  }

  updateStatus(id: number, status: string): Observable<void> {
    return this.http.patch<void>(
      `${environment.apiUrl}/Appointments/${id}/status`,
      { status }
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/Appointments/${id}`);
  }
}