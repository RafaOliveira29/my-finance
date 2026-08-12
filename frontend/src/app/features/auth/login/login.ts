import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="auth-card">
      <h1>Entrar</h1>
      <form [formGroup]="form" (ngSubmit)="submit()">
        <label>
          E-mail
          <input type="email" formControlName="email" autocomplete="email" />
        </label>
        <label>
          Senha
          <input type="password" formControlName="password" autocomplete="current-password" />
        </label>
        @if (error()) {
          <p class="auth-error">{{ error() }}</p>
        }
        <button type="submit" [disabled]="loading()">{{ loading() ? 'Entrando…' : 'Entrar' }}</button>
      </form>
      <p class="auth-alt">Não tem conta? <a routerLink="/register">Criar conta</a></p>
    </section>
  `,
})
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading.set(true);
    this.error.set(null);
    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: (err) => {
        this.error.set(err?.error?.detail ?? err?.error?.title ?? 'E-mail ou senha inválidos.');
        this.loading.set(false);
      },
    });
  }
}
