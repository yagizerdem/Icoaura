export interface ApiResponse<T> {
  Data: T;
  Success: boolean;
  ErrorMessage: string;
  SuccessMessage: string;
}
