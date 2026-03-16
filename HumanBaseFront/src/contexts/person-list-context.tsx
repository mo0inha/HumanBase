import React from "react";
import { toast } from "sonner";
import { api } from "../lib/api";
import { getErrorMessage, type ApiResponse } from "../types/api";
import type { Person } from "../types/person";

type PersonQuery = {
  page: number;
  search?: string;
};

type PersonCacheEntry = {
  data: Person[];
  pageCount: number;
};

type PersonListState = {
  cache: Map<string, PersonCacheEntry>;
  isLoading: boolean;
  error: string | null;
  ensureLoaded: (query: PersonQuery) => Promise<void>;
  refresh: (query: PersonQuery) => Promise<void>;
  invalidate: (query?: PersonQuery) => void;
};

const PersonListContext = React.createContext<PersonListState | undefined>(
  undefined
);

const normalizeQuery = (query: PersonQuery): Required<PersonQuery> => ({
  page: Math.max(1, query.page),
  search: (query.search ?? "").trim(),
});

const makeKey = (query: Required<PersonQuery>) =>
  `${query.page}::${query.search}`;

export function PersonListProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [cache, setCache] = React.useState<Map<string, PersonCacheEntry>>(
    () => new Map()
  );
  const [loadingCount, setLoadingCount] = React.useState(0);
  const [error, setError] = React.useState<string | null>(null);
  const inFlightRef = React.useRef<Map<string, Promise<void>>>(new Map());

  const fetchPersons = React.useCallback(async (query: PersonQuery) => {
    const normalized = normalizeQuery(query);
    const key = makeKey(normalized);

    const existing = inFlightRef.current.get(key);
    if (existing) return existing;

    const request = (async () => {
      setLoadingCount((count) => count + 1);
      setError(null);

      try {
        const url = `Person?pageSize=10&page=${normalized.page}&name=${encodeURIComponent(
          normalized.search
        )}`;
        const response = await api.get<
          ApiResponse<{ data: Person[]; pageCount: number }>
        >(url);
        const payload = response.data;

        if (!payload.success || !payload.result) {
          const message = getErrorMessage(payload, "Failed to load persons");
          setError(message);
          toast.error(message);
          return;
        }

        const entry: PersonCacheEntry = {
          data: payload.result.data ?? [],
          pageCount: payload.result.pageCount ?? 1,
        };

        setCache((prev) => {
          const next = new Map(prev);
          next.set(key, entry);
          return next;
        });
      } catch {
        const message = "Failed to communicate with server";
        setError(message);
        toast.error(message);
      } finally {
        setLoadingCount((count) => Math.max(0, count - 1));
        inFlightRef.current.delete(key);
      }
    })();

    inFlightRef.current.set(key, request);
    return request;
  }, []);

  const ensureLoaded = React.useCallback(
    async (query: PersonQuery) => {
      const normalized = normalizeQuery(query);
      const key = makeKey(normalized);

      if (cache.has(key)) return;
      await fetchPersons(normalized);
    },
    [cache, fetchPersons]
  );

  const refresh = React.useCallback(
    async (query: PersonQuery) => {
      await fetchPersons(query);
    },
    [fetchPersons]
  );

  const invalidate = React.useCallback((query?: PersonQuery) => {
    if (!query) {
      setCache(new Map());
      return;
    }

    const normalized = normalizeQuery(query);
    const key = makeKey(normalized);

    setCache((prev) => {
      if (!prev.has(key)) return prev;
      const next = new Map(prev);
      next.delete(key);
      return next;
    });
  }, []);

  const value = React.useMemo(
    () => ({
      cache,
      isLoading: loadingCount > 0,
      error,
      ensureLoaded,
      refresh,
      invalidate,
    }),
    [cache, loadingCount, error, ensureLoaded, refresh, invalidate]
  );

  return (
    <PersonListContext.Provider value={value}>
      {children}
    </PersonListContext.Provider>
  );
}

export function usePersonList(query: PersonQuery) {
  const context = React.useContext(PersonListContext);

  if (!context) {
    throw new Error("usePersonList must be used within a PersonListProvider");
  }

  const normalized = React.useMemo(
    () => normalizeQuery(query),
    [query.page, query.search]
  );
  const key = React.useMemo(
    () => makeKey(normalized),
    [normalized.page, normalized.search]
  );
  const cached = context.cache.get(key);
  const { ensureLoaded } = context;

  React.useEffect(() => {
    void ensureLoaded(normalized);
  }, [ensureLoaded, normalized]);

  return {
    persons: cached?.data ?? [],
    totalPages: cached?.pageCount ?? 1,
    isLoading: context.isLoading,
    error: context.error,
    refresh: () => context.refresh(normalized),
    invalidate: () => context.invalidate(normalized),
    invalidateAll: () => context.invalidate(),
  };
}
