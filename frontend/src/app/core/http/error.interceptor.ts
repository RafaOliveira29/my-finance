import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';

/** Em 401 de sessão expirada, desloga e volta ao login; demais erros seguem para quem chamou. */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && auth.isAuthenticated()) {
        auth.logout();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    }),
  );
};
