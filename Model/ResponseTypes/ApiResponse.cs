namespace Model.ResponseTypes
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }

        private ApiResponse(T? data, bool success, string? message)
        {
            Data = data;
            Success = success;
            Message = message;
        }

        public static ApiResponse<T> Ok(T data, string? message = null)
        {
            return new ApiResponse<T>(data, true, message);
        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T>(default, false, message);
        }

        public static ApiResponse<T> OkMessage(string message)
        {
            return new ApiResponse<T>(default, true, message);
        }
    }
}
