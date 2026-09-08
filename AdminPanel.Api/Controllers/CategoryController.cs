using AdminPanel.Application.Common;
using AdminPanel.Application.DTOs.Category;
using AdminPanel.Application.DTOs.Common;
using AdminPanel.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }




    [HttpPost]
    public async Task<ActionResult<ApiMessage<bool>>> Create(
        [FromBody] CreateCategoryRequestDto request)
    {
        var result = await _categoryService.CreateAsync(request);

        if (result.HasError)
            return BadRequest(result);

        return Ok(result);
    }




    [HttpPut]
    public async Task<ActionResult<ApiMessage<bool>>> Update(
        [FromBody] UpdateCategoryRequestDto request)
    {
        var result = await _categoryService.UpdateAsync(request);

        if (result.HasError)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiMessage<bool>>> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);

        if (result.HasError)
            return NotFound(result);

        return Ok(result);
    }




    [HttpGet("ByCharity/{charityId:int}")]
    public async Task<ActionResult<ApiMessage<List<CategoryListItemDto>>>> GetByCharity(int charityId)
    {
        if (charityId <= 0)
        {
            return BadRequest(new ApiMessage<List<CategoryListItemDto>>
            {
                HasError = true,
                ErrorMessage = "شناسه خیریه نامعتبر است."
            });
        }

        var result = await _categoryService.GetByCharityAsync(charityId);

        if (result.HasError)
            return BadRequest(result);

        return Ok(result);
    }





    [HttpGet("{id}")]
    public async Task<ActionResult<ApiMessage<CategoryDetailsDto>>> GetById(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);

        if (result.HasError)
            return NotFound(result);

        return Ok(result);
    }



    [HttpGet]
    public async Task<ActionResult<ApiMessage<PagedResultDto<CategoryListItemDto>>>> GetList(
        [FromQuery] PaginationRequestDto request)
    {
        var result = await _categoryService.GetListAsync(request);

        if (result.HasError)
            return BadRequest(result);

        return Ok(result);
    }



}
