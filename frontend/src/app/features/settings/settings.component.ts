import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { TranslationService, SupportedLanguage } from '../../core/i18n/translation.service';
import { AuthService } from '../../core/auth/auth.service';
import { UserSettingsApiService } from '../../core/api/user-settings-api.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    MatCardModule,
    MatRadioModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    TranslatePipe,
  ],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss',
})
export class SettingsComponent implements OnInit {
  private ts = inject(TranslationService);
  private auth = inject(AuthService);
  private api = inject(UserSettingsApiService);
  private snack = inject(MatSnackBar);

  selectedLang = signal<SupportedLanguage>('en');
  loading = signal(false);

  readonly languages: { value: SupportedLanguage; labelKey: string }[] = [
    { value: 'en', labelKey: 'settings.langEn' },
    { value: 'uk', labelKey: 'settings.langUk' },
    { value: 'ru', labelKey: 'settings.langRu' },
  ];

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
}
