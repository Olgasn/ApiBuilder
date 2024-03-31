# DesktopApiBuilder

### config.json file example for _3-Layerd_ architecture

```
{
    "solutionFolders": [],
    "projects": [
        { 
            "name": "API", 
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
                    "contentType": "ServiceExtensions"
                }
            ],
            "dependencies": {
                "packages": [
                    "Microsoft.EntityFrameworkCore.Design",
                    "Microsoft.EntityFrameworkCore.Tools"
                ],
                "projectReferences": ["BLL"]
            },
            "rootContentTypes": [ "ProgramClass" ]
        },
        { 
            "name": "BLL", 
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
                "packages": ["AutoMapper"],
                "projectReferences": ["DAL"]
            },
            "rootContentTypes": [ "MappingProfile" ]
        },
        { 
            "name": "DAL", 
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
    "migrationsProjectName": "DAL"
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
            "name": "API", 
            "type": "webapi",
            "directories": [
                { "name": "Extensions", "parentPath": "" }
            ],
            "solutionFolderId": 3
        },
        { 
            "name": "Infrastructure", 
            "type": "classlib",
            "directories": [
                { "name": "Repositories", "parentPath": "" }
            ],
            "solutionFolderId": 2
        },
        { 
            "name": "Persistence", 
            "type": "classlib",
            "directories": [
                { "name": "Controllers", "parentPath": "" }
            ],
            "solutionFolderId": 2
        },
        { 
            "name": "Application", 
            "type": "classlib",
            "directories": [
                { "name": "Requests", "parentPath": "" },
                { "name": "RequestHandlers", "parentPath": "" },
                { "name": "Queries", "parentPath": "/Requests" },
                { "name": "Commands", "parentPath": "/Requests" },
                { "name": "QueryHandlers", "parentPath": "/RequestHandlers" },
                { "name": "CommandHandlers", "parentPath": "/RequestHandlers" }
            ],
            "solutionFolderId": 1
        },
        { 
            "name": "Domain", 
            "type": "classlib",
            "directories": [
                { "name": "Entities", "parentPath": "" },
                { "name": "Abstractions", "parentPath": "/Repositories" }
            ],
            "solutionFolderId": 1
        }
    ]
}
```
