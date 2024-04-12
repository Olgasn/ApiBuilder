namespace DesktopApiBuilder.App.Data.Enums;

public enum DirectoryContentType
{
    // Domain
    EntityClass,
    RepositoryClass,
    RepositoryInterface,
    DbContext,

    // Business logic
    DtoClass,
    MappingProfile,
    ServiceClass,
    ServiceInterface,

    // API
    ProgramClass,
    Controller,
    ServiceExtensions,

    // CQRS
    GetAllQuery,
    GetByIdQuery,

    Undefined
}
