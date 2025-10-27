import { Spinner } from "./Spinner";

interface LoadPanelProps {
  spinnerSize?: number;
  spinnerColor?: string;
}

function LoadPanel({ spinnerSize, spinnerColor }: LoadPanelProps) {
  return (
    <div className="absolute top-0 left-0 inset-0 bg-transparent w-full h-full z-99">
      <div className="absolute top-0 left-0 inset-0 bg-black opacity-85 z-100 ">
        <div className="w-full h-full flex items-center justify-center z-101">
          <Spinner size={spinnerSize} color={spinnerColor} />
        </div>
      </div>
    </div>
  );
}

export { LoadPanel };
