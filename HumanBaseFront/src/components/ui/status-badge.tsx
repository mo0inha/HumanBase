import { Badge } from "./badge";

export function StatusBadge({ active }: { active: boolean }) {
  return (
    <Badge variant={active ? "default" : "destructive"}>
      {active ? "Active" : "Inactive"}
    </Badge>
  );
}
