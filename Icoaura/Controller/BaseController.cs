using Icoaura.Exception;
using Icoaura.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Icoaura.Controller
{
    public class BaseController
    {
        public readonly l10nService _l10nService;

        public BaseController() {
            this._l10nService = DIProvider.Provider.GetRequiredService<l10nService>();
        }

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
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"));
            }
            catch (FileNotFoundException)
            {
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.FileNotFound"));
            }
            catch (DirectoryNotFoundException)
            {
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.DirectoryNotFound"));
            }
            catch (IOException)
            {
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.IOException"));
            }
            // app level exceptions
            catch (AppException ex)
            {
                if (ex.IsOperational)
                    return ApiResponse<T>.Fail(ex.UserMessage);

                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.UnexpectedOperational"));
            }
            // fallback for unpredicted exceptions
            catch (System.Exception)
            {
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.UnexpectedInternal"));
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
