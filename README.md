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

## 二、技术概念与架构基础

### 2.1 什么是微服务？

微服务是一种架构风格，它将一个应用程序分解为多个小型、独立的服务，每个服务都运行在自己的进程中，并通过轻量级的通信机制（如 HTTP REST API / gRPC 或消息队列）进行通信。

微服务架构的主要优势包括：

- 每个服务都可以独立开发、部署和扩展。
- 服务之间的通信是异步的，这提高了系统的弹性和可伸缩性。
- 不同的服务可以使用不同的技术栈，这使得技术升级更加容易。

微服务的优势：

- 自治性(Autonomy)：
    - 独立(Independent)：每个服务在资源、运行时和基础设施上独立。
    - 解耦(Decoupled)：每个服务都有自己的数据库和业务逻辑，与其他服务解耦。
    - 弹性(Resilient)：每个服务都有自己的错误处理机制，如果一个服务失败，不会影响其他服务。这提高了系统的弹性。
- 高效运行(Efficiently)：
    - 独立部署(Deployable)：每个服务都可以独立部署，无需部署整个应用，这使得部署和升级更加容易。
    - 可伸缩性(Scalable)：每个服务都可以独立扩展，这使得系统可以根据需要进行水平扩展。
- 所有权(Ownership)：
    - 聚焦业务(Business Focused)：每个服务都有自己的团队负责开发和维护。
    - 团队专属 (Team Owned)：这使得服务的开发和维护更加独立和高效。
- 敏捷(Agile)：
    - 可适应性(Adaptable)：每个服务都可以根据需求进行迭代和改进。
    - 可组合性(Composable)：服务可以组合在一起，以构建更复杂的功能。这使得系统更加灵活和可扩展。


微服务面临的挑战：

- 数据一致性与同步：
    - 使用事件驱动或定时轮询来保持数据一致性。
    - 处理分布式事务和数据一致性问题。
- 多微服务场景下增加复杂度：
    - 审核服务接受文章
    - 支付服务等待支付
    - 支付服务确认支付
    - 生产服务接管最终处理
    - 文章中心确认文章状态变更
- 测试、调试与监控：
    - 设置复杂环境的测试
    - 监控需要关联技术

### 2.2 什么是模块化单体？

模块化单体是一种软件架构，其中应用程序被结构化为单个可部署单元，内部划分为具有清晰边界的封装良好(Well-Encapsulated)模块，每个模块负责应用程序的特定部分。

- 单一部署单元，所有组件均一起构建、测试并部署。
- 模块强制实施关注点分离，同时仍属于同一系统。
- 一个模块可以被多个服务集成，避免代码重复。
- 模块共享同一个数据，支持单事务操作。
- 模块之间通过函数调用或内存消息进行通信，而非HTTP或gRPC请求以提高系统的响应速度和吞吐量。

### 2.3 模块化单体与微服务的对比

- `Independence` - 多模块共享相同的运行环境。
- `Deployment` - 整个应用由多个模块组成的单元进行部署，而微服务独立部署。
- `Scalability` - 模块可以独立扩展，而微服务需要协调多个服务的扩展。
- `Communication` - 进程内通信更快，而微服务之间通信需要网络调用。
- `Data` - 数据隔离性更好，每个模块有自己的数据库，而微服务共享数据库。
- `Technology` - 模块可以使用不同的技术栈，而微服务需要协调多个服务的技术栈。

### 2.4 文件存储系统

- `GridFS` - 将文件存储至MongoDB中，支持标签和索引功能，速度慢于操作系统中的文件系统，因为存在数据库的额外开销。
- `MinIO` - 直接存储文件到磁盘中，速度更快，支持标签但不支持索引，实现了S3 API。
- `Azure Blob Storage` - 微软的云存储服务，提供高可用、高可扩展性的对象存储，支持标签和索引，成本较高。

**服务：**

- `Submission` - 投稿服务，文件类型比较少，且上传和下载频率低，文件均为私有，保留策略较短，可以在一段时间之后删除或归档，因此无需大容量存储，性能要求也不高，但需要具有安全策略，因此 MongoDB 的 GridFS 方案更为适合。
- `Review` - 审核服务，包含所有提交文件，增加几种文件类型，其余和投稿服务一样，因此可以使用相同的存储方案。
- `Production` - 生产服务，包含提交和查看的所有文件，新增多种文件类型并支持版本控制，文件不可删除，保留策略更长，因此需要高性能私有存储方案，如 MinIO。
- `PublicPages` - 公开页面服务，所有内容都是公开的，没有版本控制，可能会被大量下载，因此需要良好的性能而不需要很高的安全性，可以直接采用云存储方案。

#### 为什么文件存储是模块而不是微服务？

- `代码复杂度降低` - 因为文件存储嵌入在每个微服务中，意味着无需维护与部署独立的微服务。
- `更好的性能` - 文件操作无需向外部服务发送网络请求，这减少了延迟避免了服务间通信问题。
- `易于扩展` - 每个微服务都有自己的文件存储模块，存储能力随着服务的扩展而扩展。
- `存储访问规则` - 每个微服务中定义自己的存储访问规则，例如只读、读写等，这使得文件存储更加安全。
- `不同的存储策略` - 每个微服务可以根据自己的需求选择不同的存储策略，例如本地存储、云存储等。

#### 什么时候将文件存储作为微服务？

- 多个微服务需要共享文件存储时。
- 统一的文件存储服务，多个微服务可以通过调用该服务来进行文件存储和检索。


### 2.5 文章工作流程

1. 作者(Author)编写草稿文章。
2. 编辑(Editor)检查文章是否值得发表。
3. 若文章通过初步审核，编辑(Editor)邀请审稿人(Reviewer)进行审核并提供反馈。
4. 作者(Author)根据审稿人(Reviewer)的反馈进行修改，直到文章符合要求。
5. 编辑(Editor)再次审核文章，若通过，排版人员(Typesetter)将负责文章最终排版并确保多格式完美呈现。
6. 排版人员(Typesetter)完成排版后，将文章发布至公开页面服务(PublicPages)，并更新文章状态为已发布。

### 2.6 什么是领域驱动设计(DDD)？

DDD是一种帮助我们围绕现实世界问题构建软件的方法，在领域驱动设计的核心中存在一个简单但强大的概念：`Domain` 即领域。

#### 什么是领域？

领域驱动设计的核心思想是将软件构建围绕着业务领域进行，而不是技术实现。

- 领域是核心业务问题。
- 包含现实世界中的规则、约束和限制。

#### 领域驱动设计的核心支柱(Pillars)

1. 聚焦于业务(Business Focus)：DDD的核心在于对业务领域进行建模。
    - 帮助架构师设计更好的模型，帮助开发者理解业务需求。
2. 解决业务问题(Problem Solving)：DDD的目标是确保解决业务问题。
    - 关注业务需求，而不是实现细节。
3. 模型创建(Model Creation)：DDD强调使用业务语言来创建模型。
    - 良好的模型同时包含属性和行为，反应业务实际运作方式。
4. 限界上下文(Bounded Context)：DDD将系统分解为有界限的上下文区域，明确术语和规则。
    - 每个上下文都有自己的模型和业务逻辑，避免了不同上下文之间的混淆。
5. 事件驱动思考(Event-Driven Thinking)：DDD强调基于事件的通信，而不是传统的请求-响应模式。
    - 这使得系统更具弹性和可扩展性，能够处理复杂的业务场景。
6. 策略设计(Strategy Design)：DDD鼓励使用设计模式来解决业务问题。
    - 例如，策略模式(Strategy Pattern)用于定义算法族，使它们可以互换使用。

## 三、微服务 Submission 与 MediatR、FluentValidation & EF Core

### 3.1 微服务 Submission 架构

- 构建 Minimal APIs 端点
- 使用 MediatR 处理 CQRS
- 使用 FluentValidation 进行请求验证
- 使用 EF Core 进行数据库操作
- 基于 FileStorage 模块实现文件上传与下载
- 发布领域事件与审核服务(Review Service)集成

### 3.2 定义 User Stories 

- 创建文章(Create Article) 
    - 作为作者，我想创建一篇新文章，以便开始准备投稿。
- 分配作者(Assign Author)
    - 作为作者，我想为文章分配共同作者，以便他们的贡献得到适当的识别。
- 创建和分配作者(Create and Assign Author)
    - 作为作者，我想创建并分配新的共同作者，以便我可以添加尚未注册的协作者。
- 上传稿件文件(Upload Manuscript File)
    - 作为作者，我想上传文章的原稿文件，以便我的文章的核心内容可以被审阅。
- 上传附加补充材料(Upload Supplementary Material)
    - 作为作者，我想上传文章的补充材料，例如图表、表格、代码等，以便审稿人可以更好地理解我的文章。
- 提交文章(Submit Article)
    - 作为作者，我想提交我的文章，以便进入评审流程（此操作锁定文章并转入编辑审阅阶段）。
- 批准文章(Approve Article)
    - 作为编辑，我需要审批已提交的文章，以便将文章能在流程中继续推进。
- 拒绝文章(Reject Article)
    - 作为投稿编辑，我需要拒绝已提交的文章，以便在任何阶段过滤掉不合格的投稿。
- 获取文章(Get Article)
    - 作为作者或编辑，我想查看文章的详细信息，以便根据当前阶段进行审核或采取行动。
- 下载文件(Download File)
    - 作为作者或编辑，我想下载已上传的文件、审阅手稿或补充材料，以便进行查看或编辑。

### 3.3 定义 API 端点

| 端点                                             | 方法 | 描述                |
| ----------------------------------------------- | ---- | ------------------ |
| `/api/articles`                                 | POST | 创建新文章           |
| `/api/articles/{articleId}/author/{authorId}`   | PUT  | 为文章分配共同作者    |
| `/api/articles/{articleId}/authors`             | POST | 创建和分配新的共同作者 |
| `/api/articles/{articleId}/files/manuscript`    | POST | 上传文章原稿文件      |
| `/api/articles/{articleId}/files/supplementary` | POST | 上传文章补充材料      |
| `/api/articles/{articleId}:submit`              | PUT  | 提交文章             |
| `/api/articles/{articleId}:approve`             | PUT  | 批准文章             |
| `/api/articles/{articleId}:reject`              | PUT  | 拒绝文章             |
| `/api/articles/{articleId}`                     | GET  | 获取文章             |
| `/api/articles/{articleId}/file/{fileId}`       | GET  | 下载文件             |

### 3.4 非功能性需求

- 系统支持两种角色：作者(Author)与编辑(Editor)
- 作者只能访问自己的文章和文件
- 每个操作都需要进行审计(Audit)与跟踪(Tracking)，例如创建、提交、批准、拒绝等。
- 系统每年需处理约一千篇文章。
- 文章提交状态持续约一周时间，因此任何时间都预计有两千篇活跃文章。
- 每篇文章对应两名用户，相当于约四千名潜在活跃用户，并发用户约四百人。
- 需要应对每秒十次以上的上传或下载峰值。
- 每篇文章包含约13MB的文件数据，例如1份原稿文件(约5MB)、4份补充材料文件(每份约2MB)。
- 文件需存储(retention)两年，存储空间占用约2.5TB。
- 目标是达成99.9%的系统可用性。
- 95%的API响应时间需在1秒内完成。

**安全性(Security)**

- 采用基于角色的访问控制(RBAC, Role-Based Access Control)，确保只有授权用户才能访问系统资源。
- 投稿服务的内容将扩展安全策略。

**可扩展性(Scalability)**

- 基于以上数字，系统的使用情况是可预测的，不会突然增加或出现高峰。
- 投稿服务不会被频繁使用，也不会成为系统的瓶颈，因此我们不需要投入高配置的缩放设置。

**性能与可用性(Performance & Availability)**

- 根据以上数字，性能不是一个主要问题，大多数操作都是简单快速的，我们也不期望高负载或大量数据。
- 99.9%的时间系统是可用的 约43分钟的停机时间(downtime)/月

**数据验证与集成**

- 所有字段必须验证（包括文件大小、类型、必要的元数据等）
- 提交或最终决策(final decision)时，文章将被锁定，这意味着需要状态机定义各阶段允许操作和转换。

**审计与追踪(Audit & Tracking)**

- 每个操作都需要进行审计(Audit)与跟踪(Tracking)，例如创建、提交、批准、拒绝等。
- 审计服务必须是可查询并且是全局的。

**数据存储和保留(Data Storage & Retention)**

- 需要确实可靠且长期保留的存储。

### 3.5 高层架构：技术栈、分层与依赖关系

**系统整体架构设计**

![High Level Architecture | C4 Level 2](docs/imgs/high-level-architecture.png)

**投稿服务架构设计**

投稿服务与其他服务之间的所有通信都通过gRPC来完成，这为我们提供了基于合约的高性能服务间通信。

1. 作者或编辑的请求首先进入系统的网关入口(Gateway)，它可能通过HTTP接口或gRPC服务调用其他服务。
2. 网关将请求路由到相应的服务(Service)，例如投稿服务(Submission Service)。
3. 投稿服务的API层包含Minimal APIs端点，这是请求进入系统的入口点。我们将传入的请求映射为命令或查询对象(CQRS)，并使用中介者模式转发到应用层(Application Layer)。这使得我们的API层保持简洁，同时也将业务逻辑与表示层分离。
4. 应用层是投稿服务的核心，它包含了所有的业务逻辑。每个功能都会存放在单独的文件夹中，例如提交文章、上传资产以及分配作者等。这使得应用层保持清晰和可维护。我们还使用Mapster在领域对象和DTO之间进行转换。在每个文件夹内部，通常有一个命令或查询对象、一个使用 FluentValidation 的验证器，以及一个处理该命令或查询的处理程序器(Handler)。
5. 数据存储将使用 SQL Server。
6. 文件存储模块，包括 文件存储、文件上传或下载等功能，将采用 MongoDB GridFS以及MinIO。

我们使用SQL Server，但保持轻量级，没有服务类和领域服务类。每个用例都将直接由其功能处理器及领域行为进行处理，请求验证始终在应用层完成，业务规则验证则在领域层完成。

![Submission Architecture | C4 Level 2](docs/imgs/submission-architecture.png)、

### 3.6 DDD战术设计：聚合、实体、值对象和领域事件

![Tactical Design Diagram (DDD) | Level 4 (Class)](docs/imgs/tactical-design-diagram.png)

- 聚合是一组相关的对象，它们总是作为一个整体行动。
- 每个聚合都有一个称为聚合根的主要入口点，可以将其视为领域内的小的、自包含的世界，就像一个子领域，其中内容必须始终保持一致。

> 聚合定义了一致性的边界，有助于保持领域模型的清晰、可预测，并避免意外的副作用。

### 3.7 整洁架构：原则、边界与实施计划

整洁架构使我们的代码库保持良好组织、可维护和可测试，并独立于框架或基础设施细节。
- API/表示层(Presentation)
    - 使用 Minimal APIs 开发接口。
    - 集成认证及其他中间件。
    - 充当组合根，用于连接所有外部基础设施和模块依赖项，例如文件存储、邮件服务等。
    - 使用 MediatR 将传入的命令或查询对象传递给应用层。
    - 依赖于应用层。
- 应用层(Application)
    - 负责协调业务逻辑的执行。
    - 每个功能切片包含一个命令或查询对象，一个使用 FluentValidation 构建的验证器、一个通过处理实际功能逻辑的处理器。
    - 一个使用 Mapster 的映射配置，用于在领域模型和DTO之间进行转换。
    - 依赖于领域层、持久层和其他基础设施。
- 领域层(Domain)
    - 核心业务逻辑
    - 包含聚合、实体、值对象、领域事件等。
    - 领域函数，每个功能的业务规则与行为
    - 完全独立，不依赖于任何外部系统或模块。
- 持久层(Persistence)/基础设施层(Infrastructure)
    - 负责处理所有技术问题与集成功能点
    - 包括EFCore(DbContext/Repositoroies)、实体配置、SaveChangeInterceptor(派发领域事件)
    - 依赖于领域层。

### 3.8 实战概览与设置计划：构建内容与方法

- 整洁架构(Clean Architecture)项目设置
    - 创建解决方案与四个类库：API、Application、Domain、Persistence
    - 添加项目引用与必要的Nuget包。
- 设计领域模式
    - 定义聚合、实体、值对象、事件与领域行为
- 配置持久化
    - 设置 DbContext 与 EFCore 配置
    - 创建迁移并应用到数据库
- 实现垂直切片(Vertical Slice)架构
    - 遵循垂直切片架构的原则，将为每个项目创建一个文件夹。
    - 实现命令/查询、验证器与处理器。
    - 应用业务逻辑规则与触发领域逻辑。
- 暴露接口
    - 添加 Minimal APIs 并设置路由
    - 在 API 中的启动文件中完成所有配置。
- Docker 与 端到端测试(End-to-End)
    - 添加 Dockerfile 与 docker-compose 文件
    - 使用 scalar 和 postman 测试所有流程。

### 3.9 从创建解决方案与项目设置

(1). 创建解决方案

在 `Articles/src` 文件夹下创建解决方案。

