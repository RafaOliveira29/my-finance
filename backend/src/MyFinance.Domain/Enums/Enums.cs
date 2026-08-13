namespace MyFinance.Domain.Enums;

/// <summary>Natureza da categoria: receita ou despesa. Persistido como string.</summary>
public enum CategoryType
{
    Income = 1,
    Expense = 2,
}

/// <summary>Tipo de despesa recorrente (atributo da ExpenseSource — não confundir com a dimensão Nature do lançamento).</summary>
public enum ExpenseKind
{
    Fixed = 1,
    Variable = 2,
}

/// <summary>Recorrência de uma origem. No MVP o motor gera apenas Monthly; OneTime é reservado para o futuro.</summary>
public enum RecurrenceType
{
    OneTime = 1,
    Monthly = 2,
}