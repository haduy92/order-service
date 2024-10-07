using AutoMapper;
using FlashCard.Application.Interfaces.Application;
using FlashCard.Application.Interfaces.Persistence.Cards;
using FlashCard.Application.Models;
using FlashCard.Domain.Entities;
using FlashCard.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlashCard.Infrastructure.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly ITextGeneratingService _textGeneratingService;
    private readonly IMapper _mapper;

    public CardService(ICardRepository cardRepository,
        ITextGeneratingService textGeneratingService,
        IMapper mapper)
    {
        _cardRepository = cardRepository;
        _textGeneratingService = textGeneratingService;
        _mapper = mapper;
    }

    public async Task<SearchCardResponse> SearchAsync(SearchCardRequest request, CancellationToken cancellationToken = default)
    {
        var query = _cardRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x => EF.Functions.ToTsVector("english", x.Title + " " + x.Description)
                .Matches(request.Search))
            .OrderByDescending(x => x.CreationTime);
        }

        var list = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        var totalCount = await query.CountAsync();

        return new SearchCardResponse
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Items = list.Select(_mapper.Map<CardDto>)
        };
    }

    public async Task<int> CreateAsync(CreateCardRequest request, CancellationToken cancellationToken = default)
    {
        var card = _mapper.Map<Card>(request);
        card = await _cardRepository.InsertAsync(card);
        return card.Id;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var card = await EnsureGetByIdAsync(id);
        await _cardRepository.DeleteAsync(card);
    }

    public async Task<CardDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var card = await EnsureGetByIdAsync(id);
        return _mapper.Map<CardDto>(card);
    }

    public async Task UpdateAsync(int id, UpdateCardRequest request, CancellationToken cancellationToken = default)
    {
        var card = await EnsureGetByIdAsync(id);
        card.Title = request.Title;
        card.Description = request.Description;
        await _cardRepository.UpdateAsync(card);
    }

    public async Task<string> GenerateDescriptionByTitleAsync(string title)
    {
        return await _textGeneratingService.GenerateTextAsync(title);
    }

    private async Task<Card> EnsureGetByIdAsync(int id)
    {
        var card = await _cardRepository.GetAsync(id);
        if (card is null)
        {
            throw new EntityNotFoundException(typeof(Card), id);
        }
        return card;
    }
}
