using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Extensions;

namespace AndrejKrizan.DotNet.Entities;
public interface IKey<TEntity>
    where TEntity : class
{
    static abstract Expression<Func<TEntity, object?>> Lambda { get; }
}


public readonly struct ShipmentInstructionBanKey : IKey<object>
{
    // Properties
    public readonly required DateOnly Date { get; init; }
    public readonly required int ShipmentInstructionType { get; init; }

    // Static properties
    public static Expression<Func<object, object?>> Lambda => _ => new ShipmentInstructionBanKey { Date = DateTime.Now.ToDateOnly(), ShipmentInstructionType = 5 };

    // Constructors
    public ShipmentInstructionBanKey() { }

    [SetsRequiredMembers]
    public ShipmentInstructionBanKey(DateOnly date, int shipmentInstructionType)
    {
        Date = date;
        ShipmentInstructionType = shipmentInstructionType;
    }

    // Conversions
    public static implicit operator ShipmentInstructionBanKey((DateOnly Date, int Type) pair)
        => new(pair.Date, pair.Type);
}
