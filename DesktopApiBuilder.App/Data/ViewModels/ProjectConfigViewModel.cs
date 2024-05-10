namespace DesktopApiBuilder.App.Data.ViewModels;

public class ProjectConfigViewModel
{
    public string? OldName { get; set; }
    public string? NewName { get; set; }
    public List<DirectoryConfigViewModel> Directories { get; set; }
}
