import { useState, useRef, useEffect } from "react";
import { cn } from "../util/twUtil";

interface ModernSliderProps {
  value: number;
  onChange: (value: number) => void;
  min?: number;
  max?: number;
  step?: number;
  className?: string;
  label?: string;
  showValue?: boolean;
  disabled?: boolean;
  leftColor?: string;
  rightColor?: string;
}

function ModernSlider({
  value,
  onChange,
  min = 0,
  max = 100,
  step = 1,
  className,
  label,
  showValue = true,
  disabled = false,
  leftColor = "var(--clr-info-10)",
  rightColor = "var(--clr-surface-600)",
}: ModernSliderProps) {
  const [isDragging, setIsDragging] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (inputRef.current) {
      const percent = ((value - min) / (max - min)) * 100;
      inputRef.current.style.background = `
        linear-gradient(to right,
          ${leftColor} 0%,
          ${leftColor} ${percent}%,
          ${rightColor} ${percent}%,
          ${rightColor} 100%)
      `;
    }
  }, [value, min, max, leftColor, rightColor]);

  return (
    <div className={cn("flex flex-col gap-2 w-full", className)}>
      {label && (
        <label className="text-sm font-medium text-[var(--clr-text-primary)]">
          {label}
        </label>
      )}
      <div className="flex items-center gap-3">
        <input
          ref={inputRef}
          type="range"
          min={min}
          max={max}
          step={step}
          disabled={disabled}
          value={value}
          onChange={(e) => onChange(Number(e.target.value))}
          onMouseDown={() => setIsDragging(true)}
          onMouseUp={() => setIsDragging(false)}
          className={cn(
            `
            w-full appearance-none cursor-pointer
            rounded-full
            bg-[var(--clr-surface-600)]
            h-1.5
            focus:outline-none
            [&::-webkit-slider-thumb]:appearance-none
            [&::-webkit-slider-thumb]:w-4 [&::-webkit-slider-thumb]:h-4
            [&::-webkit-slider-thumb]:rounded-full
            [&::-webkit-slider-thumb]:bg-[var(--clr-info-0)]
            [&::-webkit-slider-thumb]:shadow-md
            [&::-webkit-slider-thumb]:transition-all [&::-webkit-slider-thumb]:duration-150
            [&::-webkit-slider-thumb]:hover:scale-110
            [&::-webkit-slider-thumb]:active:scale-125
            [&::-moz-range-thumb]:w-4 [&::-moz-range-thumb]:h-4 [&::-moz-range-thumb]:rounded-full [&::-moz-range-thumb]:bg-[var(--clr-info-0)]
            `,
            disabled && "opacity-60 cursor-not-allowed"
          )}
        />
        {showValue && (
          <span
            className={cn(
              "min-w-[2.5rem] text-right text-sm",
              isDragging
                ? "text-[var(--clr-info-0)] font-medium"
                : "text-[var(--clr-text-secondary)]"
            )}
          >
            {value}
          </span>
        )}
      </div>
    </div>
  );
}

export { ModernSlider };
