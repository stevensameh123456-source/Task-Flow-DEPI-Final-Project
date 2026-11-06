# TaskFlow -- Backend & Database Core Architecture

> **Project:** TaskFlow\
> **Module owner:** Member 1 -- Backend & Database Core Architecture\
> **Application architecture:** ASP.NET Core MVC\
> **Target framework:** .NET 8\
> **ORM:** Entity Framework Core 8\
> **Database:** Microsoft SQL Server

------------------------------------------------------------------------

## 1. Module Scope

This document describes the backend foundation implemented by Member 1.
The scope covers solution structure, domain entities, database
relationships, EF Core configuration, migrations, ASP.NET Core Identity
infrastructure, and data-access patterns.

### Included

-   Solution and project setup.
-   Domain entities and enums.
-   EF Core `AppDbContext`, SQL Server integration, and migrations.
-   Fluent API mapping through separate `IEntityTypeConfiguration<T>`
    classes.
-   ASP.NET Core Identity with integer keys and default role seeding.
-   Generic Repository and Unit of Work patterns.
-   Repository CRUD verification.

### Outside this module's scope

Business workflows and application-specific task operations belong to
the relevant business-logic module. This includes task
creation/assignment rules, workflow transitions, approval processing,
role-assignment workflows, reporting calculations, and business
authorization policies.

No Azure deployment is included in this module.

------------------------------------------------------------------------

## 2. Solution Architecture

The solution is divided into three projects:

  -----------------------------------------------------------------------
  Project                             Responsibility
  ----------------------------------- -----------------------------------
  `TaskFlow.Web`                      ASP.NET Core MVC application,
                                      startup configuration, middleware,
                                      Identity configuration, and
                                      dependency injection registration.

  `TaskFlow.Data`                     Domain entities, database context,
                                      EF Core configurations and
                                      migrations, repository interfaces
                                      and implementations, and Unit of
                                      Work.

  `TaskFlow.Services`                 Service/business-logic layer used
                                      by the application. Business
                                      workflows are outside Member 1's
                                      implementation scope.
  -----------------------------------------------------------------------

### Data project structure

``` text
TaskFlow.Data/
├── Configurations/
│   ├── ApplicationUserConfiguration.cs
│   ├── ProjectConfiguration.cs
│   ├── TeamConfiguration.cs
│   ├── TaskItemConfiguration.cs
│   ├── TaskCommentConfiguration.cs
│   ├── TaskAttachmentConfiguration.cs
│   ├── TaskSubmissionConfiguration.cs
│   ├── TaskApprovalConfiguration.cs
│   ├── NotificationConfiguration.cs
│   ├── ReportConfiguration.cs
│   ├── EmployeePerformanceConfiguration.cs
│   └── ActivityLogConfiguration.cs
├── Context/
│   └── AppDbContext.cs
├── Entities/
├── Interfaces/
│   ├── IGenericRepository.cs
│   └── IUnitOfWork.cs
├── Migrations/
├── Repositories/
│   └── GenericRepository.cs
└── UnitOfWork/
    └── UnitOfWork.cs
```

The exact folder contents should be kept in sync with the repository as
the project evolves.

------------------------------------------------------------------------

## 3. Technology Stack

-   C#
-   .NET 8 / ASP.NET Core MVC
-   Entity Framework Core 8
-   SQL Server
-   ASP.NET Core Identity
-   Dependency Injection (built into ASP.NET Core)

The database used during development is named `TaskFlowDb`. The
connection string is configured through application configuration under
`DefaultConnection`; connection details and credentials must not be
committed to source control.

------------------------------------------------------------------------

## 4. Domain Model

### 4.1 Entities

  -----------------------------------------------------------------------
  Entity                              Purpose
  ----------------------------------- -----------------------------------
  `ApplicationUser`                   Application user based on
                                      `IdentityUser<int>`, with full
                                      name, activation state, creation
                                      date, and optional team membership.

  `Project`                           Project information, status, dates,
                                      and assigned project manager.

  `Team`                              Team belonging to a project, with a
                                      required team leader and employee
                                      collection.

  `TaskItem`                          Task details, dates, status,
                                      priority, team, assigned employee,
                                      and creator.

  `TaskComment`                       Comment written by a user on a
                                      task.

  `TaskAttachment`                    File metadata and uploader
                                      information associated with a task.

  `TaskSubmission`                    Employee submission associated with
                                      a task, including description,
                                      timestamp, and submission status.

  `TaskApproval`                      Review/approval record associated
                                      with a task and reviewer.

  `Notification`                      User notification with type, read
                                      state, and creation timestamp.

  `Report`                            Report metadata and the user who
                                      generated it.

  `EmployeePerformance`               Employee performance snapshot and
                                      calculated task counters/rate.

  `ActivityLog`                       User activity record with action
                                      and target entity information.
  -----------------------------------------------------------------------

