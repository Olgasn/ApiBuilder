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
    DtoForCreationClass,
    DtoForUpdateClass,
    MappingProfile,
    ServiceClass,
    ServiceInterface,

    // API
    ProgramClass,
    ProgramClassWithMediatr,
    Controller,
    ControllerWithMediatr,
    ServiceExtensions,
    ServiceExtensionsWithServices,

    // CQRS
    GetAllQuery,
    GetByIdQuery,
    CreateCommand,
    UpdateCommand,
    DeleteCommand,
    GetAllQueryHandler,
    GetByIdQueryHandler,
    CreateCommandHandler,
    UpdateCommandHandler,
    DeleteCommandHandler,

    Undefined
}
