namespace DesktopApiBuilder.App.Data.Models.Results;

public class MapResult<T>
{
    public T Value { get; set; }
    public string ErrorMessage { get; set; }
}