```bash 
dotnet new sln -n Articles
```

(2). 创建 WebApi 项目

在 `Services/Submission` 文件夹下创建 WebApi 项目。

```bash
dotnet new webapi -n Submission.API
```

将 `Submission.API` 项目添加至解决方案。

```bash
dotnet sln Articles.sln add Services/Submission/Submission.API/Submission.API.csproj
```

(3). 创建 Application 类库

在 `Services/Submission` 文件夹下创建 Application 类库。

```bash
dotnet new classlib -n Submission.Application
```

将 `Submission.Application` 项目添加至解决方案。

```bash
dotnet sln Articles.sln add Services/Submission/Submission.Application/Submission.Application.csproj
```

(4). 创建 Domain 类库

在 `Services/Submission` 文件夹下创建 Domain 类库。

```bash
dotnet new classlib -n Submission.Domain
```

将 `Submission.Domain` 项目添加至解决方案。

```bash
dotnet sln Articles.sln add Services/Submission/Submission.Domain/Submission.Domain.csproj
```

(5). 创建 Persistence 类库

在 `Services/Submission` 文件夹下创建 Persistence 类库。

```bash
dotnet new classlib -n Submission.Persistence
```

将 `Submission.Persistence` 项目添加至解决方案。

```bash
dotnet sln Articles.sln add Services/Submission/Submission.Persistence/Submission.Persistence.csproj
```

### 3.10 使用 Minimal APIs 开发接口

#### 3.10.1 创建 BuildingBlocks

我们将创建一个名为 `Articles.Abstractions` 的类库，用于定义共享的抽象类、接口、记录等。

(1). 创建 Articles.Abstractions 类库

在 `BuildingBlocks` 文件夹下创建 Articles.Abstractions 类库。

```bash
dotnet new classlib -n Articles.Abstractions
```

将 `Articles.Abstractions` 项目添加至解决方案。

```bash
dotnet sln Articles.sln add BuildingBlocks/Articles.Abstractions/Articles.Abstractions.csproj
```

(2). 添加 IdResponse 类

在 `Articles.Abstractions` 项目中添加 `IdResponse.cs` 类。

```csharp
namespace Articles.Abstractions;

public record IdResponse(int Id);
```

(3). 添加 ArticleType 枚举

在 `Articles.Abstractions` 项目中添加 `ArticleType.cs` 枚举。

```csharp
namespace Articles.Abstractions;

public enum ArticleType
{
    Scientific = 1,
    NonScientific = 2
}
```

#### 3.10.2 在应用层添加创建文章命令

(1). 安装类库

```bash
dotnet add package MediatR
dotnet add package FluentValidation
```

(2). 在 `Submission.Application` 项目中添加 `CreateArticleCommand.cs` 类。

定义 `CreateArticleCommand` 命令对象，用于创建文章。并添加 `CreateArticleCommandValidator` 验证器。

```csharp
using Articles.Abstractions;
using FluentValidation;
using MediatR;

namespace Submission.Application.Features.CreateArticle;

public record CreateArticleCommand(int JournalId, string Title, string Scope, ArticleType ArticleType)
    : IRequest<IdResponse>;

public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(x => x.Scope)
            .NotEmpty()
            .WithMessage("Scope is required.");

        RuleFor(x => x.JournalId)
            .GreaterThan(0)
            .WithMessage("Invalid journal id.");
    }
}
```

(3). 在 `Submission.Application` 项目中添加 `CreateArticleCommandHandler.cs` 类。

定义 `CreateArticleCommandHandler` 命令处理器，用于处理 `CreateArticleCommand` 命令。

```csharp
using Articles.Abstractions;
using MediatR;

namespace Submission.Application.Features.CreateArticle;

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, IdResponse>
{
    public Task<IdResponse> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
```

#### 3.10.3 添加创建文章命令接口

(1). 在 `Submission.API` 项目中添加 `CreateArticleEndpoint.cs` 类。

定义 `CreateArticleEndpoint` 类，用于映射创建文章的接口。

```csharp
using MediatR;
using Submission.Application.Features.CreateArticle;

namespace Submission.API.Endpoints;

public static class CreateArticleEndpoint
{
    public static void Map(this IEndpointRouteBuilder app)
    {
        app
            .MapPost("/api/articles", async (CreateArticleCommand command, ISender sender) =>
            {
                var response = sender.Send(command);
                return Results.Created($"/api/articles/{response.Id}", response);
            })
            .RequireAuthorization(policy => policy.RequireRole("AUT"))
            .WithName("CreateArticle")
            .WithTags("Articles")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }
}
```

(2). 在 `Submission.API` 项目中添加 `EndpointRegistration.cs` 类。

定义 `EndpointRegistration` 类，用于注册所有接口。

```csharp
namespace Submission.API.Endpoints;

public static class EndpointRegistration
{
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        CreateArticleEndpoint.Map(app);
        return app;
    }
}
```

(3). 在 `Submission.API` 项目中添加 `Program.cs` 类。

在 `Program.cs` 类中添加 `EndpointRegistration` 类的映射。

```csharp
using Submission.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapAllEndpoints();

app.Run();
```

### 3.11 基于DDD原则实现领域层

#### 3.11.1 创建文章实体

(1). 在 `Articles.Domain` 类库中的 `Entities` 文件夹中添加 `Article.cs` 类。

定义 `Article` 实体类，用于表示文章。

```csharp
using Articles.Abstractions.Enums;

namespace Submission.Domain.Entities;

public class Article
{
    public int Id { get; init; }
    public required string Title { get; set; }
    public required string Scope { get; set; }
    public required ArticleType Type { get; set; }
    public ArticleStage Stage { get; internal set; }
    public int JournalId { get; set; }
    public required Journal Journal { get; init; }
}
```

(2). 在 `Articles.Abstractions` 类库中的 `Enums` 文件夹中添加 `ArticleStage.cs` 枚举。

定义 `ArticleStage` 枚举，用于表示文章的状态。

```csharp
namespace Articles.Abstractions.Enums;

public enum ArticleStage
{
    Created
}
```

#### 3.11.2 创建期刊实体

(1). 在 `Articles.Domain` 类库中的 `Entities` 文件夹中添加 `Journal.cs` 类。

定义 `Journal` 实体类，用于表示期刊。

```csharp
namespace Submission.Domain.Entities;

public partial class Journal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abreviation { get; set; }

    private readonly List<Article> _articles = new();
    public IReadOnlyCollection<Article> Articles => _articles.AsReadOnly();
}
```

(2). 在 `Articles.Domain` 类库中的 `Behaviors` 文件夹中添加 `Journal.cs` 类。

将创建文章的行为添加到 `Journal` 类中。

```csharp
using Articles.Abstractions.Enums;

namespace Submission.Domain.Entities;

public partial class Journal
{
    public Article CreateArticle(string title, ArticleType type, string scope)
    {
        var article = new Article
        {
            Title = title,
            Type = type,
            Scope = scope,
            Journal = this,
            Stage = ArticleStage.Created,
        };
        _articles.Add(article);

        return article;
    }
}
```

### 3.12 在持久化层中使用EF Core

#### 3.12.1 安装EF Core NuGet包

在 `Articles.Persistence` 类库中安装NuGet包。

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

#### 3.12.2 引用领域层类库

在 `Articles.Persistence` 类库中添加对 `Articles.Domain` 类库的引用。

```bash
dotnet add reference ../Articles.Domain/Articles.Domain.csproj
```

#### 3.12.3 创建数据库上下文

(1). 在 `Articles.Persistence` 类库中添加 `SubmissionDbContext.cs` 类。

定义 `SubmissionDbContext` 类，用于表示数据库上下文。

```csharp
using Microsoft.EntityFrameworkCore;
using Submission.Domain.Entities;

namespace Submission.Persistence;

public class SubmissionDbContext : DbContext
{
    public virtual DbSet<Article> Articles { get; set; }
    public virtual DbSet<Journal> Journals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
```

#### 3.12.4 创建 EntityFramework 构建块

(2). 创建 `BuildingBlocks.EntityFramework` 类库。

```bash
dotnet new classlib -n BuildingBlocks.EntityFramework
```

(3). 安装类库

```bash
dotnet add package Microsoft.EntityFrameworkCore
```

(4). 创建扩展方法类 `BuilderExtensions.cs`。

```csharp
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.EntityFramework;

public static class BuilderExtensions
{
    public static PropertyBuilder<TEnum> HasEnumConversion<TEnum>(this PropertyBuilder<TEnum> builder)
        where TEnum : Enum
    {
        return builder.HasConversion(
            x => x.ToString(),
            x => (TEnum)Enum.Parse(typeof(TEnum), x)
        );
    }
}
```

#### 3.12.5 配置实体映射

(1). 在 `Submission.Persistence` 类库中的 `EntityConfigurations` 文件夹中添加 `ArticleEntityConfiguration.cs` 类。

```csharp
using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission.Domain.Entities;

namespace Submission.Persistence.EntityConfigurations;

public class ArticleEntityConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        builder.Property(x => x.Title).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Scope).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.Type).HasEnumConversion();
        builder.Property(x => x.Stage).HasEnumConversion();

        builder.HasOne(x => x.Journal)
            .WithMany(x => x.Articles)
            .HasForeignKey(x => x.JournalId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
```

#### 3.12.6 仓储实现

(1). 在 `BuildingBlocks.EntityFramework` 类库中定义仓储接口 `IRepository.cs`。

```csharp
using BuildingBlocks.Domain.Entities;

namespace BuildingBlocks.EntityFramework;

public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<bool> DeleteByIdAsync(int id);
    TEntity Update(TEntity entity);
    void Remove(TEntity entity);
}
```

(2). 在 `BuildingBlocks.EntityFramework` 类库中实现仓储接口 `Repository.cs`。

```csharp
using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EntityFramework;

public class Repository<TContext, TEntity> : IRepository<TEntity>
    where TContext : DbContext
    where TEntity : class, IEntity
{
    protected readonly TContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public string TableName;

    public Repository(TContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();

        TableName = Context.Model.FindEntityType(typeof(TEntity)).GetTableName();
    }

    public virtual DbSet<TEntity> Entity => DbSet;
    protected virtual IQueryable<TEntity> Query() => DbSet;

    public virtual Task<TEntity?> GetByIdAsync(int id)
        => Query().SingleOrDefaultAsync(x => x.Id == id);

    public virtual async Task<TEntity> AddAsync(TEntity entity)
        => (await DbSet.AddAsync(entity)).Entity;

    public virtual async Task<bool> DeleteByIdAsync(int id)
    {
        return await Context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM {TableName} WHERE Id = {id}") > 0;
    }

    public virtual TEntity Update(TEntity entity)
        => DbSet.Update(entity).Entity;

    public virtual void Remove(TEntity entity)
        => DbSet.Remove(entity);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);
}
```

(3). 在 `Submission.Persistence` 类库中添加 `Repository.cs` 类。

```csharp
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.EntityFramework;

namespace Submission.Persistence.Repositories;

public class Repository<TEntity>(SubmissionDbContext dbContext) : Repository<SubmissionDbContext, TEntity>(dbContext)
    where TEntity : class, IEntity
{
}
```

(4). 在 `Submission.Persistence` 类库中添加 `ArticleRepository.cs` 类。

```csharp
using Submission.Domain.Entities;

namespace Submission.Persistence.Repositories;

public class ArticleRepository(SubmissionDbContext dbContext) : Repository<Article>(dbContext)
{
    
}
```

### 3.13 使用 MediatR 和 FluentValidation 实现命令处理器

```csharp
using Articles.Abstractions;
using MediatR;
using Submission.Domain.Entities;
using Submission.Persistence.Repositories;

namespace Submission.Application.Features.CreateArticle;

public class CreateArticleCommandHandler(Repository<Journal> journalRepository)
    : IRequestHandler<CreateArticleCommand, IdResponse>
{
    public async Task<IdResponse> Handle(CreateArticleCommand command, CancellationToken cancellationToken)
    {
        var journal = await journalRepository.GetByIdAsync(command.JournalId);

        var article = journal.CreateArticle(command.Title, command.Type, command.Scope);
        await journalRepository.SaveChangesAsync(cancellationToken);

        return new IdResponse(article.Id);
    }
}
```

### 3.14 通过依赖注入配置服务与模块

依赖注入是.NET应用程序中实现松耦合的核心机制。在本项目中，我们采用了分层的依赖注入配置方式，每个层都有自己的`DependencyInjection.cs`类来注册该层的服务。

#### 实现步骤

1. **创建各层的DependencyInjection类**
   - 在API层创建`DependencyInjection.cs`，负责注册API相关服务
   - 在Application层创建`DependencyInjection.cs`，负责注册MediatR、验证器等应用层服务
   - 在Persistence层创建`DependencyInjection.cs`，负责注册数据库上下文、仓储等持久层服务

2. **API层依赖注入配置**
   ```csharp
   public static class DependencyInjection
   {
       public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
       {
           return services
               .AddMemoryCache()
               .AddOpenApi()
               .AddMongoFileStorage(configuration);
       }
   }
   ```

3. **Application层依赖注入配置**
   ```csharp
   public static class DependencyInjection
   {
       public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
       {
           return services
               // 注册FluentValidation验证器
               .AddValidatorsFromAssemblyContaining<CreateArticleCommandValidator>()
               // 注册MediatR和相关管道行为
               .AddMediatR(config =>
               {
                   config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                   config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                   config.AddOpenBehavior(typeof(SetUserIdBehavior<,>));
               });
       }
   }
   ```

4. **Persistence层依赖注入配置**
   ```csharp
   public static class DependencyInjection
   {
       public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
       {
           var connection = configuration.GetConnectionString("Database");

           // 注册数据库上下文
           services.AddDbContext<SubmissionDbContext>((provider, options) =>
           {
               // 配置数据库连接
           });

           // 注册仓储
           services.AddScoped(typeof(Repository<>));
           services.AddScoped(typeof(ArticleRepository));

           return services;
       }
   }
   ```

5. **在Program.cs中集成所有服务**
   ```csharp
   var builder = WebApplication.CreateBuilder(args);

   builder.Services
       .AddApiServices(builder.Configuration)
       .AddApplicationServices(builder.Configuration)
       .AddPersistenceServices(builder.Configuration);

   var app = builder.Build();
   ```

这种分层的依赖注入配置方式使得代码更加模块化，各层的职责更加清晰，同时也便于测试和维护。

### 3.15 实现未找到异常

在分布式系统中，良好的异常处理机制对于提供清晰的错误信息和维护系统稳定性至关重要。我们创建了独立的异常处理模块来统一管理系统中的异常。

#### 实现步骤

1. **创建BuildingBlocks.Exceptions类库**
   - 创建一个独立的类库项目，用于集中管理系统中的异常类型
   - 设置目标框架为net10.0，启用隐式using和可空引用类型

2. **实现基类HttpException**
   ```csharp
   public class HttpException : Exception
   {
       public HttpException(HttpStatusCode statusCode, string message)
           : base(string.IsNullOrEmpty(message) ? statusCode.ToString() : message)
       {}
       
       public HttpException(HttpStatusCode statusCode, string message, Exception innerException)
           : base(message, innerException)
       {
           HttpStatusCode = statusCode;
       }

       public HttpStatusCode HttpStatusCode { get; }
   }
   ```
   - HttpException继承自Exception，添加了HttpStatusCode属性
   - 提供了两个构造函数，支持基本错误信息和内部异常

3. **实现NotFoundException**
   ```csharp
   public class NotFoundException : HttpException
   {
       public NotFoundException(string message) : base(HttpStatusCode.NotFound, message)
       {}

       public NotFoundException(string message, Exception exception) : base(HttpStatusCode.NotFound, message, exception)
       {}
   }
   ```
   - NotFoundException继承自HttpException，默认HTTP状态码为404
   - 用于表示请求的资源不存在的情况

4. **将异常类库添加到各层项目引用**
   - 在API、Application、Domain和Persistence层中添加对BuildingBlocks.Exceptions的引用
   - 这样各层都可以抛出和处理这些标准化的异常

#### 优势

- **集中管理**：所有异常类型都集中在一个模块中，便于维护和扩展
- **标准化**：提供了一致的异常处理方式，提高了代码的可读性和可维护性
- **HTTP集成**：异常直接包含HTTP状态码，便于API层将异常转换为适当的HTTP响应
- **可扩展性**：可以根据需要轻松添加其他类型的异常（如BadRequestException、UnauthorizedException等）

这个异常处理模块为整个系统提供了坚实的基础，确保了错误信息的一致性和清晰度。

### 3.16 在领域层实现将当前用户设为作者

为了实现将当前用户设为作者的功能，我们在领域层设计了一系列实体和值对象，构建了完整的用户-作者-文章关系模型。

#### 核心实体与值对象设计

