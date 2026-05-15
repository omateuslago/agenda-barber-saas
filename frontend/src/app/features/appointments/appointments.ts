import { Component, OnInit } from '@angular/core';
import { DatePipe, DecimalPipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  AppointmentResponse,
  AppointmentService
} from '../../core/services/appointment.service';
import {
  BarberResponse,
  BarberService
} from '../../core/services/barber.service';
import {
  BarberServiceService,
  ServiceResponse
} from '../../core/services/barber-service.service';

@Component({
  selector: 'app-appointments',
  imports: [FormsModule, NgIf, NgFor, DatePipe, DecimalPipe],
  templateUrl: './appointments.html',
  styleUrl: './appointments.scss'
})
export class Appointments implements OnInit {
  appointments: AppointmentResponse[] = [];
  barbers: BarberResponse[] = [];
  services: ServiceResponse[] = [];

  customerName = '';
  customerPhone = '';
  startsAt = '';
  barberId: number | null = null;
  serviceId: number | null = null;

  selectedDate = this.getTodayInputDate();

  isLoading = true;
  isSaving = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private appointmentService: AppointmentService,
    private barberService: BarberService,
    private barberServiceService: BarberServiceService
  ) { }

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.barberService.getAll().subscribe({
      next: (barbers) => {
        this.barbers = barbers;

        this.barberServiceService.getAll().subscribe({
          next: (services) => {
            this.services = services;
            this.loadAppointments();
          },
          error: (error) => {
            console.error('Erro ao buscar serviços:', error);
            this.errorMessage = 'Não foi possível carregar os serviços.';
            this.isLoading = false;
          }
        });
      },
      error: (error) => {
        console.error('Erro ao buscar barbeiros:', error);
        this.errorMessage = 'Não foi possível carregar os barbeiros.';
        this.isLoading = false;
      }
    });
  }

  loadAppointments(): void {
    this.errorMessage = '';

    this.appointmentService.getAll(this.selectedDate).subscribe({
      next: (appointments) => {
        this.appointments = appointments;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erro ao buscar agendamentos:', error);
        this.errorMessage = 'Não foi possível carregar os agendamentos.';
        this.isLoading = false;
      }
    });
  }

  onDateChange(): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.isLoading = true;
    this.loadAppointments();
  }

  onSubmit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (!this.customerName.trim() || !this.customerPhone.trim()) {
      this.errorMessage = 'Informe o nome e o WhatsApp do cliente.';
      return;
    }

    if (!this.startsAt || !this.barberId || !this.serviceId) {
      this.errorMessage = 'Escolha o horário, barbeiro e serviço.';
      return;
    }

    this.isSaving = true;

    const payload = {
      customerName: this.customerName.trim(),
      customerPhone: this.formatPhone(this.customerPhone.trim()),
      startsAt: new Date(this.startsAt).toISOString(),
      barberId: Number(this.barberId),
      serviceId: Number(this.serviceId)
    };

    this.appointmentService.create(payload).subscribe({
      next: () => {
        this.errorMessage = '';
        this.successMessage = 'Agendamento criado com sucesso.';
        this.isSaving = false;
        this.resetForm();
        this.loadAppointments();
      },
      error: (error) => {
        console.error('Erro ao criar agendamento:', error);
        this.errorMessage =
          error.error?.message ?? 'Não foi possível criar o agendamento.';
        this.isSaving = false;
      }
    });
  }

  updateStatus(appointment: AppointmentResponse, status: string): void {
    this.appointmentService.updateStatus(appointment.id, status).subscribe({
      next: () => {
        this.successMessage = 'Status atualizado com sucesso.';
        this.loadAppointments();
      },
      error: (error) => {
        console.error('Erro ao atualizar status:', error);
        this.errorMessage = 'Não foi possível atualizar o status.';
      }
    });
  }

  deleteAppointment(appointment: AppointmentResponse): void {
    const confirmed = confirm(
      `Deseja excluir o agendamento de "${appointment.customerName}"?`
    );

    if (!confirmed) {
      return;
    }

    this.appointmentService.delete(appointment.id).subscribe({
      next: () => {
        this.successMessage = 'Agendamento removido com sucesso.';
        this.loadAppointments();
      },
      error: (error) => {
        console.error('Erro ao excluir agendamento:', error);
        this.errorMessage = 'Não foi possível excluir o agendamento.';
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

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      Scheduled: 'Agendado',
      Completed: 'Concluído',
      Cancelled: 'Cancelado'
    };

    return labels[status] ?? status;
  }

  resetForm(): void {
    this.customerName = '';
    this.customerPhone = '';
    this.startsAt = '';
    this.barberId = null;
    this.serviceId = null;
  }

  private getTodayInputDate(): string {
    const today = new Date();
    return today.toISOString().slice(0, 10);
  }
}