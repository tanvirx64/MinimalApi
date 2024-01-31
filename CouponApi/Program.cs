using CouponApi.Data;
using CouponApi.Models;
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

app.MapGet("/api/coupons",() => {
    return Results.Ok(CouponStore.couponList);
}).WithName("Coupons");

app.MapGet("/api/coupon/{id:int}", (int id) => {
    return Results.Ok(CouponStore.couponList.FirstOrDefault(s=>s.Id == id));
}).WithName("GetCoupon");

app.MapPost("/api/coupon", ([FromBody] Coupon coupon) => {
    if (coupon.Id != 0 || string.IsNullOrEmpty(coupon.Name))
    {
        return Results.BadRequest("Invalid Coupon Id or Name!");
    }
    if (CouponStore.couponList.FirstOrDefault(x => x.Name.ToLower() == coupon.Name.ToLower()) != null)
    {
        return Results.BadRequest("Coupon Name Already Exist!");
    }
    coupon.Id = CouponStore.couponList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
    coupon.Created = DateTime.Now;
    CouponStore.couponList.Add(coupon);

    //return Results.Created($"/api/coupon/{coupon.Id}",coupon);
    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);
}).WithName("CreateCoupon");

app.MapPut("api/coupon", () => { });

app.MapDelete("api/coupon/{id:int}", (int id) => { });

app.UseHttpsRedirection();

app.Run();

