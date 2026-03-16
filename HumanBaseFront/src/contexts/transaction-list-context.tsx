import React from "react";
import { toast } from "sonner";
import { api } from "../lib/api";
import { getErrorMessage, type ApiResponse } from "../types/api";
import type { AppTransaction } from "../types/transaction";

type TransactionListState = {
  transactions: AppTransaction[];
  isLoading: boolean;
  error: string | null;
  ensureLoaded: () => Promise<void>;
  refresh: () => Promise<void>;
  invalidate: () => void;
};

const TransactionListContext = React.createContext<
  TransactionListState | undefined
>(undefined);

export function TransactionListProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [transactions, setTransactions] = React.useState<AppTransaction[]>([]);
  const [hasLoaded, setHasLoaded] = React.useState(false);
  const [isLoading, setIsLoading] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);
  const inFlightRef = React.useRef<Promise<void> | null>(null);

  const fetchTransactions = React.useCallback(async () => {
    if (inFlightRef.current) return inFlightRef.current;

    const request = (async () => {
      setIsLoading(true);
      setError(null);

      try {
        const response = await api.get<ApiResponse<{ data: AppTransaction[] }>>(
          "AppTransaction?pageSize=10&page=1"
        );
        const payload = response.data;

        if (!payload.success || !payload.result) {
          const message = getErrorMessage(
            payload,
            "Failed to load transactions"
          );
          setError(message);
          toast.error(message);
          return;
        }

        setTransactions(payload.result.data ?? []);
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
    await fetchTransactions();
  }, [fetchTransactions, hasLoaded]);

  const refresh = React.useCallback(async () => {
    await fetchTransactions();
  }, [fetchTransactions]);

  const invalidate = React.useCallback(() => {
    setHasLoaded(false);
    setTransactions([]);
    setError(null);
  }, []);

  const value = React.useMemo(
    () => ({
      transactions,
      isLoading,
      error,
      ensureLoaded,
      refresh,
      invalidate,
    }),
    [transactions, isLoading, error, ensureLoaded, refresh, invalidate]
  );

  return (
    <TransactionListContext.Provider value={value}>
      {children}
    </TransactionListContext.Provider>
  );
}

export function useTransactionList() {
  const context = React.useContext(TransactionListContext);

  if (!context) {
    throw new Error(
      "useTransactionList must be used within a TransactionListProvider"
    );
  }

  React.useEffect(() => {
    void context.ensureLoaded();
  }, [context.ensureLoaded]);

  return context;
}
