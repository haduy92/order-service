using Asp.Versioning;
using FlashCard.Application.Interfaces.Application;
using FlashCard.Application.Models;
using FlashCard.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FlashCard.Api.Controllers;


[ApiExplorerSettings(GroupName = "card")]
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/card")]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;
    private readonly ILogger<AuthController> _logger;

    public CardController(ICardService cardService,
        ILogger<AuthController> logger)
    {
        _cardService = cardService;
        _logger = logger;
    }

    /// <summary>
    /// Search cards by card title
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully return search result</response>
    /// <response code="500">Unexpected server error</response>
    /// <returns>True or False</returns>
    [ProducesResponseType(typeof(SearchCardResponse), StatusCodes.Status200OK)]
    [HttpGet("search")]
    public async Task<ActionResult<SearchCardResponse>> SearchAsync([FromQuery] SearchCardRequest request)
    {
        try
        {
            var response = await _cardService.SearchAsync(request);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Create a new card
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Card is created successfully</response>
    /// <response code="500">Unexpected server error</response>
    /// <returns>True or False</returns>
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<ActionResult<int>> CreateAsync([FromBody] CreateCardRequest request)
    {
        try
        {
            var response = await _cardService.CreateAsync(request);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Update a card
    /// </summary>
    /// <param name="id">Card ID</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Card is updated successfully</response>
    /// <response code="404">Card id is not existed</response>
    /// <response code="500">Unexpected server error</response>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateCardRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _cardService.UpdateAsync(id, request, cancellationToken);

            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Delete a card by card ID
    /// </summary>
    /// <param name="id">Card ID</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Card is deleted successfully</response>
    /// <response code="404">Card id is not existed</response>
    /// <response code="500">Unexpected server error</response>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            await _cardService.DeleteAsync(id, cancellationToken);

            return Ok();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
