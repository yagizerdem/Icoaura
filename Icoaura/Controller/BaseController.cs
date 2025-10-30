using Icoaura.Context;
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

        public readonly loggerServcie _logger;

        public BaseController() {
            this._l10nService = DIProvider.Provider.GetRequiredService<l10nService>();
            this._logger = DIProvider.Provider.GetRequiredService<loggerServcie>();
        }

        public ApiResponse<T> ExecuteSafe<T>(Func<T> func)
        {
            try
            {
                T result = func();
                return ApiResponse<T>.Ok(result);
            }
            // --- predicted user related exceptions hataları ---
            catch (UnauthorizedAccessException ex)
            {
                _logger.Log(ex.Message, TraceContext.TraceId, Enum.LogLevel.Error);
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"));
            }
            catch (FileNotFoundException ex)
            {
                _logger.Log(ex.Message, TraceContext.TraceId, Enum.LogLevel.Error);
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.FileNotFound"));
            }
            catch (DirectoryNotFoundException ex) 
            {
                _logger.Log(ex.Message, TraceContext.TraceId, Enum.LogLevel.Error);
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.DirectoryNotFound"));
            }
            catch (IOException ex)
            {
                _logger.Log(ex.Message, TraceContext.TraceId, Enum.LogLevel.Error);
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.IOException"));
            }
            // app level exceptions
            catch (AppException ex)
            {
                _logger.Log(ex.Message, TraceContext.TraceId, ex.LogLevel);

                if (ex.IsOperational)
                    return ApiResponse<T>.Fail(ex.UserMessage);

                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.UnexpectedOperational"));
            }
            // fallback for unpredicted exceptions
            catch (System.Exception ex)
            {
                _logger.Log(ex.Message, TraceContext.TraceId, Enum.LogLevel.Fatal);
                return ApiResponse<T>.Fail(this._l10nService.GetLocalizedMessage("Errors.UnexpectedInternal"));
            }
        }
 
        public void EnsureSuccess<T>(ApiResponse<T> response)
        {
            _logger.Log(response.ErrorMessage, TraceContext.TraceId, Enum.LogLevel.Error);
            if (!response.Success)
            {
                throw new AppException(response.ErrorMessage, isOperational: true);
            }
        }
    }
}
