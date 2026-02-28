import {
  Component,
  OnInit,
  ViewChild,
  signal,
  computed,
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

import { HiringProcess, HiringProcessQuery } from '../../../core/api/hiring-process.model';
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

const COLUMNS_KEY = 'hp_columns';

const ALL_COLUMNS: ColumnDef[] = [
  { key: 'companyName',       label: 'Company',         visible: true  },
  { key: 'currentStage',      label: 'Current Stage',   visible: true  },
  { key: 'contactChannel',    label: 'Channel',         visible: true  },
  { key: 'contactPerson',     label: 'Contact Person',  visible: false },
  { key: 'applicationDate',   label: 'Applied',         visible: true  },
  { key: 'firstContactDate',  label: 'First Contact',   visible: false },
  { key: 'lastContactDate',   label: 'Last Contact',    visible: false },
  { key: 'salaryRange',       label: 'Salary Range',    visible: false },
  { key: 'appliedWith',       label: 'Applied With',    visible: false },
  { key: 'hiringStages',      label: 'Stages',          visible: false },
  { key: 'updatedAt',         label: 'Updated',         visible: true  },
  { key: 'actions',           label: 'Actions',         visible: true  },
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
  ],
  templateUrl: './hiring-process-list.component.html',
  styleUrl: './hiring-process-list.component.scss',
})
export class HiringProcessListComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // State
  data = signal<HiringProcess[]>([]);
  totalCount = signal(0);
  loading = signal(false);
  columns = signal<ColumnDef[]>(this.loadColumns());

  searchTerm = '';
  page = 0; // 0-based for MatPaginator, +1 when calling API
  pageSize = 20;
  sortBy = 'updatedAt';
  sortDirection: 'asc' | 'desc' = 'desc';

  readonly displayedColumns = computed(() =>
    this.columns().filter(c => c.visible).map(c => c.key)
  );

  private readonly search$ = new Subject<string>();

  constructor(
    private api: HiringProcessApiService,
    private dialog: MatDialog,
    private snack: MatSnackBar,
    public auth: AuthService,
  ) {
    // Debounce search input
    this.search$
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(term => {
        this.searchTerm = term;
        this.page = 0;
        this.load();
      });
  }

  ngOnInit(): void { this.load(); }

  // Data loading

  load(): void {
    this.loading.set(true);
    const query: HiringProcessQuery = {
      page: this.page + 1,
      pageSize: this.pageSize,
      search: this.searchTerm || undefined,
      sortBy: this.sortBy,
      sortDirection: this.sortDirection,
    };

    this.api.getAll(query).subscribe({
      next: (res) => {
        this.data.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snack.open('Failed to load records.', 'Close', { duration: 3000 });
      },
    });
  }

  // Pagination & sort

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

  // CRUD actions

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
          title: 'Delete Application',
          message: `Delete "${record.companyName}"? This cannot be undone.`,
          confirmLabel: 'Delete',
        },
      })
      .afterClosed()
      .subscribe(confirmed => {
        if (!confirmed) return;
        this.api.delete(record.id).subscribe({
          next: () => {
            this.snack.open('Record deleted.', 'Close', { duration: 3000 });
            this.load();
          },
          error: () => this.snack.open('Delete failed.', 'Close', { duration: 3000 }),
        });
      });
  }

  downloadFile(record: HiringProcess): void {
    window.open(this.api.getFileDownloadUrl(record.id), '_blank');
  }

  // Column management

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

  // Persistence (localStorage)

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
