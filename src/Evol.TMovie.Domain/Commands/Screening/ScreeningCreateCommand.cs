﻿using Evol.TMovie.Domain.Commands.Dto;
using Evol.Domain.Commands;

namespace Evol.TMovie.Domain.Commands
{
    public class ScreeningCreateCommand : Command
    {
        public ScreeningCreateDto Input { get; set; }
    }
}