1. **Person 实体 - 用户的基础信息**
   ```csharp
   // Submission.Domain/Entities/Person.cs
   public class Person : IEntity
   {
       public int Id { get; init; }
       public required string Name { get; init; }
       public string? Title { get; set; }
       public required EmailAddress EmailAddress { get; init; }
       public required string Affiliation { get; init; }
       public int? UserId { get; init; }
       public string TypeDiscriminator { get; init; }
       public IReadOnlyList<ArticleActor> Actors { get; private set; } = new List<ArticleActor>();
   }
   ```
   - 实现了 `IEntity` 接口，是所有人员类型的基类
   - 包含基本信息如姓名、头衔、邮箱、所属机构等
   - 通过 `UserId` 关联系统用户
   - 具有 `TypeDiscriminator` 用于区分不同类型的人员

2. **EmailAddress 值对象 - 类型安全的邮箱地址**
   ```csharp
   // Submission.Domain/ValueObjects/EmailAddress.cs
   public class EmailAddress : StringValueObject
   {
       private EmailAddress(string value) => Value = value;

       public static EmailAddress Create(string value)
       {
           Guard.ThrowIfNullOrWhiteSpace(value);
           if (!IsValidEmail(value))
               throw new ArgumentException($"Invalid email address '{value}'");

           return new EmailAddress(value);
       }

       private static bool IsValidEmail(string email)
       {
           const string regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
           return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
       }
   }
   ```
   - 继承自 `StringValueObject`，确保邮箱地址的类型安全
   - 包含邮箱格式验证逻辑，使用正则表达式验证
   - 使用静态工厂方法创建实例，确保数据有效性

3. **Guard 类 - 输入验证工具**
   ```csharp
   // BuildingBlocks.Core/Guard.cs
   public static class Guard
   {
       public static void ThrowIfNullOrWhiteSpace(string? value, [CallerArgumentExpression("value")] string? parameterName = null)
       {
           if (string.IsNullOrWhiteSpace(value))
               throw new ArgumentNullException(parameterName, "Value cannot be null or whitespace.");
       }

       public static void ThrowIfZeroOrNegative(int value, [CallerArgumentExpression("value")] string? parameterName = null)
       {
           if (value <= 0)
               throw new ArgumentOutOfRangeException(parameterName, "Value must be greater than zero.");
       }

       // 其他验证方法...
   }
   ```
   - 提供了常用的输入验证方法，如空值检查、相等性检查
   - 简化了参数验证逻辑，提高代码可读性
   - 位于 `BuildingBlocks.Core` 命名空间，可在整个系统中重用

4. **Author 实体 - 作者的扩展信息**
   ```csharp
   // Submission.Domain/Entities/Author.cs
   public partial class Author : Person
   {
       public string? Degree { get; init; }
       public string? Discipline { get; init; }
   }
   ```
   - 继承自 `Person` 类，添加了作者特有的属性
   - 包含学位、学科领域等作者相关信息
   - 使用部分类设计，便于未来扩展

5. **ArticleActor 实体 - 文章参与者基类**
   ```csharp
   // Submission.Domain/Entities/ArticleActor.cs
   public class ArticleActor
   {
       public int ArticleId { get; init; }
       public Article Article { get; init; }
       public int PersonId { get; set; }
       public Person Person { get; init; }
       public UserRoleType Role { get; init; }
   }
   ```

6. **ArticleAuthor 实体 - 文章与作者的关系**
   ```csharp
   // Submission.Domain/Entities/ArticleAuthor.cs
   public class ArticleAuthor : ArticleActor
   {
       public HashSet<ContributeArea> ContributeAreas { get; init; }
   }
   ```
   - 继承自 `ArticleActor`，表示文章的参与者
   - 使用 `HashSet<ContributeArea>` 记录作者的贡献领域
   - 体现了文章与作者之间的多对多关系

7. **ContributeArea 枚举 - 贡献领域**
   ```csharp
   // Articles.Abstractions/Enums/ContributeArea.cs
   public enum ContributeArea
   {
       // Mandatory
       OriginalDraft = 1,       // 原始草稿
       ReviewAndEditing = 2,    // 审阅和编辑
       
       // Optional
       Conceptualization = 3,   // 概念化
       FormalAnalysis = 4,      // 形式分析
       Investigation = 5,       // 调查
       Methodology = 6,         // 方法论
       Visualization = 7,       // 可视化
   }
   ```
   - 定义了作者可能的贡献领域
   - 分为必填项（初稿撰写、审阅与编辑）和可选项（概念化、形式分析等）
   - 每个贡献领域都有明确的数值标识

8. **UserRoleType 枚举 - 用户角色类型**
   ```csharp
   // Articles.Abstractions/Enums/UserRoleType.cs
   public enum UserRoleType
   {
       // Cross Domain: 1-9
       [Description("Editorial Office")] EOF = 1,
       
       // Submission: 11-19
       [Description("Author")] AUT = 11,
       [Description("Corresponding Author")] CORAUT = 12,
   }

   public static class Role
   {
       public const string EOF = nameof(UserRoleType.EOF);
       public const string AUT = nameof(UserRoleType.AUT);
       public const string CORAUT = nameof(UserRoleType.CORAUT);
   }
   ```
   - 定义了系统中的用户角色，如编辑部、作者、通讯作者
   - 使用 Description 属性提供友好名称
   - 提供了常量类，便于代码中引用

#### 实现将当前用户设为作者的逻辑

在领域层实现将当前用户设为作者，需要考虑以下几点：

1. **身份关联**：通过 `Person` 实体的 `UserId` 属性关联系统用户
2. **角色验证**：确保用户具有作者角色
3. **自动分配**：在创建文章时自动将当前用户添加为作者
4. **贡献领域**：为默认作者设置适当的贡献领域（如初稿撰写）

这部分逻辑通常在文章创建的领域行为中实现，确保业务规则的一致性和完整性。

#### Article 实体中的 AssignAuthor 方法

为了支持将用户设为作者的功能，我们在 `Article` 实体中添加了 `AssignAuthor` 方法：

```csharp
// Submission.Domain/Entities/Article.cs
public partial class Article
{
    public void AssignAuthor(Author author, HashSet<ContributeArea> contributeAreas, bool isCorrespondingAuthor)
    {
        // 验证必填贡献领域
        if (!contributeAreas.Contains(ContributeArea.OriginalDraft))
            throw new DomainException("Original draft contribution is required.");
        
        // 确定角色类型
        var role = isCorrespondingAuthor ? UserRoleType.CORAUT : UserRoleType.AUT;
        
        // 创建文章作者关联
        var articleAuthor = new ArticleAuthor
        {
            Article = this,
            Person = author,
            Role = role,
            ContributeAreas = contributeAreas
        };
        
        // 添加到文章参与者列表
        _actors.Add(articleAuthor);
    }
}
```

这个方法实现了以下功能：
- 验证必填贡献领域（原始草稿）
- 根据是否为通讯作者确定角色类型
- 创建文章作者关联
- 将作者添加到文章参与者列表

通过这些领域模型的设计和实现，我们可以在创建文章时自动将当前用户设为作者，并记录其贡献领域和角色类型。这符合DDD原则，通过清晰的实体关系和业务规则封装，确保了数据的一致性和业务逻辑的正确性。

### 3.17 配置持久层将当前用户设为作者

在持久层，我们需要配置EF Core以正确映射领域层的实体和关系，特别是与用户和作者相关的部分。这涉及到多个实体配置类的实现，这些配置确保了当创建文章时系统能够自动将当前用户设为作者。

#### 核心实体配置

1. **PersonEntityConfiguration - 人员基础信息配置**
   ```csharp
   public class PersonEntityConfiguration : EntityConfiguration<Person>
   {
       public override void Configure(EntityTypeBuilder<Person> builder)
       {
           base.Configure(builder);

           // 索引配置
           builder.HasIndex(x => x.UserId).IsUnique();

           // 鉴别器配置（用于表继承）
           builder.HasDiscriminator(x => x.TypeDiscriminator)
               .HasValue<Person>(nameof(Person))
               .HasValue<Author>(nameof(Author));
           
           // 基本属性配置
           builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
           builder.Property(x => x.Title).HasMaxLength(64).IsRequired();
           builder.Property(x => x.Affiliation).HasMaxLength(512).IsRequired()
               .HasComment("Institution or organization they are associated with when they conduct their research.");

           builder.Property(x => x.UserId).IsRequired(false);

           // 复杂属性配置（值对象）
           builder.ComplexProperty(x => x.EmailAddress, builder =>
           {
               builder.Property(x => x.Value)
                   .HasColumnName(builder.Metadata.PropertyInfo.Name)
                   .HasMaxLength(64);
           });
       }
   }
   ```
   - 继承自 `EntityConfiguration<Person>`，这是一个通用的实体配置基类
   - 配置了 `UserId` 的唯一索引
   - 使用鉴别器模式实现表继承，区分普通人员和作者
   - 配置了属性的最大长度和必填性
   - 使用 `ComplexProperty` 配置 `EmailAddress` 值对象

2. **AuthorEntityConfiguration - 作者扩展信息配置**
   ```csharp
   public class AuthorEntityConfiguration : IEntityTypeConfiguration<Author>
   {
       public void Configure(EntityTypeBuilder<Author> builder)
       {
           builder.Property(x => x.Discipline).HasMaxLength(64)
               .HasComment("The author's main field or study or research (e.g. Biology, Computer Science).");
           builder.Property(x => x.Degree).HasMaxLength(64)
               .HasComment("The author's highest academic qualification (e.g. PhD in Mathematics, MSc in Chemistry).");
       }
   }
   ```
   - 实现了 `IEntityTypeConfiguration<Author>` 接口
   - 配置了作者特有的属性：学科领域和学位
   - 为每个属性添加了最大长度限制和注释

3. **ArticleActorEntityConfiguration - 文章参与者配置**
   ```csharp
   public class ArticleActorEntityConfiguration : IEntityTypeConfiguration<ArticleActor>
   {
       public void Configure(EntityTypeBuilder<ArticleActor> builder)
       {
           // 复合主键配置
           builder.HasKey(x => new { x.ArticleId, x.PersonId, x.Role });

           // 角色属性配置
           builder.Property(x => x.Role).HasEnumConversion().HasDefaultValue(UserRoleType.AUT);

           // 关系配置
           builder.HasOne(x => x.Article)
               .WithMany(x => x.Actors)
               .HasForeignKey(x => x.ArticleId)
               .OnDelete(DeleteBehavior.Cascade);
           
           builder.HasOne(x => x.Person)
               .WithMany(x => x.Actors)
               .HasForeignKey(x => x.PersonId)
               .OnDelete(DeleteBehavior.Restrict);
       }
   }
   ```
   - 配置了复合主键：`ArticleId`, `PersonId`, `Role`
   - 使用 `HasEnumConversion()` 配置角色枚举的转换
   - 设置了角色的默认值为 `UserRoleType.AUT`（作者）
   - 配置了与 `Article` 和 `Person` 实体的关系
   - 设置了删除行为：文章删除时级联删除参与者，人员删除时限制删除

4. **ArticleAuthorEntityConfiguration - 文章作者配置**
   ```csharp
   public class ArticleAuthorEntityConfiguration : IEntityTypeConfiguration<ArticleAuthor>
   {
       public void Configure(EntityTypeBuilder<ArticleAuthor> builder)
       {
           builder.Property(x => x.ContributeAreas)
               .HasJsonCollectionConversion()
               .IsRequired();
       }
   }
   ```
   - 配置了 `ContributeAreas` 属性，使用 JSON 集合转换
   - 设置为必填属性
   - 这使得 EF Core 能够将 `HashSet<ContributeArea>` 正确存储为 JSON 格式

#### 配置要点与最佳实践

1. **表继承策略**：使用鉴别器模式实现表继承，将 `Person` 和 `Author` 存储在同一个表中
2. **值对象映射**：使用 `ComplexProperty` 配置值对象，如 `EmailAddress`
3. **枚举处理**：使用 `HasEnumConversion()` 和 `HasJsonCollectionConversion()` 处理枚举和枚举集合
4. **关系管理**：仔细配置实体之间的关系和删除行为，确保数据完整性
5. **注释与文档**：为属性添加注释，提高数据库的可读性和可维护性
6. **索引优化**：为频繁查询的字段（如 `UserId`）添加索引

这些配置确保了领域层的实体能够正确地映射到数据库表，同时保持了领域模型的完整性和一致性。通过这种方式，当创建文章时，系统能够自动将当前用户设为作者，并正确地持久化到数据库中。

### 3.18 在处理器中实现将当前用户设为作者

在处理器中实现将当前用户设为作者是连接领域层和应用层的关键步骤。这涉及到命令处理程序、领域行为和MediatR管道的协同工作。

#### 核心实现组件

1. **CreateArticleCommandHandler - 创建文章命令处理程序**
   ```csharp
   public class CreateArticleCommandHandler(Repository<Journal> journalRepository)
       : IRequestHandler<CreateArticleCommand, IdResponse>
   {
       public async Task<IdResponse> Handle(CreateArticleCommand command, CancellationToken cancellationToken)
       {
           // 查找期刊或抛出异常
           var journal = await journalRepository.FindByIdOrThrowAsync(command.JournalId);

           // 创建文章
           var article = journal.CreateArticle(command.Title, command.Type, command.Scope);

           // 将当前用户设为作者
           await assignCurrentUserAsAuthor(article, command);
           
           // 保存更改
           await journalRepository.SaveChangesAsync(cancellationToken);

           return new IdResponse(article.Id);
       }

       private async Task assignCurrentUserAsAuthor(Article article, CreateArticleCommand command)
       {
           // 根据用户ID查找作者
           var author = await journalRepository.Context.Authors
               .SingleOrDefaultAsync(x => x.UserId == command.CreatedByUserId);

           // 如果找到作者，将其分配为文章作者
           if (author is not null)
               article.AssignAuthor(author, [ContributeArea.OriginalDraft], true);
       }
   }
   ```
   - 实现了 `IRequestHandler<CreateArticleCommand, IdResponse>` 接口
   - 首先查找期刊，然后创建文章
   - 调用 `assignCurrentUserAsAuthor` 方法将当前用户设为作者
   - 保存更改并返回文章ID
   - 在私有方法中，根据 `CreatedByUserId` 查找作者并分配

2. **Article 分部类 - 分配作者的领域行为**
   ```csharp
   public partial class Article
   {
       public void AssignAuthor(Author author, HashSet<ContributeArea> contributeAreas, bool isCorrespondingAuthor)
       {
           // 根据是否为通讯作者设置角色
           var role = isCorrespondingAuthor ? UserRoleType.CORAUT : UserRoleType.AUT;
           
           // 实现分配作者的逻辑
           // 注意：这里的实现可能不完整，需要根据具体需求扩展
       }

       // 其他领域行为...
   }
   ```
   - 在领域层实现了分配作者的核心逻辑
   - 接受作者、贡献领域集合和是否为通讯作者的参数
   - 根据是否为通讯作者设置适当的角色
   - 这是一个部分类，便于维护和扩展

3. **SetUserIdBehavior - MediatR管道行为**
   ```csharp
   public class SetUserIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IAuditableAction
   {
       public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
           CancellationToken cancellationToken)
       {
           // 临时实现：将用户ID硬编码为1
           request.CreatedByUserId = 1;
           
           // 继续执行管道
           return next(cancellationToken);
       }
   }
   ```
   - 实现了 `IPipelineBehavior<TRequest, TResponse>` 接口
   - 用于在命令处理前设置创建者用户ID
   - 当前是临时实现，将用户ID硬编码为1，实际项目中应该从认证上下文中获取
   - 只处理实现了 `IAuditableAction` 接口的请求

4. **IAuditableAction 接口 - 审计动作标记**
   ```csharp
   // 在 BuildingBlocks.Domain 命名空间中
   public interface IAuditableAction
   {
       int CreatedByUserId { get; set; }
   }
   ```
   - 定义了审计动作的接口
   - 包含 `CreatedByUserId` 属性，用于记录操作的创建者
   - 所有需要审计的命令都应该实现这个接口

5. **领域异常类 - 业务规则验证**
   - 在 `BuildingBlocks.Domain` 中实现了领域异常类
   - 用于在领域层验证业务规则时抛出异常
   - 例如，在 `Article` 类的 `CreateAsset` 方法中检查文件数量限制

#### 实现流程分析

1. **请求处理流程**：
   - 用户发送创建文章请求
   - 请求通过 API 端点进入系统
   - 请求被映射到 `CreateArticleCommand` 命令

2. **MediatR 管道处理**：
   - `CreateArticleCommand` 实现了 `IAuditableAction` 接口
   - `SetUserIdBehavior` 管道行为设置 `CreatedByUserId` 为1
   - 验证行为（如果有）验证命令
   - 命令被传递给 `CreateArticleCommandHandler`

3. **命令处理逻辑**：
   - 查找期刊或抛出异常
   - 创建文章
   - 根据 `CreatedByUserId` 查找作者
   - 如果找到作者，调用 `article.AssignAuthor` 方法分配作者
   - 保存更改到数据库

4. **领域行为执行**：
   - `AssignAuthor` 方法根据是否为通讯作者设置角色
   - 实现文章与作者之间的关联
   - 可以在这个方法中添加更多的业务规则验证

#### 关键设计要点

