import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { TranslationService } from '../i18n/translation.service';

/** Attaches JWT Bearer token and Accept-Language header to every outgoing request. */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const ts = inject(TranslationService);
  const token = auth.token();
  const lang = ts.lang();

  const headers: Record<string, string> = { 'Accept-Language': lang };
  if (token) headers['Authorization'] = `Bearer ${token}`;

  return next(req.clone({ setHeaders: headers }));
};
