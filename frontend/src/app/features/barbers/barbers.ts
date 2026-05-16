import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  BarberResponse,
  BarberService
} from '../../core/services/barber.service';
import {
  BarberShopResponse,
  BarberShopService
} from '../../core/services/barber-shop.service';

@Component({
  selector: 'app-barbers',
  imports: [FormsModule, NgIf, NgFor],
  templateUrl: './barbers.html',
  styleUrl: './barbers.scss'
})
export class Barbers implements OnInit {
  barberShop: BarberShopResponse | null = null;
  barbers: BarberResponse[] = [];

  editingBarber: BarberResponse | null = null;

  name = '';

  isLoading = true;
  isSaving = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private barberShopService: BarberShopService,
    private barberService: BarberService
  ) { }

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
          this.errorMessage = 'Cadastre sua barbearia antes de adicionar barbeiros.';
          return;
        }

        this.loadBarbers();
      },
      error: (error) => {
        console.error('Erro ao buscar barbearia:', error);
        this.errorMessage = 'Não foi possível carregar sua barbearia.';
        this.isLoading = false;
      }
    });
  }

  loadBarbers(): void {
    this.barberService.getAll().subscribe({
      next: (barbers) => {
        this.barbers = barbers;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erro ao buscar barbeiros:', error);
        this.errorMessage = 'Não foi possível carregar os barbeiros.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (!this.barberShop) {
      this.errorMessage = 'Cadastre sua barbearia antes de adicionar barbeiros.';
      return;
    }

    if (!this.name.trim()) {
      this.errorMessage = 'Informe o nome do barbeiro.';
      return;
    }

    this.isSaving = true;

    const payload = {
      name: this.name.trim(),
      barberShopId: this.barberShop.id
    };

    if (this.editingBarber) {
      this.barberService.update(this.editingBarber.id, payload).subscribe({
        next: () => {
          this.successMessage = 'Barbeiro atualizado com sucesso.';
          this.isSaving = false;
          this.resetForm();
          this.loadBarbers();
        },
        error: (error) => {
          console.error('Erro ao atualizar barbeiro:', error);
          this.errorMessage = 'Não foi possível atualizar o barbeiro.';
          this.isSaving = false;
        }
      });

      return;
    }

    this.barberService.create(payload).subscribe({
      next: () => {
        this.successMessage = 'Barbeiro cadastrado com sucesso.';
        this.isSaving = false;
        this.resetForm();
        this.loadBarbers();
      },
      error: (error) => {
        console.error('Erro ao criar barbeiro:', error);
        this.errorMessage = 'Não foi possível cadastrar o barbeiro.';
        this.isSaving = false;
      }
    });
  }

  startEdit(barber: BarberResponse): void {
    this.editingBarber = barber;
    this.name = barber.name;
    this.successMessage = '';
    this.errorMessage = '';

    setTimeout(() => {
      document.getElementById('barber-form')?.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
      });
    }, 0);
  }

  cancelEdit(): void {
    this.resetForm();
  }

  deleteBarber(barber: BarberResponse): void {
    const confirmed = confirm(`Deseja excluir o barbeiro "${barber.name}"?`);

    if (!confirmed) {
      return;
    }

    this.barberService.delete(barber.id).subscribe({
      next: () => {
        this.successMessage = 'Barbeiro removido com sucesso.';
        this.loadBarbers();
      },
      error: (error) => {
        console.error('Erro ao excluir barbeiro:', error);
        this.errorMessage = 'Não foi possível excluir o barbeiro.';
      }
    });
  }

  resetForm(): void {
    this.editingBarber = null;
    this.name = '';
  }
}