1. **关注点分离**：
   - API 层负责处理请求和响应
   - 应用层负责协调业务流程
   - 领域层负责实现核心业务规则
   - 持久层负责数据存储

2. **领域驱动设计**：
   - 核心业务逻辑封装在领域实体中
   - 使用值对象确保数据完整性
   - 抛出领域异常来验证业务规则

3. **MediatR 管道**：
   - 使用管道行为分离横切关注点
   - 实现了审计、验证等横切功能
   - 提高了代码的可维护性和可扩展性

4. **临时实现**：
   - `SetUserIdBehavior` 目前是临时实现，硬编码用户ID为1
   - 在实际项目中，应该从认证上下文中获取真实的用户ID

这个实现确保了在创建文章时，系统会自动将当前用户设为作者，并记录其贡献领域和是否为通讯作者。这符合科学文章管理系统的业务需求，提高了用户体验和系统的自动化程度。

### 3.19 使用Minimal API实现上传稿件接口

在科学文章管理系统中，上传稿件是核心功能之一。我们使用Minimal API实现了简洁高效的上传稿件接口，结合CQRS模式和FluentValidation验证。

#### 1. UploadManuscriptFileEndpoint - 上传稿件接口
```csharp
// Submission.API/Endpoints/UploadManuscriptFileEndpoint.cs
public static class UploadManuscriptFileEndpoint
{
    public static void Map(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/articles/{article:int}/assets/manuscript:upload", async (
                [FromRoute] int articleId,
                [FromForm] UploadManuscriptFileCommand command,
                ISender sender) =>
            {
                // 设置文章ID并发送命令
                var response = await sender.Send(command with { ArticleId = articleId });
                // 返回创建成功响应，包含下载链接
                return Results.Created($"/api/articles/{articleId}/assets/{response.Id}:download", response);
            })
            // 要求通讯作者角色授权
            .RequireRoleAuthorization(Role.CORAUT)
            .WithName("UploadManuscript")
            .WithTags("Assets")
            .Produces<IdResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            // 禁用防伪令牌，因为这是文件上传接口
            .DisableAntiforgery();
    }
}
```

#### 2. AssetType 枚举 - 资产类型定义
```csharp
// Submission.Domain/Enums/AssetType.cs
public enum AssetType
{
    // 稿件
    Manuscript = 1,
    // 补充材料
    SupplementaryMaterial = 2,
    // 图
    Figure = 3,
    // 表
    Table = 4,
    // 代码
    Code = 5,
}
```

#### 3. CQRS与验证器 - 命令和验证逻辑

##### UploadManuscriptFileCommand - 上传稿件命令
```csharp
// Submission.Application/Features/UploadFile/UploadManuscriptFile/UploadManuscriptFileCommand.cs
public record UploadManuscriptFileCommand : ArticleCommand
{
    /// <summary>
    /// 资产类型
    /// </summary>
    [Required]
    public AssetType AssetType { get; init; }

    /// <summary>
    /// 上传文件
    /// </summary>
    [Required]
    public IFormFile File { get; init; }

    public override ArticleActionType ActionType => ArticleActionType.Upload;
}
```

##### UploadManuscriptFileCommandValidator - 上传稿件命令验证器
```csharp
// Submission.Application/Features/UploadFile/UploadManuscriptFile/UploadManuscriptFileCommandValidator.cs
public class UploadManuscriptFileCommandValidator : ArticleCommandValidator<UploadManuscriptFileCommand>
{
    public UploadManuscriptFileCommandValidator()
    {
        // 验证文件是否存在
        RuleFor(x => x.File)
            .WithMessageForNotNull();

        // 验证资产类型是否允许
        RuleFor(x => x.AssetType).Must(IsAssetTypeAllowed)
            .WithMessage(x => $"{x.AssetType} is not allowed");
    }

    // 允许的资产类型列表
    public IReadOnlyCollection<AssetType> AllowedAssetTypes => new HashSet<AssetType>
    {
        AssetType.Manuscript
    };

    // 检查资产类型是否允许的方法
    private bool IsAssetTypeAllowed(AssetType assetType)
        => AllowedAssetTypes.Contains(assetType);
}
```

#### 实现特点与优势

1. **Minimal API设计**：使用简洁的Minimal API语法，减少冗余代码
2. **角色授权**：使用`RequireRoleAuthorization(Role.CORAUT)`确保只有通讯作者可以上传稿件
3. **命令模式**：使用MediatR实现命令模式，解耦请求处理逻辑
4. **表单数据处理**：使用`[FromForm]`属性处理文件上传
5. **响应规范**：使用`Results.Created`返回201状态码和资源位置
6. **详细的API文档**：使用`Produces`和`ProducesProblem`方法定义API响应类型
7. **文件上传优化**：禁用防伪令牌以优化文件上传体验
8. **严格验证**：使用FluentValidation验证文件和资产类型

#### 工作流程
1. 用户（通讯作者）发送POST请求，包含文章ID和文件数据
2. API端点验证用户角色和请求数据
3. 创建UploadManuscriptFileCommand并设置文章ID
4. 通过MediatR发送命令到对应的处理器
5. 处理器执行文件上传逻辑并将文件信息保存到数据库
6. 返回创建成功的响应，包含新创建的资源ID和下载链接

这个实现提供了一个安全、高效、易于维护的上传稿件接口，符合科学文章管理系统的业务需求。

### 3.20 使用 Asset 聚合 与 File 值对象

在科学文章管理系统中，Asset（资产）聚合和File（文件）值对象是处理文件上传、存储和管理的核心组件。它们共同构成了稿件和补充材料的文件管理基础。

#### 1. 核心概念设计

**Asset 聚合** 是文件资产的核心实体，它将文件元数据与文章关联起来。**File 值对象** 封装了文件的物理存储信息，包括文件名、扩展名、大小和存储ID等。

#### 2. StringValueObject 抽象值对象

`StringValueObject` 是所有字符串类型值对象的基础，它提供了统一的字符串值封装和比较功能：

```csharp
// BuildingBlocks.Domain/ValueObjects/StringValueObject.cs
public abstract class StringValueObject : IEquatable<StringValueObject>, IEquatable<string>
{
    public string Value { get; protected set; }

    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode();

    public bool Equals(StringValueObject? other)
        => Value.Equals(other?.Value);

    public bool Equals(string? other)
        => Value.Equals(other);
    
    public static implicit operator string(StringValueObject stringValueObject) => stringValueObject.ToString();
}
```

这个抽象类确保了所有继承的字符串值对象都具有一致的行为，包括相等性比较和字符串转换。

#### 3. Asset 实体（聚合根）

`Asset` 实体是文件资产的聚合根，它将文件与文章关联起来：

```csharp
// Submission.Domain/Entities/Asset.cs
public partial class Asset : IEntity
{
    public int Id { get; init; }
    public AssetName Name { get; private set; }
    public AssetType Type { get; private set; }
    public int ArticleId { get; private set; }
    public Article Article { get; private set; }
    public ValueObjects.File File { get; set; }
}
```

- `Id`：资产的唯一标识符
- `Name`：资产名称（值对象）
- `Type`：资产类型（如稿件、补充材料等）
- `ArticleId`：关联的文章ID
- `Article`：关联的文章实体
- `File`：文件信息（值对象）

#### 4. AssetName 值对象

`AssetName` 封装了资产的名称信息：

```csharp
// Submission.Domain/ValueObjects/AssetName.cs
public class AssetName : StringValueObject
{
    private AssetName(string value) => Value = value;

    public static AssetName FromAssetType(AssetTypeDefinition assetTypeDefinition)
        => new AssetName(assetTypeDefinition.Name.ToString());
}
```

它提供了从资产类型定义创建资产名称的工厂方法，确保资产名称的一致性。

#### 5. File 值对象

`File` 值对象封装了文件的核心信息：

```csharp
// Submission.Domain/ValueObjects/File.cs
public class File
{
    public required string OriginalName { get; init; }
    public required string FileServerId { get; init; }
    public required long Size { get; init; }
    public required FileName Name { get; init; }
    public required FileExtension Extension { get; init; }
}
```

- `OriginalName`：文件的原始名称
- `FileServerId`：文件服务器上的唯一标识符
- `Size`：文件大小（字节）
- `Name`：规范化的文件名（值对象）
- `Extension`：文件扩展名（值对象）

#### 6. FileName 值对象

`FileName` 封装了规范化的文件名：

```csharp
// Submission.Domain/ValueObjects/FileName.cs
public class FileName : StringValueObject
{
    private FileName(string value) => Value = value;

    public static FileName Create(Asset asset, string extension)
    {
        var assetName = asset.Name;
        return new FileName($"{assetName}.{extension}");
    }
}
```

它提供了从资产和扩展名创建规范化文件名的工厂方法。

#### 7. FileExtension 值对象

`FileExtension` 封装了文件的扩展名信息：

```csharp
// Submission.Domain/ValueObjects/FileExtension.cs
public class FileExtension : StringValueObject
{
    private FileExtension(string value) => Value = value;

    public static FileExtension FromFileExtension(string fileName, AssetType assetType)
    {
        var extension = Path.GetExtension(fileName).Remove(0, 1);
        Guard.ThrowIfNullOrWhiteSpace(extension);

        return new FileExtension(extension);
    }
}
```

它提供了从文件名和资产类型提取扩展名的工厂方法，并确保扩展名不为空。

#### 8. AssetTypeDefinition 实体

`AssetTypeDefinition` 实体定义了系统支持的资产类型：

```csharp
// Submission.Domain/Entities/AssetTypeDefinition.cs
public class AssetTypeDefinition : EnumEntity<AssetType>
{
    public required string Description { get; init; }
    public required FileExtensions AllowedFileExtensions { get; init; }
    public required long MaxFileSizeInBytes { get; init; }
    public required int MaxFileCount { get; init; }
}
```

它继承自 `EnumEntity<AssetType>`，提供了资产类型的元数据，包括允许的文件扩展名、最大文件大小和最大文件数量等。

#### 9. 关系与交互

这些组件之间的关系如下：
- `Asset` 聚合包含 `File` 值对象
- `File` 值对象包含 `FileName` 和 `FileExtension` 值对象
- `AssetName`、`FileName` 和 `FileExtension` 都继承自 `StringValueObject`
- `Asset` 通过 `ArticleId` 与 `Article` 实体关联

#### 10. 设计优势

- **类型安全**：使用值对象确保了文件名、扩展名等数据的类型安全
- **封装性**：将文件相关信息封装在值对象中，提高了代码的可维护性
- **不变性**：值对象一旦创建就不可变，确保了数据的一致性
- **可扩展性**：通过继承 `StringValueObject`，可以轻松创建新的字符串类型值对象
- **业务规则集成**：将业务规则（如文件扩展名验证）集成到值对象中

这个设计为科学文章管理系统提供了坚实的文件资产管理基础，支持稿件和补充材料的上传、存储和管理功能。

### 3.21 配置EFCore复杂属性配置

在科学文章管理系统中，EF Core的复杂属性配置是将领域模型映射到数据库表的关键步骤。特别是对于值对象和复杂类型的映射，需要特殊的配置来确保数据的正确持久化。

#### 1. 实体配置基类

项目中使用了一个通用的实体配置基类`EntityConfiguration<T>`，它提供了基本的实体配置功能，如主键配置、软删除支持等。所有实体配置类都继承自这个基类或直接实现`IEntityTypeConfiguration<T>`接口。

#### 2. AssetEntityConfiguration - 资产实体配置

`AssetEntityConfiguration`负责配置`Asset`实体及其与其他实体的关系：

```csharp
// Submission.Persistence/EntityConfigurations/AssetEntityConfiguration.cs
public class AssetEntityConfiguration : EntityConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Type).HasEnumConversion();

        builder.ComplexProperty(x => x.Name, builder =>
        {
            builder.Property(x => x.Value)
                .HasColumnName(builder.Metadata.PropertyInfo.Name)
                .HasMaxLength(MaxLength.C64)
                .IsRequired();
        });
    }
}
```

关键配置点：
- 使用`HasEnumConversion()`配置`AssetType`枚举的转换
- 使用`ComplexProperty()`配置`AssetName`复杂属性，将其映射到数据库列
- 设置属性的最大长度和必填性

#### 3. FileEntityConfiguration - 文件值对象配置

`FileEntityConfiguration`负责配置`File`值对象的持久化方式：

```csharp
// Submission.Persistence/EntityConfigurations/FileEntityConfiguration.cs
internal class FileEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Domain.ValueObjects.File> builder)
    {
        builder.Property(x => x.OriginalName).HasMaxLength(MaxLength.C256)
            .HasComment("Original full file name, with extension");
        builder.Property(x => x.FileServerId).HasMaxLength(MaxLength.C64);
        builder.Property(x => x.Size).HasComment("Size of the file in kilobytes");

        builder.ComplexProperty(x => x.Extension, complextBuilder =>
        {
            complextBuilder.Property(x => x.Value)
                .HasColumnName($"{builder.Metadata.ClrType.Name}_{complextBuilder.Metadata.PropertyInfo.Name}")
                .HasMaxLength(MaxLength.C8);
        });

        builder.ComplexProperty(x => x.Name, complextBuilder =>
        {
            complextBuilder.Property(x => x.Value)
                .HasColumnName($"{builder.Metadata.ClrType.Name}_{complextBuilder.Metadata.PropertyInfo.Name}")
                .HasMaxLength(MaxLength.C64).HasComment("Final name of the file after renaming");
        });
    }
}
```

关键配置点：
- 为`OriginalName`、`FileServerId`和`Size`等简单属性配置列名、最大长度和注释
- 使用`ComplexProperty()`配置`Extension`和`Name`复杂属性
- 通过`HasColumnName()`为复杂属性的子属性生成唯一的列名

#### 4. AssetTypeDefinitionEntityConfiguration - 资产类型定义配置

`AssetTypeDefinitionEntityConfiguration`负责配置`AssetTypeDefinition`实体：

```csharp
// Submission.Persistence/EntityConfigurations/AssetTypeDefinitionEntityConfiguration.cs
public class AssetTypeDefinitionEntityConfiguration : IEntityTypeConfiguration<AssetTypeDefinition>
{
    public void Configure(EntityTypeBuilder<AssetTypeDefinition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).HasEnumConversion()
            .HasMaxLength(MaxLength.C64)
            .IsRequired()
            .HasColumnOrder(1);

        builder.Property(x => x.MaxFileSizeInMB).HasDefaultValue(5);
        builder.Property(x => x.DefaultFileExtension)
            .HasMaxLength(MaxLength.C8)
            .HasDefaultValue("pdf")
            .IsRequired();

        builder.ComplexProperty(x => x.AllowedFileExtensions, builder =>
        {
            var converter = BuilderExtensions.BuildJsonListConvertor<string>();

            builder.Property(x => x.Extensions)
                .HasConversion(converter)
                .HasColumnName(builder.Metadata.PropertyInfo.Name)
                .IsRequired();
        });
    }
}
```

关键配置点：
- 配置主键和唯一索引
- 使用`HasEnumConversion()`配置枚举属性
- 设置属性的默认值、最大长度和必填性
- 使用JSON转换器将`AllowedFileExtensions`复杂属性转换为JSON格式存储

#### 5. ArticleAuthorEntityConfiguration - 文章作者关系配置

`ArticleAuthorEntityConfiguration`负责配置`ArticleAuthor`实体，它表示文章与作者之间的多对多关系：

```csharp
// Submission.Persistence/EntityConfigurations/ArticleAuthorEntityConfiguration.cs
public class ArticleAuthorEntityConfiguration : IEntityTypeConfiguration<ArticleAuthor>
{
    public void Configure(EntityTypeBuilder<ArticleAuthor> builder)
    {
        // 复合主键配置
        builder.HasKey(x => new { x.ArticleId, x.PersonId, x.Role });

        // 贡献领域配置
        builder.Property(x => x.ContributeAreas)
            .HasJsonCollectionConversion()
            .IsRequired();
    }
}
```

关键配置点：
- 使用复合主键配置文章、作者和角色的唯一组合
- 使用JSON转换器将`ContributeAreas`枚举集合转换为JSON格式存储

#### 6. 修改 SubmissionDbContext

在`SubmissionDbContext`中，需要将这些实体配置类应用到模型构建器中：

```csharp
// Submission.Persistence/SubmissionDbContext.cs
public class SubmissionDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 应用实体配置
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 或者逐个应用配置
        // modelBuilder.ApplyConfiguration(new AssetEntityConfiguration());
        // modelBuilder.ApplyConfiguration(new FileEntityConfiguration());
        // modelBuilder.ApplyConfiguration(new AssetTypeDefinitionEntityConfiguration());
        // modelBuilder.ApplyConfiguration(new ArticleAuthorEntityConfiguration());
    }
}
```

使用`ApplyConfigurationsFromAssembly()`方法可以自动应用程序集中的所有实体配置类，这是一种更简洁的方式。

#### 7. 配置要点与最佳实践

