import { cn } from "../util/twUtil";
import { ChevronUp, ChevronDown } from "lucide-react";

interface ModernNumberInputProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  label?: string;
  allowDecimal?: boolean;
  disabled?: boolean;
  className?: string;
  min?: number;
  max?: number;
  step?: number;
}

export function ModernNumberInput({
  value,
  onChange,
  placeholder = "",
  label,
  allowDecimal = true,
  disabled = false,
  className,
  min,
  max,
  step = 1,
}: ModernNumberInputProps) {
  function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    let val = e.target.value;
    const regex = allowDecimal ? /^[0-9]*\.?[0-9]*$/ : /^[0-9]*$/;
    if (!regex.test(val)) return;
    onChange(val);
  }

  function adjustValue(delta: number) {
    if (disabled) return;
    let current = parseFloat(value || "0");
    if (isNaN(current)) current = 0;
    let newVal = current + delta;

    if (min !== undefined && newVal < min) newVal = min;
    if (max !== undefined && newVal > max) newVal = max;

    onChange(String(newVal));
  }

  return (
    <div className="flex flex-col w-full">
      {label && (
        <label className="text-sm font-medium text-(--clr-text-secondary) mb-1 select-none">
          {label}
        </label>
      )}

      <div className="relative flex items-center">
        <input
          type="text"
          inputMode="decimal"
          value={value}
          placeholder={placeholder}
          disabled={disabled}
          onChange={handleChange}
          className={cn(
            "w-full px-3 py-2 rounded-lg bg-(--clr-surface-700) text-(--clr-text-primary)",
            "border border-(--clr-surface-400) focus:border-(--clr-accent-500)",
            "focus:outline-none transition-colors duration-200",
            "placeholder-(--clr-text-secondary) pr-8",
            disabled && "opacity-50 cursor-not-allowed",
            className
          )}
        />

        <div className="absolute right-1 top-1/2 -translate-y-1/2 flex flex-col">
          <button
            type="button"
            disabled={disabled}
            onClick={() => adjustValue(step)}
            className={cn(
              "p-[2px] rounded-sm hover:bg-(--clr-surface-500) transition",
              disabled && "opacity-50 cursor-not-allowed"
            )}
          >
            <ChevronUp size={14} className="text-(--clr-text-secondary)" />
          </button>
          <button
            type="button"
            disabled={disabled}
            onClick={() => adjustValue(-step)}
            className={cn(
              "p-[2px] rounded-sm hover:bg-(--clr-surface-500) transition",
              disabled && "opacity-50 cursor-not-allowed"
            )}
          >
            <ChevronDown size={14} className="text-(--clr-text-secondary)" />
          </button>
        </div>
      </div>
    </div>
  );
}
