﻿using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Roles;

public class RoleNotFoundException : MyException
{
    public RoleNotFoundException() : base(
        (int) HttpStatusCode.NotFound, 
        "Không tìm thấy vai trò")
    {
    }
}
