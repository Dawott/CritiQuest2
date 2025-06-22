using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CritiQuest2.Server.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Gets the current user's ID from JWT claims in a consistent way
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <returns>User ID string or null if not found</returns>
        public static string? GetCurrentUserId(this ControllerBase controller)
        {
            return controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Gets the current user's ID and throws UnauthorizedAccessException if not found
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <returns>User ID string</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when user ID is not found</exception>
        public static string GetCurrentUserIdOrThrow(this ControllerBase controller)
        {
            var userId = controller.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or user ID not found in token");
            }
            return userId;
        }
    }
}
