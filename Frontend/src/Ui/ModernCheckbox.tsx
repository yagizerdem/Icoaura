import React from "react";
import { cn } from "../util/twUtil";

export interface ModernCheckboxProps {
  checked?: boolean;
  onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
  disabled?: boolean;
  onMouseUp?: (e: React.MouseEvent<HTMLInputElement>) => void;
  onClick?: (e: React.MouseEvent<HTMLInputElement>) => void;
  onMouseDown?: (e: React.MouseEvent<HTMLInputElement>) => void;
  label?: string;
  className?: string;
  size?: "sm" | "md" | "lg";
}

export const ModernCheckBox: React.FC<ModernCheckboxProps> = ({
  checked,
  onChange,
  disabled,
  onMouseDown,
  onMouseUp,
  onClick,
  label,
  className,
  size = "md",
}) => {
  const id = React.useId();

  const sizeMap = {
    sm: "w-4 h-4",
    md: "w-5 h-5",
    lg: "w-6 h-6",
  };
  const sizeCls = sizeMap[size];

  return (
    <label
      htmlFor={id}
      className={cn(
        "flex items-center gap-2 select-none cursor-pointer",
        disabled && "cursor-not-allowed opacity-60",
        className
      )}
    >
      <div
        className={cn(
          "relative flex items-center justify-center rounded-sm transition-all duration-150 border-2",
          // normal state
          "border-(--clr-surface-500) bg-(--clr-surface-800)",
          // hover
          "hover:border-(--clr-surface-300)",
          // focus
          "focus-within:ring-2 focus-within:ring-(--clr-info-10)",
          // checked
          checked && "bg-(--clr-info-10) border-(--clr-info-10)",
          sizeCls
        )}
      >
        <input
          id={id}
          type="checkbox"
          checked={checked}
          onChange={onChange}
          disabled={disabled}
          onClick={onClick}
          onMouseUp={onMouseUp}
          onMouseDown={onMouseDown}
          className="absolute inset-0 opacity-0 cursor-pointer"
        />
        {checked && (
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="none"
            stroke="var(--clr-text-primary)"
            strokeWidth="3"
            className="w-3.5 h-3.5 pointer-events-none"
          >
            <polyline points="4 10 8 14 16 6" />
          </svg>
        )}
      </div>

      {label && (
        <span className="text-(--clr-text-primary) text-sm font-medium">
          {label}
        </span>
      )}
    </label>
  );
};
