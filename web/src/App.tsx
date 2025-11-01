import { FadeParticles } from "./FadeParticles";

function App() {
  return (
    <div className="w-screen h-screen overflow-hidden bg-[#37353E] relative">
      <div className="fixed inset-0 z-20 pointer-events-none">
        <FadeParticles />
      </div>

      <div
        className="w-full h-120 relative z-10 overflow-hidden"
        style={{
          backgroundColor: "#6a5acd",
          backgroundImage:
            "linear-gradient(319deg, #b284be 0%, #c54b8c 37%, #6a5acd 100%)",
          clipPath: "polygon(0 0, 100% 0, 100% 80%, 0 100%)",
        }}
      >
        <div
          className="absolute top-0 left-[-50%] w-3 h-full animate-sweep pointer-events-none"
          style={{
            background: "rgba(255, 255, 255, 0.1)",
            transform: "skewX(-25deg)",
          }}
        ></div>

        <style>{`
          @keyframes sweep {
            0% { left: -50%; }
            100% { left: 120%; }
          }

          .animate-sweep {
            animation: sweep 10s linear infinite;
          }
        `}</style>
      </div>
    </div>
  );
}

export default App;
