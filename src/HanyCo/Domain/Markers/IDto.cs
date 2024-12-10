namespace HanyCo.Infra.Markers;

public interface IDto;

public interface IIdenticalDto<TIdType> : IDto
{
    TIdType Id { get; set; }
}

public interface IMesDto : IIdenticalDto<long>;