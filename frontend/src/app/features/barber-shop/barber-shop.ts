import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  BarberShopResponse,
  BarberShopService
} from '../../core/services/barber-shop.service';

@Component({
  selector: 'app-barber-shop',
  imports: [FormsModule, NgIf],
  templateUrl: './barber-shop.html',
  styleUrl: './barber-shop.scss'
})
export class BarberShop implements OnInit {
  barberShop: BarberShopResponse | null = null;

  name = '';
  phone = '';

  isLoading = true;
  isSaving = false;
  successMessage = '';
  errorMessage = '';

  constructor(private barberShopService: BarberShopService) {}

  ngOnInit(): void {
    this.loadBarberShop();
  }

  loadBarberShop(): void {
    this.isLoading = true;

    this.barberShopService.getAll().subscribe({
      next: (barberShops) => {
        this.barberShop = barberShops[0] ?? null;

        if (this.barberShop) {
          this.name = this.barberShop.name;
          this.phone = this.formatPhone(this.barberShop.phone);
        }

        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erro ao buscar barbearia:', error);
        this.errorMessage = 'Não foi possível carregar os dados da barbearia.';
        this.isLoading = false;
      }
    });
  }

  formatPhone(value: string): string {
    const numbers = value.replace(/\D/g, '').slice(0, 11);

    if (numbers.length <= 2) {
      return numbers;
    }

    if (numbers.length <= 7) {
      return `(${numbers.slice(0, 2)}) ${numbers.slice(2)}`;
    }

    return `(${numbers.slice(0, 2)}) ${numbers.slice(2, 7)}-${numbers.slice(7)}`;
  }

  onSubmit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (!this.name.trim() || !this.phone.trim()) {
      this.errorMessage = 'Informe o nome e o WhatsApp da barbearia.';
      return;
    }

    this.isSaving = true;

    const payload = {
      name: this.name.trim(),
      phone: this.formatPhone(this.phone.trim())
    };

    if (this.barberShop) {
      this.barberShopService.update(this.barberShop.id, payload).subscribe({
        next: () => {
          this.successMessage = 'Dados da barbearia atualizados com sucesso.';
          this.isSaving = false;
          this.loadBarberShop();
        },
        error: (error) => {
          console.error('Erro ao atualizar barbearia:', error);
          this.errorMessage = 'Não foi possível atualizar a barbearia.';
          this.isSaving = false;
        }
      });

      return;
    }

    this.barberShopService.create(payload).subscribe({
      next: (createdBarberShop) => {
        this.barberShop = createdBarberShop;
        this.phone = this.formatPhone(createdBarberShop.phone);
        this.successMessage = 'Barbearia cadastrada com sucesso.';
        this.isSaving = false;
      },
      error: (error) => {
        console.error('Erro ao criar barbearia:', error);
        this.errorMessage = 'Não foi possível cadastrar a barbearia.';
        this.isSaving = false;
      }
    });
  }
}