1. **复杂属性映射**：使用`ComplexProperty()`配置值对象和复杂类型
2. **枚举处理**：使用`HasEnumConversion()`处理枚举类型
3. **JSON转换**：使用`HasJsonCollectionConversion()`或自定义转换器处理集合类型
4. **命名规范**：保持数据库列名与领域模型属性名的一致性
5. **注释与文档**：为属性添加注释，提高数据库的可读性
6. **默认值设置**：为可选属性设置合理的默认值
7. **长度限制**：为字符串属性设置适当的最大长度
8. **索引优化**：为频繁查询的字段添加索引

这些配置确保了领域模型能够正确地映射到数据库表，同时保持了领域模型的完整性和一致性。通过这种方式，当应用程序运行时，EF Core能够自动创建和维护数据库结构，并正确地持久化和检索数据。

### 3.22 使用 MediatR 和 仓储实现命令处理器

在科学文章管理系统中，命令处理器是应用层的核心组件，它负责处理用户的命令请求并执行业务逻辑。结合MediatR和仓储模式，可以实现松耦合、可测试的命令处理架构。

#### 1. MediatR 命令处理器基础

命令处理器实现了`IRequestHandler<TRequest, TResponse>`接口，负责处理特定类型的命令请求：

```csharp
public interface IRequestHandler<in TRequest, TResponse> : IRequestHandler
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
```

#### 2. 仓储模式基础

项目中使用了通用的仓储模式，提供了数据访问的抽象层：

```csharp
// BuildingBlocks.EntityFramework/Repository.cs
public class Repository<TContext, TEntity> : IRepository<TEntity>
    where TContext : DbContext
    where TEntity : class, IEntity<int>
{
    public readonly TContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(TContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
    }

    public virtual Task<TEntity?> GetByIdAsync(int id)
        => Query().SingleOrDefaultAsync(x => x.Id == id);

    public virtual async Task<TEntity> AddAsync(TEntity entity)
        => (await DbSet.AddAsync(entity)).Entity;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);
}
```

#### 3. 具体命令处理器实现 - UploadManuscriptFileCommandHandler

以上传稿件文件的命令处理器为例，展示如何结合MediatR和仓储模式：

```csharp
// Submission.Application/Features/UploadFile/UploadManuscriptFile/UploadManuscriptFileCommandHandler.cs
public class UploadManuscriptFileCommandHandler(
    ArticleRepository articleRepository,
    AssetTypeDefinitionRepository assetTypeDefinitionRepository,
    IFileService fileService) : IRequestHandler<UploadManuscriptFileCommand, IdResponse>
{
    public async Task<IdResponse> Handle(UploadManuscriptFileCommand command, CancellationToken cancellationToken)
    {
        var article = await articleRepository.FindByIdOrThrowAsync(command.ArticleId);

        var assetType = assetTypeDefinitionRepository.GetById(command.AssetType);

        Asset asset = null;
        if (!assetType.AllowsMultipleAssets)
            asset = article.Assets.SingleOrDefault(x => x.Type == assetType.Id);

        if (asset is null)
            asset = article.CreateAsset(assetType);

        var filePath = asset.GenerateStorageFilePath(command.File.FileName);
        await fileService.UploadFileAsync(filePath, command.File, true, new Dictionary<string, string>
        {
            { "entity", nameof(Asset) },
            { "entityId", asset.Id.ToString() },
        });

        await articleRepository.SaveChangesAsync();

        return new IdResponse(asset.Id);
    }
}
```

#### 4. 命令处理器的工作流程

1. **依赖注入**：通过构造函数注入所需的仓储和服务
2. **数据验证**：MediatR管道会自动调用验证器验证命令
3. **数据获取**：使用仓储获取所需的实体（如文章）
4. **业务逻辑执行**：
   - 检查是否已存在同类型的资产
   - 如果不存在，创建新的资产
   - 生成存储文件路径
   - 使用文件服务上传文件
5. **数据持久化**：调用仓储的`SaveChangesAsync`方法保存更改
6. **返回结果**：返回操作结果（如资产ID）

#### 5. 仓储的扩展 - ArticleRepository

项目中对通用仓储进行了扩展，以满足特定业务需求：

```csharp
// Submission.Persistence/Repositories/ArticleRepository.cs
public class ArticleRepository(SubmissionDbContext dbContext) : Repository<Article>(dbContext)
{
    // 可以在这里添加文章特定的查询方法
}
```

#### 6. MediatR 管道行为

项目中还使用了MediatR的管道行为来处理横切关注点，如验证、审计等：

```csharp
// Submission.Application/DependencyInjection.cs
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(SetUserIdBehavior<,>));
});
```

#### 7. 设计优势

- **松耦合**：命令处理器只依赖于接口，不依赖于具体实现
- **可测试性**：可以轻松地模拟仓储和服务，进行单元测试
- **职责分离**：每个命令处理器只负责处理一种命令，符合单一职责原则
- **可扩展性**：可以轻松地添加新的命令和处理器
- **横切关注点处理**：通过管道行为统一处理验证、审计等横切关注点

这个设计为科学文章管理系统提供了灵活、可维护的命令处理架构，支持稿件上传、文章创建、作者分配等核心功能。

### 3.23 为 AssetType 实现内存缓存仓储

在科学文章管理系统中，资产类型（AssetType）是相对稳定的数据，不会频繁变化。为了提高系统性能，减少数据库查询，我们为AssetType实现了内存缓存仓储。

#### 1. 内存缓存仓储的概念与优势

内存缓存仓储是对传统仓储模式的扩展，它在内存中缓存查询结果，避免频繁访问数据库。这种模式特别适合于：
- 数据不频繁变化的场景
- 查询频率高的数据
- 对性能要求较高的系统

#### 2. 缓存仓储基类 - CachedRepository

项目中实现了一个通用的缓存仓储基类`CachedRepository`，它提供了基本的缓存功能：

```csharp
// BuildingBlocks.EntityFramework/CachedRepository.cs
public abstract class CachedRepository<TDbContext, TEntity, TId>(TDbContext dbContext, IMemoryCache cache)
    where TDbContext : DbContext
    where TEntity : class, IEntity<TId>, ICacheable
    where TId : struct
{
    public IEnumerable<TEntity> GetAll()
        => cache.GetOrCreateByType(entry => dbContext.Set<TEntity>().AsNoTracking().ToList());

    public TEntity GetById(TId id)
        => cache.GetOrCreate($"{typeof(TEntity).FullName}_{id}",
            entry => dbContext.Set<TEntity>().AsNoTracking().Single(x => x.Id.Equals(id)));
}
```

关键设计点：
- 接收`DbContext`和`IMemoryCache`作为依赖
- 支持泛型类型参数，提高复用性
- 提供`GetAll()`和`GetById()`方法，从缓存或数据库获取数据
- 使用`AsNoTracking()`提高查询性能

#### 3. 内存缓存扩展 - MemoryCacheExtensions

为了简化缓存操作，项目中实现了`MemoryCacheExtensions`静态扩展类：

```csharp
// BuildingBlocks.Core/Cache/MemoryCacheExtensions.cs
public static class MemoryCacheExtensions
{
    public static T GetOrCreateByType<T>(this IMemoryCache memoryCache, Func<ICacheEntry, T> factory)
        => memoryCache.GetOrCreate(typeof(T).Name, factory);
}
```

这个扩展方法提供了一个简洁的方式，使用类型名称作为缓存键，获取或创建缓存条目。

#### 4. AssetTypeDefinitionRepository - 资产类型定义仓储

基于`CachedRepository`，我们实现了`AssetTypeDefinitionRepository`，用于管理资产类型定义：

```csharp
// Submission.Persistence/Repositories/AssetTypeDefinitionRepository.cs
public class AssetTypeDefinitionRepository(SubmissionDbContext dbContext, IMemoryCache memoryCache)
    : CachedRepository<SubmissionDbContext, AssetTypeDefinition, AssetType>(dbContext, memoryCache)
{
    // 可以在这里添加资产类型特定的查询方法
}
```

这个类继承自`CachedRepository`，并指定了具体的类型参数：
- `SubmissionDbContext`：数据库上下文
- `AssetTypeDefinition`：实体类型
- `AssetType`：实体ID类型（枚举）

#### 5. 缓存仓储的使用

在命令处理器中，可以直接使用`AssetTypeDefinitionRepository`来获取资产类型定义：

```csharp
// Submission.Application/Features/UploadFile/UploadManuscriptFile/UploadManuscriptFileCommandHandler.cs
public class UploadManuscriptFileCommandHandler(
    ArticleRepository articleRepository,
    AssetTypeDefinitionRepository assetTypeDefinitionRepository,
    IFileService fileService) : IRequestHandler<UploadManuscriptFileCommand, IdResponse>
{
    public async Task<IdResponse> Handle(UploadManuscriptFileCommand command, CancellationToken cancellationToken)
    {
        // 从缓存或数据库获取文章
        var article = await articleRepository.FindByIdOrThrowAsync(command.ArticleId);

        // 从缓存获取资产类型定义
        var assetType = assetTypeDefinitionRepository.GetById(command.AssetType);

        // 其他业务逻辑...
    }
}
```

#### 6. 缓存配置与依赖注入

在API层的依赖注入配置中，需要注册内存缓存服务：

```csharp
// Submission.API/DependencyInjection.cs
builder.Services.AddMemoryCache();
```

然后在Persistence层注册仓储服务：

```csharp
// Submission.Persistence/DependencyInjection.cs
builder.Services.AddScoped<AssetTypeDefinitionRepository>();
```

#### 7. 设计优势

- **性能提升**：减少数据库查询次数，提高系统响应速度
- **代码复用**：通过泛型基类实现缓存逻辑的复用
- **可扩展性**：可以轻松为其他实体类型添加缓存功能
- **透明性**：对调用者来说，缓存操作是透明的
- **灵活性**：可以根据需要调整缓存策略（如过期时间、优先级等）

这个内存缓存仓储设计为科学文章管理系统提供了高性能的资产类型查询功能，同时保持了代码的简洁性和可维护性。

### 3.24 使用 Mongo GridFS 实现文件存储模块

文件存储模块采用分层设计，将接口定义与具体实现分离，便于扩展和测试。模块主要由以下几个部分组成：

#### 1. 接口定义 (`FileStorage.Contracts`)

定义了文件存储服务的核心接口 `IFileService`，提供文件上传、下载和删除功能：

```csharp
// FileStorage.Contracts/IFileService.cs
using Microsoft.AspNetCore.Http;

namespace FileStorage.Contracts;

public interface IFileService
{
    Task<UploadResponse> UploadFileAsync(string filePath, IFormFile file, bool overwrite = false,
        Dictionary<string, string>? tags = null);

    Task<(Stream FileStream, string ContentType)> DownloadFileAsync(string fileId);

    Task<bool> TryDeleteFileAsync(string fileId);
}
```

同时定义了上传响应的数据传输对象：

```csharp
// FileStorage.Contracts/UploadResponse.cs
namespace FileStorage.Contracts;

public record UploadResponse(string FilePath, string FileName, long FileSize, string FileId);
```

#### 2. MongoDB GridFS 实现 (`FileStorage.MongoGridFS`)

基于 Mongo GridFS 实现了文件存储服务，提供高性能的大文件存储能力：

```csharp
// FileStorage.MongoGridFS/FileService.cs
using FileStorage.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace FileStorage.MongoGridFS;

public class FileService : IFileService
{
    private readonly GridFSBucket _gridFsBucket;
    private readonly MongoGridFSFileStorageOptions _options;

    private const string FilePathMetadataKey = "filePath";
    private const string ContentTypeMetadataKey = "contentType";
    private const string DefaultContentType = "application/octet-stream";

    public FileService(GridFSBucket gridFSBucket, IOptions<MongoGridFSFileStorageOptions> options)
        => (_gridFsBucket, _options) = (gridFSBucket, options.Value);

    public async Task<UploadResponse> UploadFileAsync(string filePath, IFormFile file, bool overwrite = false,
        Dictionary<string, string>? tags = null)
    {
        if (file.Length > _options.FileSizeLimitInBytes)
            throw new InvalidOperationException(
                $"File exceeds maximum allowed size of {_options.FileSizeLimitInMB} MB.");

        var metadata = new BsonDocument(tags ?? new())
        {
            { FilePathMetadataKey, filePath },
            { ContentTypeMetadataKey, file.ContentType },
        };

        var uploadOptions = new GridFSUploadOptions
        {
            Metadata = metadata,
            ChunkSizeBytes = _options.ChunkSizeBytes,
        };

        using var stream = file.OpenReadStream();
        var fileId = await _gridFsBucket.UploadFromStreamAsync(file.FileName, stream, uploadOptions);

        return new UploadResponse(
            FilePath: filePath,
            FileName: file.FileName,
            FileSize: file.Length,
            FileId: fileId.ToString());
    }

    public async Task<(Stream FileStream, string ContentType)> DownloadFileAsync(string fileId)
    {
        if (!ObjectId.TryParse(fileId, out ObjectId objectId))
            throw new FileNotFoundException($"Invalid file id '{fileId}'.");

        var fileInfo = await _gridFsBucket
            .Find(Builders<GridFSFileInfo>.Filter.Eq("_id", fileId))
            .FirstOrDefaultAsync();

        if (fileInfo is null)
            throw new FileNotFoundException($"File not found with id: '{fileId}'.");

        var stream = await _gridFsBucket.OpenDownloadStreamAsync(fileId);
        var contentType = fileInfo.Metadata
            .GetValue(ContentTypeMetadataKey, DefaultContentType)
            ?.AsString ?? DefaultContentType;

        return (stream, contentType);
    }

    public async Task<bool> TryDeleteFileAsync(string fileId)
    {
        if (!ObjectId.TryParse(fileId, out ObjectId objectId))
            return false;

        try
        {
            await _gridFsBucket.DeleteAsync(objectId);
            return true;
        }
        catch (GridFSFileNotFoundException)
        {
            return false;
        }
    }
}
```

#### 3. 配置类 (`MongoGridFSFileStorageOptions`)

定义了文件存储服务的配置选项，便于从配置文件中读取设置：

```csharp
// FileStorage.MongoGridFS/MongoGridFSFileStorageOptions.cs
using System.ComponentModel.DataAnnotations;

namespace FileStorage.MongoGridFS;

public class MongoGridFSFileStorageOptions
{
    [Required]
    public string ConnectionStringName { get; init; }
    [Required]
    public string DatabaseName { get; init; }
    [Required]
    public string BucketName { get; init; } = "files";
    public int ChunkSizeBytes { get; init; } = 1048576; // 1MB
    public long FileSizeLimitInMB { get; init; } = 50;
    public long FileSizeLimitInBytes => FileSizeLimitInMB * 1024 * 1024;
}
```

#### 4. 服务注册 (`FileStorageRegistration`)

提供了扩展方法，简化服务注册过程：

```csharp
// FileStorage.MongoGridFS/FileStorageRegistration.cs
using BuildingBlocks.Core;
using FileStorage.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace FileStorage.MongoGridFS;

public static class FileStorageRegistration
{
    public static IServiceCollection AddMongoFileStorage(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAndValidateOptions<MongoGridFSFileStorageOptions>(configuration);

        var options = configuration.GetSectionByTypeName<MongoGridFSFileStorageOptions>();

        // 注册 MongoDB 客户端
        services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(configuration.GetConnectionStringOrThrow(options.ConnectionStringName)));

        // 注册 MongoDB 数据库
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.DatabaseName);
        });

        // 注册 GridFS Bucket
        services.AddSingleton(sp =>
        {
            var db = sp.GetRequiredService<IMongoDatabase>();
            return new GridFSBucket(db, new GridFSBucketOptions
            {
                BucketName = options.BucketName,
                ChunkSizeBytes = options.ChunkSizeBytes,
                WriteConcern = WriteConcern.WMajority,
                ReadPreference = ReadPreference.Primary,
            });
        });

        // 注册文件服务
        services.AddSingleton<IFileService, FileService>();

        return services;
    }
}
```

#### 5. 配置文件示例

在应用程序的配置文件中，可以设置文件存储服务的相关参数：

```json
// appsettings.json
{
  "ConnectionStrings": {
    "MongoFileStorage": "mongodb://admin:admin@localhost:27017/gridfs_db?authSource=admin"
  },
  "MongoGridFSFileStorageOptions": {
    "ConnectionStringName": "MongoFileStorage",
    "DatabaseName": "gridfs_db",
    "BucketName": "article_files",
    "ChunkSizeBytes": 2097152, // 2MB
    "FileSizeLimitInMB": 100
  }
}
```

#### 6. 使用示例

在应用程序中使用文件存储服务：

```csharp
// 在 API 层注册服务
// Submission.API/DependencyInjection.cs
builder.Services.AddMongoFileStorage(configuration);

// 在业务逻辑中使用
// Submission.Application/Features/UploadFile/UploadManuscriptFile/UploadManuscriptFileCommandHandler.cs
public class UploadManuscriptFileCommandHandler(
    ArticleRepository articleRepository,
    IFileService fileService) : IRequestHandler<UploadManuscriptFileCommand, IdResponse>
{
    public async Task<IdResponse> Handle(UploadManuscriptFileCommand command, CancellationToken cancellationToken)
    {
        // 业务逻辑...
        
        // 上传文件
        var uploadResponse = await fileService.UploadFileAsync(
            filePath: $"articles/{command.ArticleId}/manuscript",
            file: command.File,
            tags: new Dictionary<string, string>
            {
                { "articleId", command.ArticleId.ToString() },
                { "fileType", "manuscript" }
            });
        
        // 业务逻辑...
    }
}
```

