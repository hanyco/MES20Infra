namespace HanyCo.Infra.Markers;

public interface IHasMerchant<IIdType>
    where IIdType : struct
{
    IIdType? MerchantId { get; }
}

public interface IHasMerchant : IHasMerchant<long>;