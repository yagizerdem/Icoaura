import { cn } from "../util/twUtil";

interface ModernTextAreaProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  label?: string;
  rows?: number;
  disabled?: boolean;
  className?: string;
}

export function ModernTextArea({
  value,
  onChange,
  placeholder = "",
  label,
  rows = 4,
  disabled = false,
  className,
}: ModernTextAreaProps) {
  return (
    <div className="flex flex-col w-full">
      {label && (
        <label className="text-sm font-medium text-(--clr-text-secondary) mb-1 select-none">
          {label}
        </label>
      )}

      <textarea
        value={value}
        placeholder={placeholder}
        rows={rows}
        disabled={disabled}
        onChange={(e) => onChange(e.target.value)}
        className={cn(
          "w-full px-3 py-2 rounded-lg",
          "bg-(--clr-surface-700) text-(--clr-text-primary)",
          "border border-(--clr-surface-400) focus:border-(--clr-accent-500)",
          "focus:outline-none transition-colors duration-200",
          "placeholder-(--clr-text-secondary)",
          disabled && "opacity-50 cursor-not-allowed",
          className
        )}
      />
    </div>
  );
}
