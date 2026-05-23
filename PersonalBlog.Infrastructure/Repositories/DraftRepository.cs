using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Core.Exceptions;
using PersonalBlog.Core.Interfaces.Repositories;
using PersonalBlog.Models.DatabaseModels;
using System;

namespace PersonalBlog.Infrastructure.Repositories
{
    public class DraftRepository(PersonalBlogDbContext context, IMapper mapper) : IDraftRepository
    {
        private readonly PersonalBlogDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<int> CreateAsync(Draft draft)
        {
            _context.Drafts.Add(draft);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid draftId)
        {
            var draft = await _context.Drafts.FindAsync(draftId);
            if (draft == null) return false;
            _context.Drafts.Remove(draft);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Guid draftId)
        {
            var existingDraft = await _context.Drafts.FindAsync(draftId);
            return existingDraft != null;
        }

        public async Task<List<DraftDTO>> GetAllAsync()
        {
            var drafts = await _context.Drafts
                .Include(d => d.User)
                .ProjectTo<DraftDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return drafts;
        }

        public async Task<DraftDTO?> GetByIdAsync(Guid draftId)
        {
            var draft = await _context.Drafts
                .Include(d => d.User)
                .ProjectTo<DraftDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(d => d.Id == draftId);
            return draft;
        }

        public Task<List<DraftDTO>> GetByUserIdAsync(Guid userId)
        {
            var drafts = _context.Drafts
                .Where(d => d.User.IdentityUserId == userId)
                .Include(d => d.User)
                .ProjectTo<DraftDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return drafts;
        }

        public async Task<int> UpdateAsync(Guid draftId, Guid userId, SaveDraftDTO draftDto)
        {
            var existing = await _context.Drafts
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == draftId);

            if (existing.User.IdentityUserId != userId)
            {
                throw new UnauthorizedException("You are not authorized to update this draft.");
            }

            existing.Title = draftDto.Title;
            existing.Content = draftDto.Content;
            existing.Preview = draftDto.Preview;
            existing.Lastmodifieddate = DateTime.UtcNow;

            _context.Drafts.Update(existing);
            return await _context.SaveChangesAsync();
        }
    }
}