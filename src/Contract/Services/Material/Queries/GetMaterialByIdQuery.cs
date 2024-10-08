﻿using Contract.Abstractions.Messages;
using Contract.Services.Material.ShareDto;

namespace Contract.Services.Material.Query;

public record GetMaterialByIdQuery(Guid Id) : IQuery<MaterialResponse>;
