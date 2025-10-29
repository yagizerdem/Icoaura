import { cn } from "../util/twUtil";

interface ModernSwitchProps {
  checked: boolean;
  onChange: (checked: boolean) => void;
  disabled?: boolean;
  className?: string;
}

function ModernSwitch({
  checked,
  onChange,
  disabled,
  className,
}: ModernSwitchProps) {
  return (
    <button
      type="button"
      disabled={disabled}
      onClick={() => !disabled && onChange(!checked)}
      className={cn(
        "relative inline-flex h-7 w-12 rounded-full transition-all duration-300 ease-out cursor-pointer",
        checked ? "bg-(--clr-surface-500)" : "bg-(--clr-surface-600)",
        disabled && "opacity-50 cursor-not-allowed",
        "focus:outline-none focus:ring-2 focus:ring-(--clr-surface-500)",
        className
      )}
    >
      <span
        className={cn(
          "absolute top-[2px] left-[2px] h-6 w-6 rounded-full bg-(--clr-surface-950) shadow-md transition-all duration-300 ease-out",
          checked && "translate-x-5 bg-(--clr-surface-50)"
        )}
      />
    </button>
  );
}

export { ModernSwitch };
