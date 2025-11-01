import { FadeParticles } from "./FadeParticles";
import { PanelsShowcase } from "./PanelsShowcase";
import demo from "./assets/demo.mp4";

function App() {
  const handleDownload = () => {
    const link = document.createElement("a");
    link.href = `${import.meta.env.BASE_URL}Icoaura.zip`;
    link.download = "IcoauraApp.zip";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  return (
    <div
      id="style-4"
      className="w-screen h-screen overflow-y-auto bg-[#37353E] relative"
    >
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

      <div className="w-full h-fit -my-[100px] relative z-80 bg-transparent flex flex-col items-center text-center px-6">
        <h1
          className="text-6xl md:text-7xl font-extrabold bg-clip-text text-transparent 
               bg-gradient-to-r from-pink-400 via-purple-400 to-indigo-400 
               drop-shadow-[0_0_12px_rgba(200,100,255,0.3)] 
               animate-[fadeIn_1.5s_ease-in-out]"
          style={{
            textShadow: `
    0 0 10px rgba(255,255,255,0.1),
    0 0 20px rgba(255,255,255,0.2),
    0 0 30px rgba(255,255,255,0.05),
    0 0 40px rgba(255,255,255,0.05)
  `,
          }}
        >
          Icoaura
        </h1>

        <p
          className="mt-4 max-w-[700px] text-lg md:text-xl text-gray-300 
               leading-relaxed font-light animate-[fadeIn_2.5s_ease-in-out]"
        >
          Create, design, and personalize icon packs effortlessly — where
          creativity meets precision.
        </p>
        <div className="flex flex-row gap-4 justify-center mt-10">
          {/* Download Button */}
          <button
            onMouseUp={() => handleDownload()}
            className="cursor-pointer relative px-6 py-3 rounded-lg font-semibold text-white 
               bg-gradient-to-r from-pink-500 via-purple-500 to-indigo-500 
               shadow-[0_0_20px_rgba(150,100,255,0.4)] 
               transition-all duration-300 ease-out 
               hover:scale-105 hover:shadow-[0_0_35px_rgba(180,120,255,0.7)] 
               active:scale-95"
          >
            <span className="relative z-10">Download</span>
          </button>

          {/* GitHub Button */}
          <button
            className="cursor-pointer relative px-6 py-3 rounded-lg font-semibold text-white 
               bg-[rgba(255,255,255,0.1)] backdrop-blur-md border border-[rgba(255,255,255,0.2)] 
               hover:border-[rgba(255,255,255,0.5)] hover:bg-[rgba(255,255,255,0.15)] 
               transition-all duration-300 ease-out 
               hover:scale-105 active:scale-95"
            onMouseUp={() =>
              window.open("https://github.com/yagizerdem/Icoaura")
            }
          >
            <div className="flex items-center gap-2">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="currentColor"
                viewBox="0 0 16 16"
                className="w-5 h-5"
              >
                <path
                  d="M8 0C3.58 0 0 3.58 0 8a8 8 0 0 0 5.47 7.59c.4.07.55-.17.55-.38 
                 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52
                 0-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07
                 -1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 
                 0 0 .67-.21 2.2.82a7.5 7.5 0 0 1 2-.27 7.5 7.5 0 0 1 2 .27c1.53-1.03 2.2-.82 
                 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 
                 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 
                 0 1.07-.01 1.93-.01 2.19 0 .21.15.46.55.38A8.001 8.001 0 0 0 16 8
                 c0-4.42-3.58-8-8-8z"
                />
              </svg>
              <span>GitHub</span>
            </div>
          </button>
        </div>

        <div
          className="max-w-[480px] w-5/6  h-[270px] mt-10 mx-auto rounded-xl overflow-hidden 
               shadow-[0_0_30px_rgba(255,255,255,0.2)] 
               animate-[fadeInUp_2s_ease-out]"
        >
          <video
            className="w-full h-full object-cover rounded-lg"
            autoPlay
            loop
            controls
            muted
            playsInline
          >
            <source src={demo} type="video/mp4" />
            Your browser does not support the video tag.
          </video>
        </div>

        {/* Animations */}
        <style>{`
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }
    @keyframes fadeInUp {
      from { opacity: 0; transform: translateY(30px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `}</style>

        <div className="w-full h-fit my-10">
          <PanelsShowcase />
        </div>
      </div>
    </div>
  );
}

export default App;
