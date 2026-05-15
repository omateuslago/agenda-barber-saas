import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ServiceResponse {
  id: number;
  name: string;
  price: number;
  durationMinutes: number;
  barberShopId: number;
}

export interface CreateServiceRequest {
  name: string;
  price: number;
  durationMinutes: number;
  barberShopId: number;
}

export interface UpdateServiceRequest {
  name: string;
  price: number;
  durationMinutes: number;
  barberShopId: number;
}

@Injectable({
  providedIn: 'root'
})
export class BarberServiceService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<ServiceResponse[]> {
    return this.http.get<ServiceResponse[]>(`${environment.apiUrl}/Services`);
  }

  create(data: CreateServiceRequest): Observable<ServiceResponse> {
    return this.http.post<ServiceResponse>(`${environment.apiUrl}/Services`, data);
  }

  update(id: number, data: UpdateServiceRequest): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/Services/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/Services/${id}`);
  }
}