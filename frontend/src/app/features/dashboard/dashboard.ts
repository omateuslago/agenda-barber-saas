import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import {
  DashboardResponse,
  DashboardService
} from '../../core/services/dashboard.service';
import {
  BarberShopResponse,
  BarberShopService
} from '../../core/services/barber-shop.service';

@Component({
  selector: 'app-dashboard',
  imports: [NgIf, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  fullName = '';
  email = '';

  isLoadingUser = true;
  isLoadingDashboard = true;

  dashboard: DashboardResponse | null = null;

  barberShop: BarberShopResponse | null = null;
  isLoadingBarberShop = true;

  constructor(
    private authService: AuthService,
    private dashboardService: DashboardService,
    private barberShopService: BarberShopService
  ) { }

  ngOnInit(): void {
    this.loadUser();
    this.loadDashboard();
    this.loadBarberShop();
  }

  private loadUser(): void {
    this.authService.me().subscribe({
      next: (user) => {
        this.fullName = user.fullName;
        this.email = user.email;
        this.isLoadingUser = false;
      },
      error: (error) => {
        console.error('Erro ao buscar usuário:', error);
        this.isLoadingUser = false;
      }
    });
  }

  private loadDashboard(): void {
    this.dashboardService.getDashboard().subscribe({
      next: (dashboard) => {
        this.dashboard = dashboard;
        this.isLoadingDashboard = false;
      },
      error: (error) => {
        console.error('Erro ao buscar dashboard:', error);
        this.isLoadingDashboard = false;
      }
    });
  }

  private loadBarberShop(): void {
    this.barberShopService.getAll().subscribe({
      next: (barberShops) => {
        this.barberShop = barberShops[0] ?? null;
        this.isLoadingBarberShop = false;
      },
      error: (error) => {
        console.error('Erro ao buscar barbearia:', error);
        this.isLoadingBarberShop = false;
      }
    });
  }
}