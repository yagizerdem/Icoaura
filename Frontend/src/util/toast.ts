type ToastType = "success" | "error" | "info" | "warning";

interface ToastOptions {
  text: string;
  duration?: number;
  type?: ToastType;
  position?: "top-left" | "top-right" | "bottom-left" | "bottom-right";
  stopOnHover?: boolean;
  onClick?: () => void;
}

let activeToasts = 0;
const MAX_TOASTS = 5;

export function showToast({
  text,
  duration = 3000,
  type = "info",
  position = "top-right",
  stopOnHover = true,
  onClick,
}: ToastOptions) {
  if (activeToasts >= MAX_TOASTS) return;

  activeToasts++;

  // Toast container
  const containerId = `toast-container-${position}`;
  let container = document.getElementById(containerId);
  if (!container) {
    container = document.createElement("div");
    container.id = containerId;
    container.className = `fixed z-[9999] flex flex-col gap-2 pointer-events-none ${
      position.includes("top") ? "top-4" : "bottom-4"
    } ${position.includes("right") ? "right-4" : "left-4"}`;
    document.body.appendChild(container);
  }

  // Toast element
  const toast = document.createElement("div");
  toast.className = `
    pointer-events-auto
    min-w-[250px] max-w-sm px-4 py-3 rounded-md shadow-lg
    text-(--clr-text-primary) flex justify-between items-center gap-3
    border-l-4 transition-all duration-300 opacity-0 translate-y-2
  `;

  // color palette from your dark-default theme
  const colorMap: Record<ToastType, string> = {
    success: "bg-(--clr-success-20)/25 border-(--clr-success-10)",
    error: "bg-(--clr-danger-20)/25 border-(--clr-danger-10)",
    info: "bg-(--clr-info-20)/25 border-(--clr-info-10)",
    warning: "bg-(--clr-warning-20)/25 border-(--clr-warning-10)",
  };

  toast.className += ` ${colorMap[type]}`;

  toast.innerHTML = `
    <span class="flex-1 text-sm font-medium">${text}</span>
    <button class="text-(--clr-text-secondary) hover:text-(--clr-text-primary)">✕</button>
  `;

  // add behavior
  const closeBtn = toast.querySelector("button");
  closeBtn?.addEventListener("click", () => removeToast(toast, container!));

  if (onClick) toast.addEventListener("click", onClick);

  if (stopOnHover) {
    toast.addEventListener("mouseenter", () => clearTimeout(timeout));
  }

  container.appendChild(toast);

  // animate in
  requestAnimationFrame(() => {
    toast.classList.remove("opacity-0", "translate-y-2");
    toast.classList.add("opacity-100", "translate-y-0");
  });

  const timeout = setTimeout(() => removeToast(toast, container!), duration);
}

function removeToast(toast: HTMLElement, container: HTMLElement) {
  toast.classList.add("opacity-0", "translate-y-2");
  setTimeout(() => {
    toast.remove();
    activeToasts--;
    if (container.children.length === 0) {
      container.remove();
    }
  }, 300);
}

// Shortcut APIs
export const Toast = {
  success: (text: string, opts: Partial<ToastOptions> = {}) =>
    showToast({ ...opts, text, type: "success" }),
  error: (text: string, opts: Partial<ToastOptions> = {}) =>
    showToast({ ...opts, text, type: "error" }),
  info: (text: string, opts: Partial<ToastOptions> = {}) =>
    showToast({ ...opts, text, type: "info" }),
  warning: (text: string, opts: Partial<ToastOptions> = {}) =>
    showToast({ ...opts, text, type: "warning" }),
};
