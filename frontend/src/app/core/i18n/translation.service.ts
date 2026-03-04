import { Injectable, signal } from '@angular/core';
import { en } from './translations/en';
import { uk } from './translations/uk';
import { ru } from './translations/ru';

export type SupportedLanguage = 'en' | 'uk' | 'ru';

const TRANSLATIONS: Record<SupportedLanguage, Record<string, string>> = { en, uk, ru };

@Injectable({ providedIn: 'root' })
export class TranslationService {
  private readonly _lang = signal<SupportedLanguage>('en');
  readonly lang = this._lang.asReadonly();

  setLanguage(lang: SupportedLanguage): void {
    this._lang.set(lang);
  }

  t(key: string, params?: Record<string, string>): string {
    const dict = TRANSLATIONS[this._lang()];
    let str = dict[key] ?? key;
    if (params) {
      for (const [k, v] of Object.entries(params)) {
        str = str.replace(`{${k}}`, v);
      }
    }
    return str;
  }
}
