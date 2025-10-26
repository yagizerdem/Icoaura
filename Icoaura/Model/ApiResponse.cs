
namespace Icoaura.Model
{
    class ApiResponse<T>
    {
        public T Data { get; set; } = default!;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public static ApiResponse<T> Ok(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Success = true,
                SuccessMessage = message ?? string.Empty
            };
        }

        public static ApiResponse<T> OkMessage(string message)
        {
            return new ApiResponse<T>
            {
                Success = true,
                SuccessMessage = message
            };
        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                ErrorMessage = message
            };
        }

        public static ApiResponse<T> Fail(string message, T data)
        {
            return new ApiResponse<T>
            {
                Success = false,
                ErrorMessage = message,
                Data = data
            };
        }
    }

}
