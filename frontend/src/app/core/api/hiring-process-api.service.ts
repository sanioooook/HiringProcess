import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  HiringProcess,
  HiringProcessForm,
  HiringProcessQuery,
  PagedResult,
} from './hiring-process.model';

@Injectable({ providedIn: 'root' })
export class HiringProcessApiService {
  private readonly base = `${environment.apiUrl}/hiring-processes`;

  constructor(private http: HttpClient) {}

  getAll(query: HiringProcessQuery = {}): Observable<PagedResult<HiringProcess>> {
    let params = new HttpParams();
    if (query.page) params = params.set('page', query.page);
    if (query.pageSize) params = params.set('pageSize', query.pageSize);
    if (query.search) params = params.set('search', query.search);
    if (query.currentStage) params = params.set('currentStage', query.currentStage);
    if (query.sortBy) params = params.set('sortBy', query.sortBy);
    if (query.sortDirection) params = params.set('sortDirection', query.sortDirection);

    return this.http.get<PagedResult<HiringProcess>>(this.base, { params });
  }

  getById(id: string): Observable<HiringProcess> {
    return this.http.get<HiringProcess>(`${this.base}/${id}`);
  }

  create(form: HiringProcessForm): Observable<HiringProcess> {
    return this.http.post<HiringProcess>(this.base, form);
  }

  update(id: string, form: HiringProcessForm): Observable<HiringProcess> {
    return this.http.put<HiringProcess>(`${this.base}/${id}`, form);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  uploadFile(id: string, file: File): Observable<{ storedFileName: string }> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<{ storedFileName: string }>(`${this.base}/${id}/file`, form);
  }

  getFileDownloadUrl(id: string): string {
    return `${this.base}/${id}/file`;
  }

  downloadFile(id: string): Observable<Blob> {
    return this.http.get(`${this.base}/${id}/file`, { responseType: 'blob' });
  }
}
