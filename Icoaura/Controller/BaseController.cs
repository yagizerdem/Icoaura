using Icoaura.Exception;
using Icoaura.Model;
using System;
using System.IO;

namespace Icoaura.Controller
{
    public class BaseController
    {
        public BaseController() { }

        public ApiResponse<T> ExecuteSafe<T>(Func<T> func)
        {
            try
            {
                T result = func();
                return ApiResponse<T>.Ok(result);
            }
            // --- predicted user related exceptions hataları ---
            catch (UnauthorizedAccessException)
            {
                return ApiResponse<T>.Fail("You do not have permission to access this resource.");
            }
            catch (FileNotFoundException)
            {
                return ApiResponse<T>.Fail("The specified file could not be found.");
            }
            catch (DirectoryNotFoundException)
            {
                return ApiResponse<T>.Fail("The specified folder could not be found.");
            }
            catch (IOException)
            {
                return ApiResponse<T>.Fail("A file I/O error occurred while processing your request.");
            }
            // app level exceptions
            catch (AppException ex)
            {
                if (ex.IsOperational)
                    return ApiResponse<T>.Fail(ex.UserMessage);

                return ApiResponse<T>.Fail("Unexpected operational error occurred.");
            }
            // fallback for unpredicted exceptions
            catch (System.Exception)
            {
                return ApiResponse<T>.Fail("Unexpected internal error occurred.");
            }
        }
 
        public void EnsureSuccess<T>(ApiResponse<T> response)
        {
            if (!response.Success)
            {
                throw new AppException(response.ErrorMessage, isOperational: true);
            }
        }
    }
}
