# SimRMS - Risk Management System - Claude Development Guide

## Project Overview
SimRMS is a comprehensive Risk Management System built with .NET 8, following Clean Architecture principles with a focus on simplicity, maintainability, and performance.

## Architecture & Design Principles

### Clean Architecture Structure
```
src/
├── SimRMS.Domain/           # Core business entities and domain logic
├── SimRMS.Application/      # Use cases, DTOs, interfaces, validators
├── SimRMS.Infrastructure/   # Data access, external services
├── SimRMS.Shared/          # Common models, constants, utilities, extensions
└── SimRMS.WebAPI/          # Controllers, middleware, configuration
```

### Layer Responsibilities

#### SimRMS.Shared
- **Constants** (`AppConstants`, `ActionTypeEnum`, etc.) - Application-wide constants
- **Common Models** (`ApiResponse<T>`, `PagedResult<T>`, `BulkOperationResult`) - Shared data structures
- **Extensions** (`ExpressionExtensions`) - Utility extension methods
- **Cross-cutting Concerns** - Utilities used across all layers

#### Core Development Rules

1. **ONLY Use Stored Procedures** - Never write inline SQL. All database operations must use stored procedures.
2. **Generic Repository Pattern** - Use the existing `IGenericRepository` for all data access operations.
3. **Single Service Per Section** - Create one service interface/implementation per business area.
4. **Consolidated Validators** - Use single validation file per section with extension methods for reusability.
5. **Clean Architecture** - Maintain strict separation of concerns across layers.
6. **Shared Components** - Use `SimRMS.Shared` for common models, constants, and utilities.

## Database Integration

### Stored Procedure Conventions
- **CRUD Operations**: Use pattern `LB_SP_Crud{EntityName}` with Action parameter (1=Create, 2=Update, 3=Delete)
- **List Operations**: Use pattern `LB_SP_Get{EntityName}List` with pagination support
- **Single Record**: Use pattern `LB_SP_Get{EntityName}_By{KeyField}`
- **OUTPUT Parameters**: Use `@RowsAffected`, `@StatusCode`, `@StatusMsg`, `@TotalCount` for result handling

### Generic Repository Usage
```csharp
// The repository automatically handles OUTPUT parameters
var result = await _repository.ExecuteAsync("LB_SP_CrudEntity", parameters, isStoredProcedure: true);

// For complex OUTPUT scenarios
var result = await _repository.ExecuteWithOutputAsync("StoredProcedure", parameters);
var statusCode = result.GetOutputValue<int>("StatusCode");
```

## Code Patterns & Standards

### Entity Structure
- Inherit from `BaseEntity` for audit fields
- Use database table names for entity classes (e.g., `MstCoBrch`)
- Apply proper data annotations for validation and constraints

### DTO Pattern
```csharp
// Keep DTOs simple and focused
public class EntityDto
{
    public string Id { get; set; }
    public string Description { get; set; }
    // Only include fields needed for the specific operation
}
```

### Service Layer Pattern
```csharp
public interface IBusinessAreaService
{
    Task<PagedResult<EntityDto>> GetListAsync(int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken);
    Task<EntityDto?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<EntityDto> CreateAsync(CreateEntityRequest request, CancellationToken cancellationToken);
    Task<EntityDto> UpdateAsync(string id, UpdateEntityRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string id, DeleteEntityRequest request, CancellationToken cancellationToken);
}
```

### Controller Pattern
```csharp
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class BusinessAreaController : BaseController
{
    // Use meaningful controller names, not database table names
    // Follow RESTful conventions
    // Keep actions simple and delegate to services
}
```

### Validation Pattern - Single File Approach
```csharp
// Create one validator file per business area
public static class EntityValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidEntityCode<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().MaximumLength(10).Matches("^[A-Z0-9]+$");
    }
}

// Multiple validators in same file
public class CreateEntityRequestValidator : AbstractValidator<CreateEntityRequest> { }
public class UpdateEntityRequestValidator : AbstractValidator<UpdateEntityRequest> { }
public class DeleteEntityRequestValidator : AbstractValidator<DeleteEntityRequest> { }
```

## Development Workflow

### Adding New Business Section
1. **Create Entity** in `Domain/Entities/` (use database table name)
2. **Create DTOs** in `Application/Models/DTOs/` (descriptive names)
3. **Create Requests** in `Application/Models/Requests/`
4. **Create Single Validator File** in `Application/Validators/` with extension methods
5. **Create Service Interface** in `Application/Interfaces/Services/`
6. **Implement Service** in `Infrastructure/Services/`
7. **Create Controller** in `WebAPI/Controllers/V1/` (meaningful name)
8. **Register Services** in DI containers
9. **Add Constants** to `Shared/Constants/` if needed (permissions, roles, etc.)

### Shared Project Usage Guidelines