### 4.2 Enumerations

The domain model includes these enums:

-   `ProjectStatus`
-   `TaskStatus`
-   `TaskPriority`
-   `SubmissionStatus`
-   `ApprovalStatus`
-   `NotificationType`

Enum values are stored through EF Core's default enum mapping unless a
specific conversion is configured later.

### 4.3 Key relationships

-   A user may belong to zero or one team (`ApplicationUser.TeamId` is
    nullable).
-   A project may contain multiple teams; each team belongs to one
    project.
-   A team has one required leader.
-   A team may contain multiple employees.
-   A task belongs to one team.
-   A task is assigned to one employee and records the user who created
    it.
-   A task may have multiple comments, attachments, submissions, and
    approval records.
-   Comments, attachments, submissions, approvals, notifications,
    reports, performance records, and activity logs reference their
    associated users.
-   Identity roles and user-role assignments are managed by ASP.NET Core
    Identity (`AspNetRoles` and `AspNetUserRoles`); no duplicate
    user-role assignment entity is introduced.

### 4.4 Relationship and business-rule boundary

The database model enforces required foreign keys and configured delete
behavior. Rules such as requiring the assigned employee to belong to the
task's team, restricting task creation to team leaders, or requiring
approval before completion are business/workflow rules and are not
implemented by these EF Core configurations.

------------------------------------------------------------------------

## 5. Entity Framework Core and Database

### 5.1 DbContext

`AppDbContext` inherits from:

``` csharp
IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
```

It exposes DbSets for the domain entities and calls the base
`OnModelCreating` implementation before applying the project's
configuration classes.

All entity configurations are discovered from the `TaskFlow.Data`
assembly using:

``` csharp
builder.ApplyConfigurationsFromAssembly(
    typeof(AppDbContext).Assembly
);
```

### 5.2 Separate entity configurations

Each domain entity has a dedicated class implementing
`IEntityTypeConfiguration<T>`. This keeps property constraints and
relationship mappings out of `AppDbContext` and makes the persistence
model easier to maintain.

The configurations define required properties, maximum string lengths,
foreign keys, relationship cardinality, indexes where specified, and
delete behavior.

### 5.3 Delete behavior

Relationships are configured with `DeleteBehavior.Restrict` where
specified. In SQL Server, EF Core migrations represent this as
`ON DELETE NO ACTION`; this is expected SQL Server behavior for the
configured restriction.

### 5.4 Indexes

EF Core creates indexes for foreign-key properties by convention where
appropriate. The Identity model also includes its standard
username/email indexes, and the application model explicitly configures
a unique filtered index on `ApplicationUser.NormalizedEmail` named
`EmailIndex`.

Additional composite or query-specific indexes should be introduced
based on actual query patterns and measured performance, rather than
added speculatively.

### 5.5 Migrations

Migrations are maintained in `TaskFlow.Data/Migrations`. The domain and
Identity schema migrations were applied to the development SQL Server
database.

During the configuration refactoring, a temporary migration named
`VerifyConfigurationRefactoring` was generated. Its `Up()` and `Down()`
methods were empty, confirming that the refactoring did not introduce
model/schema differences. The temporary migration was then removed, and
the model snapshot was reverted.

Do not run `Update-Database` or create new migrations without reviewing
the generated migration first.

------------------------------------------------------------------------

## 6. ASP.NET Core Identity

Identity is configured with `ApplicationUser` and `IdentityRole<int>`,
using integer primary keys and EF Core stores backed by `AppDbContext`.

### Password policy

-   Minimum length: 8 characters.
-   Requires a digit.
-   Requires uppercase and lowercase letters.
-   Requires a non-alphanumeric character.

### User and lockout settings

-   Unique email is required.
-   Lockout duration: 15 minutes.
-   Maximum failed access attempts: 5.
-   Lockout is enabled for new users.
-   Confirmed email is not required by the current development
    configuration.

### Authentication cookie

-   Login path: `/Account/Login`
-   Access-denied path: `/Account/AccessDenied`
-   Expiration: 8 hours
-   Sliding expiration: enabled

### Default roles

The application seeds these roles if they do not already exist:

-   `Employee`
-   `TeamLeader`
-   `ProjectManager`
-   `HR`
-   `CEO`

Role seeding is performed through `RoleManager<IdentityRole<int>>`
during application startup. Business rules for assigning roles to users
remain outside this infrastructure scope.

------------------------------------------------------------------------

## 7. Repository and Unit of Work

### 7.1 Generic Repository

