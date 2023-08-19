﻿using Syntax.Core.Models;

namespace Syntax.Core.Services.Base
{
    public interface IUserService
    {
        Task<UserAccount> GetUserByIdAsync(string id);
        Task<Blob> GetUserProfilePictureAsync(string userId);
        Task SetUserProfilePictureAsync(string userId, Blob blob);
    }
}