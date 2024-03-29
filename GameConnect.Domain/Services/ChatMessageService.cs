﻿using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameConnect.Domain.Services
{

    public class ChatMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ChatMessageStatusService _chatMessageStatusService;


        public ChatMessageService(ApplicationDbContext context, ChatMessageStatusService chatMessageStatusService)
        {
            _context = context;
            _chatMessageStatusService = chatMessageStatusService;
        }

        public async Task<List<ChatMessage>> ChatMessagesFromSessionIdAsync(int id)
        {
            return await _context.ChatMessage
                .Where(x => x.SessionId == id)
                .Include(x => x.User)
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task CreateChatMessageAsync(ChatMessage message, ClaimsPrincipal loggedInUser)
        {
            await _context.ChatMessage.AddAsync(message);
            await _context.SaveChangesAsync();
            await _chatMessageStatusService.AddMessageAsync(message.SessionId, message.Id, message.Session.Participants, loggedInUser);
        }

    }
}
