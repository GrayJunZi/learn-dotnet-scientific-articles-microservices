# learn-dotnet-scientific-articles-microservices

我们将从零开始构建真正的C#微服务和.NET 10解决方案中的模块化单体：一个完整的科学文章管理MVP。你将学习如何将领域驱动设计（DDD）和垂直切片架构应用于真实的业务工作流程——而非玩具示例。

你将看到如何将业务需求转化为有界上下文、聚合、价值对象和领域事件，然后用C#、ASP.NET Core、EF Core、gRPC、消息和Docker端到端实现它们。每个功能都构建为垂直切片：从API合同和验证，经过命令处理程序和域逻辑，再到持久化和测试。

我们实现了服务之间的同步和异步通信。你将学会区分核心/关键服务（使用直接的gRPC以提升可靠性）和领域服务（采用事件驱动架构并带有消息传递）。这让你在真实分布式系统中获得CQRS和事件驱动模式的实际经验。

在此过程中，我们关注实际架构决策：何时使用微服务，何时使用模块化单体，如何设计丰富的域而非“贫乏”模型，如何在 MediatR 上应用 CQRS，以及如何在服务通过 gRPC 和消息传递通信时保持代码干净、简洁且可测试。

## 一、介绍

### 1.1 技术栈

- 架构设计
    - 领域驱动设计（Domain-Driven Design, DDD）
    - 垂直切片架构（Vertical Slicing Architecture）
- 类库
    - .NET 10
    - Minimal API
    - MediatR
    - FastEndpoints
    - Carter
    - MassTransit
    - EF Core
    - GraphQL
    - Mapster
    - gRPC
    - Docker
- 数据存储
    - SQL Server
    - PostgreSQL
    - MongoDB
    - Redis

### 1.2 学习目标

**架构设计**

- 应用领域驱动设计来建模真实的业务需求。
- 使用垂直切片架构(Vertical Slicing Architecture)和整洁架构(Clean Architecture)来组织微服务。
- 根据需要设计模块化单体(Modular Monolith)并将其拆分为微服务。
- 使用c4图、序列流和战术ddd模式来可视化架构。

**实现模式**

- 使用 MediatR 和 FastEndpoints 来实现 CQRS 模式。
- 使用 SaveChangesInterceptor 实现领域驱动设计(Domain-Driven Design)。
- 应用事件驱动设计来解耦通信。

**API与通信**

- 使用 ASP.NET Core 和 Carter 来构建 Minimal APIs。
- 使用 gRPC 来实现同步通信。
- 使用 MassTransit 来实现异步通信。
- 集成 GraphQL 和 Hasura 来实现从 PostgreSQL 中高效查询数据。

**数据存储**

- 使用 SQL Server、PostgreSQL 和 Redis 存储数据。
- 使用 Mongo GridFS 存储文件。

**工具与基础设施**

- 使用 Mapster 或 AutoMapper 来映射 DTOs 和 集成事件(Integration Events)。
- 使用 .NET Identity 和 JWT 来实现用户认证和授权。
- 使用 Docker 来容器化应用程序。