#### 7. 设计优势

- **接口与实现分离**：便于替换不同的存储实现（如本地存储、AWS S3等）
- **配置驱动**：通过配置文件灵活调整存储参数
- **错误处理**：提供清晰的错误信息和异常处理
- **元数据支持**：允许附加自定义元数据到文件
- **性能优化**：支持大文件分块存储，提高传输效率
- **依赖注入**：便于测试和维护

这个文件存储模块为科学文章管理系统提供了可靠、高性能的文件存储能力，支持大文件上传和下载，满足了科学文章管理的需求。

## 四、微服务 Auth 与 FastEndpoints、JWT认证 & Role-Based Access Control

### 4.1 认证流程、用户故事与接口设计

#### 4.1.1 认证流程(Auth Workflow)

1. 管理员创建一个账户并分配角色
2. 系统会发送一封确认邮件以验证用户身份。
3. 账户一经确认，用户即可使用其凭据登录。
4. 用户必须确认其账户并设置密码，然后才允许登录。
5. 登录期间，系统会生成一个短期的访问令牌(用于验证请求的JWT)，同时会生成一个长期 的刷新令牌。
6. 当访问令牌过期时，用户可以使用刷新令牌获取新的访问令牌，而无需重新登录。

身份验证服务负责处理认证与授权，颁发访问令牌和刷新令牌，实施基于角色的访问控制，并且是系统中所有其他服务所依赖的核心组件。

#### 4.1.2 用户故事(User Stories)

1. 创建用户
    - 作为管理员，我想创建一个新用户账户，以便为其分配角色和权限。
    - 系统会发送一封确认邮件，用户需要点击链接确认账户。
    - 账户确认后，用户即可使用其凭据登录。 
2. 设置密码
    - 作为用户，确认邮件包含一个设置密码的安全链接。
    - 用户点击链接，进入设置密码页面。
    - 用户输入新密码并确认，密码设置成功。
3. 重置密码
    - 作为用户，我想重置我的密码，以便在忘记密码时能够登录。
    - 系统会发送一封包含重置密码安全链接的邮件给用户。
    - 用户点击链接，进入重置密码页面。
    - 用户输入新密码并确认，密码重置成功。
4. 登录
    - 作为用户，登录后系统会返回一个访问令牌和刷新令牌。
    - 访问令牌用于验证后续请求，刷新令牌用于获取新的访问令牌。
5. 刷新访问令牌
    - 当访问令牌过期时，用户可以使用刷新令牌获取新的访问令牌。
    - 刷新令牌也有过期时间，过期后需要重新登录获取新的刷新令牌。
6. 获取用户信息
    - 作为服务，系统中的某些服务需要获取当前登录用户的信息，例如显示姓名、检查角色或发送电子邮件。
    - 这些服务可以通过访问令牌中的用户ID来获取用户信息。

#### 4.1.3 接口设计(API Endpoints)

| 接口名称    | 请求方式 | 角色   | 接口路由                          |
| ---------- | ------- | ----- | -------------------------------- |
| 创建用户    | POST    | ADMIN | /api/users                       |
| 设置密码    | POST    | USER  | /api/password                    |
| 重置密码    | POST    | USER  | /api/password:reset              |
| 用户登录    | POST    | USER  | /api/login                       |
| 刷新访问令牌 | POST    | USER  | /api/token:refresh               |
| 获取用户信息 | gRPC    | SYSTEM | UserService.GetUserById(userId) |

### 4.2.1 需求、功能、性能和安全

- 功能性需求：
    1. 创建用户
    2. 设置密码
    3. 重置密码
    4. 登录
    5. 刷新令牌
    6. 获取用户信息
- 非功能性需求：
    1. 身份认证
        - Token类型： JWT(Access + Refresh)
        - 过期时间：Access 15分钟，Refresh 7天
    2. 密码管理
        - 存储：使用 ASP.NET Core Identity
        - 要求：至少8个字符、1个大写字母、1个数字、1个特殊字符
    3. 安全
        - 基于角色的权限访问控制
        - 部分接口允许匿名访问
    4. 可扩展性
        - 水平扩展：能够在需要时添加更多实例
        - 无状态接口：每个请求都包含所有必要的信息，服务器不存储会话状态。
    5. 性能
        - 响应时间：登录和生成令牌时间应在200ms以内。
        - gRPC响应时间应在100ms以内。

### 4.3 身份认证与鉴权流程

1. 用户通过身份认证服务登录，收到一个JWT令牌，然后使用该令牌访问其他服务中受保护的接口。
2. 其他服务收到请求后，会验证JWT令牌的有效性。
    - 检查令牌是否过期。
    - 验证令牌签名是否正确。
    - 检查令牌中包含的用户角色是否有权限访问该接口。
3. 如果令牌验证通过，服务会处理请求并返回响应。
    - 如果令牌验证失败，服务会返回401 Unauthorized错误。

### 4.4 基于JWT令牌的单点登录跨服务认证

#### 4.4.1 JWT令牌基础概念

JSON Web Token (JWT) 是一种开放标准 (RFC 7519)，用于在各方之间安全地传输信息。JWT 由三部分组成：

1. **Header (头部)**：包含令牌类型和签名算法
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

2. **Payload (载荷)**：包含声明（Claims），如用户信息、角色、过期时间等
```json
{
  "sub": "1234567890",
  "name": "John Doe",
  "role": "Author",
  "exp": 1516239022
}
```

3. **Signature (签名)**：用于验证令牌的完整性
```
HMACSHA256(
  base64UrlEncode(header) + "." +
  base64UrlEncode(payload),
  secret)
```

#### 4.4.2 访问令牌与刷新令牌

**访问令牌 (Access Token)**
- 有效期短（通常15分钟）
- 用于访问受保护的资源
- 包含用户身份和权限信息
- 存储在客户端内存中

**刷新令牌 (Refresh Token)**
- 有效期长（通常7天）
- 用于获取新的访问令牌
- 仅包含用户ID和令牌ID
- 存储在HttpOnly Cookie中，防止XSS攻击

#### 4.4.3 单点登录实现原理

单点登录（Single Sign-On, SSO）允许用户使用一组凭据登录多个应用程序。在我们的系统中：

1. 用户在身份认证服务登录
2. 系统验证凭据并颁发JWT令牌
3. 用户使用JWT令牌访问其他微服务
4. 每个微服务独立验证JWT令牌
5. 无需在每个服务中维护会话状态

#### 4.4.4 跨服务认证流程

```
用户 → 网关 → 投稿服务 → 验证JWT → 处理请求
         ↓
      提交JWT
```

**认证流程详细步骤**：

1. **用户登录**
   - 用户提交用户名和密码
   - 身份认证服务验证凭据
   - 生成访问令牌和刷新令牌
   - 返回令牌给客户端

2. **访问受保护资源**
   - 客户端在请求头中携带访问令牌：`Authorization: Bearer {access_token}`
   - 网关将请求路由到目标微服务
   - 微服务验证JWT令牌的有效性
   - 验证通过后处理请求

3. **令牌刷新**
   - 访问令牌过期时，客户端使用刷新令牌申请新令牌
   - 身份认证服务验证刷新令牌
   - 颁发新的访问令牌
   - 可选地颁发新的刷新令牌

#### 4.4.5 JWT令牌生成实现

```csharp
// Auth.Application/Features/Login/GenerateToken.cs
public static class GenerateToken
{
    public static (string AccessToken, string RefreshToken) Generate(
        User user,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var accessTokenExpirationMinutes = int.Parse(jwtSettings["AccessTokenExpirationMinutes"]);
        var refreshTokenExpirationDays = int.Parse(jwtSettings["RefreshTokenExpirationDays"]);

        // 生成访问令牌
        var accessToken = GenerateAccessToken(
            user,
            secretKey,
            issuer,
            audience,
            accessTokenExpirationMinutes);

        // 生成刷新令牌
        var refreshToken = GenerateRefreshToken();

        return (accessToken, refreshToken);
    }

    private static string GenerateAccessToken(
        User user,
        string secretKey,
        string issuer,
        string audience,
        int expirationMinutes)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
```

#### 4.4.6 JWT令牌验证中间件

```csharp
// BuildingBlocks.AspNetCore/JwtAuthenticationMiddleware.cs
public class JwtAuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var principal = ValidateToken(token);

            if (principal != null)
            {
                context.User = principal;
            }
        }

        await next(context);
    }

    private static ClaimsPrincipal ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("your-secret-key-here");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "your-issuer",
                ValidAudience = "your-audience",
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}
```

#### 4.4.7 配置JWT认证

```csharp
// Auth.API/DependencyInjection.cs
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = configuration.GetSection("JwtSettings");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User", "Admin"));
});
```

#### 4.4.8 配置文件示例

```json
{
  "JwtSettings": {
    "SecretKey": "your-very-long-secret-key-at-least-32-characters-long",
    "Issuer": "https://auth.yourdomain.com",
    "Audience": "https://api.yourdomain.com",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

#### 4.4.9 设计优势

- **无状态性**：服务器不需要存储会话状态，便于水平扩展
- **跨域支持**：JWT令牌可以在不同域之间传递
- **安全性**：令牌签名确保数据完整性，过期时间增强安全性
- **性能**：减少数据库查询，提高响应速度
- **灵活性**：可以在令牌中携带自定义声明
- **标准化**：基于开放标准，便于集成第三方工具

#### 4.4.10 安全最佳实践

1. **密钥管理**：使用强密钥，定期轮换
2. **HTTPS传输**：始终使用HTTPS传输令牌
3. **令牌过期**：设置合理的过期时间
4. **刷新令牌存储**：使用HttpOnly Cookie存储刷新令牌
5. **令牌撤销**：实现令牌黑名单机制
6. **声明最小化**：只在令牌中包含必要的声明

### 4.5 聚合 实体、值对象、领域事件

#### 4.5.1 DDD战术设计概述

领域驱动设计（DDD）的战术设计提供了一套建模工具，帮助我们构建丰富的领域模型。核心概念包括：

- **聚合（Aggregate）**：定义了一致性边界，是一组相关对象的集合
- **实体（Entity）**：具有唯一标识的对象，标识符不变但属性可变
- **值对象（Value Object）**：通过属性值定义的对象，不可变且可替换
- **领域事件（Domain Event）**：表示领域内发生的重要业务事件

#### 4.5.2 聚合（Aggregate）

**聚合的定义**

聚合是一组相关领域对象的集合，它们作为一个整体进行操作和维护一致性。聚合根（Aggregate Root）是聚合的唯一入口点，外部只能通过聚合根访问聚合内部的对象。

**聚合的设计原则**

1. **一致性边界**：聚合确保内部对象始终保持一致的状态
2. **单一根**：每个聚合只有一个聚合根
3. **外部引用**：聚合只能通过ID引用其他聚合
4. **事务边界**：一个事务只能修改一个聚合
5. **最小化聚合**：聚合应该尽可能小，以提高性能

**用户聚合示例**

```csharp
// Auth.Domain/Aggregates/User.cs
public class User : Entity<int>, IAggregateRoot
{
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<UserRole> _userRoles = new();

    public string Email { get; private set; }
    public string FullName { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsEmailConfirmed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private User() { }

    public static User Create(string email, string fullName, string passwordHash)
    {
        Guard.ThrowIfNullOrWhiteSpace(email);
        Guard.ThrowIfNullOrWhiteSpace(fullName);
        Guard.ThrowIfNullOrWhiteSpace(passwordHash);

        var user = new User
        {
            Email = email,
            FullName = fullName,
            PasswordHash = passwordHash,
            IsEmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email));

        return user;
    }

    public void ConfirmEmail()
    {
        if (IsEmailConfirmed)
            throw new DomainException("Email is already confirmed.");

        IsEmailConfirmed = true;
        AddDomainEvent(new EmailConfirmedEvent(Id, Email));
    }

    public void UpdatePassword(string newPasswordHash)
    {
        Guard.ThrowIfNullOrWhiteSpace(newPasswordHash);
        PasswordHash = newPasswordHash;
        AddDomainEvent(new PasswordUpdatedEvent(Id));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
        AddDomainEvent(new RefreshTokenAddedEvent(Id, refreshToken.Token));
    }

    public void RevokeRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
        if (refreshToken is null)
            throw new NotFoundException("Refresh token not found.");

        refreshToken.Revoke();
        AddDomainEvent(new RefreshTokenRevokedEvent(Id, token));
    }

    public void AssignRole(UserRole role)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.RoleId))
            throw new DomainException("User already has this role.");

        _userRoles.Add(role);
        AddDomainEvent(new RoleAssignedEvent(Id, role.RoleId));
    }

    public void RemoveRole(int roleId)
    {
        var role = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (role is null)
            throw new NotFoundException("Role not found.");

        _userRoles.Remove(role);
        AddDomainEvent(new RoleRemovedEvent(Id, roleId));
    }

    public bool HasRole(string roleName)
    {
        return _userRoles.Any(ur => ur.Role.Name == roleName);
    }

    public bool IsValidRefreshToken(string token)
    {
        return _refreshTokens.Any(rt => rt.Token == token && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow);
    }
}
```

#### 4.5.3 实体（Entity）

**实体的特征**

1. **唯一标识**：每个实体都有唯一的标识符，标识符在生命周期内不变
2. **可变性**：实体的属性可以变化，但标识符不变
3. **生命周期**：实体有明确的生命周期，可以被创建、修改和删除
4. **相等性**：两个实体相等当且仅当它们的标识符相等

**实体基类实现**

```csharp
// BuildingBlocks.Domain/Entity.cs
public abstract class Entity<TId> : IEntity<TId>
{
    public TId Id { get; protected set; }
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity() { }

    protected Entity(TId id)
    {
        Id = id;
    }

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id is null || other.Id is null)
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }
}
```

**RefreshToken 实体示例**

```csharp
// Auth.Domain/Entities/RefreshToken.cs
public class RefreshToken : Entity<string>
{
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedReason { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(int userId, string token, int expiryDays)
    {
        return new RefreshToken
        {
            Id = token,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(expiryDays),
            IsRevoked = false
        };
    }

    public void Revoke(string? reason = null)
    {
        if (IsRevoked)
            throw new DomainException("Refresh token is already revoked.");

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
    }

    public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
}
```

#### 4.5.4 值对象（Value Object）

**值对象的特征**

1. **不可变性**：值对象创建后不能修改
2. **基于值相等**：两个值对象相等当且仅当所有属性值相等
3. **无标识**：值对象没有唯一标识符
4. **可替换性**：可以完全替换一个值对象而不影响业务逻辑

**值对象的优势**

- **简化代码**：减少对标识符的依赖
- **提高性能**：可以安全地缓存和共享
- **增强可读性**：通过类型表达业务概念
- **减少错误**：编译时类型检查

**Email 值对象示例**

```csharp
// Auth.Domain/ValueObjects/Email.cs
public class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        Guard.ThrowIfNullOrWhiteSpace(value);

        if (!IsValidEmail(value))
            throw new DomainException($"Invalid email address: {value}");

        return new Email(value.ToLowerInvariant().Trim());
    }

    private static bool IsValidEmail(string email)
    {
        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;
}
```

**值对象基类实现**

```csharp
// BuildingBlocks.Domain/ValueObject.cs
public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ValueObject);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
```

#### 4.5.5 领域事件（Domain Event）

**领域事件的用途**

1. **解耦业务逻辑**：通过事件驱动的方式解耦不同的业务操作
2. **实现最终一致性**：在分布式系统中保持数据的一致性
3. **审计和追踪**：记录重要的业务操作
4. **触发副作用**：在业务操作完成后执行其他操作

**领域事件基类实现**

```csharp
// BuildingBlocks.Domain/DomainEvent.cs
public abstract class DomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}
```

**用户相关领域事件示例**

```csharp
// Auth.Domain/Events/UserCreatedEvent.cs
public class UserCreatedEvent : DomainEvent
{
    public int UserId { get; }
    public string Email { get; }

