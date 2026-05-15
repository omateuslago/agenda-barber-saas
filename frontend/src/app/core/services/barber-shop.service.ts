import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface BarberShopResponse {
  id: number;
  name: string;
  phone: string;
  createdAt: string;
}

export interface CreateBarberShopRequest {
  name: string;
  phone: string;
}

export interface UpdateBarberShopRequest {
  name: string;
  phone: string;
}

@Injectable({
  providedIn: 'root'
})
export class BarberShopService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<BarberShopResponse[]> {
    return this.http.get<BarberShopResponse[]>(`${environment.apiUrl}/BarberShops`);
  }

  create(data: CreateBarberShopRequest): Observable<BarberShopResponse> {
    return this.http.post<BarberShopResponse>(`${environment.apiUrl}/BarberShops`, data);
  }

  update(id: number, data: UpdateBarberShopRequest): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/BarberShops/${id}`, data);
  }
}