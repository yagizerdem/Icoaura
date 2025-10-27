import { Minus, Maximize, X } from "lucide-react";
import appLogo from "../../assets/Logo.png";

function TitleBar() {
  const onMouseDown = () => {
    window.chrome.webview.hostObjects.windowBridge.StartNativeDrag();
  };

  return (
    <div className="w-full h-10 flex justify-between items-center  select-none  bg-(--clr-surface-700) text-(--clr-text-primary)">
      <div
        className="flex-1 h-full items-center flex px-3 gap-2"
        onMouseDown={onMouseDown}
      >
        <img src={appLogo} className="w-8 h-8 mr-2 rounded-md" />
        <span className="font-medium">Icoaura</span>
      </div>
      <div className="flex flex-row">
        <button
          onMouseUp={() => {
            window.chrome.webview.hostObjects.windowBridge.Minimize();
          }}
          className="p-2 hover:bg-(--clr-surface-400) transition-colors duration-250 text-(--clr-text-primary) cursor-pointer"
        >
          <Minus />
        </button>
        <button
          onMouseUp={() => {
            window.chrome.webview.hostObjects.windowBridge.Maximize();
          }}
          className="p-2 hover:bg-(--clr-surface-400) transition-colors duration-250 text-(--clr-text-primary) cursor-pointer"
        >
          <Maximize />
        </button>
        <button
          onMouseUp={() => {
            window.chrome.webview.hostObjects.windowBridge.Close();
          }}
          className="p-2 hover:bg-(--clr-danger-20) transition-colors duration-250 text-(--clr-text-primary) cursor-pointer "
        >
          <X />
        </button>
      </div>
    </div>
  );
}

export { TitleBar };
