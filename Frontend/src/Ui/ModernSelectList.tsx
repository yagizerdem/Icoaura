import { ChevronDown, ChevronUp, MagnetIcon, Search } from "lucide-react";
import { cn } from "../util/twUtil";
import gsap from "gsap";
import { useEffect, useRef, useState } from "react";
import { useDebounce } from "../hook/useDebounce";

interface ModernSelectListProps {
  className?: string;
  options?: string[];
  selectedOption?: string;
  onSelectOption: (option: any) => void;
}

function ModernSelectList({
  className,
  options,
  selectedOption,
  onSelectOption,
}: ModernSelectListProps) {
  const containerRef = useRef<HTMLDivElement | null>(null);
  const popupRef = useRef<HTMLDivElement | null>(null);
  const [optionsDeepCopy, setOptionsDeepCopy] = useState<string[]>(
    options ? [...options] : []
  );
  const [popupHeight, setPopupHeight] = useState<number>(0);
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [isOptionHovered, setIsOptionHovered] = useState<boolean>(false);
  const [searchQuery, setSearchQuery] = useState<string>("");

  const debouncedQuery = useDebounce(searchQuery, 300);

  useEffect(() => {
    if (debouncedQuery.trim() === "") {
      setOptionsDeepCopy(options || []);
      return;
    }

    const filteredOptions = options?.filter((option) =>
      option.toLowerCase().includes(debouncedQuery.toLowerCase())
    );
    setOptionsDeepCopy(filteredOptions || []);
  }, [debouncedQuery]);

  useEffect(() => {
    if (!popupRef.current) return;
    setPopupHeight(popupRef.current.clientHeight + 4);
    popupRef.current.style.display = "none";
    popupRef.current.style.height = "0px";
  }, []);

  useEffect(() => {
    if (!popupRef.current) return;

    const el = popupRef.current;

    if (isOpen) {
      el.style.display = "block";
      el.style.transformOrigin = "right top";
      el.style.width = "80%";
      el.style.height = "80%";

      gsap.fromTo(
        el,
        {
          scaleX: 0,
          scaleY: 0,
          opacity: 0,
        },
        {
          scaleX: 1,
          scaleY: 1,
          width: "100%",
          height: popupHeight,
          opacity: 1,
          duration: 0.3,
          ease: "power2.out",
        }
      );
    } else {
      gsap.to(el, {
        scaleX: 0,
        scaleY: 0,
        opacity: 0,
        width: "80%",
        height: "80%",
        duration: 0.25,
        ease: "power2.inOut",
        transformOrigin: "right top",
        onComplete: () => {
          el.style.display = "none";
        },
      });
    }
  }, [isOpen, popupHeight]);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (
        containerRef.current &&
        !containerRef.current.contains(e.target as Node)
      ) {
        setIsOpen(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  return (
    <div
      ref={containerRef}
      className={cn("relative w-48 h-8 text-(--clr-text-secondary)", className)}
    >
      <div
        onMouseUp={() => setIsOpen((prev) => !prev)}
        className="relative hover:bg-(--clr-surface-600) px-2 py-5 w-full h-full bg-(--clr-surface-800)
                   transition-colors duration-200 rounded-md flex flex-row items-center justify-between 
                   cursor-pointer border border-(--clr-surface-600)"
      >
        <span>{selectedOption}</span>
        <div className="w-6 h-6 flex flex-col items-center justify-center relative">
          <ChevronUp size={12} className="absolute top-0.5" />
          <ChevronDown size={12} className="absolute bottom-0.5" />
        </div>
      </div>

      <div
        ref={popupRef}
        className="w-full h-fit absolute top-10.5 left-0 border border-(--clr-surface-600)
                   bg-(--clr-surface-800) rounded-md shadow-md mt-1 z-10 overflow-hidden"
      >
        <div className="w-full h-12 mb-3 px-4 flex flex-row   items-center">
          <input
            type="text"
            className="w-full h-12 p-1 border-0 border-b border-(--clr-surface-600) focus:outline-none"
            placeholder="Search..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <Search className="text-(--clr-text-secondary) w-4 h-4" />
        </div>
        {optionsDeepCopy.map((option, index) => (
          <div
            key={index}
            className={cn(
              "hover:bg-(--clr-surface-600)  rounded-sm cursor-pointer p-1",
              option === selectedOption && !isOptionHovered
                ? "bg-(--clr-surface-600)"
                : ""
            )}
            onMouseEnter={() => setIsOptionHovered(true)}
            onMouseLeave={() => setIsOptionHovered(false)}
            onMouseUp={() => onSelectOption(option)}
          >
            {option}
          </div>
        ))}
      </div>
    </div>
  );
}

export { ModernSelectList };
