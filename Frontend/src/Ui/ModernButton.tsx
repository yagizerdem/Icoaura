import React from "react";
import { cn } from "../util/twUtil";

type ModernButtonType = "info" | "danger" | "success" | "warning" | "ghost";

interface ModernButtonProps {
  text: string;
  className?: string;
  type?: ModernButtonType;
  onMouseDown?: () => void;
  onMouseUp?: () => void;
  onClick?: () => void;
  disabled?: boolean;
}

/**
 * ModernButton — Tailwind-polished, theme-variable driven button
 *  - uses consistent opacity, hover, and active states
 *  - integrates seamlessly with .dark-default color variables
 */
function ModernButton({
  type = "info",
  text,
  onMouseDown,
  onMouseUp,
  onClick,
  disabled = false,
  className,
}: ModernButtonProps) {
  const baseStyle = `
    px-4 py-2 rounded-md text-sm font-medium select-none
    transition-all duration-150 ease-out
    active:scale-[.97]
    focus:outline-none focus-visible:ring-2 focus-visible:ring-offset-2
    disabled:opacity-60 disabled:cursor-not-allowed
  `;

  const typeStyles: Record<ModernButtonType, string> = {
    info: `
      bg-[var(--clr-info-20)] text-white
      hover:bg-[var(--clr-info-10)] active:bg-[var(--clr-info-0)]
      focus-visible:ring-[var(--clr-info-0)]
    `,
    success: `
      bg-[var(--clr-success-20)] text-white
      hover:bg-[var(--clr-success-10)] active:bg-[var(--clr-success-0)]
      focus-visible:ring-[var(--clr-success-0)]
    `,
    warning: `
      bg-[var(--clr-warning-20)] text-white
      hover:bg-[var(--clr-warning-10)] active:bg-[var(--clr-warning-0)]
      focus-visible:ring-[var(--clr-warning-0)]
    `,
    danger: `
      bg-[var(--clr-danger-20)] text-white
      hover:bg-[var(--clr-danger-10)] active:bg-[var(--clr-danger-0)]
      focus-visible:ring-[var(--clr-danger-0)]
    `,
    ghost: `
      bg-transparent text-[var(--clr-text-secondary)]
      border border-[var(--clr-surface-600)]
      hover:bg-[var(--clr-surface-700)]
      active:bg-[var(--clr-surface-800)]
      focus-visible:ring-[var(--clr-surface-400)]
    `,
  };

  return (
    <button
      onMouseDown={onMouseDown}
      onMouseUp={onMouseUp}
      onClick={onClick}
      disabled={disabled}
      className={cn(baseStyle, typeStyles[type], className)}
    >
      {text}
    </button>
  );
}

export { ModernButton };
