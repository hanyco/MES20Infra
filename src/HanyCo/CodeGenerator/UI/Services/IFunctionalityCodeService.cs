﻿using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

public interface IFunctionalityCodeService : IService, ICodeGeneratorService<FunctionalityViewModel>
{
    
}