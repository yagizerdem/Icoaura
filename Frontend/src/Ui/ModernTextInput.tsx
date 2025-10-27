import { cn } from "../util/twUtil"; // opsiyonel: tailwind-merge yardımı varsa

interface ModernTextInputProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  label?: string;
  type?: string;
  disabled?: boolean;
  className?: string;
}

export function ModernTextInput({
  value,
  onChange,
  placeholder = "",
  label,
  type = "text",
  disabled = false,
  className,
}: ModernTextInputProps) {
  return (
    <div className="flex flex-col w-full">
      {label && (
        <label className="text-sm font-medium text-(--clr-text-secondary) mb-1 select-none">
          {label}
        </label>
      )}

      <input
        type={type}
        value={value}
        placeholder={placeholder}
        disabled={disabled}
        onChange={(e) => onChange(e.target.value)}
        className={cn(
          "w-full px-3 py-2 rounded-lg bg-(--clr-surface-700) text-(--clr-text-primary)",
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
