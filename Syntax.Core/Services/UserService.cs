﻿using Microsoft.AspNetCore.Identity;
using Syntax.Core.Models;
using Syntax.Core.Repositories;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly IUserStore<UserAccount> _userStore;
        private readonly UnitOfWork _unitOfWork;
        private readonly IUserEmailStore<UserAccount> _emailStore;

        public UserService(UserManager<UserAccount> userManager, IUserStore<UserAccount> userStore, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _userStore = userStore;
            _unitOfWork = unitOfWork;
            _emailStore = GetEmailStore();
        }

        public async Task SetUserProfilePictureAsync(string userId, Blob blob)
        {
            await _unitOfWork.User.SetUserProfilePictureAsync(userId, blob);
            await _unitOfWork.User.SaveChangesAsync();
        }

        public async Task<Blob> GetUserProfilePictureAsync(string userId)
            => await _unitOfWork.User.GetUserProfilePictureAsync(userId);

        public async Task<UserAccount> GetUserByIdAsync(string id)
            => await _unitOfWork.User.GetUserById(id);

        public async Task<IdentityResult> CreateUser(UserAccount user, string username, string password, string email)
        {
            await _userStore.SetUserNameAsync(user, username, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
            return await _userManager.CreateAsync(user, password);
        }

        private IUserEmailStore<UserAccount> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");

            return (IUserEmailStore<UserAccount>)_userStore;
        }
    }
}
