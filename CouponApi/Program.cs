using CouponApi.Data;
using CouponApi.Models;
using CouponApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/coupons",(ILogger<Program> _logger) => {
    _logger.Log(LogLevel.Information, "Fetching all coupons");
    return Results.Ok(CouponStore.couponList);
}).WithName("Coupons").Produces<IEnumerable<Coupon>>(200); ;

app.MapGet("/api/coupon/{id:int}", (int id) => {
    return Results.Ok(CouponStore.couponList.FirstOrDefault(s=>s.Id == id));
}).WithName("GetCoupon").Produces<Coupon>(200);

app.MapPost("/api/coupon", ([FromBody] CouponCreateDTO coupon_C_DTO) => {
    if (string.IsNullOrEmpty(coupon_C_DTO.Name))
    {
        return Results.BadRequest("Invalid Coupon Id or Name!");
    }
    if (CouponStore.couponList.FirstOrDefault(x => x.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
    {
        return Results.BadRequest("Coupon Name Already Exist!");
    }
    Coupon coupon = new Coupon() { 
        Name = coupon_C_DTO.Name,
        Percent = coupon_C_DTO.Percent,
        IsActive = coupon_C_DTO.IsActive,
    };

    coupon.Id = CouponStore.couponList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
    coupon.Created = DateTime.Now;
    CouponStore.couponList.Add(coupon);

    CouponDTO couponDTO = new CouponDTO()
    {
        Id = coupon.Id,
        Name = coupon.Name,
        Percent = coupon.Percent,
        IsActive = coupon.IsActive,
        Created = coupon.Created,
    };

    //return Results.Created($"/api/coupon/{coupon.Id}",coupon);
    return Results.CreatedAtRoute("GetCoupon", new { id = couponDTO.Id }, couponDTO);
}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<CouponDTO>(201).Produces(400);

app.MapPut("api/coupon", () => { });

app.MapDelete("api/coupon/{id:int}", (int id) => { });

app.UseHttpsRedirection();

app.Run();

