using DesktopApiBuilder.App.Data.ViewModels;
using DesktopApiBuilder.App.Helpers;

namespace DesktopApiBuilder.App.Services;

public static class ClassService
{
    private const string Path = "C:\\D\\Projects\\test";

    private const string EntityTemplatePath = "wwwroot\\templates\\domain\\EntityClassTemplate.txt";
    private const string DtoTemplatePath = "wwwroot\\templates\\core\\DtoClassTemplate.txt";
    private const string EntityPropTemplate = "\r\n\tpublic {0} {1} {{ get; set; }}";

    private const string RepositoryInterfaceTemplatePath = "wwwroot\\templates\\domain\\RepositoryInterfaceTemplate.txt";
    private const string RepositoryTemplatePath = "wwwroot\\templates\\domain\\RepositoryTemplate.txt";

    private const string DbContextTemplatePath = "wwwroot\\templates\\domain\\DbContextTemplate.txt";
    private const string DbSetTemplate = "\r\n\tpublic DbSet<{0}> {1} {{ get; set; }}";

    private const string BaseRepositoryInterfaceTemplatePath = "wwwroot\\templates\\domain\\BaseRepositoryInterfaceTemplate.txt";
    private const string BaseRepositoryTemplatePath = "wwwroot\\templates\\domain\\BaseRepositoryTemplate.txt";

    private const string ServiceInterfaceTemplatePath = "wwwroot\\templates\\core\\ServiceInterfaceTemplate.txt";
    private const string ServiceTemplatePath = "wwwroot\\templates\\core\\ServiceTemplate.txt";
    
    private const string ControllerTemplatePath = "wwwroot\\templates\\api\\ControllerTemplate.txt";
    
    private const string MappingProfileTemplatePath = "wwwroot\\templates\\core\\MappingProfileTemplate.txt";
    private const string MappingItemTemplate = "\r\n\t\tCreateMap<{0}, {0}Dto>();";
    
    private const string ProgramClassTemplatePath = "wwwroot\\templates\\api\\ApiProgramClassTemplate.txt";
    
    private const string ServiceExtensionsTemplatePath = "wwwroot\\templates\\api\\ServiceExtensionsTemplate.txt";
    private const string RepositoryRegTemplate = "\r\n\t\tservices.AddScoped<I{0}Repository, {0}Repository>();";
    private const string ServiceRegTemplate = "\r\n\t\tservices.AddScoped<I{0}Service, {0}Service>();";

