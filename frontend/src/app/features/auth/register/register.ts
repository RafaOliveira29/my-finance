import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { apiErrorMessage } from '../../../core/http/api-error';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="auth-card">
      <h1>Criar conta</h1>
      <form [formGroup]="form" (ngSubmit)="submit()">
        <label>
          Nome
          <input type="text" formControlName="name" autocomplete="name" />
        </label>
        <label>
          E-mail
          <input type="email" formControlName="email" autocomplete="email" />
        </label>
        <label>
          Senha
          <input type="password" formControlName="password" autocomplete="new-password" />
          <small>Mínimo 8 caracteres.</small>
        </label>
        @if (error()) {
          <p class="auth-error">{{ error() }}</p>
        }
        <button type="submit" [disabled]="loading()">{{ loading() ? 'Criando…' : 'Criar conta' }}</button>
      </form>
      <p class="auth-alt">Já tem conta? <a routerLink="/login">Entrar</a></p>
    </section>
  `,
})
export class Register {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading.set(true);
    this.error.set(null);
    this.auth.register(this.form.getRawValue()).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Não foi possível criar a conta.'));
        this.loading.set(false);
      },
    });
  }
}
