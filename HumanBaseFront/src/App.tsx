import React from "react";
import { Card } from "./components/ui/card";
import { Button } from "./components/ui/button";
import { Input } from "./components/ui/input";
import { DatePicker } from "./components/ui/date-picker";

import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
} from "./components/ui/select";

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "./components/ui/collapsible";

import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from "./components/ui/pagination";

import { toast } from "sonner";
import { api } from "./lib/api";
import {
  hasDuplicateByKeys,
  sanitizeString,
  validateDate,
  validatePositiveNumber,
  validateRequired,
} from "./lib/validation";
import { TypeFinancial } from "./types/type-financial";
import { StatusBadge } from "./components/ui/status-badge";
import { handleApi } from "./lib/handle-api";
import {
  CategoryListProvider,
  useCategoryList,
} from "./contexts/category-list-context";
import {
  PersonListProvider,
  usePersonList,
} from "./contexts/person-list-context";
import {
  TransactionListProvider,
  useTransactionList,
} from "./contexts/transaction-list-context";

function formatCurrency(value: number | null | undefined) {
  const safe = typeof value === "number" && Number.isFinite(value) ? value : 0;
  return new Intl.NumberFormat(undefined, {
    style: "currency",
    currency: "BRL",
  }).format(safe);
}

function formatTypeFinancial(typeFinancial: TypeFinancial) {
  switch (typeFinancial) {
    case TypeFinancial.Income:
      return "Income";
    case TypeFinancial.Expense:
      return "Expense";
    case TypeFinancial.IncomeExpense:
      return "IncomeExpense";
    case TypeFinancial.Default:
    default:
      return "Default";
  }
}

type PersonDetails = {
  id: string;
  name: string;
  birthDate: string;
  isActive: boolean;
  totalIncome?: number | null;
  totalExpense?: number | null;
  total?: number | null;
};

type CategoryDetails = {
  id: string;
  description: string;
  typeFinancial: TypeFinancial;
  isActive: boolean;
  total?: number | null;
};

export function App() {
  return (
    <PersonListProvider>
      <CategoryListProvider>
        <TransactionListProvider>
          <main className="max-w-6xl mx-auto p-6 flex flex-col gap-6">
            <PersonSection />
            <CategoriesSection />
            <TransactionsSection />
          </main>
        </TransactionListProvider>
      </CategoryListProvider>
    </PersonListProvider>
  );
}

