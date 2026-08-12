import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { API_BASE } from '../config/api';
import { AuthService } from './auth.service';

/** Anexa o Bearer token nas chamadas à própria API. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).token;
  if (token && req.url.startsWith(API_BASE)) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }
  return next(req);
};
