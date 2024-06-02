﻿using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.UnitTests.Attendances;

internal class SearchAttendanceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IAttendanceRepository _attendanceRepository;

    public SearchAttendanceTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _attendanceRepository = new AttendanceRepository(_context);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}

