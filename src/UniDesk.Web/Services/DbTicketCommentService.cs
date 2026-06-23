using Microsoft.EntityFrameworkCore;
using UniDesk.Web.DTOs;
using UniDesk.Web.Exceptions;
using UniDesk.Web.Models;

namespace UniDesk.Web.Services;
public class DbTicketCommentService : ITicketCommentService
{
    private readonly UniDeskDbContext _context;
    private readonly ISystemClock _systemClock;
    private readonly IMarkdownFormatter _markdownFormatter;

    public DbTicketCommentService(
        UniDeskDbContext context,
        ISystemClock systemClock,
        IMarkdownFormatter markdownFormatter)
    {
        _context = context;
        _systemClock = systemClock;
        _markdownFormatter = markdownFormatter;
    }

    public async Task<List<TicketCommentDto>> GetForTicketAsync(int ticketId)
    {
        var comments = await _context.TicketComments
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return comments
            .Select(c => new TicketCommentDto
            {
                Id = c.Id,
                TicketId = c.TicketId,
                AuthorName = c.AuthorName,
                Message = c.Message,
                MessageHtml = _markdownFormatter.ToSafeHtml(c.Message),
                CreatedAt = c.CreatedAt
            })
            .ToList();
    }

    public async Task<TicketCommentDto> CreateAsync(
        int ticketId,
        string authorId,
        string authorName,
        CreateTicketCommentRequest request)
    {
        var message = request.Message.Trim();

        if (message.Length < CreateTicketCommentRequest.MinMessageLength)
        {
            throw new InvalidOperationException("Komentarz jest za krotki.");
        }

        if (message.Length > CreateTicketCommentRequest.MaxMessageLength)
        {
            throw new InvalidOperationException("Komentarz jest za dlugi.");
        }

        var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == ticketId);

        if (!ticketExists)
        {
            throw new EntityNotFoundException($"Nie znaleziono zgloszenia o id {ticketId}.");
        }

        var comment = new TicketComment
        {
            TicketId = ticketId,
            AuthorId = authorId,
            AuthorName = authorName,
            Message = message,
            CreatedAt = _systemClock.UtcNow
        };

        _context.TicketComments.Add(comment);
        await _context.SaveChangesAsync();

        return new TicketCommentDto
        {
            Id = comment.Id,
            TicketId = comment.TicketId,
            AuthorName = comment.AuthorName,
            Message = comment.Message,
            MessageHtml = _markdownFormatter.ToSafeHtml(comment.Message),
            CreatedAt = comment.CreatedAt
        };
    }
}

