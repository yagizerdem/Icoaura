import React from "react";
import { cn } from "../util/twUtil";

type ModernButtonType = "info" | "danger" | "success" | "warning" | "ghost";
type IconPosition = "left" | "right" | "only";

interface ModernIconButtonProps {
  icon?: React.ReactNode;
  text?: string;
  iconPosition?: IconPosition;
  type?: ModernButtonType;
  className?: string;
  onMouseDown?: () => void;
  onMouseUp?: () => void;
  onClick?: () => void;
  disabled?: boolean;
  size?: "sm" | "md" | "lg";
}

function ModernIconButton({
  icon,
  text,
  iconPosition = "left",
  type = "info",
  size = "md",
  onMouseDown,
  onMouseUp,
  onClick,
  disabled = false,
  className,
}: ModernIconButtonProps) {
  const baseStyle = `
    inline-flex items-center justify-center gap-2
    rounded-md select-none
    transition-all duration-150 ease-out
    active:scale-[.95]
    focus:outline-none focus-visible:ring-2 focus-visible:ring-offset-2
    disabled:opacity-60 disabled:cursor-not-allowed
  `;

  const sizeStyles: Record<"sm" | "md" | "lg", string> = {
    sm: "px-2 py-1 text-sm",
    md: "px-3 py-2 text-base",
    lg: "px-4 py-2 text-lg",
  };

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
      className={cn(baseStyle, sizeStyles[size], typeStyles[type], className)}
    >
      {iconPosition === "left" && icon && (
        <span className="flex items-center justify-center">{icon}</span>
      )}
      {text && <span>{text}</span>}
      {iconPosition === "right" && icon && (
        <span className="flex items-center justify-center">{icon}</span>
      )}
      {iconPosition === "only" && !text && icon && (
        <span className="flex items-center justify-center">{icon}</span>
      )}
    </button>
  );
}

export { ModernIconButton };
