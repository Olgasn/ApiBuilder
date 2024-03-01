# DesktopApiBuilder

### config.json file example for _3-Layerd_ architecture

```
{
    "solutionFolders" : [],
    "projects" : [
        { 
            "name" : "API", 
            "type" : "webapi",
            "directories": [
                { "name" : "Controllers", "parentPath" : "" },
                { "name" : "Extensions", "parentPath" : "" }
            ]
        },
        { 
            "name" : "BLL", 
            "type" : "classlib",
            "directories": [
                { "name" : "Dtos", "parentPath" : "" },
                { "name" : "Services", "parentPath" : "" },
                { "name" : "Abstractions", "parentPath" : "/Services" }
            ]
        },
        { 
            "name" : "DAL", 
            "type" : "classlib",
            "directories": [
                { "name" : "Entities", "parentPath" : "" },
                { "name" : "Repositories", "parentPath" : "" },
                { "name" : "Abstractions", "parentPath" : "/Repositories" }
            ]
        }
    ]
}
```

### config.json file example for _Clean_ architecture

```
{
    "solutionFolders" : [
        {
            "id" : 1,
            "name" : "Core"
        },
        {
            "id" : 2,
            "name" : "Infrastructure"
        },
        {
            "id" : 3,
            "name" : "Presentation"
        }
    ],
    "projects" : [
        { 
            "name" : "API", 
            "type" : "webapi",
            "directories": [
                { "name" : "Extensions", "parentPath" : "" }
            ],
            "solutionFolderId" : 3
        },
        { 
            "name" : "Infrastructure", 
            "type" : "classlib",
            "directories": [
                { "name" : "Repositories", "parentPath" : "" }
            ],
            "solutionFolderId" : 2
        },
        { 
            "name" : "Persistence", 
            "type" : "classlib",
            "directories": [
                { "name" : "Controllers", "parentPath" : "" }
            ],
            "solutionFolderId" : 2
        },
        { 
            "name" : "Application", 
            "type" : "classlib",
            "directories": [
                { "name" : "Requests", "parentPath" : "" },
                { "name" : "RequestHandlers", "parentPath" : "" },
                { "name" : "Queries", "parentPath" : "/Requests" },
                { "name" : "Commands", "parentPath" : "/Requests" },
                { "name" : "QueryHandlers", "parentPath" : "/RequestHandlers" },
                { "name" : "CommandHandlers", "parentPath" : "/RequestHandlers" }
            ],
            "solutionFolderId" : 1
        },
        { 
            "name" : "Domain", 
            "type" : "classlib",
            "directories": [
                { "name" : "Entities", "parentPath" : "" },
                { "name" : "Abstractions", "parentPath" : "/Repositories" }
            ],
            "solutionFolderId" : 1
        }
    ]
}
```