`IGenericRepository<T>` defines common data-access operations:

-   `GetAllAsync()`
-   `GetAllNoTrackingAsync()`
-   `GetByIdAsync(params object[] keyValues)`
-   `AddAsync(T entity)`
-   `Update(T entity)`
-   `Delete(T entity)`
-   `ExistsAsync(Expression<Func<T, bool>> predicate)`

`GenericRepository<T>` implements these operations using the EF Core
`DbSet<T>`.

`GetAllAsync()` returns tracked entities. `GetAllNoTrackingAsync()` uses
`AsNoTracking()` and is intended for read-only retrieval where change
tracking is not needed.

Repository methods that add, update, or delete entities do not
independently commit the transaction. Changes are persisted through the
Unit of Work.

### 7.2 Unit of Work

`IUnitOfWork` exposes:

-   `Repository<T>()`
-   `SaveChangesAsync()`
-   `IDisposable`

`UnitOfWork` provides a repository instance per entity type within the
Unit of Work and shares the same scoped `AppDbContext` across those
repositories. `SaveChangesAsync()` commits all pending tracked changes
through that context.

The Unit of Work is registered as scoped in dependency injection. The
scoped dependency-injection container manages the lifetime of
`AppDbContext`; the Unit of Work does not manually dispose the context.

### 7.3 Dependency injection

The application registers the repository and Unit of Work using scoped
lifetimes:

``` csharp
builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>)
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

The generic repository registration is available for direct injection;
repositories obtained through `IUnitOfWork` share the Unit of Work's
context.

------------------------------------------------------------------------

## 8. Verification and Testing

The following checks were completed during development:

  -----------------------------------------------------------------------
  Check                               Result
  ----------------------------------- -----------------------------------
  SQL Server connection and applied   Successful
  migrations

  Identity schema and default role    Configured
  seeding

  Dependency injection resolution for Successful
  Unit of Work

  Generic repository resolution       Successful

  Repository `GetAllAsync()` and      Successful
  `ExistsAsync()` smoke test

  Repository CRUD test (Create, Read, Passed
  Update, Delete)

  Existence check after deletion      Passed

  Test transaction rollback           Passed

  Rebuild of `TaskFlow.Data`,         Successful
  `TaskFlow.Services`, and
  `TaskFlow.Web` after configuration
  refactoring

  Rebuild after Unit of Work and      Successful
  repository updates

  Configuration-refactoring migration Empty migration; removed afterward
  comparison
  -----------------------------------------------------------------------

The CRUD test used a temporary Identity role record inside a database
transaction and rolled the transaction back. The test data was not
intended to remain in the database.

A successful build verifies compilation; it does not replace integration
tests against all production workflows or authorization scenarios.

------------------------------------------------------------------------

## 9. Running the Project

1.  Install the .NET 8 SDK and SQL Server.
2.  Configure `DefaultConnection` in the local application
    configuration.
3.  Set `TaskFlow.Web` as the startup project.
4.  Ensure `TaskFlow.Data` is selected as the migrations/default project
    when using Package Manager Console.
5.  Apply the reviewed migrations to the intended development database.
6.  Run `TaskFlow.Web`.

Example Package Manager Console command:

``` powershell
Update-Database
```

Run this only when the database needs to be brought up to the reviewed
migration state. Do not commit local credentials or production
connection strings.

------------------------------------------------------------------------

## 10. Security and Repository Hygiene

Before pushing to GitHub:

-   Keep passwords, SQL credentials, API keys, and private connection
    strings out of source control.
-   Use user secrets or environment variables for local/deployment
    secrets.
-   Review `.gitignore` and exclude `bin/`, `obj/`, IDE-local files, and
    other generated artifacts as appropriate.
-   Do not commit test credentials or real user data.
-   Review migrations before applying them to shared or production
    databases.

------------------------------------------------------------------------

## 11. Handover and Ownership Boundaries

Member 1 provides the persistence and identity foundation for the
application:

-   Entity model and relationships.
-   SQL Server/EF Core database context and migrations.
-   Entity configuration classes.
-   Identity infrastructure and base role seeding.
-   Generic Repository and Unit of Work.
-   Database/repository verification.

The business-logic module can build on these foundations to implement
task operations, assignment validation, workflow transitions, approval
processing, notifications, reporting calculations, and business
authorization policies.

------------------------------------------------------------------------

## 12. Current Status

The Member 1 backend and database foundation has been implemented and
successfully rebuilt across all three solution projects. Repository CRUD
operations were tested successfully, and the EF Core configuration
refactoring was verified to produce no schema changes.

Documentation should be updated if the implementation, database model,
Identity settings, or project structure changes.
