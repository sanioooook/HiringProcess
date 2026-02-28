// Read model
export interface HiringProcess {
  id: string;
  companyName: string;
  contactChannel: string;
  contactPerson?: string;
  firstContactDate?: string; // ISO date "yyyy-MM-dd"
  lastContactDate?: string;
  vacancyPublishedDate?: string;
  applicationDate?: string;
  appliedWith?: string;
  appliedLink?: string;
  coverLetter?: string;
  salaryRange?: string;
  hiringStages: string[];
  currentStage?: string;
  vacancyLink?: string;
  hasVacancyFile: boolean;
  vacancyText?: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

// Write model (shared for create & update)
export interface HiringProcessForm {
  companyName: string;
  contactChannel: string;
  contactPerson?: string | null;
  firstContactDate?: string | null;
  lastContactDate?: string | null;
  vacancyPublishedDate?: string | null;
  applicationDate?: string | null;
  appliedWith?: string | null;
  appliedLink?: string | null;
  coverLetter?: string | null;
  salaryRange?: string | null;
  hiringStages?: string[] | null;
  currentStage?: string | null;
  vacancyLink?: string | null;
  vacancyText?: string | null;
  notes?: string | null;
}

// Pagination
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Query params
export interface HiringProcessQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  currentStage?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}
