﻿using Evol.TMovie.Domain.Commands.Dto;
using Evol.Domain.Commands;

namespace Evol.TMovie.Domain.Commands
{
    public class ScreeningRoomUpdateCommand : Command
    {
        public ScreeningRoomUpdateDto Input { get; set; }
    }
}
