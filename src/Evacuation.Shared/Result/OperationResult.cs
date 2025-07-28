namespace Evacuation.Shared.Result
{
    public class OperationResult<T>
    {
        public bool Success { get; private set; }
        public string? Message { get; private set; }
        public T? Data { get; private set; }
        public Exception? Exception { get; private set; }
        public Dictionary<string, string[]>? Errors { get; private set; }

        private OperationResult(bool success, string? message = null, T? data = default, Exception? exception = null, Dictionary<string, string[]>? errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Exception = exception;
            Errors = errors;
        }

        public static OperationResult<T> Ok(T data, string? message = null) 
            => new OperationResult<T>(true, message, data);
        
        public static OperationResult<T> Ok(string? message = null) 
            => new OperationResult<T>(true, message);

        public static OperationResult<T> Fail(string message, T? data, Exception? exception = null)
            => new OperationResult<T>(false, message, data, exception);

        public static OperationResult<T> Fail(Dictionary<string, string[]> errors, string? message = null)
            => new OperationResult<T>(false, message, default, null, errors);

    }


}
