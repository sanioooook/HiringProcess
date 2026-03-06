import { Injectable } from '@angular/core';
import { NativeDateAdapter } from '@angular/material/core';

/**
 * Date adapter that displays and parses dates in DD.MM.YYYY format.
 * Extends NativeDateAdapter so the calendar UI works as usual.
 */
@Injectable()
export class CustomDateAdapter extends NativeDateAdapter {
  /** Display as DD.MM.YYYY */
  override format(date: Date, _displayFormat: object): string {
    const d = String(date.getDate()).padStart(2, '0');
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const y = date.getFullYear();
    return `${d}.${m}.${y}`;
  }

  /** Parse DD.MM.YYYY typed input */
  override parse(value: string | null): Date | null {
    if (!value) return null;
    const match = /^(\d{1,2})\.(\d{1,2})\.(\d{4})$/.exec(value.trim());
    if (match) {
      return new Date(+match[3], +match[2] - 1, +match[1]);
    }
    return super.parse(value);
  }
}
