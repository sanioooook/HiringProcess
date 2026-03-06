import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from './auth.service';
import { TranslationService } from '../i18n/translation.service';

/** Attaches JWT Bearer token and Accept-Language header to every outgoing request.
 *  On 401 (token expired): silently refreshes and retries.
 *  If refresh also fails: auto-logout + session-expired snackbar. */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const ts = inject(TranslationService);
  const snack = inject(MatSnackBar);
  const token = auth.token();
  const lang = ts.lang();

  const headers: Record<string, string> = { 'Accept-Language': lang };
  if (token) headers['Authorization'] = `Bearer ${token}`;

  return next(req.clone({ setHeaders: headers })).pipe(
    catchError((error: HttpErrorResponse) => {
      // Only try refresh when a token was sent AND the request wasn't the refresh itself
      if (error.status === 401 && token && !req.url.includes('/auth/refresh')) {
        return auth.refresh().pipe(
          switchMap(newToken =>
            // Retry the original request with the new access token
            next(req.clone({
              setHeaders: { ...headers, Authorization: `Bearer ${newToken}` },
            }))
          ),
          catchError(() => {
            // Refresh token expired or invalid — force logout
            auth.logout();
            snack.open(ts.t('snack.sessionExpired'), 'OK', { duration: 5000 });
            return throwError(() => error);
          }),
        );
      }
      return throwError(() => error);
    }),
  );
};
