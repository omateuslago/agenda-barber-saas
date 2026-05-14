import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import {
  DashboardResponse,
  DashboardService
} from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  imports: [NgIf],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  fullName = '';
  email = '';

  isLoadingUser = true;
  isLoadingDashboard = true;

  dashboard: DashboardResponse | null = null;

  constructor(
    private authService: AuthService,
    private dashboardService: DashboardService
  ) {}

  ngOnInit(): void {
    this.loadUser();
    this.loadDashboard();
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
}