export interface AuthResponse {
  userId: string;
  email: string;
  displayName: string;
  token: string;
  language: string;
}

export interface RegisterRequest {
  email: string;
  displayName: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface GoogleAuthRequest {
  idToken: string;
}

export interface CurrentUser {
  userId: string;
  email: string;
  displayName: string;
  language: string;
}
