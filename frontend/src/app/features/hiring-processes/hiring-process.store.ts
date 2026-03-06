import { inject } from '@angular/core';
import { EMPTY, firstValueFrom } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { HiringProcess, HiringProcessQuery } from '../../core/api/hiring-process.model';
import { HiringProcessApiService } from '../../core/api/hiring-process-api.service';
import { TranslationService } from '../../core/i18n/translation.service';

type HiringProcessState = {
  items: HiringProcess[];
  total: number;
  loading: boolean;
  error: string | null;
  query: HiringProcessQuery;
};

export const HiringProcessStore = signalStore(
  { providedIn: 'root' },
  withState<HiringProcessState>({
    items: [],
    total: 0,
    loading: false,
    error: null,
    query: { page: 1, pageSize: 20, sortBy: 'updatedAt', sortDirection: 'desc' },
  }),
  withMethods((store,
    api = inject(HiringProcessApiService),
    ts = inject(TranslationService),
  ) => ({

    load: rxMethod<Partial<HiringProcessQuery>>(source$ =>
      source$.pipe(
        switchMap(query => {
          const merged: HiringProcessQuery = { ...store.query(), ...query };
          patchState(store, { loading: true, error: null, query: merged });
          return api.getAll(merged).pipe(
            tap(res => patchState(store, {
              items: res.items,
              total: res.totalCount,
              loading: false,
            })),
            catchError(() => {
              patchState(store, { loading: false, error: ts.t('snack.loadFailed') });
              return EMPTY;
            }),
          );
        }),
      )
    ),

    async delete(id: string): Promise<boolean> {
      try {
        await firstValueFrom(api.delete(id));
        this.load(store.query());
        return true;
      } catch {
        return false;
      }
    },

  })),
);
