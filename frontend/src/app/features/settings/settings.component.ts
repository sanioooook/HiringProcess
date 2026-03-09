import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { TranslationService, SupportedLanguage } from '../../core/i18n/translation.service';
import { AuthService } from '../../core/auth/auth.service';
import { UserSettingsApiService } from '../../core/api/user-settings-api.service';

function passwordMatch(control: AbstractControl): ValidationErrors | null {
  const pw = control.get('newPassword')?.value;
  const confirm = control.get('confirm')?.value;
  return pw && confirm && pw !== confirm ? { mismatch: true } : null;
}

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatRadioModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDividerModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    TranslatePipe,
  ],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss',
})
export class SettingsComponent implements OnInit {
  private ts = inject(TranslationService);
  readonly auth = inject(AuthService);
  private api = inject(UserSettingsApiService);
  private snack = inject(MatSnackBar);

  selectedLang = signal<SupportedLanguage>('en');
  loading = signal(false);
  pwLoading = signal(false);
  emailLoading = signal(false);
  hideCurrentPw = true;
  hideNewPw = true;
  hideNewPwConfirm = true;
  hideEmailPw = true;

  readonly languages: { value: SupportedLanguage; labelKey: string }[] = [
    { value: 'en', labelKey: 'settings.langEn' },
    { value: 'uk', labelKey: 'settings.langUk' },
    { value: 'ru', labelKey: 'settings.langRu' },
  ];

  pwForm = new FormGroup(
    {
      currentPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
      newPassword: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(8), Validators.pattern(/(?=.*[A-Z])(?=.*[0-9])/)] }),
      confirm: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    },
    { validators: passwordMatch },
  );

  emailForm = new FormGroup({
    newEmail: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    currentPassword: new FormControl('', { nonNullable: true }),
  });

  ngOnInit(): void {
    this.selectedLang.set((this.auth.user()?.language ?? 'en') as SupportedLanguage);
  }

  save(): void {
    this.loading.set(true);
    this.api.update(this.selectedLang()).subscribe({
      next: () => {
        this.auth.updateLanguage(this.selectedLang());
        this.loading.set(false);
        this.snack.open(this.ts.t('snack.settingsSaved'), 'OK', { duration: 3000 });
      },
      error: () => {
        this.loading.set(false);
        this.snack.open(this.ts.t('snack.settingsFailed'), 'Close', { duration: 4000 });
      },
    });
  }

  changePassword(): void {
    if (this.pwForm.invalid) return;
    const { currentPassword, newPassword } = this.pwForm.getRawValue();
    this.pwLoading.set(true);
    this.auth.changePassword(currentPassword, newPassword).subscribe({
      next: () => {
        this.pwLoading.set(false);
        this.pwForm.reset();
        this.snack.open(this.ts.t('settings.changePassword.success'), 'OK', { duration: 3000 });
      },
      error: (err: any) => {
        this.pwLoading.set(false);
        this.snack.open(err?.error?.message ?? this.ts.t('snack.saveFailed'), 'Close', { duration: 4000 });
      },
    });
  }

  changeEmail(): void {
    if (this.emailForm.invalid) return;
    const { newEmail, currentPassword } = this.emailForm.getRawValue();
    this.emailLoading.set(true);
    this.auth.changeEmail(newEmail, this.auth.user()?.hasPassword ? currentPassword : null).subscribe({
      next: () => {
        this.emailLoading.set(false);
        this.emailForm.reset();
        this.snack.open(this.ts.t('settings.changeEmail.success'), 'OK', { duration: 4000 });
      },
      error: (err: any) => {
        this.emailLoading.set(false);
        this.snack.open(err?.error?.message ?? this.ts.t('snack.saveFailed'), 'Close', { duration: 4000 });
      },
    });
  }
}
