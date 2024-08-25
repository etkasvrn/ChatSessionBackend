namespace ChatSessionBackend.Helpers
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        private Result(bool isSuccess, string message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result Success() => new Result(true);
        public static Result Fail(string message) => new Result(false, message);
    }

}
