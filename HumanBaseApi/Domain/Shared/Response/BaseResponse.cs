namespace Domain.Shared.Response
{
    public abstract class BaseResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public Guid? Id { get; set; }

        public decimal TotalPages { get; set; }
        public int TotalCount { get; set; }

        public List<string> Errors { get; private set; } = new List<string>();

        public BaseResponse() { }

        public void AddError(string error)
        {
            Success = false;
            Errors.Add(error);
        }

        public void SetError(string message, int statusCode)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
        }

        public bool HasErrors => Errors.Count > 0;
    }
}