function PersonSection() {
  const [personNameError, setPersonNameError] = React.useState<string | null>(
    null
  );
  const [personBirthError, setPersonBirthError] = React.useState<string | null>(
    null
  );

  const [pagePerson, setPagePerson] = React.useState(1);
  const [searchPerson, setSearchPerson] = React.useState("");
  const [queryPerson, setQueryPerson] = React.useState("");
  const [showPersons, setShowPersons] = React.useState(true);

  const [newPersonName, setNewPersonName] = React.useState("");
  const [newPersonBirth, setNewPersonBirth] = React.useState("");

  const [personFinancialById, setPersonFinancialById] = React.useState<
    Record<string, Pick<PersonDetails, "totalIncome" | "totalExpense" | "total">>
  >({});
  const personFinancialInFlightRef = React.useRef<Set<string>>(new Set());

  const { persons, totalPages, refresh, invalidateAll } = usePersonList({
    page: pagePerson,
    search: queryPerson,
  });

  React.useEffect(() => {
    let cancelled = false;

    async function hydrateFinancials(ids: string[]) {
      await Promise.allSettled(
        ids.map(async (id) => {
          if (personFinancialInFlightRef.current.has(id)) return;
          if (personFinancialById[id]) return;

          personFinancialInFlightRef.current.add(id);
          try {
            const data = await handleApi(api.get(`Person/${id}`), undefined);
            if (!data?.result || cancelled) return;

            const resultArray = data.result as unknown;
            const first = Array.isArray(resultArray)
              ? (resultArray[0] as PersonDetails | undefined)
              : (resultArray as PersonDetails | undefined);
            if (!first) return;
            if (first.id && first.id !== id) return;

            setPersonFinancialById((prev) => ({
              ...prev,
              [id]: {
                totalIncome: first.totalIncome ?? 0,
                totalExpense: first.totalExpense ?? 0,
                total: first.total ?? 0,
              },
            }));
          } finally {
            personFinancialInFlightRef.current.delete(id);
          }
        })
      );
    }

    const ids = persons.map((p) => p.id).filter(Boolean);
    void hydrateFinancials(ids);

    return () => {
      cancelled = true;
    };
  }, [persons, personFinancialById]);

  async function deletePerson(id: string) {
    const person = persons.find((p) => p.id === id);

    if (
      !window.confirm(
        person
          ? `Are you sure you want to delete "${person.name}"? This action cannot be undone.`
          : "Are you sure you want to delete this person? This action cannot be undone."
      )
    ) {
      toast.info("Delete cancelled.");
      return;
    }

    const data = await handleApi(
      api.delete(`Person/${id}`),
      "Person deleted successfully"
    );

    if (!data) return;

    invalidateAll();
    await refresh();
  }

  async function createPerson() {
    const nameResult = validateRequired(newPersonName, "Name", {
      maxLength: 120,
    });
    const birthResult = validateDate(newPersonBirth, "Birth date");

    setPersonNameError(nameResult.valid ? null : nameResult.message ?? null);
    setPersonBirthError(birthResult.valid ? null : birthResult.message ?? null);

    if (!nameResult.valid || !birthResult.valid) {
      toast.error(
        "Please fix the highlighted fields before creating a person."
      );
      return;
    }

    if (
      hasDuplicateByKeys(
        persons,
        {
          name: sanitizeString(newPersonName),
          birthDate: sanitizeString(newPersonBirth),
        },
        ["name", "birthDate"]
      )
    ) {
      toast.warning(
        "A person with the same name and birth date already exists."
      );
      return;
    }

    const data = await handleApi(
      api.post("Person", {
        name: sanitizeString(newPersonName),
        birthDate: sanitizeString(newPersonBirth),
      }),
      "Person created successfully"
    );

    if (!data) return;

    setNewPersonName("");
    setNewPersonBirth("");
    invalidateAll();
    await refresh();
  }

  return (
    <Card className="p-6 space-y-4">
      <Collapsible open={showPersons} onOpenChange={setShowPersons}>
        <div className="flex justify-between items-center">
          <h2 className="text-xl font-bold">Persons</h2>

          <CollapsibleTrigger>
            <Button variant="outline">{showPersons ? "Hide" : "Show"}</Button>
          </CollapsibleTrigger>
        </div>

        <div className="grid grid-cols-3 gap-2">
          <Input
            placeholder="Name"
            value={newPersonName}
            onChange={(e) => {
              const value = e.target.value;
              setNewPersonName(value);

              if (personNameError) {
                const result = validateRequired(value, "Name", {
                  maxLength: 120,
                });
                setPersonNameError(
                  result.valid ? null : result.message ?? null
                );
              }
            }}
            aria-invalid={!!personNameError}
            aria-describedby="person-name-error"
          />

          <DatePicker
            value={newPersonBirth}
            onChange={(value) => {
              setNewPersonBirth(value);
              const result = validateDate(value, "Birth date");
              setPersonBirthError(result.valid ? null : result.message ?? null);
            }}
            aria-invalid={!!personBirthError}
            aria-describedby="person-birth-error"
          />

          <Button onClick={createPerson}>Create</Button>
        </div>

        {(personNameError || personBirthError) && (
          <div className="text-sm text-destructive space-y-1 mt-1">
            {personNameError && <p id="person-name-error">{personNameError}</p>}
            {personBirthError && (
              <p id="person-birth-error">{personBirthError}</p>
            )}
          </div>
        )}

        <div className="flex gap-2">
          <Input
            placeholder="Search person"
            value={searchPerson}
            onChange={(e) => setSearchPerson(e.target.value)}
          />

          <Button
            onClick={() => {
              setPagePerson(1);
              setQueryPerson(searchPerson);
            }}
          >
            Search
          </Button>
        </div>

        <CollapsibleContent className="space-y-2">
          {persons?.map((person) => (
            <div
              key={person.id}
              className="flex justify-between items-center border rounded p-3"
            >
              <div>
                <p className="font-semibold">{person.name}</p>

                <p className="text-sm text-muted-foreground">
                  {person.birthDate}
                </p>

                <StatusBadge active={person.isActive} />

                <div className="grid grid-cols-3 gap-3 text-sm mt-2">
                  <div>
                    <p className="text-muted-foreground">Total Income</p>
                    <p className="font-medium">
                      {personFinancialById[person.id]
                        ? formatCurrency(personFinancialById[person.id].totalIncome)
                        : "…"}
                    </p>
                  </div>
                  <div>
                    <p className="text-muted-foreground">Total Expense</p>
                    <p className="font-medium">
                      {personFinancialById[person.id]
                        ? formatCurrency(personFinancialById[person.id].totalExpense)
                        : "…"}
                    </p>
                  </div>
                  <div>
                    <p className="text-muted-foreground">Balance</p>
                    {personFinancialById[person.id] ? (
                      <p
                        className={
                          (personFinancialById[person.id].total ?? 0) >= 0
                            ? "font-semibold text-emerald-600"
                            : "font-semibold text-destructive"
                        }
                      >
                        {formatCurrency(personFinancialById[person.id].total)}
                      </p>
                    ) : (
                      <p className="font-medium">…</p>
                    )}
                  </div>
                </div>
              </div>

              <Button
                variant="destructive"
                onClick={() => deletePerson(person.id)}
              >
                Delete
              </Button>
            </div>
          ))}

          <Pagination>
            <PaginationContent>
              <PaginationItem>
                <PaginationPrevious
                  onClick={() => pagePerson > 1 && setPagePerson((p) => p - 1)}
                />
              </PaginationItem>

              <PaginationItem>
                <span className="px-4 text-sm">
                  Page {pagePerson} / {totalPages}
                </span>
              </PaginationItem>

              <PaginationItem>
                <PaginationNext
                  onClick={() =>
                    pagePerson < totalPages && setPagePerson((p) => p + 1)
                  }
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        </CollapsibleContent>
      </Collapsible>
    </Card>
  );
}

