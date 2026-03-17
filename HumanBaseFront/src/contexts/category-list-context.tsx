import React from "react";
import { toast } from "sonner";
import { api } from "../lib/api";
import { getErrorMessage, type ApiResponse } from "../types/api";
import type { Category } from "../types/category";

export type CategorySummary = {
  totalIncome: number;
  totalExpense: number;
  total: number;
};

type CategoryListState = {
  categories: Category[];
  summary: CategorySummary | null;
  isLoading: boolean;
  error: string | null;
  ensureLoaded: () => Promise<void>;
  refresh: () => Promise<void>;
  invalidate: () => void;
};

const CategoryListContext = React.createContext<CategoryListState | undefined>(
  undefined
);

export function CategoryListProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [categories, setCategories] = React.useState<Category[]>([]);
  const [summary, setSummary] = React.useState<CategorySummary | null>(null);
  const [hasLoaded, setHasLoaded] = React.useState(false);
  const [isLoading, setIsLoading] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);
  const inFlightRef = React.useRef<Promise<void> | null>(null);

  const fetchCategories = React.useCallback(async () => {
    if (inFlightRef.current) return inFlightRef.current;

    const request = (async () => {
      setIsLoading(true);
      setError(null);

      try {
        const response = await api.get<
          ApiResponse<{
            data: Category[];
          }> & {
            summary?: CategorySummary | null;
          }
        >("Category?pageSize=10&page=1");
        const payload = response.data;

        if (!payload.success || !payload.result) {
          const message = getErrorMessage(payload, "Failed to load categories");
          setError(message);
          toast.error(message);
          return;
        }

        setCategories(payload.result.data ?? []);
        setSummary(payload.summary ?? null);
        setHasLoaded(true);
      } catch {
        const message = "Failed to communicate with server";
        setError(message);
        toast.error(message);
      } finally {
        setIsLoading(false);
        inFlightRef.current = null;
      }
    })();

    inFlightRef.current = request;
    return request;
  }, []);

  const ensureLoaded = React.useCallback(async () => {
    if (hasLoaded) return;
    await fetchCategories();
  }, [fetchCategories, hasLoaded]);

  const refresh = React.useCallback(async () => {
    await fetchCategories();
  }, [fetchCategories]);

  const invalidate = React.useCallback(() => {
    setHasLoaded(false);
    setCategories([]);
    setSummary(null);
    setError(null);
  }, []);

  const value = React.useMemo(
    () => ({
      categories,
      summary,
      isLoading,
      error,
      ensureLoaded,
      refresh,
      invalidate,
    }),
    [categories, summary, isLoading, error, ensureLoaded, refresh, invalidate]
  );

  return (
    <CategoryListContext.Provider value={value}>
      {children}
    </CategoryListContext.Provider>
  );
}

export function useCategoryList() {
  const context = React.useContext(CategoryListContext);

  if (!context) {
    throw new Error("useCategoryList must be used within a CategoryListProvider");
  }

  React.useEffect(() => {
    void context.ensureLoaded();
  }, [context.ensureLoaded]);

  return context;
}
