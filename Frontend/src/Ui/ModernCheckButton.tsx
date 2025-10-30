import { cn } from "../util/twUtil";

interface ModernCheckButtonProps {
  className?: string;
  text: string;
  checked: boolean;
  onChange: (checked: boolean) => void;
}

function ModernCheckButton({
  className: className,
  text,
  checked,
  onChange,
}: ModernCheckButtonProps) {
  return (
    <button
      className={cn(
        "relative modern-check-button h-fit hover:bg-(--clr-surface-600) text-(--clr-text-primary) cursor-pointer font-bold py-2 px-4 bg-transparent rounded-sm w-fit flex flex-row items-center justify-start transition-all duration-200",
        checked && "bg-(--clr-surface-950)",
        className
      )}
      onClick={() => onChange(!checked)}
    >
      {text}
    </button>
  );
}

export { ModernCheckButton };
