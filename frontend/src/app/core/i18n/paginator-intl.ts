import { Injectable, effect, inject } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { Subject } from 'rxjs';
import { TranslationService } from './translation.service';

@Injectable()
export class AppPaginatorIntl extends MatPaginatorIntl {
  private readonly ts = inject(TranslationService);
  override readonly changes = new Subject<void>();

  constructor() {
    super();
    effect(() => {
      this.ts.lang(); // reactive dependency - re-runs on language change
      this.refresh();
    });
  }

  private refresh(): void {
    this.itemsPerPageLabel = this.ts.t('paginator.itemsPerPage');
    this.nextPageLabel = this.ts.t('paginator.nextPage');
    this.previousPageLabel = this.ts.t('paginator.previousPage');
    this.firstPageLabel = this.ts.t('paginator.firstPage');
    this.lastPageLabel = this.ts.t('paginator.lastPage');
    this.changes.next();
  }

  override getRangeLabel = (page: number, pageSize: number, length: number): string => {
    if (length === 0 || pageSize === 0) {
      return this.ts.t('paginator.rangeOf', { start: '0', total: String(length) });
    }
    const start = page * pageSize + 1;
    const end = Math.min((page + 1) * pageSize, length);
    return this.ts.t('paginator.range', {
      start: String(start),
      end: String(end),
      total: String(length),
    });
  };
}
