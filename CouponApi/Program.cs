using AutoMapper;
using CouponApi;
using CouponApi.Data;
using CouponApi.Models;
using CouponApi.Models.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/coupons",(ILogger<Program> _logger) => {

    var response = new APIResponse();

    _logger.Log(LogLevel.Information, "Getting all coupons");
    
    response.IsSuccess = true;
    response.Result = CouponStore.couponList;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("Coupons").Produces<APIResponse>(200); ;

app.MapGet("/api/coupon/{id:int}", (int id) => {
    var response = new APIResponse();
    response.IsSuccess = true;
    response.Result = CouponStore.couponList.FirstOrDefault(s => s.Id == id);
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCoupon").Produces<APIResponse>(200);

app.MapPost("/api/coupon", async (IMapper _mapper, IValidator<CouponCreateDTO> _validator, [FromBody] CouponCreateDTO coupon_C_DTO) => {
    var validationResult = await _validator.ValidateAsync(coupon_C_DTO);
    var response = new APIResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    if (CouponStore.couponList.FirstOrDefault(x => x.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon Name Already Exist!"); 
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

    coupon.Id = CouponStore.couponList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
    coupon.Created = DateTime.Now;
    CouponStore.couponList.Add(coupon);

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    response.Result = couponDTO;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);

    //return Results.CreatedAtRoute("GetCoupon", new { id = couponDTO.Id }, couponDTO);
    //return Results.Created($"/api/coupon/{coupon.Id}",coupon);

}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

app.MapPut("/api/coupon", async (IMapper _mapper, IValidator<CouponUpdateDTO> _validator, [FromBody] CouponUpdateDTO coupon_U_DTO) => {
    var validationResult = await _validator.ValidateAsync(coupon_U_DTO);
    var response = new APIResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    if (CouponStore.couponList.FirstOrDefault(x => x.Name.ToLower() == coupon_U_DTO.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon Name Already Exist!");
        return Results.BadRequest(response);
    }
    if (CouponStore.couponList.FirstOrDefault(x=>x.Id == coupon_U_DTO.Id) == null)
    {
        response.ErrorMessages.Add("No Coupon Exists with this Id : " + coupon_U_DTO.Id);
        return Results.BadRequest(response);
    }

    Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(x => x.Id == coupon_U_DTO.Id);
    couponFromStore.Name = coupon_U_DTO.Name;
    couponFromStore.Percent = coupon_U_DTO.Percent;
    couponFromStore.IsActive = coupon_U_DTO.IsActive;
    couponFromStore.LastUpdated = DateTime.Now;

    response.Result = _mapper.Map<CouponDTO>(couponFromStore);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

app.MapDelete("api/coupon/{id:int}", (int id) => {
    var response = new APIResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(x => x.Id == id);
    
    if(couponFromStore != null)
    {
        CouponStore.couponList.Remove(couponFromStore);
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.NoContent;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMessages.Add("Invalid Coupon Id!");
        return Results.BadRequest(response);
    }
    
});

app.UseHttpsRedirection();

app.Run();