function CategoriesSection() {
  const [newCategoryDesc, setNewCategoryDesc] = React.useState("");
  const [newCategoryTypeFinancial, setNewCategoryTypeFinancial] =
    React.useState<TypeFinancial | null>(null);
  const [categoryDescError, setCategoryDescError] = React.useState<
    string | null
  >(null);
  const [categoryTypeFinancialError, setCategoryTypeFinancialError] =
    React.useState<string | null>(null);

  const [categoryTotalById, setCategoryTotalById] = React.useState<
    Record<string, number>
  >({});
  const categoryTotalInFlightRef = React.useRef<Set<string>>(new Set());

  const { categories, summary, refresh, invalidate } = useCategoryList();

  React.useEffect(() => {
    let cancelled = false;

    async function hydrateTotals(ids: string[]) {
      await Promise.allSettled(
        ids.map(async (id) => {
          if (categoryTotalInFlightRef.current.has(id)) return;
          if (typeof categoryTotalById[id] === "number") return;

          categoryTotalInFlightRef.current.add(id);
          try {
            const data = await handleApi(api.get(`Category/${id}`), undefined);
            if (!data?.result || cancelled) return;

            const resultArray = data.result as unknown;
            const first = Array.isArray(resultArray)
              ? (resultArray[0] as CategoryDetails | undefined)
              : (resultArray as CategoryDetails | undefined);
            if (!first) return;
            if (first.id && first.id !== id) return;

            setCategoryTotalById((prev) => ({
              ...prev,
              [id]: first.total ?? 0,
            }));
          } finally {
            categoryTotalInFlightRef.current.delete(id);
          }
        })
      );
    }

    const ids = categories.map((c) => c.id).filter(Boolean);
    void hydrateTotals(ids);

    return () => {
      cancelled = true;
    };
  }, [categories, categoryTotalById]);

  async function createCategory() {
    const descResult = validateRequired(newCategoryDesc, "Description", {
      maxLength: 160,
    });
    const typeFinancialValid = newCategoryTypeFinancial !== null;

    setCategoryDescError(descResult.valid ? null : descResult.message ?? null);
    setCategoryTypeFinancialError(
      typeFinancialValid ? null : "Type financial is required."
    );

    if (!descResult.valid || !typeFinancialValid) {
      toast.error(
        "Please fix the highlighted fields before creating a category."
      );
      return;
    }

    if (
      hasDuplicateByKeys(
        categories,
        { description: sanitizeString(newCategoryDesc) },
        ["description"]
      )
    ) {
      toast.warning("A category with this description already exists.");
      return;
    }

    const data = await handleApi(
      api.post("Category", {
        description: sanitizeString(newCategoryDesc),
        typeFinancial: newCategoryTypeFinancial as TypeFinancial,
      }),
      "Category created successfully"
    );

    if (!data) return;

    setNewCategoryDesc("");
    setNewCategoryTypeFinancial(null);
    invalidate();
    await refresh();
  }

  return (
    <Card className="p-6 space-y-4">
      <h2 className="text-xl font-bold">Categories</h2>

      <div className="flex gap-2 flex-wrap">
        <Input
          placeholder="Description"
          value={newCategoryDesc}
          onChange={(e) => {
            const value = e.target.value;
            setNewCategoryDesc(value);

            if (categoryDescError) {
              const result = validateRequired(value, "Description", {
                maxLength: 160,
              });
              setCategoryDescError(
                result.valid ? null : result.message ?? null
              );
            }
          }}
          aria-invalid={!!categoryDescError}
          aria-describedby="category-description-error"
        />

        <Select
          value={
            newCategoryTypeFinancial === null
              ? undefined
              : String(newCategoryTypeFinancial)
          }
          onValueChange={(value) => {
            const nextValue = Number(value) as TypeFinancial;
            setNewCategoryTypeFinancial(nextValue);
            if (categoryTypeFinancialError) {
              setCategoryTypeFinancialError(
                Number.isNaN(nextValue) ? "Type financial is required." : null
              );
            }
          }}
        >
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Type Financial" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={String(TypeFinancial.Income)}>Income</SelectItem>
            <SelectItem value={String(TypeFinancial.Expense)}>
              Expense
            </SelectItem>
            <SelectItem value={String(TypeFinancial.IncomeExpense)}>
              IncomeExpense
            </SelectItem>
          </SelectContent>
        </Select>

        <Button onClick={createCategory}>Create</Button>
      </div>

      {(categoryDescError || categoryTypeFinancialError) && (
        <div className="text-sm text-destructive space-y-1 mt-1">
          {categoryDescError && (
            <p id="category-description-error">{categoryDescError}</p>
          )}
          {categoryTypeFinancialError && (
            <p id="category-typeFinancial-error">{categoryTypeFinancialError}</p>
          )}
        </div>
      )}

      {categories.map((cat) => (
        <div
          key={cat.id}
          className="flex items-start justify-between border rounded p-3 gap-4"
        >
          <div className="min-w-0">
            <p className="font-medium">{cat.description}</p>
            <p className="text-sm text-muted-foreground">
              Type: {formatTypeFinancial(cat.typeFinancial)}
            </p>
            <p className="text-sm text-muted-foreground">
              Total:{" "}
              {typeof categoryTotalById[cat.id] === "number"
                ? formatCurrency(categoryTotalById[cat.id])
                : "…"}
            </p>
          </div>

          <StatusBadge active={cat.isActive} />
        </div>
      ))}

      <Card className="p-4">
        <h3 className="font-semibold mb-2">Summary</h3>
        <div className="grid grid-cols-3 gap-3 text-sm">
          <div>
            <p className="text-muted-foreground">Total Income</p>
            <p className="font-medium">{formatCurrency(summary?.totalIncome)}</p>
          </div>
          <div>
            <p className="text-muted-foreground">Total Expense</p>
            <p className="font-medium">
              {formatCurrency(summary?.totalExpense)}
            </p>
          </div>
          <div>
            <p className="text-muted-foreground">Total Balance</p>
            <p
              className={
                (summary?.total ?? 0) >= 0
                  ? "font-semibold text-emerald-600"
                  : "font-semibold text-destructive"
              }
            >
              {formatCurrency(summary?.total)}
            </p>
          </div>
        </div>
      </Card>
    </Card>
  );
}

