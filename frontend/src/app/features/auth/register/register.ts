import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  imports: [RouterLink, FormsModule, NgIf],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {
  fullName = '';
  email = '';
  password = '';

  isLoading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    this.errorMessage = '';

    if (!this.fullName || !this.email || !this.password) {
      this.errorMessage = 'Preencha todos os campos para criar sua conta.';
      return;
    }

    this.isLoading = true;

    this.authService.register({
      fullName: this.fullName.trim(),
      email: this.email.trim(),
      password: this.password
    }).subscribe({
      next: (response) => {
        console.log('Conta criada:', response);

        this.isLoading = false;
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        console.error('Erro no cadastro:', error);

        this.isLoading = false;
        this.errorMessage =
          error.error?.message ?? 'Não foi possível criar sua conta. Tente novamente.';
      }
    });
  }
}