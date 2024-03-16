namespace Domain.Commands;

public sealed class InsertProductCommandResponse
{
    public InsertProductCommandResponse(int id) => this.Id = id;

    public int Id { get; }
}