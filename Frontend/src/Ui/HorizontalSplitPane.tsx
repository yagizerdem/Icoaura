import { useEffect, useState } from "react";
import { Fragment } from "react/jsx-runtime";
import { cn } from "../util/twUtil";

interface HorizontalSplitPaneProps {
  leftChildren: React.ReactNode;
  rightChildren: React.ReactNode;
  leftPanelInitialSize?: number;
  leftPanelMinSize?: number;
  leftPanelMaxSize?: number;
  splitterClassName?: string;
}

function HorizontalSplitPane({
  leftChildren,
  rightChildren,
  leftPanelInitialSize,
  leftPanelMinSize,
  leftPanelMaxSize,
  splitterClassName,
}: HorizontalSplitPaneProps) {
  const [leftPanelSize, setLeftPanelSize] = useState<number>(
    leftPanelInitialSize || 200
  );

  const [isDragging, setIsDragging] = useState<boolean>(false);

  useEffect(() => {
    if (isDragging) {
      // Disable pointer events globally or on a specific element
      document.body.style.pointerEvents = "none";
      document.body.style.userSelect = "none";
    }

    // Cleanup (when isDragging becomes false or component unmounts)
    return () => {
      document.body.style.pointerEvents = "auto";
      document.body.style.userSelect = "auto";
    };
  }, [isDragging]);

  useEffect(() => {
    function onMouseUp(event: MouseEvent) {
      if (isDragging) {
        const MIN_WIDTH = leftPanelMinSize || 0;
        const MAX_WIDTH = leftPanelMaxSize || window.innerWidth;
        const clientX = Math.max(MIN_WIDTH, Math.min(event.clientX, MAX_WIDTH));
        setLeftPanelSize(clientX);
      }

      setIsDragging(false);
    }

    function onMouseMove(event: MouseEvent) {
      if (isDragging) {
        const MIN_WIDTH = leftPanelMinSize || 0;
        const MAX_WIDTH = leftPanelMaxSize || window.innerWidth;
        const clientX = Math.max(MIN_WIDTH, Math.min(event.clientX, MAX_WIDTH));
        setLeftPanelSize(clientX);
      }
    }

    window.addEventListener("mouseup", onMouseUp);
    window.addEventListener("mousemove", onMouseMove);
    return () => {
      window.removeEventListener("mouseup", onMouseUp);
      window.removeEventListener("mousemove", onMouseMove);
    };
  }, [isDragging, leftPanelMaxSize, leftPanelMinSize]);

  return (
    <Fragment>
      <div className="w-full h-full flex flex-row ">
        <div
          className="flex flex-col"
          style={{
            width: `${leftPanelSize}px`,
          }}
        >
          {leftChildren}
        </div>
        <div
          className={cn(
            "w-1 h-full bg-[var(--clr-surface-300)] cursor-e-resize",
            splitterClassName
          )}
          onMouseDown={() => setIsDragging(true)}
        ></div>
        <div className="flex-1 ">{rightChildren}</div>
      </div>
    </Fragment>
  );
}

export { HorizontalSplitPane };