function TransactionsSection() {
  const [newTransactionDesc, setNewTransactionDesc] = React.useState("");
  const [newTransactionValue, setNewTransactionValue] = React.useState(0);
  const [newTransactionTypeFinancial, setNewTransactionTypeFinancial] =
    React.useState<TypeFinancial | null>(null);
  const [selectedPerson, setSelectedPerson] = React.useState<string | null>(
    null
  );
  const [selectedCategory, setSelectedCategory] = React.useState<string | null>(
    null
  );

  const [transactionDescError, setTransactionDescError] = React.useState<
    string | null
  >(null);
  const [transactionValueError, setTransactionValueError] = React.useState<
    string | null
  >(null);
  const [transactionTypeFinancialError, setTransactionTypeFinancialError] =
    React.useState<string | null>(null);
  const [transactionPersonError, setTransactionPersonError] = React.useState<
    string | null
  >(null);
  const [transactionCategoryError, setTransactionCategoryError] =
    React.useState<string | null>(null);

  const { transactions, refresh, invalidate } = useTransactionList();
  const { categories } = useCategoryList();
  const { persons } = usePersonList({ page: 1, search: "" });

  async function createTransaction() {
    const descResult = validateRequired(newTransactionDesc, "Description", {
      maxLength: 200,
    });
    const valueResult = validatePositiveNumber(newTransactionValue, "Value", {
      min: 0.01,
    });
    const typeFinancialValid = newTransactionTypeFinancial !== null;
    const personResult = validateRequired(selectedPerson, "Person");
    const categoryResult = validateRequired(selectedCategory, "Category");

    setTransactionDescError(
      descResult.valid ? null : descResult.message ?? null
    );
    setTransactionValueError(
      valueResult.valid ? null : valueResult.message ?? null
    );
    setTransactionTypeFinancialError(
      typeFinancialValid ? null : "Type financial is required."
    );
    setTransactionPersonError(
      personResult.valid ? null : personResult.message ?? null
    );
    setTransactionCategoryError(
      categoryResult.valid ? null : categoryResult.message ?? null
    );

    if (
      !descResult.valid ||
      !valueResult.valid ||
      !typeFinancialValid ||
      !personResult.valid ||
      !categoryResult.valid
    ) {
      toast.error(
        "Please fix the highlighted fields before creating a transaction."
      );
      return;
    }

    const data = await handleApi(
      api.post("AppTransaction", {
        description: sanitizeString(newTransactionDesc),
        value: newTransactionValue,
        typeFinancial: newTransactionTypeFinancial as TypeFinancial,
        personId: selectedPerson,
        categoryId: selectedCategory,
      }),
      "Transaction created successfully"
    );

    if (!data) return;

    setNewTransactionDesc("");
    setNewTransactionValue(0);
    setNewTransactionTypeFinancial(null);
    setSelectedPerson(null);
    setSelectedCategory(null);
    invalidate();
    await refresh();
  }

  return (
    <Card className="p-6 space-y-4">
      <h2 className="text-xl font-bold">Transactions</h2>

      <div className="flex gap-2 flex-wrap">
        <Input
          placeholder="Description"
          value={newTransactionDesc}
          onChange={(e) => {
            const value = e.target.value;
            setNewTransactionDesc(value);

            if (transactionDescError) {
              const result = validateRequired(value, "Description", {
                maxLength: 200,
              });
              setTransactionDescError(
                result.valid ? null : result.message ?? null
              );
            }
          }}
          aria-invalid={!!transactionDescError}
          aria-describedby="transaction-description-error"
        />

        <Input
          type="number"
          placeholder="Value"
          value={Number.isNaN(newTransactionValue) ? "" : newTransactionValue}
          onChange={(e) => {
            const raw = e.target.value;
            const numericValue = raw === "" ? 0 : Number(raw);
            setNewTransactionValue(numericValue);

            if (transactionValueError) {
              const result = validatePositiveNumber(numericValue, "Value", {
                min: 0.01,
              });
              setTransactionValueError(
                result.valid ? null : result.message ?? null
              );
            }
          }}
          aria-invalid={!!transactionValueError}
          aria-describedby="transaction-value-error"
        />

        <Select
          value={
            newTransactionTypeFinancial === null
              ? undefined
              : String(newTransactionTypeFinancial)
          }
          onValueChange={(value) => {
            const nextValue = Number(value) as TypeFinancial;
            setNewTransactionTypeFinancial(nextValue);
            if (transactionTypeFinancialError) {
              setTransactionTypeFinancialError(
                Number.isNaN(nextValue) ? "Type financial is required." : null
              );
            }
          }}
        >
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Type Financial" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={String(TypeFinancial.Income)}>Income</SelectItem>
            <SelectItem value={String(TypeFinancial.Expense)}>
              Expense
            </SelectItem>
            <SelectItem value={String(TypeFinancial.IncomeExpense)}>
              IncomeExpense
            </SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={selectedPerson ?? undefined}
          onValueChange={(value) => {
            const nextValue = (value ?? null) as string | null;
            setSelectedPerson(nextValue);

            if (transactionPersonError) {
              const result = validateRequired(nextValue, "Person");
              setTransactionPersonError(
                result.valid ? null : result.message ?? null
              );
            }
          }}
        >
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Select Person" />
          </SelectTrigger>

          <SelectContent>
            {persons.map((p) => (
              <SelectItem key={p.id} value={p.id}>
                {p.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>

        <Select
          value={selectedCategory ?? undefined}
          onValueChange={(value) => {
            const nextValue = (value ?? null) as string | null;
            setSelectedCategory(nextValue);

            if (transactionCategoryError) {
              const result = validateRequired(nextValue, "Category");
              setTransactionCategoryError(
                result.valid ? null : result.message ?? null
              );
            }
          }}
        >
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Select Category" />
          </SelectTrigger>

          <SelectContent>
            {categories.map((c) => (
              <SelectItem key={c.id} value={c.id}>
                {c.description}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>

        <Button onClick={createTransaction}>Create</Button>
      </div>

      {(transactionDescError ||
        transactionValueError ||
        transactionTypeFinancialError ||
        transactionPersonError ||
        transactionCategoryError) && (
        <div className="text-sm text-destructive space-y-1 mt-1">
          {transactionDescError && (
            <p id="transaction-description-error">{transactionDescError}</p>
          )}
          {transactionValueError && (
            <p id="transaction-value-error">{transactionValueError}</p>
          )}
          {transactionTypeFinancialError && (
            <p id="transaction-typeFinancial-error">
              {transactionTypeFinancialError}
            </p>
          )}
          {transactionPersonError && (
            <p id="transaction-person-error">{transactionPersonError}</p>
          )}
          {transactionCategoryError && (
            <p id="transaction-category-error">{transactionCategoryError}</p>
          )}
        </div>
      )}

      {transactions.map((t) => (
        <div key={t.id} className="border rounded p-3">
          <p className="font-semibold">{t.description}</p>

          <p className="text-sm text-muted-foreground">Value: {t.value}</p>
          <p className="text-sm text-muted-foreground">
            Type: {formatTypeFinancial(t.typeFinancial)}
          </p>

          <StatusBadge active={t.isActive} />
        </div>
      ))}
    </Card>
  );
}
