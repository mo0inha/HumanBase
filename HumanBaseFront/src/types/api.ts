export type ApiResponse<T = unknown> = {
  success: boolean;
  message?: string | null;
  errors?: string[];
  result?: T;
};

export function getErrorMessage(data: ApiResponse, fallback: string) {
  return data.errors?.[0] ?? data.message ?? fallback;
}
