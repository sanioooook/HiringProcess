import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MarkdownModule } from 'ngx-markdown';
import { finalize } from 'rxjs';
import { HiringProcess, HiringProcessForm } from '../../../core/api/hiring-process.model';
import { HiringProcessApiService } from '../../../core/api/hiring-process-api.service';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';
import { TranslationService } from '../../../core/i18n/translation.service';

export interface FormDialogData {
  mode: 'create' | 'edit';
  record?: HiringProcess;
}

const CONTACT_CHANNELS = ['LinkedIn', 'Email', 'Referral', 'Job Board', 'Company Website', 'Recruiter', 'Cold Outreach'];
const APPLIED_WITH_OPTIONS = ['Resume', 'Resume + Cover Letter', 'LinkedIn Profile', 'Portfolio', 'GitHub', 'Other'];
const STAGE_PRESETS = [
  'stage.applied', 'stage.phoneScreen', 'stage.technicalInterview', 'stage.takeHomeTask',
  'stage.onsiteInterview', 'stage.offerReceived', 'stage.offerAccepted', 'stage.rejected', 'stage.withdrawn',
];

@Component({
  selector: 'app-hiring-process-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatSelectModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatSnackBarModule,
    MatTooltipModule,
    MarkdownModule,
    TranslatePipe,
  ],
  templateUrl: './hiring-process-form-dialog.component.html',
  styleUrl: './hiring-process-form-dialog.component.scss',
})
export class HiringProcessFormDialogComponent implements OnInit {
  readonly contactChannels = CONTACT_CHANNELS;
  readonly appliedWithOpts = APPLIED_WITH_OPTIONS;
  readonly stagePresets = STAGE_PRESETS;

  customContactChannel = '';
  customAppliedWith = '';
  newStageInput = '';

  loading = false;
  uploadFile: File | null = null;

  // Markdown preview state
  coverLetterPreview = false;
  notesPreview = false;

  private fb = inject(FormBuilder);
  private api = inject(HiringProcessApiService);
  private snack = inject(MatSnackBar);
  private ts = inject(TranslationService);

  form!: FormGroup;
  stages: string[] = [];

  constructor(
    public dialogRef: MatDialogRef<HiringProcessFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FormDialogData,
  ) {}

  ngOnInit(): void {
    this.form = this.fb.nonNullable.group({
      companyName: ['', [Validators.required, Validators.maxLength(500)]],
      contactChannel: ['', Validators.required],
      contactPerson: [''],
      salaryRange: [''],
      appliedWith: [''],
      appliedLink: ['', Validators.pattern(/^(https?:\/\/.*)?$/)],
      vacancyLink: ['', Validators.pattern(/^(https?:\/\/.*)?$/)],
      currentStage: [''],
      firstContactDate: [null as Date | null],
      lastContactDate: [null as Date | null],
      vacancyPublishedDate: [null as Date | null],
      applicationDate: [null as Date | null],
      coverLetter: [''],
      vacancyText: [''],
      notes: [''],
    });

    if (this.data.mode === 'edit' && this.data.record) {
      const r = this.data.record;
      this.form.patchValue({
        companyName: r.companyName,
        contactChannel: r.contactChannel,
        contactPerson: r.contactPerson ?? '',
        salaryRange: r.salaryRange ?? '',
        appliedWith: r.appliedWith ?? '',
        appliedLink: r.appliedLink ?? '',
        vacancyLink: r.vacancyLink ?? '',
        currentStage: r.currentStage ?? '',
        firstContactDate: r.firstContactDate ? new Date(r.firstContactDate + 'T00:00:00') : null,
        lastContactDate: r.lastContactDate ? new Date(r.lastContactDate + 'T00:00:00') : null,
        vacancyPublishedDate: r.vacancyPublishedDate ? new Date(r.vacancyPublishedDate + 'T00:00:00') : null,
        applicationDate: r.applicationDate ? new Date(r.applicationDate + 'T00:00:00') : null,
        coverLetter: r.coverLetter ?? '',
        vacancyText: r.vacancyText ?? '',
        notes: r.notes ?? '',
      });
      this.stages = [...(r.hiringStages ?? [])];
    }
  }

  // Stages chip management

  addStageFromPreset(stage: string): void {
    if (stage && !this.stages.includes(stage)) {
      this.stages = [...this.stages, stage];
    }
  }

