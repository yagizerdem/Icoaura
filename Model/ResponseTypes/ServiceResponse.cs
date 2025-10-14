namespace Model.ResponseTypes
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }

        private ServiceResponse(T? data, bool success, string? message)
        {
            Data = data;
            Success = success;
            Message = message;
        }

        public static ServiceResponse<T> Ok(T data, string? message = null)
        {
            return new ServiceResponse<T>(data, true, message);
        }

        public static ServiceResponse<T> Fail(string message)
        {
            return new ServiceResponse<T>(default, false, message);
        }

        public static ServiceResponse<T> OkMessage(string message)
        {
            return new ServiceResponse<T>(default, true, message);
        }
    }
}
