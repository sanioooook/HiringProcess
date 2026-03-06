import {
  Component,
  OnInit,
  ViewChild,
  signal,
  computed,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule, MatTable } from '@angular/material/table';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { HiringProcess } from '../../../core/api/hiring-process.model';
import { HiringProcessApiService } from '../../../core/api/hiring-process-api.service';
import {
  HiringProcessFormDialogComponent,
} from '../form-dialog/hiring-process-form-dialog.component';
import {
  ColumnSelectorComponent,
  ColumnDef,
} from '../column-selector/column-selector.component';
import {
  ConfirmDialogComponent,
} from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AuthService } from '../../../core/auth/auth.service';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';
import { TranslationService } from '../../../core/i18n/translation.service';
import { HiringProcessStore } from '../hiring-process.store';

const COLUMNS_KEY = 'hp_columns';

const ALL_COLUMNS: ColumnDef[] = [
  { key: 'companyName', labelKey: 'col.company', visible: true },
  { key: 'currentStage', labelKey: 'col.stage', visible: true },
  { key: 'contactChannel', labelKey: 'col.channel', visible: true },
  { key: 'contactPerson', labelKey: 'col.contactPerson', visible: false },
  { key: 'applicationDate', labelKey: 'col.applied', visible: true },
  { key: 'firstContactDate', labelKey: 'col.firstContact', visible: false },
  { key: 'lastContactDate', labelKey: 'col.lastContact', visible: false },
  { key: 'salaryRange', labelKey: 'col.salary', visible: false },
  { key: 'appliedWith', labelKey: 'col.appliedWith', visible: false },
  { key: 'hiringStages', labelKey: 'col.stages', visible: false },
  { key: 'updatedAt', labelKey: 'col.updated', visible: true },
  { key: 'actions', labelKey: 'col.actions', visible: true },
];

@Component({
  selector: 'app-hiring-process-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatChipsModule,
    MatTooltipModule,
    MatProgressBarModule,
    MatSnackBarModule,
    MatDialogModule,
    MatMenuModule,
    TranslatePipe,
  ],
  templateUrl: './hiring-process-list.component.html',
  styleUrl: './hiring-process-list.component.scss',
})
export class HiringProcessListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  private ts = inject(TranslationService);
  private snack = inject(MatSnackBar);
  private api = inject(HiringProcessApiService);
  protected hpStore = inject(HiringProcessStore);

  // Signals from store
  data = this.hpStore.items;
  totalCount = this.hpStore.total;
  loading = this.hpStore.loading;

  // Local UI state
  columns = signal<ColumnDef[]>(this.loadColumns());
  searchTerm = '';
  page = 0;
  pageSize = 20;
  sortBy = 'updatedAt';
  sortDirection: 'asc' | 'desc' = 'desc';

  readonly displayedColumns = computed(() =>
    this.columns().filter(c => c.visible).map(c => c.key)
  );

  private readonly search$ = new Subject<string>();

  constructor(
    private dialog: MatDialog,
    public auth: AuthService,
  ) {
    this.search$
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(term => {
        this.searchTerm = term;
        this.page = 0;
        this.load();
      });
  }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.hpStore.load({
      page: this.page + 1,
      pageSize: this.pageSize,
      search: this.searchTerm || undefined,
      sortBy: this.sortBy,
      sortDirection: this.sortDirection,
    });
  }

  onPage(event: PageEvent): void {
    this.page = event.pageIndex;
    this.pageSize = event.pageSize;
    this.load();
  }

  onSort(sort: Sort): void {
    this.sortBy = sort.active || 'updatedAt';
    this.sortDirection = (sort.direction as 'asc' | 'desc') || 'desc';
    this.page = 0;
    this.load();
  }

  onSearch(term: string): void { this.search$.next(term); }

  openCreate(): void {
    this.dialog
      .open(HiringProcessFormDialogComponent, {
        data: { mode: 'create' },
        panelClass: 'form-dialog-panel',
        disableClose: true,
      })
      .afterClosed()
      .subscribe(saved => { if (saved) this.load(); });
  }

  openEdit(record: HiringProcess): void {
    this.dialog
      .open(HiringProcessFormDialogComponent, {
        data: { mode: 'edit', record },
        panelClass: 'form-dialog-panel',
        disableClose: true,
      })
      .afterClosed()
      .subscribe(saved => { if (saved) this.load(); });
  }

  openDelete(record: HiringProcess): void {
    this.dialog
      .open(ConfirmDialogComponent, {
        data: {
          title: this.ts.t('confirm.deleteTitle'),
          message: this.ts.t('confirm.deleteMessage', { name: record.companyName }),
          confirmLabel: this.ts.t('confirm.deleteLabel'),
          cancelLabel: this.ts.t('dialog.cancel'),
        },
      })
      .afterClosed()
      .subscribe(async confirmed => {
        if (!confirmed) return;
        const ok = await this.hpStore.delete(record.id);
        const msg = ok ? this.ts.t('snack.deleted') : this.ts.t('snack.deleteFailed');
        this.snack.open(msg, 'Close', { duration: 3000 });
      });
  }

  downloadFile(record: HiringProcess): void {
    this.api.downloadFile(record.id).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `vacancy-${record.companyName}`;
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  openColumnSelector(): void {
    this.dialog
      .open(ColumnSelectorComponent, {
        data: { columns: this.columns() },
      })
      .afterClosed()
      .subscribe((result: ColumnDef[] | null) => {
        if (result) {
          this.columns.set(result);
          this.saveColumns(result);
        }
      });
  }

  private loadColumns(): ColumnDef[] {
    try {
      const stored = localStorage.getItem(COLUMNS_KEY);
      if (!stored) return ALL_COLUMNS;
      const saved: { key: string; visible: boolean }[] = JSON.parse(stored);
      return ALL_COLUMNS.map(col => ({
        ...col,
        visible: saved.find(s => s.key === col.key)?.visible ?? col.visible,
      }));
    } catch {
      return ALL_COLUMNS;
    }
  }

  private saveColumns(cols: ColumnDef[]): void {
    localStorage.setItem(
      COLUMNS_KEY,
      JSON.stringify(cols.map(c => ({ key: c.key, visible: c.visible }))),
    );
  }
}
