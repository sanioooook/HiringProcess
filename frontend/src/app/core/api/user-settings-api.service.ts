import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UserSettings {
  language: string;
}

const userSettingsBase = `${environment.apiUrl}/users/settings`;

@Injectable({ providedIn: 'root' })
export class UserSettingsApiService {
  constructor(private http: HttpClient) {}

  get(): Observable<UserSettings> {
    return this.http.get<UserSettings>(userSettingsBase);
  }

  update(language: string): Observable<UserSettings> {
    return this.http.put<UserSettings>(userSettingsBase, { language });
  }
}
