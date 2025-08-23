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
        public LikeService(ILikeRepository likeRepository, IBlogPostRepository blogPostRepository, ICommentRepository commentRepository, IMapper mapper)
        {
            _likeRepository = likeRepository;
            _blogPostRepository = blogPostRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<LikeDTO>> GetAllLikesByBlogPostIdAsync(Guid blogPostId)
        {
            if (await _blogPostRepository.GetByIdAsync(blogPostId) == null)
            {
                throw new KeyNotFoundException("Blog post is not found or deleted.");
            }
            var likes = await _likeRepository.GetLikesByBlogPostIdAsync(blogPostId);

            return _mapper.Map<IReadOnlyCollection<LikeDTO>>(likes);
        }

        public async Task<IReadOnlyCollection<LikeDTO>> GetAllLikesByCommentIdAsync(Guid commentId)
        {
            if (await _commentRepository.GetByIdAsync(commentId) == null)
            {
                throw new KeyNotFoundException("Comment is not found or deleted.");
            }
            var likes = await _likeRepository.GetLikesByCommentIdAsync(commentId);

            return _mapper.Map<IReadOnlyCollection<LikeDTO>>(likes);
        }

        public async Task<bool> ToggleLikeAsync(string userId, Guid? blogPostId = null, Guid? commentId = null)
        {
            if (!blogPostId.HasValue && !commentId.HasValue)
            {
                throw new ArgumentException("You must specify a post or comment.");
            }
            if (commentId.HasValue && blogPostId.HasValue)
            {
                throw new ArgumentException("You cannot like a comment and a blog post at the same time.");
            }

            var existingLike = await _likeRepository.GetByUserAndTargetAsync(userId, blogPostId, commentId);


            if (existingLike != null)
            {
                await _likeRepository.RemoveAsync(existingLike);
                return false;
            }

            var like = new Like
            {
                BlogPostId = blogPostId,
                CommentId = commentId,
                UserId = userId
            };

            await _likeRepository.AddAsync(like);
            return true;
        }

    }
}
