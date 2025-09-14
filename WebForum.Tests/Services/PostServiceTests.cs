using NSubstitute;
using Shouldly;
using WebForum.Application.DTOs;
using WebForum.Application.Services;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;

namespace WebForum.Tests.Services;

public class PostServiceTests
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUserRepository _userRepository;
    private readonly PostService _postService;

    public PostServiceTests()
    {
        _postRepository = Substitute.For<IPostRepository>();
        _tagRepository = Substitute.For<ITagRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _postService = new PostService(_postRepository, _userRepository, _tagRepository);
    }

    [Fact]
    public async Task GetPostsAsync_ReturnsAllPosts()
    {
        // Arrange
        var user = CreateTestUser();
        var posts = new List<Post>
        {
            CreateTestPost("First Post", "Body 1", user),
            CreateTestPost("Second Post", "Body 2", user)
        };

        _postRepository.GetPostsAsync().Returns(posts);

        // Act
        var result = await _postService.GetPostsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(2);
        result.First().Title.ShouldBe("First Post");
        result.Last().Title.ShouldBe("Second Post");
    }

    [Fact]
    public async Task GetPostsAsync_WithFilter_ReturnsPagedResult()
    {
        // Arrange
        var user = CreateTestUser();
        var posts = new List<Post>
        {
            CreateTestPost("Test Post", "Test Body", user)
        };

        var filter = new PostFilterRequest
        {
            Page = 1,
            PageSize = 10,
            Author = "testuser",
            Tags = "tag1,tag2",
            FromDate = DateTimeOffset.UtcNow.AddDays(-7),
            ToDate = DateTimeOffset.UtcNow,
            SortBy = PostSortField.CreatedDate,
            SortDirection = Application.DTOs.SortDirection.Desc
        };

        _postRepository.GetPostsAsync(
            filter.Page,
            filter.PageSize,
            filter.Author,
            Arg.Any<IEnumerable<string>>(),
            filter.FromDate,
            filter.ToDate,
            "CreatedDate",
            true).Returns((posts, 1));

        // Act
        var result = await _postService.GetPostsAsync(filter);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count().ShouldBe(1);
        result.Page.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task CreatePostAsync_WithValidRequest_ReturnsPostDto()
    {
        // Arrange
        var user = CreateTestUser();
        var userId = user.Id;
        var request = new CreatePostRequest
        {
            Title = "New Post",
            Body = "New Post Body"
        };

        var createdPost = CreateTestPost(request.Title, request.Body, user);

        _userRepository.GetByIdAsync(userId).Returns(user);
        _postRepository.CreatePostAsync(Arg.Any<Post>()).Returns(createdPost);

        // Act
        var result = await _postService.CreatePostAsync(request.Title, request.Body, userId);

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe(request.Title);
        result.Body.ShouldBe(request.Body);
        result.Author.ShouldBe(user.Username);
    }

    [Fact]
    public async Task CreatePostAsync_WithNonExistentUser_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreatePostRequest
        {
            Title = "New Post",
            Body = "New Post Body"
        };

        _userRepository.GetByIdAsync(userId).Returns((User?)null);

        // Act
        // Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _postService.CreatePostAsync(request.Title, request.Body, userId));
        exception.Message.ShouldContain("User not found");
        exception.ParamName.ShouldBe("userId");

        await _postRepository.DidNotReceive().CreatePostAsync(Arg.Any<Post>());
    }

    [Fact]
    public async Task AddCommentAsync_WithValidRequest_AddsComment()
    {
        // Arrange
        var user = CreateTestUser();
        var post = CreateTestPost("Test Post", "Test Body", user);
        var postId = post.Id;
        var userId = user.Id;
        var request = new CreateCommentRequest
        {
            Body = "Test comment"
        };

        var createdComment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = userId,
            Body = request.Body,
            User = user,
            Post = post
        };

        _postRepository.GetPostByIdAsync(postId).Returns(post);
        _userRepository.GetByIdAsync(userId).Returns(user);
        _postRepository.AddCommentAsync(Arg.Any<Comment>()).Returns(createdComment);

        // Act
        await _postService.AddCommentAsync(postId, request, userId);

        // Assert
        await _postRepository.Received(1).AddCommentAsync(Arg.Is<Comment>(c => c.PostId == postId && c.UserId == userId && c.Body == request.Body));
    }

    [Fact]
    public async Task AddCommentAsync_WithNonExistentPost_ThrowsArgumentException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var request = new CreateCommentRequest { Body = "Test comment" };

        _postRepository.GetPostByIdAsync(postId).Returns((Post?)null);

        // Act
        // Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => _postService.AddCommentAsync(postId, request, userId));
        exception.Message.ShouldContain("Post not found");
        exception.ParamName.ShouldBe("postId");

        await _postRepository.Received(1).GetPostByIdAsync(postId);
        await _userRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task AddLikeAsync_WithValidRequest_AddsLikeAndUpdatesLikeCount()
    {
        // Arrange
        var postAuthor = CreateTestUser("postauthor", "author@testdomain.co.jun");
        var likingUser = CreateTestUser("likinguser", "liker@testdomain.co.jun");
        var post = CreateTestPost("Test Post", "Test Body", postAuthor);
        var postId = post.Id;
        var userId = likingUser.Id;

        _postRepository.GetPostByIdAsync(postId).Returns(post);
        _userRepository.GetByIdAsync(userId).Returns(likingUser);
        _postRepository.GetLikeAsync(postId, userId).Returns((Like?)null);

        // Act
        await _postService.AddLikeAsync(postId, userId);

        // Assert
        await _postRepository.Received(1).GetPostByIdAsync(postId);
        await _userRepository.Received(1).GetByIdAsync(userId);
        await _postRepository.Received(1).GetLikeAsync(postId, userId);
        await _postRepository.Received(1).AddLikeAsync(Arg.Is<Like>(l => l.PostId == postId && l.UserId == userId));
        await _postRepository.Received(1).UpdatePostAsync(Arg.Is<Post>(p => p.Id == postId && p.LikeCount == 1));
    }

    [Fact]
    public async Task AddLikeAsync_WithExistingLike_DoesNotAddDuplicateLike()
    {
        // Arrange
        var postAuthor = CreateTestUser("postauthor", "author@testdomain.co.jun");
        var likingUser = CreateTestUser("likinguser", "liker@testdomain.co.jun");
        var post = CreateTestPost("Test Post", "Test Body", postAuthor);
        var postId = post.Id;
        var userId = likingUser.Id;
        var existingLike = new Like { PostId = postId, UserId = userId, Post = post, User = likingUser };

        _postRepository.GetPostByIdAsync(postId).Returns(post);
        _userRepository.GetByIdAsync(userId).Returns(likingUser);
        _postRepository.GetLikeAsync(postId, userId).Returns(existingLike);

        // Act
        await _postService.AddLikeAsync(postId, userId);

        // Assert
        await _postRepository.Received(1).GetLikeAsync(postId, userId);
        await _postRepository.DidNotReceive().AddLikeAsync(Arg.Any<Like>());
        await _postRepository.DidNotReceive().UpdatePostAsync(Arg.Any<Post>());
    }

    [Fact]
    public async Task RemoveLikeAsync_WithExistingLike_RemovesLikeAndUpdatesLikeCount()
    {
        // Arrange
        var user = CreateTestUser();
        var post = CreateTestPost("Test Post", "Test Body", user);
        post.LikeCount = 5;
        var postId = post.Id;
        var userId = user.Id;
        var existingLike = new Like { PostId = postId, UserId = userId, Post = post, User = user };

        _postRepository.GetPostByIdAsync(postId).Returns(post);
        _postRepository.GetLikeAsync(postId, userId).Returns(existingLike);

        // Act
        await _postService.RemoveLikeAsync(postId, userId);

        // Assert
        await _postRepository.Received(1).GetPostByIdAsync(postId);
        await _postRepository.Received(1).GetLikeAsync(postId, userId);
        await _postRepository.Received(1).RemoveLikeAsync(postId, userId);
        await _postRepository.Received(1).UpdatePostAsync(Arg.Is<Post>(p => p.Id == postId && p.LikeCount == 4));
    }

    [Fact]
    public async Task RemoveLikeAsync_WithZeroLikeCount_DoesNotGoNegative()
    {
        // Arrange
        var user = CreateTestUser();
        var post = CreateTestPost("Test Post", "Test Body", user);
        post.LikeCount = 0;
        var postId = post.Id;
        var userId = user.Id;
        var existingLike = new Like { PostId = postId, UserId = userId, Post = post, User = user };

        _postRepository.GetPostByIdAsync(postId).Returns(post);
        _postRepository.GetLikeAsync(postId, userId).Returns(existingLike);

        // Act
        await _postService.RemoveLikeAsync(postId, userId);

        // Assert
        await _postRepository.Received(1).UpdatePostAsync(Arg.Is<Post>(p => p.Id == postId && p.LikeCount == 0));
    }

    [Fact]
    public async Task AddTagToPostAsync_WithNewTag_CreatesTagAndAddsToPost()
    {
        // Arrange
        var user = CreateTestUser();
        var post = CreateTestPost("Test Post", "Test Body", user);
        var postId = post.Id;
        var request = new AddTagToPostRequest { TagName = "newtag" };

        var newTag = new Tag { Id = Guid.NewGuid(), Name = request.TagName };

        _postRepository.GetPostByIdAsync(postId).Returns(post);
        _tagRepository.GetByNameAsync(request.TagName).Returns((Tag?)null);
        _tagRepository.CreateAsync(Arg.Any<Tag>()).Returns(newTag);

        // Act
        await _postService.AddTagToPostAsync(postId, request);

        // Assert
        await _tagRepository.Received(1).GetByNameAsync(request.TagName);
        await _tagRepository.Received(1).CreateAsync(Arg.Is<Tag>(t => t.Name == request.TagName));
    }

    [Fact]
    public async Task AddTagToPostAsync_WithExistingTag_AddsExistingTagToPost()
    {
        // Arrange
        var user = CreateTestUser();
        var post = CreateTestPost("Test Post", "Test Body", user);
        var postId = post.Id;
        var request = new AddTagToPostRequest { TagName = "existingtag" };

        var existingTag = new Tag { Id = Guid.NewGuid(), Name = request.TagName };

        _postRepository.GetPostByIdAsync(postId).Returns(post);
        _tagRepository.GetByNameAsync(request.TagName).Returns(existingTag);

        // Act
        await _postService.AddTagToPostAsync(postId, request);

        // Assert
        await _postRepository.Received(1).GetPostByIdAsync(postId);
        await _tagRepository.Received(1).GetByNameAsync(request.TagName);
        await _tagRepository.DidNotReceive().CreateAsync(Arg.Any<Tag>());
        await _postRepository.Received(1).UpdatePostAsync(Arg.Is<Post>(p => p.Id == postId && p.Tags.Contains(existingTag)));
    }

    [Fact]
    public async Task AddTagToPostAsync_WithTagAlreadyOnPost_DoesNotAddDuplicate()
    {
        // Arrange
        var user = CreateTestUser();
        var existingTag = new Tag { Id = Guid.NewGuid(), Name = "existingtag" };
        var post = CreateTestPost("Test Post", "Test Body", user);
        post.Tags.Add(existingTag);
        var postId = post.Id;
        var request = new AddTagToPostRequest { TagName = "existingtag" };

        _postRepository.GetPostByIdAsync(postId).Returns(post);
        _tagRepository.GetByNameAsync(request.TagName).Returns(existingTag);

        // Act
        await _postService.AddTagToPostAsync(postId, request);

        // Assert
        await _postRepository.Received(1).GetPostByIdAsync(postId);
        await _tagRepository.Received(1).GetByNameAsync(request.TagName);
        await _postRepository.DidNotReceive().UpdatePostAsync(Arg.Any<Post>());
    }

    private static User CreateTestUser(string username = "testuser", string email = "test@testdomain.co.jun", bool isModerator = false)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = "hash",
            IsModerator = isModerator
        };
    }

    private static Post CreateTestPost(string title, string body, User user)
    {
        return new Post
        {
            Id = Guid.NewGuid(),
            Title = title,
            Body = body,
            UserId = user.Id,
            User = user,
            LikeCount = 0,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            Tags = [],
            Comments = [],
            Likes = []
        };
    }
}