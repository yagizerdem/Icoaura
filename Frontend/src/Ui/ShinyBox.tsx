import { useEffect, useRef } from "react";
import { gsap } from "gsap";

function ShinyBox() {
  const containerRef = useRef<HTMLDivElement>(null);
  const shinyRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!containerRef.current || !shinyRef.current) return;

    gsap.to(shinyRef.current, {
      x: 50,
      y: 50,
      duration: 2,
      repeat: -1,
      repeatDelay: 3,
      delay: 1,
    });
  }, []);

  return (
    <div
      ref={containerRef}
      className="relative w-12 h-12 bg-gray-800 overflow-hidden  rounded-sm"
    >
      <div
        ref={shinyRef}
        className="
    absolute w-20 h-1 rotate-135
    bg-gradient-to-br from-white/70 via-white/10 to-transparent
    bg-[length:200%_200%]
    animate-[shine_2s_linear_infinite]
    opacity-60 rounded-full shadow-lg
    top-0 left-0 transform -translate-x-1/2 -translate-y-1/2
  "
      ></div>
    </div>
  );
}

export { ShinyBox };
