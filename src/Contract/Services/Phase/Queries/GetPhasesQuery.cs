﻿using Contract.Abstractions.Messages;
using Contract.Services.Phase.ShareDto;

namespace Contract.Services.Phase.Queries;

public record GetPhasesQuery : IQuery<List<PhaseResponse>>;

