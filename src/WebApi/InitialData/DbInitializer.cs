﻿using Application.Abstractions.Services;
using Contract.Services.Company.Create;
using Contract.Services.Company.Shared;
using Contract.Services.Phase.Creates;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.SharedDto;
using Contract.Services.Role.Create;
using Contract.Services.Slot.Create;
using Contract.Services.User.CreateUser;
using Domain.Entities;
using Persistence;
using System.Collections.Generic;

namespace WebApi.InitialData;

public class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            SeedData(scope.ServiceProvider);
        }
    }

    public static void SeedData(IServiceProvider provider)
    {
        var context = provider.GetService<AppDbContext>();
        if (!context.Roles.Any())
        {
            SeedRoleData(context);
        }

        if (!context.Companies.Any())
        {
            SeedCompanyData(context);
        }

        if (!context.Users.Any())
        {
            var passwordService = provider.GetService<IPasswordService>();
            SeedUserData(context, passwordService);
        }

        if (!context.Slots.Any())
        {
            SeedSlotData(context);
        }

        if (!context.Products.Any())
        {
            SeedProductData(context);
        }

        if (!context.Phases.Any())
        {
            SeedPhaseData(context);
        }

    }

    public static void SeedPhaseData(AppDbContext context)
    {
        var phases = new List<Phase>
        {
            Phase.Create(new CreatePhaseRequest("PH_001", "Giai đoạn tạo khung")),
            Phase.Create(new CreatePhaseRequest("PH_002", "Giai đoạn gia công")),
            Phase.Create(new CreatePhaseRequest("PH_003", "Giai đoạn hoàn thiện đóng gói")),
        };

        context.Phases.AddRange(phases);
        context.SaveChanges();
    }

    public static void SeedProductData(AppDbContext context)
    {
        var products = new List<Product>
        {
            Product.Create(
                new CreateProductRequest(
                    Code: "PD001",
                    Price: 50.00m,
                    Size: "M",
                    Description: "First product",
                    Name: "Product 1",
                    ImageRequests: new List<ImageRequest>
                    {
                        new ImageRequest("image_01.png", false, true),
                        new ImageRequest("image_02.png", true, false),
                    }
                ),
                createdBy: "001201011091"
            ),
            Product.Create(
                new CreateProductRequest(
                    Code: "PD002",
                    Price: 100.00m,
                    Size: "L",
                    Description: "Second product",
                    Name: "Product 2",
                    ImageRequests: null
                ),
                createdBy: "001201011091"
            ),
        };

        var images = new List<ProductImage>
        {
            ProductImage.Create(products[0].Id, new ImageRequest("image_01.png", false, true)),
            ProductImage.Create(products[0].Id, new ImageRequest("image_02.png", true, false)),
            ProductImage.Create(products[1].Id, new ImageRequest("image_03.png", false, true)),
            ProductImage.Create(products[1].Id, new ImageRequest("image_04.png", true, false)),
        };

        context.Products.AddRange(products);
        context.ProductImages.AddRange(images);
        context.SaveChanges();
    }

    public static void SeedRoleData(AppDbContext context)
    {
        var roles = new List<Role>
        {
            Role.Create(new CreateRoleCommand("MAIN_ADMIN", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("BRAND_ADMIN", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("COUNTER", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("DRIVER", "HuyVu's father")),
            Role.Create(new CreateRoleCommand("USER", "HuyVu's father"))
        };

        context.Roles.AddRange(roles);
        context.SaveChanges();
    }

    public static void SeedSlotData(AppDbContext context)
    {
        var slots = new List<Slot>
        {
            Slot.Create(new CreateSlotCommand("Morning")),
            Slot.Create(new CreateSlotCommand("Afternoon")),
            Slot.Create(new CreateSlotCommand("OverTime"))
        };

        context.Slots.AddRange(slots);
        context.SaveChanges();
    }

    public static void SeedCompanyData(AppDbContext context)
    {
        var companies = new List<Company>
        {
            Company.Create(new CreateCompanyRequest(new Contract.Services.Company.ShareDto.CompanyRequest("Cơ sở chính", "Hà Nội", "Vũ Đức Huy",
            "0976099789", "admin@admin.com",CompanyType.FACTORY))),
            Company.Create(new CreateCompanyRequest(new Contract.Services.Company.ShareDto.CompanyRequest("Cơ sở phụ", "Hà Nội", "Vũ Đức Huy",
            "0976099789", "admin2@admin.com", CompanyType.FACTORY))),
            Company.Create(new CreateCompanyRequest(new Contract.Services.Company.ShareDto.CompanyRequest("Công ty đối tác sản xuất", "Hà Nội", "Vũ Đức Huy",
            "0976099789", "admin@admin.com", CompanyType.THIRD_PARTY_COMPANY))),
            Company.Create(new CreateCompanyRequest(new Contract.Services.Company.ShareDto.CompanyRequest("Công ty cổ phần ABC", "Hà Nội", "Vũ Đức Huy",
            "0976099789", "admin@admin.com", CompanyType.CUSTOMER_COMPANY))),
        };

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }

    public static void SeedUserData(AppDbContext context, IPasswordService passwordService)
    {
        var adminRole = context.Roles.FirstOrDefault();
        var companyId = context.Companies.FirstOrDefault(c => c.CompanyType == CompanyType.FACTORY).Id;
        var userCreateRequest = new CreateUserRequest(
            "001201011091",
            "Son",
            "Nguyen",
            "0976099351",
            "Ha Noi",
            "12345",
            "Male",
            "10/03/2001",
            200000,
            companyId,
            adminRole.Id
            );
        var hash = passwordService.Hash(userCreateRequest.Password);
        var user = User.Create(userCreateRequest, hash, userCreateRequest.Id);

        context.Users.Add(user);
        context.SaveChanges();
    }

}
