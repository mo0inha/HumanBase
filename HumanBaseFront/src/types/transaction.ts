import type { TypeFinancial } from "./type-financial";

export type AppTransaction = {
  id: string;
  description: string;
  value: number;
  typeFinancial: TypeFinancial;
  personId: string;
  categoryId: string;
  isActive: boolean;
};
