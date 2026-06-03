using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Core.Interfaces.Repositories;
using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Infrastructure.Repositories
{
    public class EmailPostSendEventRepository(PersonalBlogDbContext context, IMapper mapper) : IEmailPostSendEventRepository
    {
        private readonly PersonalBlogDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<int> CreateAsync(EmailPostSendEvent emailPostSendEvent)
        {
            _context.EmailPostSendEvents.Add(emailPostSendEvent);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmailPostSendEventDTO>> GetByPostIdAsync(Guid postId)
        {
            var events = await _context.EmailPostSendEvents
                .Where(e => e.PostId == postId)
                .ProjectTo<EmailPostSendEventDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return events;
        }
    }
}