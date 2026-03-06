import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { EMPTY } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { AuthService } from './auth.service';
import { LoginRequest, RegisterRequest } from './models/user.model';
import { TranslationService } from '../i18n/translation.service';

type AuthState = {
  loading: boolean;
  error: string | null;
};

export const AuthStore = signalStore(
  { providedIn: 'root' },
  withState<AuthState>({ loading: false, error: null }),
  withMethods((store,
    authService = inject(AuthService),
    router = inject(Router),
    ts = inject(TranslationService),
  ) => ({

    login: rxMethod<LoginRequest>(source$ =>
      source$.pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(request =>
          authService.login(request).pipe(
            tap(() => {
              patchState(store, { loading: false });
              router.navigate(['/app']);
            }),
            catchError((err: any) => {
              patchState(store, {
                loading: false,
                error: err?.error?.message ?? ts.t('snack.saveFailed'),
              });
              return EMPTY;
            }),
          )
        ),
      )
    ),

    register: rxMethod<RegisterRequest>(source$ =>
      source$.pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(request =>
          authService.register(request).pipe(
            tap(() => {
              patchState(store, { loading: false });
              router.navigate(['/app']);
            }),
            catchError((err: any) => {
              patchState(store, {
                loading: false,
                error: err?.error?.message ?? ts.t('snack.saveFailed'),
              });
              return EMPTY;
            }),
          )
        ),
      )
    ),

    clearError(): void {
      patchState(store, { error: null });
    },

  })),
);
