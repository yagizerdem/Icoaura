namespace Model.ResponseTypes
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }

        public bool IsOperational { get; set; } // default false -> means there is system exception do not expose to client but write to system logs

        private ServiceResponse(T? data, bool success, string? message)
        {
            Data = data;
            Success = success;
            Message = message;
        }

        private ServiceResponse(T? data, bool success, string? message, bool isOperational)
        {
            Data = data;
            Success = success;
            Message = message;
            IsOperational = isOperational;
        }

        public static ServiceResponse<T> Ok(T data, string? message = null)
        {
            return new ServiceResponse<T>(data, true, message);
        }

        public static ServiceResponse<T> Fail(string message, bool isOperational = false)
        {
            return new ServiceResponse<T>(default, false, message, isOperational);
        }

        public static ServiceResponse<T> OkMessage(string message)
        {
            return new ServiceResponse<T>(default, true, message);
        }
    }
}
