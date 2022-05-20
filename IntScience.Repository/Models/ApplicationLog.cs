namespace IntScience.Repository.Models;

public class ApplicationLog
{
    public int ApplicationLogId { get; set; }
    public string Severity { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
}
