using Blog_API.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace Blog_API.Services.Interface
{
    public interface ICommentService
    {
        // Brainstorming
        // 1) Get All Comments for a Blog Post
        // 2) Post New Comment
        // 3) Edit Existing Comment
        // 4) Delete Comment
        // 5) Get Comment by ID
        Task<IReadOnlyCollection<CommentDTO>> GetAllCommentsForBlogPostAsync(Guid blogPostId);
        Task<CommentDTO> CreateCommentAsync(CreateCommentDTO commentDTO, string userId);
        Task<CommentDTO?> GetCommentByIdAsync(Guid commentId);
        Task<CommentDTO> UpdateCommentAsync(Guid commentId, JsonPatchDocument<UpdateCommentDTO> patchDoc, string currentUserId);
        Task DeleteCommentAsync(Guid commendId, string currentUserId);
    }
}
