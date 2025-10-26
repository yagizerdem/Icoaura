
namespace Icoaura.Controller
{
    class FileController
    {
        public FileController()
        {
         
            
        }

        public ServiceResponse<string> ReadFileContentAsText(string path)
        {
            return this.ExecuteSafe(() =>
            {
                EnsureFileExist(path);
                string serialized = File.ReadAllText(path);
                return serialized;
            }, nameof(ReadFileContentAsText));
        }


    }
}
