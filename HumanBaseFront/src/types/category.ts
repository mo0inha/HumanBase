import type { TypeFinancial } from "./type-financial";

export type Category = {
  id: string;
  description: string;
  typeFinancial: TypeFinancial;
  isActive: boolean;
};
