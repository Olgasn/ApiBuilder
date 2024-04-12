﻿namespace DesktopApiBuilder.App.Data.Constants;

public static class TemplatePaths
{
    public const string EntityTemplatePath = "wwwroot\\templates\\domain\\EntityClassTemplate.txt";
    public const string DtoTemplatePath = "wwwroot\\templates\\core\\DtoClassTemplate.txt";

    public const string RepositoryInterfaceTemplatePath = "wwwroot\\templates\\domain\\RepositoryInterfaceTemplate.txt";
    public const string RepositoryTemplatePath = "wwwroot\\templates\\domain\\RepositoryTemplate.txt";

    public const string DbContextTemplatePath = "wwwroot\\templates\\domain\\DbContextTemplate.txt";

    public const string ServiceInterfaceTemplatePath = "wwwroot\\templates\\core\\ServiceInterfaceTemplate.txt";
    public const string ServiceTemplatePath = "wwwroot\\templates\\core\\ServiceTemplate.txt";

    public const string ControllerTemplatePath = "wwwroot\\templates\\api\\ControllerTemplate.txt";

    public const string MappingProfileTemplatePath = "wwwroot\\templates\\core\\MappingProfileTemplate.txt";

    public const string ProgramClassTemplatePath = "wwwroot\\templates\\api\\ApiProgramClassTemplate.txt";

    public const string ServiceExtensionsTemplatePath = "wwwroot\\templates\\api\\ServiceExtensionsTemplate.txt";

    public const string GetAllQueryTemplatePath = "wwwroot\\templates\\cqrs\\GetAllQueryTemplate.txt";
    public const string GetByIdQueryTemplatePath = "wwwroot\\templates\\cqrs\\GetByIdQueryTemplate.txt";
    public const string CreateCommandTemplatePath = "wwwroot\\templates\\cqrs\\CreateCommandTemplate.txt";
    public const string UpdateCommandTemplatePath = "wwwroot\\templates\\cqrs\\UpdateCommandTemplate.txt";
    public const string DeleteCommandTemplatePath = "wwwroot\\templates\\cqrs\\DeleteCommandTemplate.txt";
}
