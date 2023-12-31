﻿using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Domain.Entities.Messages;

internal static class MessageExtensions
{
    internal static Message? ToMessage(this Database.Message? message, string senderUserName)
    {
        if (message is null)
        {
            return null;
        }

        return new Message(message.Id, message.GroupId, message.Content, senderUserName, message.CreateDate);
    }
}
