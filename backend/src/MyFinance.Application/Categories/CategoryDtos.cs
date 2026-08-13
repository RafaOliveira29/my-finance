using MyFinance.Domain.Enums;

namespace MyFinance.Application.Categories;

public sealed record CreateCategoryRequest(string Name, CategoryType Type, string? Color, string? Icon);

public sealed record UpdateCategoryRequest(string Name, CategoryType Type, string? Color, string? Icon, bool IsActive);

public sealed record CategoryResponse(Guid Id, string Name, CategoryType Type, string? Color, string? Icon, bool IsActive);