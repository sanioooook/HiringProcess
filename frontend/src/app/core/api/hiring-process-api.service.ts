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

const hiringProcessBase = `${environment.apiUrl}/hiring-processes`;
const hiringProcessEndpoints = {
  list: hiringProcessBase,
  byId: (id: string) => `${hiringProcessBase}/${id}`,
  file: (id: string) => `${hiringProcessBase}/${id}/file`,
};

@Injectable({ providedIn: 'root' })
export class HiringProcessApiService {
  constructor(private http: HttpClient) {}

  getAll(query: HiringProcessQuery = {}): Observable<PagedResult<HiringProcess>> {
    let params = new HttpParams();
    if (query.page) params = params.set('page', query.page);
    if (query.pageSize) params = params.set('pageSize', query.pageSize);
    if (query.search) params = params.set('search', query.search);
    if (query.currentStage) params = params.set('currentStage', query.currentStage);
    if (query.sortBy) params = params.set('sortBy', query.sortBy);
    if (query.sortDirection) params = params.set('sortDirection', query.sortDirection);

    return this.http.get<PagedResult<HiringProcess>>(hiringProcessEndpoints.list, { params });
  }

  getById(id: string): Observable<HiringProcess> {
    return this.http.get<HiringProcess>(hiringProcessEndpoints.byId(id));
  }

  create(form: HiringProcessForm): Observable<HiringProcess> {
    return this.http.post<HiringProcess>(hiringProcessEndpoints.list, form);
  }

  update(id: string, form: HiringProcessForm): Observable<HiringProcess> {
    return this.http.put<HiringProcess>(hiringProcessEndpoints.byId(id), form);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(hiringProcessEndpoints.byId(id));
  }

  uploadFile(id: string, file: File): Observable<{ storedFileName: string }> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<{ storedFileName: string }>(hiringProcessEndpoints.file(id), form);
  }

  getFileDownloadUrl(id: string): string {
    return hiringProcessEndpoints.file(id);
  }

  downloadFile(id: string): Observable<Blob> {
    return this.http.get(hiringProcessEndpoints.file(id), { responseType: 'blob' });
  }
}
