namespace BuildingBlocks.Domain;

public interface IAuditableAction
{
    DateTime CreatedOn => DateTime.UtcNow;
    int CreatedByUserId { get; set; }
}

public interface IAuditableAction<TActionType> : IAuditableAction
    where TActionType : Enum
{
    TActionType ActionType { get; }
}