#### What Goes in SimRMS.Shared
- ✅ **Constants** - Application-wide constants (permissions, roles, headers)
- ✅ **Common Models** - Models used across multiple layers (`ApiResponse<T>`, `PagedResult<T>`)
- ✅ **Extension Methods** - Utility extensions used across layers
- ✅ **Enums** - Shared enumerations (`ActionTypeEnum`)
- ✅ **Common Exceptions** - Base exception classes if needed

#### What Doesn't Go in SimRMS.Shared
- ❌ **Business Logic** - Keep in Domain/Application layers
- ❌ **Data Access** - Keep in Infrastructure layer
- ❌ **Web-specific Code** - Keep in WebAPI layer
- ❌ **Domain Entities** - Keep in Domain layer

### Key Implementation Rules

#### Database Access
- ✅ Use stored procedures exclusively
- ✅ Handle OUTPUT parameters for result status
- ✅ Use generic repository for all data operations
- ❌ Never write inline SQL
- ❌ Don't use Entity Framework or raw SQL

#### Service Layer
- ✅ Create one service per business area
- ✅ Use meaningful service names (e.g., `BrokerBranchService` not `MstCoBrchService`)
- ✅ Implement business validation and rules
- ✅ Use transactions for complex operations
- ❌ Don't put business logic in controllers
- ❌ Avoid repository pattern per entity

#### Validation
- ✅ Consolidate validators in single file per business area
- ✅ Use extension methods for reusable validation rules
- ✅ Validate at service layer, not just controller
- ❌ Don't create separate files for each validator
- ❌ Don't duplicate validation logic

#### Error Handling
- ✅ Use domain-specific exceptions
- ✅ Let middleware handle error responses
- ✅ Provide clear, user-friendly error messages
- ❌ Don't catch and rethrow without adding value
- ❌ Don't expose internal error details

## Authentication & Security

### Custom Token Authentication
- Uses `TokenAuthenticationMiddleware` for actual authentication
- `TokenAuthenticationSchemeHandler` provides ASP.NET Core integration
- Supports handshake tokens and user tokens
- Handles authentication challenges properly

### Authorization
- Use policy-based authorization
- Implement permission-based access control
- Follow principle of least privilege

## Performance Guidelines

### Database Performance
- Use pagination for list operations
- Implement proper indexing strategies
- Monitor stored procedure execution plans
- Use OUTPUT parameters instead of separate queries

### Caching Strategy
- Cache frequently accessed reference data
- Use distributed caching for scalability
- Implement cache invalidation strategies

## Testing Approach

### Unit Testing
- Test business logic in services
- Mock repository dependencies
- Focus on edge cases and validation

### Integration Testing
- Test complete workflows
- Use test database with realistic data
- Test stored procedure interactions

## Common Pitfalls to Avoid

1. **Over-engineering** - Keep solutions simple and focused
2. **Inline SQL** - Always use stored procedures
3. **Multiple Repository Classes** - Use the generic repository
4. **Separate Validator Files** - Consolidate per business area
5. **Business Logic in Controllers** - Keep controllers thin
6. **Missing Error Handling** - Always handle exceptions properly
7. **Ignoring OUTPUT Parameters** - Use them for proper result handling

## Development Tools & Commands

### Build & Test
```bash
dotnet build                    # Build solution
dotnet test                     # Run tests
dotnet run --project SimRMS.WebAPI  # Run application
```

### Database Operations
- Use SQL Server Management Studio for stored procedure development
- Test procedures independently before integration
- Document procedure parameters and expected outputs

## Code Quality Standards

### Naming Conventions
- **Entities**: Use database table names (e.g., `MstCoBrch`)
- **DTOs**: Use descriptive names (e.g., `BrokerBranchDto`)
- **Services**: Use business-meaningful names (e.g., `BrokerBranchService`)
- **Controllers**: Use descriptive names (e.g., `BrokerBranchController`)

### Documentation
- Add XML documentation for public APIs
- Include purpose and usage examples
- Document complex business rules
- Maintain this CLAUDE.md file with updates

## Deployment Considerations

### Environment Configuration
- Use appsettings for environment-specific settings
- Implement health checks for dependencies
- Configure logging appropriately for each environment

### Monitoring
- Implement structured logging
- Monitor database performance
- Track API response times
- Set up alerting for critical errors

---

## Quick Reference for Claude Code

When working on SimRMS:

1. **Always check this file first** for current patterns and rules
2. **Follow the established architecture** - don't introduce new patterns
3. **Use the generic repository** for all data access
4. **Create consolidated validators** with extension methods
5. **Use stored procedures exclusively** for database operations
6. **Keep services focused** on single business areas
7. **Maintain clean architecture** boundaries
8. **Test thoroughly** after any changes
9. **Update this file** when introducing new patterns or rules

Remember: **Simplicity and consistency over complexity and innovation.**