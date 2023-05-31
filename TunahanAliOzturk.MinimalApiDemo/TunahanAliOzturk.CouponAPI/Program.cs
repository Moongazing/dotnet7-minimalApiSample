using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TunahanAliOzturk.CouponAPI.Data;
using TunahanAliOzturk.CouponAPI.Mapping;
using TunahanAliOzturk.CouponAPI.Models;
using TunahanAliOzturk.CouponAPI.Models.Dto;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
    APIResponse response = new();
    _logger.Log(LogLevel.Information, "Getting all coupons");
    response.Result = CouponStore.coupons;
    response.IsSuccess = true;
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("GetCoupons").Produces<APIResponse>(200);

app.MapGet("/api/coupon/{id:int}", (ILogger<Program> _logger, int id) =>
{
    APIResponse response = new();
    _logger.Log(LogLevel.Information, $"Getting {id} coupon");
    response.Result = CouponStore.coupons.FirstOrDefault(c => c.Id == id);
    response.IsSuccess = true;
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCoupon").Produces<IEnumerable<APIResponse>>(200);


app.MapPost("/api/coupon", async (ILogger<Program> _logger,IMapper _mapper, IValidator<CouponCreateDto> _validation,[FromBody] CouponCreateDto couponCreateDto) =>
{
    APIResponse response = new(){ IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
    

    var validationResult = await _validation.ValidateAsync(couponCreateDto);

    if (!validationResult.IsValid)
    {
        StringBuilder errorMessageBuilder = new StringBuilder();
        foreach (var error in validationResult.Errors)
        {
            errorMessageBuilder.AppendLine(error.ErrorMessage);
        }
        string errorMessage = errorMessageBuilder.ToString();

        response.ErrorMessages.Add(errorMessage);

        _logger.Log(LogLevel.Error, errorMessage);
        return Results.BadRequest(response);
    }

    if (CouponStore.coupons.FirstOrDefault(c => c.Name.ToLower() == couponCreateDto.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon name already exists");
        return Results.BadRequest(response);
    }


    Coupon coupon = _mapper.Map<Coupon>(couponCreateDto);

    coupon.Id = CouponStore.coupons.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;


    CouponStore.coupons.Add(coupon);

    CouponDto couponDto = _mapper.Map<CouponDto>(coupon);

 
    _logger.Log(LogLevel.Information, $"Coupon added with this lines'{coupon.Id}-{coupon.Name}-{coupon.Percent}'");
    response.Result = couponDto;
    response.IsSuccess = true;
    response.StatusCode = System.Net.HttpStatusCode.Created;
    return Results.Ok(response);


}).WithName("CreateCoupon").Accepts<CouponCreateDto>("application/json").Produces<APIResponse>(201).Produces(400);



app.MapPut("/api/coupon", async  (ILogger<Program> _logger,IMapper _mapper, [FromBody] CouponUpdateDto couponUpdateDto, IValidator<CouponUpdateDto> _validation) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };


    var validationResult = await _validation.ValidateAsync(couponUpdateDto);

    if (!validationResult.IsValid)
    {
        StringBuilder errorMessageBuilder = new StringBuilder();
        foreach (var error in validationResult.Errors)
        {
            errorMessageBuilder.AppendLine(error.ErrorMessage);
        }
        string errorMessage = errorMessageBuilder.ToString();

        response.ErrorMessages.Add(errorMessage);

        _logger.Log(LogLevel.Error, errorMessage);
        return Results.BadRequest(response);
    }

    if (CouponStore.coupons.FirstOrDefault(c => c.Name.ToLower() == couponUpdateDto.Name.ToLower()) != null && CouponStore.coupons.FirstOrDefault(c=>c.Id == couponUpdateDto.Id) !=null)
    {
        response.ErrorMessages.Add("Coupon name or coupon id already exists");
        return Results.BadRequest(response);
    }


    Coupon couponFromStore = CouponStore.coupons.FirstOrDefault(c => c.Id == couponUpdateDto.Id);
    couponFromStore.IsActive = couponUpdateDto.IsActive;
    couponFromStore.Name = couponUpdateDto.Name;
    couponFromStore.Percent = couponUpdateDto.Percent;
    couponFromStore.LastUpdated = DateTime.Now;





    _logger.Log(LogLevel.Information, $"Coupon updated");
    response.Result = _mapper.Map<CouponDto>(couponFromStore);
  
    response.IsSuccess = true;
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("UpdateCoupon").Accepts<CouponUpdateDto>("application/json").Produces<APIResponse>(200).Produces(400); ;


app.MapDelete("/api/coupon/{id:int}", (ILogger<Program> _logger, int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };


    Coupon couponFromStore = CouponStore.coupons.FirstOrDefault(c => c.Id == id);

    if (couponFromStore != null)
    {
        CouponStore.coupons.Remove(couponFromStore);
        _logger.Log(LogLevel.Information, $"Coupon deleted {id}");
        response.IsSuccess = true;
        response.StatusCode = System.Net.HttpStatusCode.NoContent;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMessages.Add("Invalid id");
        return Results.BadRequest(response);
    }
    
});
app.UseHttpsRedirection();

app.Run();


