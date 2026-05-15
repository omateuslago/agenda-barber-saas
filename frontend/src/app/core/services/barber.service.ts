import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface BarberResponse {
  id: number;
  name: string;
  barberShopId: number;
}

export interface CreateBarberRequest {
  name: string;
  barberShopId: number;
}

export interface UpdateBarberRequest {
  name: string;
  barberShopId: number;
}

@Injectable({
  providedIn: 'root'
})
export class BarberService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<BarberResponse[]> {
    return this.http.get<BarberResponse[]>(`${environment.apiUrl}/Barbers`);
  }

  create(data: CreateBarberRequest): Observable<BarberResponse> {
    return this.http.post<BarberResponse>(`${environment.apiUrl}/Barbers`, data);
  }

  update(id: number, data: UpdateBarberRequest): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/Barbers/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/Barbers/${id}`);
  }
}