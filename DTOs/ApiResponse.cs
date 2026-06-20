namespace ceramic.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty!;
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Ok(T data, string message = "Thành công")
       => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string error)
            => new() { Success = false, Message = error, Errors = new List<string> { error } };

        public static ApiResponse<T> Fail(List<string> errors)
            => new() { Success = false, Message = "Có lỗi xảy ra.", Errors = errors };
    }

    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
