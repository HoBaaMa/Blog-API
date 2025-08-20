namespace Blog_API.Examples.ImprovedCode
{
    // EXAMPLE: Improved Model with proper nullable handling
    public class ImprovedBlogPost
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public required string Title { get; set; }
        
        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters")]
        public required string Content { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        [Required]
        public required string UserId { get; set; }
        
        // Navigation properties - properly nullable for EF Core
        public ApplicationUser? User { get; set; }
        public BlogCategory BlogCategory { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }

    // EXAMPLE: Improved DTO with validation
    public class ImprovedCreateBlogPostDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public required string Title { get; set; }
        
        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters")]
        public required string Content { get; set; }
        
        [Required(ErrorMessage = "Category is required")]
        public BlogCategory Category { get; set; }
        
        public List<string> TagNames { get; set; } = new();
    }

    // EXAMPLE: Improved Service using Repository Pattern
    public interface IImprovedBlogPostService
    {
        Task<Result<BlogPostDTO>> CreateBlogPostAsync(CreateBlogPostDTO dto, string userId);
        Task<Result<BlogPostDTO>> GetBlogPostByIdAsync(Guid id);
        Task<Result<IReadOnlyCollection<BlogPostDTO>>> GetAllBlogPostsAsync();
        Task<Result> UpdateBlogPostAsync(Guid id, UpdateBlogPostDTO dto, string userId);
        Task<Result> DeleteBlogPostAsync(Guid id, string userId);
    }

    public class ImprovedBlogPostService : IImprovedBlogPostService
    {
        private readonly IBlogPostRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ImprovedBlogPostService> _logger;

        public ImprovedBlogPostService(
            IBlogPostRepository repository, 
            IMapper mapper, 
            ILogger<ImprovedBlogPostService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<BlogPostDTO>> CreateBlogPostAsync(CreateBlogPostDTO dto, string userId)
        {
            try
            {
                var blogPost = _mapper.Map<BlogPost>(dto);
                blogPost.UserId = userId;
                blogPost.CreatedAt = DateTime.UtcNow;

                var created = await _repository.CreateAsync(blogPost);
                var result = _mapper.Map<BlogPostDTO>(created);
                
                _logger.LogInformation("Blog post created successfully with ID: {BlogPostId}", created.Id);
                return Result<BlogPostDTO>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog post for user {UserId}", userId);
                return Result<BlogPostDTO>.Failure("Failed to create blog post");
            }
        }

        // Other methods follow similar pattern...
    }

    // EXAMPLE: Result pattern for better error handling
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string? Error { get; private set; }

        protected Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);
    }

    public class Result<T> : Result
    {
        public T? Data { get; private set; }

        private Result(bool isSuccess, T? data, string? error) : base(isSuccess, error)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(true, data, null);
        public static Result<T> Failure(string error) => new(false, default, error);
    }

    // EXAMPLE: Improved Controller with proper separation of concerns
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ImprovedBlogPostsController : ControllerBase
    {
        private readonly IImprovedBlogPostService _blogPostService;
        private readonly ILogger<ImprovedBlogPostsController> _logger;

        public ImprovedBlogPostsController(
            IImprovedBlogPostService blogPostService,
            ILogger<ImprovedBlogPostsController> logger)
        {
            _blogPostService = blogPostService ?? throw new ArgumentNullException(nameof(blogPostService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new blog post
        /// </summary>
        /// <param name="dto">Blog post creation data</param>
        /// <returns>Created blog post</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BlogPostDTO>> CreateBlogPost([FromBody] CreateBlogPostDTO dto)
        {
            var userId = User.GetUserId();
            var result = await _blogPostService.CreateBlogPostAsync(dto, userId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(GetBlogPostById), 
                new { id = result.Data!.Id }, 
                result.Data);
        }

        /// <summary>
        /// Gets a blog post by ID
        /// </summary>
        /// <param name="id">Blog post ID</param>
        /// <returns>Blog post details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BlogPostDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BlogPostDTO>> GetBlogPostById(Guid id)
        {
            var result = await _blogPostService.GetBlogPostByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Data);
        }

        // Other endpoints follow similar pattern...
    }

    // EXAMPLE: Extension methods for cleaner code
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) 
                ?? throw new UnauthorizedAccessException("User ID not found in token");
        }

        public static string? GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }
    }

    // EXAMPLE: Global Exception Handling Middleware
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                KeyNotFoundException => new { message = "Resource not found", statusCode = 404 },
                UnauthorizedAccessException => new { message = "Access denied", statusCode = 403 },
                ArgumentException => new { message = "Invalid request", statusCode = 400 },
                _ => new { message = "Internal server error", statusCode = 500 }
            };

            context.Response.StatusCode = response.statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}