    public UserCreatedEvent(int userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

// Auth.Domain/Events/EmailConfirmedEvent.cs
public class EmailConfirmedEvent : DomainEvent
{
    public int UserId { get; }
    public string Email { get; }

    public EmailConfirmedEvent(int userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

// Auth.Domain/Events/PasswordUpdatedEvent.cs
public class PasswordUpdatedEvent : DomainEvent
{
    public int UserId { get; }

    public PasswordUpdatedEvent(int userId)
    {
        UserId = userId;
    }
}

// Auth.Domain/Events/RefreshTokenAddedEvent.cs
public class RefreshTokenAddedEvent : DomainEvent
{
    public int UserId { get; }
    public string Token { get; }

    public RefreshTokenAddedEvent(int userId, string token)
    {
        UserId = userId;
        Token = token;
    }
}

// Auth.Domain/Events/RoleAssignedEvent.cs
public class RoleAssignedEvent : DomainEvent
{
    public int UserId { get; }
    public int RoleId { get; }

    public RoleAssignedEvent(int userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
```

#### 4.5.6 领域事件派发器

```csharp
// BuildingBlocks.EntityFramework/DomainEventDispatcher.cs
public class DomainEventDispatcher(IPublisher publisher) : ISaveChangesInterceptor
{
    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        var entities = dbContext.ChangeTracker
            .Entries<Entity<int>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }

        return result;
    }
}
```

#### 4.5.7 领域事件处理器示例

```csharp
// Auth.Application/Events/UserCreatedEventHandler.cs
public class UserCreatedEventHandler(
    IEmailService emailService,
    ILogger<UserCreatedEventHandler> logger) : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling UserCreatedEvent for user {UserId}", notification.UserId);

        var confirmationLink = GenerateConfirmationLink(notification.UserId);
        await emailService.SendEmailConfirmationAsync(
            notification.Email,
            confirmationLink,
            cancellationToken);

        logger.LogInformation("Confirmation email sent to {Email}", notification.Email);
    }

    private static string GenerateConfirmationLink(int userId)
    {
        var token = GenerateSecureToken();
        return $"https://auth.yourdomain.com/confirm-email?userId={userId}&token={token}";
    }

    private static string GenerateSecureToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
```

#### 4.5.8 DDD战术设计最佳实践

**聚合设计**
- 保持聚合尽可能小
- 确保聚合内的业务规则得到强制执行
- 通过聚合根控制所有对聚合的访问
- 避免聚合之间的直接引用

**实体设计**
- 使用有意义的标识符
- 封装业务逻辑在实体内部
- 确保实体的不变性约束
- 使用领域语言命名实体和属性

**值对象设计**
- 优先使用值对象而不是基本类型
- 确保值对象的不可变性
- 为值对象提供验证逻辑
- 重写相等性比较方法

**领域事件设计**
- 使用过去时态命名事件
- 事件应该包含必要的上下文信息
- 避免在事件中包含敏感信息
- 事件应该是不可变的

#### 4.5.9 设计优势

- **业务逻辑封装**：将业务规则封装在领域模型中
- **代码可读性**：使用领域语言提高代码可读性
- **可测试性**：领域模型易于单元测试
- **可维护性**：清晰的边界和职责分离
- **可扩展性**：易于添加新的业务规则
- **一致性保证**：聚合确保数据一致性

### 4.6 创建用户:用FluentValidation实现命令与逻辑

#### 4.6.1 FluentValidation 概述

FluentValidation 是一个 .NET 库，用于使用流畅的接口构建强类型的验证规则。它提供了以下优势：

- **强类型验证**：编译时类型检查，减少运行时错误
- **可读性强**：流畅的接口使验证规则易于理解
- **可重用性**：验证器可以在多个地方重用
- **分离关注点**：将验证逻辑与业务逻辑分离
- **丰富的验证规则**：内置多种验证规则，支持自定义验证
- **国际化支持**：支持多语言错误消息

#### 4.6.2 命令对象设计

在 CQRS 模式中，命令对象表示用户执行的意图。创建用户的命令对象包含用户的基本信息：

```csharp
// Auth.Application/Features/CreateUser/CreateUserCommand.cs
using MediatR;

namespace Auth.Application.Features.CreateUser;

public record CreateUserCommand : IRequest<IdResponse>, IAuditableAction
{
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public List<string> RoleNames { get; init; } = new();
    public int CreatedByUserId { get; set; }
}

public record IdResponse(int Id);
```

**命令对象设计要点**：

- 使用 `record` 类型确保不可变性
- 实现 `IRequest<TResponse>` 接口，符合 MediatR 规范
- 实现 `IAuditableAction` 接口，支持审计跟踪
- 使用初始化器属性，确保对象创建后不可修改
- 提供默认值，避免空引用异常

#### 4.6.3 验证器实现

FluentValidation 验证器继承自 `AbstractValidator<T>`，并使用流畅的接口定义验证规则：

```csharp
// Auth.Application/Features/CreateUser/CreateUserCommandValidator.cs
using FluentValidation;

namespace Auth.Application.Features.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
            .MaximumLength(128).WithMessage("Full name must not exceed 128 characters.")
            .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("Full name can only contain letters, spaces, hyphens and apostrophes.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.RoleNames)
            .NotEmpty().WithMessage("At least one role must be assigned.")
            .Must(x => x.All(role => !string.IsNullOrWhiteSpace(role)))
                .WithMessage("Role names cannot be empty or whitespace.")
            .Must(x => x.All(role => role.Length <= 64))
                .WithMessage("Role names must not exceed 64 characters.");

        RuleFor(x => x.CreatedByUserId)
            .GreaterThan(0).WithMessage("Created by user ID must be greater than zero.");
    }
}
```

#### 4.6.4 验证规则详解

**内置验证规则**

1. **NotEmpty()**：验证字符串不为空或空白
```csharp
RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
```

2. **EmailAddress()**：验证邮箱地址格式
```csharp
RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be a valid email address.");
```

3. **MaximumLength()**：验证字符串最大长度
```csharp
RuleFor(x => x.Email).MaximumLength(256).WithMessage("Email must not exceed 256 characters.");
```

4. **MinimumLength()**：验证字符串最小长度
```csharp
RuleFor(x => x.FullName).MinimumLength(2).WithMessage("Full name must be at least 2 characters.");
```

5. **Matches()**：使用正则表达式验证
```csharp
RuleFor(x => x.Password).Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.");
```

6. **Equal()**：验证两个属性相等
```csharp
RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
```

7. **GreaterThan()**：验证数值大于指定值
```csharp
RuleFor(x => x.CreatedByUserId).GreaterThan(0).WithMessage("Created by user ID must be greater than zero.");
```

**自定义验证规则**

使用 `Must()` 方法实现自定义验证逻辑：

```csharp
RuleFor(x => x.Email)
    .Must(BeUniqueEmail).WithMessage("Email already exists.");

private bool BeUniqueEmail(string email)
{
    return !_userRepository.EmailExistsAsync(email).Result;
}
```

**条件验证**

使用 `When()` 方法实现条件验证：

```csharp
RuleFor(x => x.PhoneNumber)
    .NotEmpty().WithMessage("Phone number is required.")
    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be a valid international phone number.")
    .When(x => x.ReceiveSmsNotifications)
    .WithMessage("Phone number is required when SMS notifications are enabled.");
```

**集合验证**

验证集合中的每个元素：

```csharp
RuleFor(x => x.RoleNames)
    .NotEmpty().WithMessage("At least one role must be assigned.")
    .Must(roles => roles.All(role => !string.IsNullOrWhiteSpace(role)))
        .WithMessage("Role names cannot be empty or whitespace.")
    .Must(roles => roles.All(role => role.Length <= 64))
        .WithMessage("Role names must not exceed 64 characters.");
```

#### 4.6.5 错误消息处理

**自定义错误消息**

使用 `WithMessage()` 方法提供自定义错误消息：

```csharp
RuleFor(x => x.Email)
    .NotEmpty().WithMessage("Email is required.")
    .EmailAddress().WithMessage("Email must be a valid email address.");
```

**参数化错误消息**

使用 `{PropertyName}` 占位符在错误消息中引用属性名：

```csharp
RuleFor(x => x.Email)
    .NotEmpty().WithMessage("{PropertyName} is required.")
    .EmailAddress().WithMessage("{PropertyName} must be a valid email address.");
```

**多语言错误消息**

使用 `WithLocalizedMessage()` 方法支持多语言：

```csharp
RuleFor(x => x.Email)
    .NotEmpty().WithLocalizedMessage(() => Messages.EmailRequired)
    .EmailAddress().WithLocalizedMessage(() => Messages.InvalidEmail);
```

#### 4.6.6 验证行为实现

MediatR 管道行为用于在命令处理前自动执行验证：

```csharp
// BuildingBlocks.MediatR/ValidationBehavior.cs
using FluentValidation;
using MediatR;

namespace BuildingBlocks.MediatR;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

#### 4.6.7 验证异常处理

自定义验证异常类，用于统一处理验证错误：

```csharp
// BuildingBlocks.Exceptions/ValidationException.cs
using FluentValidation.Results;

namespace BuildingBlocks.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException() : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public ValidationException(string propertyName, string errorMessage) : this()
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }
}
```

**全局异常处理中间件**

```csharp
// BuildingBlocks.AspNetCore/GlobalExceptionHandlerMiddleware.cs
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.AspNetCore;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Detail = exception.Message
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        context.Response.StatusCode = problemDetails.Status.Value;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
```

#### 4.6.8 注册验证服务

在依赖注入配置中注册 FluentValidation 和验证行为：

```csharp
// Auth.Application/DependencyInjection.cs
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(SetUserIdBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
```

#### 4.6.9 验证测试示例

```csharp
// Auth.Application.Tests/Features/CreateUser/CreateUserCommandValidatorTests.cs
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Auth.Application.Tests.Features.CreateUser;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new CreateUserCommand
        {
            Email = string.Empty,
            FullName = "John Doe",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            RoleNames = new List<string> { "User" }
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new CreateUserCommand
        {
            Email = "invalid-email",
            FullName = "John Doe",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            RoleNames = new List<string> { "User" }
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must be a valid email address.");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Does_Not_Meet_Requirements()
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FullName = "John Doe",
            Password = "password",
            ConfirmPassword = "password",
            RoleNames = new List<string> { "User" }
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter.");

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one digit.");

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one special character.");
    }

    [Fact]
    public void Should_Have_Error_When_Passwords_Do_Not_Match()
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FullName = "John Doe",
            Password = "Password123!",
            ConfirmPassword = "Password456!",
            RoleNames = new List<string> { "User" }
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
            .WithErrorMessage("Passwords do not match.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            FullName = "John Doe",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            RoleNames = new List<string> { "User" }
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
```

#### 4.6.10 设计优势

- **早期验证**：在命令处理前进行验证，避免无效数据进入业务逻辑
- **声明式验证**：使用声明式语法定义验证规则，提高代码可读性
- **可维护性**：验证规则集中管理，易于维护和修改
- **可测试性**：验证器易于单元测试
- **一致性**：统一的验证逻辑确保数据一致性
- **用户体验**：提供清晰的错误消息，改善用户体验
- **性能优化**：在管道中早期失败，避免不必要的处理

#### 4.6.11 最佳实践

1. **验证规则分离**：将验证规则与业务逻辑分离
2. **错误消息清晰**：提供清晰、具体的错误消息
3. **验证器命名**：使用 `CommandNameValidator` 命名约定
4. **组合验证器**：对于复杂对象，使用组合验证器
5. **自定义验证**：对于复杂业务规则，使用自定义验证
6. **测试覆盖**：为验证器编写全面的单元测试
7. **性能考虑**：避免在验证器中进行数据库查询
8. **国际化**：支持多语言错误消息

### 4.7 创建用户:基于DDD原则实现用户聚合

本节将详细介绍如何基于领域驱动设计（DDD）原则实现用户聚合，包括聚合根、值对象、领域事件等核心概念的具体实现。

#### 4.7.1 用户聚合根设计

用户聚合根是认证上下文的核心，负责维护用户数据的完整性和一致性。聚合根设计遵循以下原则：

1. **单一入口点**：所有对用户聚合的访问都必须通过聚合根
2. **一致性边界**：聚合根确保内部对象状态的一致性
3. **业务规则封装**：所有业务逻辑都封装在聚合根内部

**用户聚合根实现**：

```csharp
// Auth.Domain/Users/User.cs
public partial class User : IdentityUser<int>, IEntity
{
    public required string Name { get; set; }
    public required Gender Gender { get; set; }
    public HonorificTitle? Honorific { get; set; }
    public string? Position { get; set; }
    public string? CompanyName { get; set; }
    public string? Affiliation { get; set; }
    public ProfessionalProfile? ProfessionalProfile { get; set; }
    public string? PictureUrl { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }

    private List<UserRole> _userRoles = [];
    public virtual IReadOnlyList<UserRole> UserRoles => _userRoles;

    private List<RefreshToken> _refreshTokens = [];
    public virtual List<RefreshToken> RefreshTokens => _refreshTokens;
}
```

#### 4.7.2 工厂方法模式创建用户

使用工厂方法模式确保用户创建过程的业务规则得到强制执行：

```csharp
// Auth.Domain/Users/Behaviors/User.cs
public partial class User
{
    public static User Create(IUserCreationInfo userCreationInfo)
    {
        if (userCreationInfo.UserRoles is not { Count: > 0 })
            throw new ArgumentException("User must have at least one role.", nameof(userCreationInfo.UserRoles));

        var user = new User
        {
            UserName = userCreationInfo.Email,
            Email = userCreationInfo.Email,
            Name = userCreationInfo.Name,
            Gender = userCreationInfo.Gender,
            PhoneNumber = userCreationInfo.PhoneNumber,
            PictureUrl = userCreationInfo.PictureUrl,
            Honorific = HonorificTitle.FromEnum(userCreationInfo.Honorific),
            ProfessionalProfile = ProfessionalProfile.Create(
                userCreationInfo.Position,
                userCreationInfo.CompanyName,
                userCreationInfo.Affiliation
            ),
            _userRoles = userCreationInfo.UserRoles
                .Select(x => UserRole.Create(x))
                .ToList(),
        };

        return user;
    }
}
```

#### 4.7.3 值对象设计

值对象用于表示没有唯一标识的领域概念，确保不可变性和值相等性：

**专业档案值对象**：

```csharp
// Auth.Domain/Users/ValueObjects/ProfessionalProfile.cs
public class ProfessionalProfile : ValueObject
{
    public string? Position { get; private set; }
    public string? CompanyName { get; private set; }
    public string? Affiliation { get; private set; }

    private ProfessionalProfile() { }

    public static ProfessionalProfile Create(string? position, string? companyName, string? affiliation)
    {
        return new ProfessionalProfile
        {
            Position = position,
            CompanyName = companyName,
            Affiliation = affiliation
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Position;
        yield return CompanyName;
        yield return Affiliation;
    }
}
```

**尊称值对象**：

```csharp
// Auth.Domain/Users/ValueObjects/HonorificTitle.cs
public class HonorificTitle : ValueObject
{
    public string? Title { get; private set; }
    public Honorific? Honorific { get; private set; }

    private HonorificTitle() { }

    public static HonorificTitle? FromEnum(Honorific? honorific)
    {
        if (honorific == null) return null;

        return new HonorificTitle
        {
            Honorific = honorific,
            Title = honorific switch
            {
                Honorific.Mr => "Mr.",
                Honorific.Mrs => "Mrs.",
                Honorific.Ms => "Ms.",
                Honorific.Dr => "Dr.",
                Honorific.Prof => "Prof.",
                _ => null
            }
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Title;
        yield return Honorific;
    }
}
```

#### 4.7.4 领域事件实现

领域事件用于表示聚合内部发生的重要业务变化：

```csharp
// Auth.Domain/Users/Events/UserCreated.cs
public class UserCreated : DomainEvent
{
    public int UserId { get; }
    public string Email { get; }
    public string Name { get; }

    public UserCreated(int userId, string email, string name)
    {
        UserId = userId;
        Email = email;
        Name = name;
    }
}
```

#### 4.7.5 用户角色实体

用户角色实体表示用户与角色之间的关联关系：

```csharp
// Auth.Domain/Users/UserRole.cs
public class UserRole : Entity
{
    public int UserId { get; private set; }
    public UserRoleType Type { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? ExpiringDate { get; private set; }

    public virtual User User { get; private set; } = null!;

    private UserRole() { }

    public static UserRole Create(IUserRole userRole)
    {
        return new UserRole
        {
            Type = userRole.Type,
            StartDate = userRole.StartDate,
            ExpiringDate = userRole.ExpiringDate
        };
    }
}
```

#### 4.7.6 DDD聚合设计最佳实践

1. **聚合边界明确**：用户聚合包含用户基本信息、角色和刷新令牌
2. **业务规则内聚**：所有用户相关的业务规则都封装在聚合内部
3. **不变性保护**：通过私有setter和工厂方法保护聚合状态
4. **事件驱动**：重要状态变化通过领域事件通知外部系统
5. **值对象使用**：将相关属性组合成值对象，提高代码可读性

#### 4.7.7 聚合根接口设计

```csharp
// BuildingBlocks/Domain/IAggregateRoot.cs
public interface IAggregateRoot
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
```

通过以上设计，我们实现了符合DDD原则的用户聚合，确保了业务逻辑的完整性和数据的一致性。

### 4.8 创建用户:用EFCore和SQLServer实现持久层

本节将详细介绍如何使用EF Core和SQL Server实现用户聚合的持久化层，包括DbContext配置、实体映射、仓储模式和数据访问实现。

#### 4.8.1 AuthDbContext设计

认证上下文的DbContext负责管理用户相关的实体和关系：

```csharp
// Auth.Persistence/AuthDbContext.cs
public class AuthDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // 应用实体配置
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // 配置用户实体
        builder.Entity<User>(entity =>
        {
            // 配置复杂属性
            entity.ComplexProperty(e => e.ProfessionalProfile, profile =>
            {
                profile.Property(p => p.Position).HasMaxLength(100);
                profile.Property(p => p.CompanyName).HasMaxLength(200);
                profile.Property(p => p.Affiliation).HasMaxLength(200);
            });
            
            // 配置索引
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.RegistrationDate);
            
            // 配置关系
            entity.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();
                
            entity.HasMany(e => e.RefreshTokens)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
        });
    }
}
```

#### 4.8.2 实体配置类

使用实体配置类分离数据库映射逻辑，保持DbContext的简洁性：

```csharp
// Auth.Persistence/EntityConfigurations/UserEntityConfiguration.cs
public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // 配置表名
        builder.ToTable("Users");
        
        // 配置主键
        builder.HasKey(u => u.Id);
        
        // 配置属性
        builder.Property(u => u.Name)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();
            
        builder.Property(u => u.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);
            
        builder.Property(u => u.RegistrationDate)
            .IsRequired();
            
        builder.Property(u => u.LastLogin);
        
        // 配置复杂属性
        builder.ComplexProperty(u => u.ProfessionalProfile, profile =>
        {
            profile.Property(p => p.Position).HasMaxLength(100);
            profile.Property(p => p.CompanyName).HasMaxLength(200);
            profile.Property(p => p.Affiliation).HasMaxLength(200);
        });
        
        // 配置导航属性
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();
            
        builder.HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.UserId)
            .IsRequired();
    }
}
```

#### 4.8.3 依赖注入配置

在Program.cs中配置EF Core和SQL Server连接：

```csharp
// Auth.Persistence/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        
        services.AddDbContext<AuthDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        return services;
    }    
}
```

#### 4.8.4 ASP.NET Core Identity集成

集成ASP.NET Core Identity提供用户管理和认证功能：

```csharp
// Auth.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddIdentity<User, IdentityRole<int>>(options =>
    {
        // 密码策略
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        
        // 用户策略
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();
```

#### 4.8.5 仓储模式实现

使用通用仓储模式提供数据访问抽象层：

```csharp
// BuildingBlocks.EntityFramework/Repository.cs
public class Repository<TContext, TEntity> : IRepository<TEntity>
    where TContext : DbContext
    where TEntity : class, IEntity<int>
{
    public readonly TContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(TContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
    }

    public virtual Task<TEntity?> GetByIdAsync(int id)
        => Query().SingleOrDefaultAsync(x => x.Id == id);

    public virtual async Task<TEntity> AddAsync(TEntity entity)
        => (await DbSet.AddAsync(entity)).Entity;

    public virtual TEntity Update(TEntity entity)
        => DbSet.Update(entity).Entity;

    public virtual void Remove(TEntity entity)
        => DbSet.Remove(entity);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);
}
```

#### 4.8.6 数据库迁移

使用EF Core迁移管理数据库架构变更：

```bash
# 创建迁移
dotnet ef migrations add InitialCreate --project Auth.Persistence --startup-project Auth.API

# 应用迁移
dotnet ef database update --project Auth.Persistence --startup-project Auth.API
```

#### 4.8.7 性能优化策略

1. **索引优化**：为频繁查询的字段添加索引
2. **查询优化**：使用AsNoTracking()避免变更跟踪
3. **连接池**：配置SQL Server连接池
4. **批量操作**：使用批量插入和更新
5. **缓存策略**：实现缓存层减少数据库访问

### 4.9 创建用户:用Identity和FastEndpoints实现命令处理器

本节将详细介绍如何使用ASP.NET Core Identity和FastEndpoints实现创建用户的命令处理器，包括端点设计、验证、业务逻辑和响应处理。

#### 4.9.1 FastEndpoints端点设计

使用FastEndpoints创建简洁高效的API端点：

```csharp
// Auth.API/Features/CreateUser/CreateUserEndpoint.cs
[Authorize(Roles = Role.USERADMIN)]
[HttpPost("users")]
public class CreateUserEndpoint(UserManager<User> userManager) 
    : Endpoint<CreateUserCommand, CreateUserResponse>
{
    public override async Task HandleAsync(CreateUserCommand req, CancellationToken ct)
    {
        // 检查用户是否已存在
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user is not null)
            throw new BadRequestException($"User with email {req.Email} already exists.");

        // 使用工厂方法创建用户聚合
        user = Domain.Users.User.Create(req);

        // 使用Identity创建用户
        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new BadRequestException($"Unable to create user {req.Email} with {errorMessages}");
        }

        // 生成密码重置令牌
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        // 发布领域事件
        await PublishAsync(new UserCreated(user, resetToken));

        // 返回响应
        Response = new CreateUserResponse(req.Email, user.Id, resetToken);
    }
}
```

#### 4.9.2 命令对象设计

定义创建用户的命令对象：

```csharp
// Auth.API/Features/CreateUser/CreateUserCommand.cs
public class CreateUserCommand : IUserCreationInfo
{
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required Gender Gender { get; set; }
    public Honorific? Honorific { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PictureUrl { get; set; }
    public string? CompanyName { get; set; }
    public string? Position { get; set; }
    public string? Affiliation { get; set; }

    public required IReadOnlyList<UserRoleDto> UserRoles { get; init; } = [];

    IReadOnlyList<IUserRole> IUserCreationInfo.UserRoles => UserRoles;
}

public record UserRoleDto(UserRoleType Type, DateTime? StartDate, DateTime? ExpiringDate) : IUserRole;
```

#### 4.9.3 验证器实现

使用FluentValidation实现命令验证：

```csharp
// Auth.API/Features/CreateUser/CreateUserCommandValidator.cs
public class CreateUserCommandValidator : Validator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();

        RuleFor(x => x.Email).NotEmpty()
            .EmailAddress();

        RuleFor(x => x.UserRoles).NotEmpty()
            .Must((c, roles) => AreUserRoleDatesValid(roles))
            .WithMessage("Invalid roles");
    }

    private static bool AreUserRoleDatesValid(IReadOnlyList<UserRoleDto> roles)
    {
        return roles.All(x =>
            (!x.StartDate.HasValue || x.StartDate.Value.Date >= DateTime.UtcNow)
            &&
            (!x.ExpiringDate.HasValue || (x.StartDate ?? DateTime.UtcNow).Date < x.ExpiringDate.Value.Date)
        );
    }
}
```

#### 4.9.4 响应对象设计

定义创建用户的响应对象：

```csharp
// Auth.API/Features/CreateUser/CreateUserResponse.cs
public record CreateUserResponse(string Email, int UserId, string Token);
```

#### 4.9.5 领域事件处理

处理用户创建后的领域事件：

```csharp
// Auth.Application/Events/UserCreatedEventHandler.cs
public class UserCreatedEventHandler(
    IEmailService emailService,
    ILogger<UserCreatedEventHandler> logger) 
    : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling UserCreatedEvent for user {UserId}", notification.UserId);

        var confirmationLink = GenerateConfirmationLink(notification.UserId, notification.Token);
        await emailService.SendEmailConfirmationAsync(
            notification.Email,
            confirmationLink,
            cancellationToken);

        logger.LogInformation("Confirmation email sent to {Email}", notification.Email);
    }

    private static string GenerateConfirmationLink(int userId, string token)
    {
        return $"https://auth.yourdomain.com/confirm-email?userId={userId}&token={token}";
    }
}
```

#### 4.9.6 错误处理策略

1. **验证错误**：在验证器中捕获并返回详细错误信息
2. **业务错误**：使用自定义异常和异常中间件处理
3. **数据库错误**：使用重试策略和连接恢复机制
4. **网络错误**：实现超时和重试机制

#### 4.9.7 安全考虑

1. **授权检查**：使用[Authorize]属性保护端点
2. **输入验证**：防止SQL注入和XSS攻击
3. **数据加密**：敏感数据在传输和存储时加密
4. **审计日志**：记录所有用户创建操作

#### 4.9.8 性能优化

1. **异步操作**：所有数据库操作都使用异步方法
2. **批量处理**：支持批量用户创建
3. **缓存策略**：缓存用户查询结果
4. **连接复用**：使用连接池减少连接开销

通过以上设计，我们实现了高效、安全、可扩展的用户创建功能，结合了DDD、CQRS和垂直切片架构的最佳实践。

### 4.10 实现邮件服务模块

邮件服务模块为系统提供邮件发送功能，支持用户注册确认、密码重置等关键业务流程。

#### 4.10.1 邮件服务接口设计

定义统一的邮件服务接口，支持不同类型的邮件发送：

```csharp
// EmailService.Contracts/IEmailService.cs
public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken);
    Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken);
}
```

#### 4.10.2 SMTP邮件服务实现

使用MailKit库实现SMTP邮件发送功能：

```csharp
// EmailService.Smtp/SmtpEmailService.cs
public class SmtpEmailService : IEmailService
{
    public async Task<bool> SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken)
    {
        var message = emailMessage.ToMailKitMessage();
        
        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(
            _emailOptions.Smtp.Host,
            _emailOptions.Smtp.Port,
            _emailOptions.Smtp.UseSSL,
            cancellationToken
        );
        
        await smtpClient.AuthenticateAsync(
            _emailOptions.Smtp.Username,
            _emailOptions.Smtp.Password,
            cancellationToken
        );
        
        await smtpClient.SendAsync(message, cancellationToken);
        return true;
    }
}
```

#### 4.10.3 邮件配置选项

支持灵活的邮件配置，包括SMTP服务器设置：

```csharp
// EmailService.Smtp/EmailOptions.cs
public class EmailOptions
{
    [Required]
    public string EmailServiceProvider { get; init; }
    [Required]
    public string EmailFromAddress { get; init; }
    [Required]
    public Smtp Smtp { get; set; }
}

public class Smtp
{
    [Required]
    public string Host { get; init; }
    [Required]
    public int Port { get; init; }
    [Required]
    public string Username { get; init; }
    [Required]
    public string Password { get; init; }
    public bool UseSSL { get; set; } = true;
}
```

#### 4.10.4 邮件服务注册

使用依赖注入注册邮件服务：

```csharp
// EmailService.Smtp/MailServiceRegistration.cs
public static class MailServiceRegistration
{
    public static IServiceCollection AddSmtpEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAndValidateOptions<EmailOptions>(configuration);
        services.AddSingleton<IEmailService, SmtpEmailService>();
        return services;    
    }
}
```

### 4.11 创建用户:用FastEndpoints实现确认邮件

#### 4.11.1 用户创建事件处理器

当用户创建成功后，自动发送确认邮件：

```csharp
// Auth.API/Features/CreateUser/SendConfirmationEmailOnUserCreatedHandler.cs
public class SendConfirmationEmailOnUserCreatedHandler : IEventHandler<UserCreated>
{
    public async Task HandleAsync(UserCreated eventModel, CancellationToken ct)
    {
        var url = httpContextAccessor.HttpContext?.Request.BaseUrl()
            .AppendPathSegment("password")
            .SetQueryParams(new { eventModel.ResetToken });

        var emailMessage = buildConfirmationEmail(eventModel.User, url, emailOptions.Value.EmailFromAddress);
        await emailService.SendEmailAsync(emailMessage, ct);
    }

    private EmailMessage buildConfirmationEmail(User user, string resetLink, string fromEmailAddress)
    {
        const string confirmationEmail = 
            "Dear {0},<br/>An account has been created for you.<br/>Please set your password using the following URL: <br/>{1}";
        return new EmailMessage(
            "Your Account Has Been Created - Set Your Password",
            new Content(ContentType.Html, string.Format(confirmationEmail, user.Name, resetLink)),
            new EmailAddress("Articles", fromEmailAddress),
            [new EmailAddress(user.Name, user.Email)]);
    }
}
```

#### 4.11.2 邮件确认链接生成

生成安全的密码设置链接：

```csharp
private static string GenerateConfirmationLink(int userId, string token)
{
    return $"https://auth.yourdomain.com/confirm-email?userId={userId}&token={token}";
}
```

### 4.12 添加依赖注入配置与模块

#### 4.12.1 API层依赖注入配置

在API层统一配置所有依赖服务：

```csharp
// Auth.API/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAndValidateOptions<EmailOptions>(configuration);
        
        services
            .AddFastEndpoints()
            .AddSwaggerDocument()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddJwtIdentity(configuration);

        services.AddSmtpEmailService(configuration);
        return services;
    }

    public static IServiceCollection AddJwtIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<User>(options =>
        {
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
        }).AddRoles<Role>()
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddSignInManager<SignInManager<User>>()
        .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
        });
        return services;
    }
}
```

#### 4.12.2 应用层配置

应用层服务注册：

```csharp
// Auth.Application/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddScoped<TokenFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}
```

### 4.13 登录:用FluentValidation实现命令与逻辑

#### 4.13.1 登录命令验证器

使用FluentValidation验证登录命令的合法性：

```csharp
// Auth.API/Features/Login/LoginCommandValidator.cs
public class LoginCommandValidator : Validator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password).NotEmpty();
    }    
}
```

#### 4.13.2 登录命令对象

定义登录命令的数据结构：

```csharp
// Auth.API/Features/Login/LoginCommand.cs
public record LoginCommand(string Email, string Password);

public record LoginResponse(string Email, string AccessToken, string RefreshToken);
```

### 4.14 登录:用Identity和FastEndpoints实现命令与逻辑

#### 4.14.1 登录端点实现

使用FastEndpoints和ASP.NET Core Identity实现登录功能：

```csharp
// Auth.API/Features/Login/LoginEndpoint.cs
[AllowAnonymous]
[HttpPost("login")]
public class LoginEndpoint : Endpoint<LoginCommand, LoginResponse>
{
    public override async Task HandleAsync(LoginCommand req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user is null)
            throw new BadRequestException($"User {req.Email} not found");

        var result = await signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!result.Succeeded)
            throw new BadRequestException($"Invalid credentials for user {req.Email}");

        var userRoles = await userManager.GetRolesAsync(user);
        var jwtToken = tokenFactory.GenerateAccessToken(user.Id.ToString(), user.Name, req.Email, userRoles, Array.Empty<Claim>());
        var refreshToken = tokenFactory.GenerateRefreshToken(HttpContext.GetClientIpAddress());

        user.AddRefreshToken(refreshToken);
        await userManager.UpdateAsync(user);

        await Send.OkAsync(new LoginResponse(req.Email, jwtToken, refreshToken.Token));
    }
}
```

#### 4.14.2 登录流程

1. **用户验证**：通过Email查找用户，验证密码
2. **角色获取**：获取用户的角色信息
3. **令牌生成**：生成访问令牌和刷新令牌
4. **令牌持久化**：将刷新令牌保存到数据库
5. **响应返回**：返回登录结果和令牌信息

### 4.15 登录:生成令牌与刷新令牌并持久化

#### 4.15.1 令牌工厂实现

使用JWT标准生成安全的访问令牌：

```csharp
// Auth.Application/TokenFactory.cs
public class TokenFactory
{
    public string GenerateAccessToken(string userId, string fullName, string email,
        IEnumerable<string> roles, IEnumerable<Claim> additionalClaims)
    {
        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Email, email),
            }
            .Concat(roles.Select(x => new Claim(ClaimTypes.Role, x)))
            .Concat(additionalClaims);

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            notBefore: DateTime.UtcNow,
            expires: _jwtOptions.Expiration,
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
```

#### 4.15.2 刷新令牌生成

生成安全的刷新令牌并关联用户：

```csharp
public RefreshToken GenerateRefreshToken(string ipAddress)
{
    return new RefreshToken
    {
        Token = RandomTokenString(),
        Expires = DateTime.UtcNow.AddDays(7),
        Created = DateTime.UtcNow,
        CreatedByIp = ipAddress
    };
}

private static string RandomTokenString()
{
    using var rngCryptoServiceProvider = RandomNumberGenerator.Create();
    var randomBytes = new byte[40];
    rngCryptoServiceProvider.GetBytes(randomBytes);
    return BitConverter.ToString(randomBytes).Replace("-", "");
}
```

#### 4.15.3 刷新令牌持久化

将刷新令牌保存到用户实体中：

```csharp
// Auth.Domain/Users/User.cs
public partial class User : IdentityUser<int>, IEntity
{
    private List<RefreshToken> _refreshTokens = [];
    public virtual List<RefreshToken> RefreshTokens => _refreshTokens;

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }
}
```

#### 4.15.4 令牌安全策略

1. **访问令牌**：有效期15分钟，包含用户身份和权限信息
2. **刷新令牌**：有效期7天，仅用于获取新的访问令牌
3. **令牌撤销**：支持主动撤销刷新令牌
4. **安全存储**：刷新令牌存储在HttpOnly Cookie中防止XSS攻击

通过以上实现，我们构建了完整的认证授权系统，支持安全的用户登录、令牌管理和邮件确认功能。

