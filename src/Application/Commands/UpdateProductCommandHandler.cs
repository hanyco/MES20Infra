using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Commands;
using Domain.Dtos;
using Library.Results;

namespace Application.Commands;
internal partial class UpdateProductCommandHandler
{
    private partial Task OnHanded(UpdateProductCommand request, Result<Product> response, CancellationToken cancellationToken)
        => Task.CompletedTask;

    private partial Task OnHanding(UpdateProductCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}