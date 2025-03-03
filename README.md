# DesktopApiBuilder

**Программный комплекс** предназначен для автоматизированного формирования серверных ASP .NET Core приложений с задаваемой архитектурой на базе настраиваемых шаблонов. Он предоставляет функционал для автоматизированного формирования базовых модулей приложений на основе пользовательских настроек. Для удобства использования приложения был разработан графический пользовательский интерфейс.

Приложение формирует:

- решение на основе выбранных пользователем конфигураций;

- проекты на основе выбранных пользователем конфигураций;

- классы на основе выбранных пользователем конфигураций и доменной области;

- подключения к СУБД и миграции на основе выбранных пользователем конфигураций;

- unit-тесты.


Пользовательская настройка выполняется с помощью конфигурационных файлов следующей структуры.

**Описание структуры конфигурационных файлов.**


### config.json file example for _3-Layerd_ architecture

```
{
    "solutionFolders": [],
    "projects": [
        { 
            "name": "Web", 
            "type": "webapi",
            "directories": [
                { 
                    "name": "Controllers", 
                    "parentPath": "",
                    "contentType": "Controller"
                },
                { 
                    "name": "Extensions", 
                    "parentPath": "",
                    "contentType": "ServiceExtensionsWithServices"
                }
            ],
            "dependencies": {
                "packages": [
                    "Microsoft.EntityFrameworkCore.Design",
                    "Microsoft.EntityFrameworkCore.Tools"
                ],
                "projectReferences": [ "Core" ]
            },
            "rootContentTypes": [ "ProgramClass" ]
        },
        { 
            "name": "Core", 
            "type": "classlib",
            "directories": [
                { 
                    "name": "Dtos", 
                    "parentPath": "",
                    "contentType": "DtoClass"
                },
                { 
                    "name": "Services", 
                    "parentPath": "",
                    "contentType": "ServiceClass"
                },
                { 
                    "name": "Abstractions", 
                    "parentPath": "/Services",
                    "contentType": "ServiceInterface"
                }
            ],
            "dependencies": {
                "packages": [ "AutoMapper" ],
                "projectReferences": [ "Infrastructure" ]
            },
            "rootContentTypes": [ "MappingProfile" ]
        },
        { 
            "name": "Infrastructure", 
            "type": "classlib",
            "directories": [
                {
                    "name": "Entities",
                    "parentPath": "",
                    "contentType": "EntityClass"
                },
                { 
                    "name": "Repositories", 
                    "parentPath": "",
                    "contentType": "RepositoryClass"
                },
                { 
                    "name": "Abstractions", 
                    "parentPath": "/Repositories",
                    "contentType": "RepositoryInterface"
                }
            ],
            "dependencies": {
                "packages": [
                    "Microsoft.EntityFrameworkCore",
                    "Microsoft.EntityFrameworkCore.SqlServer", 
                    "Microsoft.EntityFrameworkCore.Relational"
                ]
            },
            "rootContentTypes": [ "DbContext" ]
        }
    ],
    "migrationsProjectName": "Infrastructure"
}
```

### config.json file example for _Clean_ architecture

```
{
    "solutionFolders": [
        {
            "id": 1,
            "name": "Core"
        },
        {
            "id": 2,
            "name": "Infrastructure"
        },
        {
            "id": 3,
            "name": "Presentation"
        }
    ],
    "projects": [
        { 
            "name": "Web", 
            "type": "webapi",
            "directories": [
                { 
                    "name": "Controllers", 
                    "parentPath": "",
                    "contentType": "ControllerWithMediatr"
                },
                { 
                    "name": "Extensions", 
                    "parentPath": "",
                    "contentType": "ServiceExtensions"
                }
            ],
            "dependencies": {
                "packages": [ 
                    "AutoMapper",
                    "Microsoft.EntityFrameworkCore.Design",
                    "Microsoft.EntityFrameworkCore.Tools"
                 ],
                "projectReferences": [ "Application", "Infrastructure" ]
            },
            "rootContentTypes": [ "ProgramClassWithMediatr" ],
            "solutionFolderId": 3
        },
        { 
            "name": "Infrastructure", 
            "type": "classlib",
            "directories": [
                { 
                    "name": "Repositories", 
                    "parentPath": "",
                    "contentType": "RepositoryClass"
                }
            ],
            "dependencies": {
                "packages": [ 
                    "Microsoft.EntityFrameworkCore",
                    "Microsoft.EntityFrameworkCore.SqlServer", 
                    "Microsoft.EntityFrameworkCore.Relational"
                 ],
                "projectReferences": [ "Domain" ]
            },
            "rootContentTypes": [ "DbContext" ],
            "solutionFolderId": 2
        },
        { 
            "name": "Application", 
            "type": "classlib",
            "directories": [
                { "name": "Requests", "parentPath": "" },
                { "name": "RequestHandlers", "parentPath": "" },
                { 
                    "name": "Dtos", 
                    "parentPath": "",
                    "contentType": "DtoClass"
                },
                { 
                    "name": "Queries", 
                    "parentPath": "/Requests",
                    "contentTypeList": [ "GetAllQuery", "GetByIdQuery" ]
                },
                { 
                    "name": "Commands", 
                    "parentPath": "/Requests",
                    "contentTypeList": [ "CreateCommand", "UpdateCommand", "DeleteCommand" ]
                },
                { 
                    "name": "QueryHandlers", 
                    "parentPath": "/RequestHandlers",
                    "contentTypeList": [ "GetAllQueryHandler", "GetByIdQueryHandler" ]
                },
                { 
                    "name": "CommandHandlers", 
                    "parentPath": "/RequestHandlers",
                    "contentTypeList": [ "CreateCommandHandler", "UpdateCommandHandler", "DeleteCommandHandler" ] 
                }
            ],
            "dependencies": {
                "packages": [ "MediatR", "AutoMapper" ],
                "projectReferences": [ "Domain" ]
            },
            "rootContentTypes": [ "MappingProfile" ],
            "solutionFolderId": 1
        },
        { 
            "name": "Domain", 
            "type": "classlib",
            "directories": [
                { 
                    "name": "Entities", 
                    "parentPath": "",
                    "contentType": "EntityClass"
                },
                { 
                    "name": "Abstractions", 
                    "parentPath": "",
                    "contentType": "RepositoryInterface"
                }
            ],
            "solutionFolderId": 1
        }
    ],
    "migrationsProjectName": "Infrastructure"
}
```
