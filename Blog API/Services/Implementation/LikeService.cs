using Blog_API.Models.Entities;
using Blog_API.Models.DTOs;
using Blog_API.Services.Interface;
using Blog_API.Repositories.Interfaces;
using AutoMapper;

namespace Blog_API.Services.Implementation
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LikeService> _logger;
        
        public LikeService(ILikeRepository likeRepository, IBlogPostRepository blogPostRepository, ICommentRepository commentRepository, IMapper mapper, ILogger<LikeService> logger)
        {
            _likeRepository = likeRepository ?? throw new ArgumentNullException(nameof(likeRepository));
            _blogPostRepository = blogPostRepository ?? throw new ArgumentNullException(nameof(blogPostRepository));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<LikeDTO>> GetAllLikesByBlogPostIdAsync(Guid blogPostId)
        {
            _logger.LogInformation("Retrieving all likes for blog post {BlogPostId}", blogPostId);
            
            try
            {
                var blogPost = await _blogPostRepository.GetByIdAsync(blogPostId);
                if (blogPost == null)
                {
                    _logger.LogWarning("Blog post {BlogPostId} not found when retrieving likes", blogPostId);
                    throw new KeyNotFoundException("Blog post is not found or deleted.");
                }

                var likes = await _likeRepository.GetLikesByBlogPostIdAsync(blogPostId);
                _logger.LogInformation("Retrieved {LikeCount} likes for blog post {BlogPostId}", likes.Count, blogPostId);

                return _mapper.Map<IReadOnlyCollection<LikeDTO>>(likes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving likes for blog post {BlogPostId}", blogPostId);
                throw;
            }
        }

        public async Task<IReadOnlyCollection<LikeDTO>> GetAllLikesByCommentIdAsync(Guid commentId)
        {
            _logger.LogInformation("Retrieving all likes for comment {CommentId}", commentId);
            
            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null)
                {
                    _logger.LogWarning("Comment {CommentId} not found when retrieving likes", commentId);
                    throw new KeyNotFoundException("Comment is not found or deleted.");
                }

                var likes = await _likeRepository.GetLikesByCommentIdAsync(commentId);
                _logger.LogInformation("Retrieved {LikeCount} likes for comment {CommentId}", likes.Count, commentId);

                return _mapper.Map<IReadOnlyCollection<LikeDTO>>(likes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving likes for comment {CommentId}", commentId);
                throw;
            }
        }

        public async Task<bool> ToggleLikeAsync(string userId, Guid? blogPostId = null, Guid? commentId = null)
        {
            var target = blogPostId.HasValue ? $"blog post {blogPostId}" : $"comment {commentId}";
            _logger.LogInformation("Toggling like for user {UserId} on {Target}", userId, target);

            try
            {
                // Validation
                if (!blogPostId.HasValue && !commentId.HasValue)
                {
                    _logger.LogWarning("User {UserId} attempted to toggle like without specifying target", userId);
                    throw new ArgumentException("You must specify a post or comment.");
                }
                
                if (commentId.HasValue && blogPostId.HasValue)
                {
                    _logger.LogWarning("User {UserId} attempted to toggle like on both blog post and comment simultaneously", userId);
                    throw new ArgumentException("You cannot like a comment and a blog post at the same time.");
                }

                // Check if like already exists
                var existingLike = await _likeRepository.GetByUserAndTargetAsync(userId, blogPostId, commentId);

                if (existingLike != null)
                {
                    // Unlike operation
                    _logger.LogDebug("Removing existing like for user {UserId} on {Target}", userId, target);
                    await _likeRepository.RemoveAsync(existingLike);
                    _logger.LogInformation("User {UserId} successfully unliked {Target}", userId, target);
                    return false; // Unliked
                }
                else
                {
                    // Like operation
                    _logger.LogDebug("Creating new like for user {UserId} on {Target}", userId, target);
                    var like = new Like
                    {
                        BlogPostId = blogPostId,
                        CommentId = commentId,
                        UserId = userId
                    };

                    await _likeRepository.AddAsync(like);
                    _logger.LogInformation("User {UserId} successfully liked {Target}", userId, target);
                    return true; // Liked
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling like for user {UserId} on {Target}", userId, target);
                throw;
            }
        }
    }
}
