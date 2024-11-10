using Asp.Versioning;
using FlashCard.Api.Exceptions;
using FlashCard.Application.Interfaces.Application;
using FlashCard.Application.Models;
using FlashCard.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashCard.Api.Controllers;

[ApiExplorerSettings(GroupName = "card")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/cards")]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;
    private readonly ILogger<CardController> _logger;

    public CardController(ICardService cardService,
        ILogger<CardController> logger)
    {
        _cardService = cardService;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CardDto>> GetById(int id)
    {
        try
        {
            var response = await _cardService.GetByIdAsync(id);

            return Ok(response);
        }
        catch (EntityNotFoundException ex)
        {
            throw new NotFoundException(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<SearchCardResponse>> SearchAsync([FromQuery] SearchCardRequest request)
    {
        var response = await _cardService.SearchAsync(request);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateAsync([FromBody] CreateCardRequest request)
    {
        var response = await _cardService.CreateAsync(request);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateCardRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _cardService.UpdateAsync(id, request, cancellationToken);

            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            throw new NotFoundException(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            await _cardService.DeleteAsync(id, cancellationToken);

            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            throw new NotFoundException(ex.Message);
        }
    }

    [HttpGet("generate-description")]
    public async Task<ActionResult> GenerateDescriptionAsync([FromQuery] string title)
    {
        try
        {
            var result = await _cardService.GenerateDescriptionByTitleAsync(title);

            return Ok(result);
        }
        catch (EntityNotFoundException ex)
        {
            throw new NotFoundException(ex.Message);
        }
    }
}
