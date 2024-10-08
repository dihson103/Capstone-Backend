﻿using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.SetProducts;

public class SetProductNotFoundException : MyException
{
    public SetProductNotFoundException() : base(
        (int) HttpStatusCode.NotFound, 
        "Không tìm thấy bộ sản phẩm")
    {
    }
}
