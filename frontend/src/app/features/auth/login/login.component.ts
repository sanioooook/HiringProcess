import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthStore } from '../../../core/auth/auth.store';
import { AuthService } from '../../../core/auth/auth.service';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';
import { TranslationService } from '../../../core/i18n/translation.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    TranslatePipe,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  protected store = inject(AuthStore);
  private auth = inject(AuthService);
  private ts = inject(TranslationService);
  private snack = inject(MatSnackBar);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  hidePass = true;
  resending = signal(false);

  submit(): void {
    if (this.form.invalid) return;
    this.store.login(this.form.getRawValue());
  }

  resendVerification(): void {
    const email = this.store.lastEmail();
    if (!email) return;
    this.resending.set(true);
    this.auth.resendVerification(email).subscribe({
      next: () => {
        this.resending.set(false);
        this.snack.open(this.ts.t('auth.login.verificationSent'), 'OK', { duration: 4000 });
      },
      error: () => {
        this.resending.set(false);
        this.snack.open(this.ts.t('auth.login.verificationSent'), 'OK', { duration: 4000 });
      },
    });
  }
}
