# MyBot 模板
一个QQ频道机器人框架 [MyBot](https://github.com/Chianne1025/QQChannelFramework) 的模板工程

### 依赖
```
- MyBot

// 日志
- Microsoft.Extensions.Logging
- Microsoft.Extensions.Logging.Console
- Serilog.Extensions.Logging.File

// 数据库
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
```

### 开发

- 克隆本仓库，并重命名文件夹为项目名称
- 运行 `TemplateRename.exe` 更改项目名称为文件夹名称
- 编辑 `App.cs`中的`AppID`、`AppToken`、`AppSecret`等信息
- 编辑 `App.cs`中的`PostgreSQL`数据库信息
- 向`Modules` 文件夹添加模块并在`Program.cs`中注册
- 运行 ▶

[English Version](README_EN.md)

