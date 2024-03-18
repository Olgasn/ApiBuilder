﻿using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.ViewModels;

namespace DesktopApiBuilder.App.Data.Models;

public class ClassTemplateSettings
{
    public EntityClassViewModel? Entity {  get; set; }
    public string? Template { get; set; }
    public string? ClassName { get; set; }
    public string? Namespace { get; set; }
    public DirectoryContentType ContentType { get; set; }
}
