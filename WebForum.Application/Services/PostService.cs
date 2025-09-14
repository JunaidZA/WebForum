using WebForum.Application.Converters;
using WebForum.Application.DTOs;
using WebForum.Application.Interfaces;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;

namespace WebForum.Application.Services;

public class PostService(IPostRepository postRepository, IUserRepository userRepository, ITagRepository tagRepository) : IPostService
{
    public async Task<IEnumerable<PostDto>> GetPostsAsync()
    {
        var posts = await postRepository.GetPostsAsync().ConfigureAwait(false);
        return posts.Select(PostConverter.MapToDto);
    }

    public async Task<PagedResult<PostDto>> GetPostsAsync(PostFilterRequest postFilterRequest)
    {
        IEnumerable<string>? tags = null;
        if (!string.IsNullOrWhiteSpace(postFilterRequest.Tags))
        {
            tags = postFilterRequest.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(t => t.Trim())
                              .Where(t => !string.IsNullOrEmpty(t));
        }

        var sortBy = postFilterRequest.SortBy switch
        {
            PostSortField.Likes => "Likes",
            _ => "CreatedDate"
        };

        var sortDescending = postFilterRequest.SortDirection == SortDirection.Desc;
        var (posts, totalCount) = await postRepository.GetPostsAsync(
            postFilterRequest.Page,
            postFilterRequest.PageSize,
            postFilterRequest.Author,
            tags,
            postFilterRequest.FromDate,
            postFilterRequest.ToDate,
            sortBy,
            sortDescending).ConfigureAwait(false);

        var postDtos = posts.Select(PostConverter.MapToDto);

        return new PagedResult<PostDto>
        {
            Items = postDtos,
            Page = postFilterRequest.Page,
            PageSize = postFilterRequest.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PostDto> CreatePostAsync(string title, string body, Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(userId));
        }

        var post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createdPost = await postRepository.CreatePostAsync(post).ConfigureAwait(false);
        return PostConverter.MapToDto(createdPost);
    }

    public async Task AddCommentAsync(Guid postId, CreateCommentRequest request, Guid userId)
    {
        var post = await postRepository.GetPostByIdAsync(postId).ConfigureAwait(false);
        if (post == null)
        {
            throw new ArgumentException("Post not found", nameof(postId));
        }

        var user = await userRepository.GetByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(userId));
        }

        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Body = request.Body,
            User = user,
            Post = post
        };

        _ = await postRepository.AddCommentAsync(comment).ConfigureAwait(false);
    }

    public async Task AddLikeAsync(Guid postId, Guid userId)
    {
        var post = await postRepository.GetPostByIdAsync(postId).ConfigureAwait(false);
        if (post == null)
        {
            throw new ArgumentException("Post not found", nameof(postId));
        }

        var user = await userRepository.GetByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(userId));
        }

        if (post.UserId == userId)
        {
            throw new InvalidOperationException("Users cannot like their own posts");
        }

        var existingLike = await postRepository.GetLikeAsync(postId, userId).ConfigureAwait(false);
        if (existingLike != null)
        {
            return;
        }

        var like = new Like
        {
            PostId = postId,
            UserId = userId,
            Post = post,
            User = user
        };

        _ = await postRepository.AddLikeAsync(like).ConfigureAwait(false);

        post.LikeCount++;
        _ = await postRepository.UpdatePostAsync(post).ConfigureAwait(false);
    }

    public async Task RemoveLikeAsync(Guid postId, Guid userId)
    {
        var post = await postRepository.GetPostByIdAsync(postId).ConfigureAwait(false);
        if (post == null)
        {
            throw new ArgumentException("Post not found", nameof(postId));
        }

        var existingLike = await postRepository.GetLikeAsync(postId, userId).ConfigureAwait(false);
        if (existingLike == null)
        {
            return;
        }

        await postRepository.RemoveLikeAsync(postId, userId).ConfigureAwait(false);

        post.LikeCount = Math.Max(0, post.LikeCount - 1);
        _ = await postRepository.UpdatePostAsync(post).ConfigureAwait(false);
    }

    public async Task AddTagToPostAsync(Guid postId, AddTagToPostRequest addTagToPostRequest)
    {
        var post = await postRepository.GetPostByIdAsync(postId).ConfigureAwait(false);
        if (post == null)
        {
            throw new ArgumentException("Post not found", nameof(postId));
        }

        var existingTag = await tagRepository.GetByNameAsync(addTagToPostRequest.TagName).ConfigureAwait(false);
        if (existingTag != null)
        {
            if (!post.Tags.Any(t => t.Id == existingTag.Id))
            {
                post.Tags.Add(existingTag);
                _ = await postRepository.UpdatePostAsync(post).ConfigureAwait(false);
            }
        }
        else
        {
            var tagToCreate = new Tag { Name = addTagToPostRequest.TagName };
            var createdTag = await tagRepository.CreateAsync(tagToCreate).ConfigureAwait(false);
            post.Tags.Add(createdTag);
            _ = await postRepository.UpdatePostAsync(post).ConfigureAwait(false);
        }
    }
}