  addCustomStage(): void {
    const val = this.newStageInput.trim();
    if (val && !this.stages.includes(val)) {
      this.stages = [...this.stages, val];
    }
    this.newStageInput = '';
  }

  removeStage(stage: string): void {
    this.stages = this.stages.filter(s => s !== stage);
    if (this.form.value['currentStage'] === stage) {
      this.form.patchValue({ currentStage: '' });
    }
  }

  // File

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.uploadFile = input.files[0];
    }
  }

  downloadExistingFile(): void {
    if (!this.data.record) return;
    this.api.downloadFile(this.data.record.id).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `vacancy-${this.data.record!.companyName}`;
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  // Markdown toolbar helpers

  wrapText(el: HTMLTextAreaElement, ctrlName: string, marker: string): void {
    const ctrl = this.form.get(ctrlName)!;
    const text: string = ctrl.value ?? '';
    const start = el.selectionStart;
    const end = el.selectionEnd;
    const selected = text.substring(start, end) || 'text';
    const newText = text.substring(0, start) + `${marker}${selected}${marker}` + text.substring(end);
    ctrl.setValue(newText);
    setTimeout(() => {
      el.setSelectionRange(start + marker.length, start + marker.length + selected.length);
      el.focus();
    });
  }

  insertLinePrefix(el: HTMLTextAreaElement, ctrlName: string, prefix: string): void {
    const ctrl = this.form.get(ctrlName)!;
    const text: string = ctrl.value ?? '';
    const pos = el.selectionStart;
    const lineStart = text.lastIndexOf('\n', pos - 1) + 1;
    const newText = text.substring(0, lineStart) + prefix + text.substring(lineStart);
    ctrl.setValue(newText);
    setTimeout(() => {
      const newPos = lineStart + prefix.length;
      el.setSelectionRange(newPos, newPos);
      el.focus();
    });
  }

  insertLink(el: HTMLTextAreaElement, ctrlName: string): void {
    const ctrl = this.form.get(ctrlName)!;
    const text: string = ctrl.value ?? '';
    const start = el.selectionStart;
    const end = el.selectionEnd;
    const selected = text.substring(start, end) || 'link text';
    const insertion = `[${selected}](url)`;
    const newText = text.substring(0, start) + insertion + text.substring(end);
    ctrl.setValue(newText);
    setTimeout(() => {
      // Select the 'url' part for easy replacement
      const urlStart = start + selected.length + 3;
      el.setSelectionRange(urlStart, urlStart + 3);
      el.focus();
    });
  }

  // Helpers

  private formatDate(date: Date | null | undefined): string | null {
    if (!date) return null;
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  private buildPayload(): HiringProcessForm {
    const v = this.form.getRawValue();
    return {
      companyName: v['companyName'],
      contactChannel: v['contactChannel'],
      contactPerson: v['contactPerson'] || null,
      salaryRange: v['salaryRange'] || null,
      appliedWith: v['appliedWith'] || null,
      appliedLink: v['appliedLink'] || null,
      vacancyLink: v['vacancyLink'] || null,
      currentStage: v['currentStage'] || null,
      firstContactDate: this.formatDate(v['firstContactDate']),
      lastContactDate: this.formatDate(v['lastContactDate']),
      vacancyPublishedDate: this.formatDate(v['vacancyPublishedDate']),
      applicationDate: this.formatDate(v['applicationDate']),
      coverLetter: v['coverLetter'] || null,
      vacancyText: v['vacancyText'] || null,
      notes: v['notes'] || null,
      hiringStages: this.stages.length ? this.stages : null,
    };
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    const payload = this.buildPayload();

    const req$ = this.data.mode === 'create'
      ? this.api.create(payload)
      : this.api.update(this.data.record!.id, payload);

    req$.pipe(finalize(() => this.loading = false)).subscribe({
      next: (saved) => {
        if (this.uploadFile) {
          this.api.uploadFile(saved.id, this.uploadFile).subscribe({
            next: () => this.dialogRef.close(true),
            error: () => {
              this.snack.open(this.ts.t('snack.savedFileUploadFailed'), 'OK', { duration: 5000 });
              this.dialogRef.close(true);
            },
          });
        } else {
          this.dialogRef.close(true);
        }
      },
      error: (err) => {
        const msg = err?.error?.message ?? this.ts.t('snack.saveFailed');
        this.snack.open(msg, 'Close', { duration: 4000 });
      },
    });
  }

  cancel(): void { this.dialogRef.close(false); }
}
