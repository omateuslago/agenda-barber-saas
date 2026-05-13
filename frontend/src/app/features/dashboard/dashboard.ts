import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  imports: [NgIf],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  fullName = '';
  email = '';
  isLoading = true;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.me().subscribe({
      next: (user) => {
        this.fullName = user.fullName;
        this.email = user.email;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erro ao buscar usuário:', error);
        this.isLoading = false;
      }
    });
  }
}