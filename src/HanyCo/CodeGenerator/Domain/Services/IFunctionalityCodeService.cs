﻿using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.DesignPatterns.Markers;
using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IFunctionalityCodeService : IBusinessService, ICodeGenerator<FunctionalityViewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs>;

public sealed record FunctionalityCodeServiceAsyncCodeGeneratorArgs(bool UpdateModelView = true, CancellationToken Token = default);