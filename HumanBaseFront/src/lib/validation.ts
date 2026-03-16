export type ValidationResult = {
  valid: boolean;
  message?: string;
};

export function sanitizeString(value: string): string {
  return value.replace(/[\u0000-\u001F\u007F]+/g, "").trim();
}

export function validateRequired(
  value: string | null | undefined,
  label: string,
  options?: { maxLength?: number },
): ValidationResult {
  const sanitized = sanitizeString(value ?? "");

  if (!sanitized) {
    return { valid: false, message: `${label} is required.` };
  }

  if (options?.maxLength && sanitized.length > options.maxLength) {
    return {
      valid: false,
      message: `${label} must be at most ${options.maxLength} characters.`,
    };
  }

  return { valid: true };
}

export function validateDate(value: string, label: string): ValidationResult {
  const sanitized = sanitizeString(value);

  if (!sanitized) {
    return { valid: false, message: `${label} is required.` };
  }

  // Enforce strict HTML date input format (4-digit year).
  // This prevents values like "10000-01-01" which some browsers may allow typing.
  if (!/^\d{4}-\d{2}-\d{2}$/.test(sanitized)) {
    return {
      valid: false,
      message: `${label} must be in YYYY-MM-DD format.`,
    };
  }

  const date = new Date(sanitized);

  if (Number.isNaN(date.getTime())) {
    return { valid: false, message: `${label} must be a valid date.` };
  }

  // Avoid clearly invalid years (e.g. year 0000 or far future)
  const year = date.getFullYear();
  if (year < 1900 || year > 2100) {
    return {
      valid: false,
      message: `${label} must be between 1900 and 2100.`,
    };
  }

  return { valid: true };
}

export function validatePositiveNumber(
  value: unknown,
  label: string,
  options?: { min?: number; max?: number },
): ValidationResult {
  const numberValue =
    typeof value === "number" ? value : Number((value as string) ?? "");

  if (Number.isNaN(numberValue)) {
    return { valid: false, message: `${label} must be a valid number.` };
  }

  if (numberValue <= 0) {
    return {
      valid: false,
      message: `${label} must be greater than 0.`,
    };
  }

  if (options?.min !== undefined && numberValue < options.min) {
    return {
      valid: false,
      message: `${label} must be at least ${options.min}.`,
    };
  }

  if (options?.max !== undefined && numberValue > options.max) {
    return {
      valid: false,
      message: `${label} must be at most ${options.max}.`,
    };
  }

  return { valid: true };
}

export function hasDuplicateByKeys<T extends Record<string, unknown>>(
  items: T[],
  candidate: Partial<T>,
  keys: (keyof T)[],
): boolean {
  return items.some((item) =>
    keys.every((key) => {
      const existing = item[key];
      const next = candidate[key];

      if (typeof existing === "string" && typeof next === "string") {
        return sanitizeString(existing).toLowerCase() ===
          sanitizeString(next).toLowerCase();
      }

      return existing === next;
    }),
  );
}

