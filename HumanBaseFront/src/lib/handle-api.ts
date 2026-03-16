import { toast } from "sonner";
import { getErrorMessage, type ApiResponse } from "../types/api";

export async function handleApi<T>(
  request: Promise<{ data: ApiResponse<T> }>,
  successMessage?: string
) {
  try {
    const response = await request;
    const data = response.data;

    if (!data.success) {
      toast.error(getErrorMessage(data, "Operation failed"));
      return null;
    }

    if (successMessage) toast.success(successMessage);

    return data;
  } catch {
    toast.error("Failed to communicate with server");
    return null;
  }
}
