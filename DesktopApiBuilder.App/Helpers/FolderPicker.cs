namespace DesktopApiBuilder.App.Helpers;

public static class FolderPicker
{
    public static async Task<string> PickFolder(string? currentLocation = null)
    {
        var folderPicker = new Windows.Storage.Pickers.FolderPicker();
        var hwnd = ((MauiWinUIWindow)App.Current!.Windows[0].Handler.PlatformView!).WindowHandle;

        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

        var folder = await folderPicker.PickSingleFolderAsync();
        var path = currentLocation;

        if (folder is not null)
        {
            path = folder.Path;
        }

        return path ?? string.Empty;
    }
}
