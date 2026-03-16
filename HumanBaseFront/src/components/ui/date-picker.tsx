import * as React from "react";
import { Input } from "./input";

type DatePickerProps = {
  value: string;
  onChange: (value: string) => void;
  "aria-invalid"?: boolean;
  "aria-describedby"?: string;
};

export function DatePicker({
  value,
  onChange,
  ...ariaProps
}: DatePickerProps) {
  return (
    <Input
      type="date"
      value={value}
      onChange={(event) => onChange(event.target.value)}
      {...ariaProps}
    />
  );
}

