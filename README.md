# DesktopApiBuilder

### config.json file example

```
{
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
