import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UserSettings {
  language: string;
}

@Injectable({ providedIn: 'root' })
export class UserSettingsApiService {
  private readonly base = `${environment.apiUrl}/users/settings`;

  constructor(private http: HttpClient) {}

  get(): Observable<UserSettings> {
    return this.http.get<UserSettings>(this.base);
  }

  update(language: string): Observable<UserSettings> {
    return this.http.put<UserSettings>(this.base, { language });
  }
}
