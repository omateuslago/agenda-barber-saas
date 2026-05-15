import { Component, OnInit } from '@angular/core';
import { DecimalPipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  BarberServiceService,
  ServiceResponse
} from '../../core/services/barber-service.service';
import {
  BarberShopResponse,
  BarberShopService
} from '../../core/services/barber-shop.service';

@Component({
  selector: 'app-services',
  imports: [FormsModule, NgIf, NgFor, DecimalPipe],
  templateUrl: './services.html',
  styleUrl: './services.scss'
})
export class Services implements OnInit {
  barberShop: BarberShopResponse | null = null;
  services: ServiceResponse[] = [];

  editingService: ServiceResponse | null = null;

  name = '';
  price: number | null = null;
  durationMinutes: number | null = null;

  isLoading = true;
  isSaving = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private barberShopService: BarberShopService,
    private barberServiceService: BarberServiceService
  ) {}

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.barberShopService.getAll().subscribe({
      next: (barberShops) => {
        this.barberShop = barberShops[0] ?? null;

        if (!this.barberShop) {
          this.isLoading = false;
          this.errorMessage = 'Cadastre sua barbearia antes de adicionar serviços.';
          return;
        }

        this.loadServices();
      },
      error: (error) => {
        console.error('Erro ao buscar barbearia:', error);
        this.errorMessage = 'Não foi possível carregar sua barbearia.';
        this.isLoading = false;
      }
    });
  }

  loadServices(): void {
    this.barberServiceService.getAll().subscribe({
      next: (services) => {
        this.services = services;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erro ao buscar serviços:', error);
        this.errorMessage = 'Não foi possível carregar os serviços.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (!this.barberShop) {
      this.errorMessage = 'Cadastre sua barbearia antes de adicionar serviços.';
      return;
    }

    if (!this.name.trim() || !this.price || !this.durationMinutes) {
      this.errorMessage = 'Informe nome, preço e duração do serviço.';
      return;
    }

    if (this.price <= 0) {
      this.errorMessage = 'O preço precisa ser maior que zero.';
      return;
    }

    if (this.durationMinutes <= 0) {
      this.errorMessage = 'A duração precisa ser maior que zero.';
      return;
    }

    this.isSaving = true;

    const payload = {
      name: this.name.trim(),
      price: this.price,
      durationMinutes: this.durationMinutes,
      barberShopId: this.barberShop.id
    };

    if (this.editingService) {
      this.barberServiceService.update(this.editingService.id, payload).subscribe({
        next: () => {
          this.successMessage = 'Serviço atualizado com sucesso.';
          this.isSaving = false;
          this.resetForm();
          this.loadServices();
        },
        error: (error) => {
          console.error('Erro ao atualizar serviço:', error);
          this.errorMessage = 'Não foi possível atualizar o serviço.';
          this.isSaving = false;
        }
      });

      return;
    }

    this.barberServiceService.create(payload).subscribe({
      next: () => {
        this.successMessage = 'Serviço cadastrado com sucesso.';
        this.isSaving = false;
        this.resetForm();
        this.loadServices();
      },
      error: (error) => {
        console.error('Erro ao criar serviço:', error);
        this.errorMessage = 'Não foi possível cadastrar o serviço.';
        this.isSaving = false;
      }
    });
  }

  startEdit(service: ServiceResponse): void {
    this.editingService = service;
    this.name = service.name;
    this.price = service.price;
    this.durationMinutes = service.durationMinutes;
    this.successMessage = '';
    this.errorMessage = '';
  }

  cancelEdit(): void {
    this.resetForm();
  }

  deleteService(service: ServiceResponse): void {
    const confirmed = confirm(`Deseja excluir o serviço "${service.name}"?`);

    if (!confirmed) {
      return;
    }

    this.barberServiceService.delete(service.id).subscribe({
      next: () => {
        this.successMessage = 'Serviço removido com sucesso.';
        this.loadServices();
      },
      error: (error) => {
        console.error('Erro ao excluir serviço:', error);
        this.errorMessage = 'Não foi possível excluir o serviço.';
      }
    });
  }

  resetForm(): void {
    this.editingService = null;
    this.name = '';
    this.price = null;
    this.durationMinutes = null;
  }
}