    public static void CreateEntityClass(string className, string solutionName, Dictionary<string, string> properties)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(EntityTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.DAL\\Entities\\{className}.cs");

            var props = string.Empty;
            foreach (var prop in properties)
            {
                props += string.Format(EntityPropTemplate, prop.Value, prop.Key);
            }

            sw.WriteLine(string.Format(fileContent, $"{solutionName}.DAL.Entities", className, props));
            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void CreateRepository(string entityName, string solutionName)
    {
        try
        {
            var interfaceTemplateContent = TemplateHelper.GetTemplateContent(RepositoryInterfaceTemplatePath);
            var classTemplateContent = TemplateHelper.GetTemplateContent(RepositoryTemplatePath);

            StreamWriter sw1 = new($"{Path}\\{solutionName}\\{solutionName}.DAL\\Repositories\\Abstractions\\I{entityName}Repository.cs");

            sw1.WriteLine(string.Format(interfaceTemplateContent, 
                $"{solutionName}.DAL.Entities",
                $"{solutionName}.DAL.Repositories.Abstractions", 
                entityName,
                $"int"));

            sw1.Close();

            StreamWriter sw2 = new($"{Path}\\{solutionName}\\{solutionName}.DAL\\Repositories\\{entityName}Repository.cs");

            sw2.WriteLine(string.Format(classTemplateContent,
                $"{solutionName}.DAL.Entities",
                $"{solutionName}.DAL.Repositories.Abstractions",
                $"{solutionName}.DAL.Repositories",
                entityName,
                $"int"));

            sw2.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void CreateDbContext(string solutionName, List<EntityClassViewModel> entities)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(DbContextTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.DAL\\AppDbContext.cs");

            var props = string.Empty;
            foreach (var entity in entities)
            {
                props += string.Format(DbSetTemplate, entity.Name, entity.PluralName);
            }

            sw.WriteLine(string.Format(fileContent,
                $"{solutionName}.DAL.Entities",
                $"{solutionName}.DAL",
                props));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        ProcessManager.ExecuteCmdCommands([
            $"cd {Path}\\{solutionName}\\{solutionName}.DAL",
            "dotnet add package Microsoft.EntityFrameworkCore"
        ]);
    }

    public static void CreateBaseRepository(string solutionName)
    {
        try
        {
            var interfaceTemplateContent = TemplateHelper.GetTemplateContent(BaseRepositoryInterfaceTemplatePath);
            var classTemplateContent = TemplateHelper.GetTemplateContent(BaseRepositoryTemplatePath);

            StreamWriter sw1 = new($"{Path}\\{solutionName}\\{solutionName}.DAL\\Repositories\\Abstractions\\IRepository.cs");

            sw1.WriteLine(string.Format(interfaceTemplateContent, $"{solutionName}.DAL.Repositories.Abstractions"));
            sw1.Close();

            StreamWriter sw2 = new($"{Path}\\{solutionName}\\{solutionName}.DAL\\Repositories\\BaseRepository.cs");

            sw2.WriteLine(string.Format(classTemplateContent, 
                $"{solutionName}.DAL.Repositories.Abstractions",
                $"{solutionName}.DAL.Repositories"));

            sw2.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void CreateDtoClass(string className, string solutionName, Dictionary<string, string> properties)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(DtoTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.BLL\\Dtos\\{className}Dto.cs");

            var props = string.Empty;
            foreach (var prop in properties)
            {
                props += string.Format(EntityPropTemplate, prop.Value, prop.Key);
            }

            sw.WriteLine(string.Format(fileContent, $"{solutionName}.BLL.Dtos", className, props));
            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void CreateService(string entityName, string solutionName)
    {
        try
        {
            var interfaceTemplateContent = TemplateHelper.GetTemplateContent(ServiceInterfaceTemplatePath);
            var classTemplateContent = TemplateHelper.GetTemplateContent(ServiceTemplatePath);

            StreamWriter sw1 = new($"{Path}\\{solutionName}\\{solutionName}.BLL\\Services\\Abstractions\\I{entityName}Service.cs");

            sw1.WriteLine(string.Format(interfaceTemplateContent,
                $"{solutionName}.BLL.Dtos",
                $"{solutionName}.BLL.Services.Abstractions",
                entityName,
                $"int"));

            sw1.Close();

            StreamWriter sw2 = new($"{Path}\\{solutionName}\\{solutionName}.BLL\\Services\\{entityName}Service.cs");

            sw2.WriteLine(string.Format(classTemplateContent,
                $"{solutionName}.DAL.Entities",
                $"{solutionName}.DAL.Repositories.Abstractions",
                $"{solutionName}.BLL.Services.Abstractions",
                $"{solutionName}.BLL.Dtos",
                $"{solutionName}.BLL.Services",
                entityName,
                $"int"));

            sw2.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void CreateController(string solutionName, EntityClassViewModel entity)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(ControllerTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.API\\Controllers\\{entity.Name}Controller.cs");

            sw.WriteLine(string.Format(fileContent, 
                $"{solutionName}.BLL.Dtos", 
                $"{solutionName}.BLL.Services.Abstractions",
                $"{solutionName}.API.Controllers",
                $"{entity.PluralName[..1].ToLower()}{entity.PluralName[1..]}",
                entity.Name,
                $"{entity.Name[..1].ToLower()}{entity.Name[1..]}",
                "int"));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void CreateMappingProfile(string solutionName, List<EntityClassViewModel> entities)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(MappingProfileTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.BLL\\MappingProfile.cs");

            var props = string.Empty;
            foreach (var entity in entities)
            {
                props += string.Format(MappingItemTemplate, entity.Name);
            }

            sw.WriteLine(string.Format(fileContent,
                $"{solutionName}.DAL.Entities",
                $"{solutionName}.BLL.Dtos",
                $"{solutionName}.BLL",
                props));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void CreateServiceExtensions(string solutionName, List<EntityClassViewModel> entities)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(ServiceExtensionsTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.API\\Extensions\\ServiceExtensions.cs");

            var repositoryProps = string.Empty;
            var serviceProps = string.Empty;
            foreach (var entity in entities)
            {
                repositoryProps += string.Format(RepositoryRegTemplate, entity.Name);
                serviceProps += string.Format(ServiceRegTemplate, entity.Name);
            }

            sw.WriteLine(string.Format(fileContent,
                $"{solutionName}.DAL",
                $"{solutionName}.DAL.Repositories",
                $"{solutionName}.DAL.Repositories.Abstractions",
                $"{solutionName}.BLL.Services",
                $"{solutionName}.BLL.Services.Abstractions",
                $"{solutionName}.API.Extensions",
                repositoryProps,
                serviceProps));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
        }
    }

    public static void AddProjectReferences(string solutionName)
    {
        ProcessManager.ExecuteCmdCommands([
            $"cd {Path}\\{solutionName}\\{solutionName}.BLL",
            "dotnet add package AutoMapper",
            $"cd {Path}\\{solutionName}",
            $"dotnet add {solutionName}.BLL/{solutionName}.BLL.csproj reference {solutionName}.DAL/{solutionName}.DAL.csproj",
            $"dotnet add {solutionName}.API/{solutionName}.API.csproj reference {solutionName}.BLL/{solutionName}.BLL.csproj",
            $"cd {Path}\\{solutionName}\\{solutionName}.API",
            "dotnet add package Microsoft.EntityFrameworkCore.SqlServer",
        ]);
    }

    public static void UpdateProgramClass(string solutionName)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(ProgramClassTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.API\\Program.cs");

            sw.WriteLine(string.Format(fileContent, 
                $"{solutionName}.API.Extensions",
                $"{solutionName}.BLL"));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
