namespace IntScience.Services;

public class ApplicationResult<TResult>
{
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public TResult ResultData { get; set; }

    public static ApplicationResult<TResult> Success(TResult resultData) => new() { IsSuccess = true, ResultData = resultData };
    public static ApplicationResult<TResult> Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors, ResultData = default };
}
