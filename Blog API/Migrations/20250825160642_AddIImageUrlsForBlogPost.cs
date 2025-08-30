using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog_API.Migrations
{
    /// <inheritdoc />
    public partial class AddIImageUrlsForBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blogPosts_AspNetUsers_UserId",
                table: "blogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTag_blogPosts_BlogPostsId",
                table: "BlogPostTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTag_tags_TagsId",
                table: "BlogPostTag");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_blogPosts_BlogPostId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_ParentCommentId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_AspNetUsers_UserId",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_blogPosts_BlogPostId",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_comments_CommentId",
                table: "likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tags",
                table: "tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_likes",
                table: "likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_comments",
                table: "comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_blogPosts",
                table: "blogPosts");

            migrationBuilder.RenameTable(
                name: "tags",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "likes",
                newName: "Likes");

            migrationBuilder.RenameTable(
                name: "comments",
                newName: "Comments");

            migrationBuilder.RenameTable(
                name: "blogPosts",
                newName: "BlogPosts");

            migrationBuilder.RenameIndex(
                name: "IX_likes_UserId_CommentId",
                table: "Likes",
                newName: "IX_Likes_UserId_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_likes_UserId_BlogPostId",
                table: "Likes",
                newName: "IX_Likes_UserId_BlogPostId");

            migrationBuilder.RenameIndex(
                name: "IX_likes_CommentId",
                table: "Likes",
                newName: "IX_Likes_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_likes_BlogPostId",
                table: "Likes",
                newName: "IX_Likes_BlogPostId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_ParentCommentId",
                table: "Comments",
                newName: "IX_Comments_ParentCommentId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_BlogPostId",
                table: "Comments",
                newName: "IX_Comments_BlogPostId");

            migrationBuilder.RenameIndex(
                name: "IX_blogPosts_UserId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_UserId");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserId",
                table: "BlogPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTag_BlogPosts_BlogPostsId",
                table: "BlogPostTag",
                column: "BlogPostsId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTag_Tags_TagsId",
                table: "BlogPostTag",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_BlogPosts_BlogPostId",
                table: "Comments",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_BlogPosts_BlogPostId",
                table: "Likes",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Comments_CommentId",
                table: "Likes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_UserId",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTag_BlogPosts_BlogPostsId",
                table: "BlogPostTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTag_Tags_TagsId",
                table: "BlogPostTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_BlogPosts_BlogPostId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_BlogPosts_BlogPostId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Comments_CommentId",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "BlogPosts");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "tags");

            migrationBuilder.RenameTable(
                name: "Likes",
                newName: "likes");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "comments");

            migrationBuilder.RenameTable(
                name: "BlogPosts",
                newName: "blogPosts");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_UserId_CommentId",
                table: "likes",
                newName: "IX_likes_UserId_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_UserId_BlogPostId",
                table: "likes",
                newName: "IX_likes_UserId_BlogPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_CommentId",
                table: "likes",
                newName: "IX_likes_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_BlogPostId",
                table: "likes",
                newName: "IX_likes_BlogPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "comments",
                newName: "IX_comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ParentCommentId",
                table: "comments",
                newName: "IX_comments_ParentCommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_BlogPostId",
                table: "comments",
                newName: "IX_comments_BlogPostId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_UserId",
                table: "blogPosts",
                newName: "IX_blogPosts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tags",
                table: "tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_likes",
                table: "likes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comments",
                table: "comments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_blogPosts",
                table: "blogPosts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_blogPosts_AspNetUsers_UserId",
                table: "blogPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTag_blogPosts_BlogPostsId",
                table: "BlogPostTag",
                column: "BlogPostsId",
                principalTable: "blogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTag_tags_TagsId",
                table: "BlogPostTag",
                column: "TagsId",
                principalTable: "tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_blogPosts_BlogPostId",
                table: "comments",
                column: "BlogPostId",
                principalTable: "blogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_ParentCommentId",
                table: "comments",
                column: "ParentCommentId",
                principalTable: "comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_likes_AspNetUsers_UserId",
                table: "likes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_likes_blogPosts_BlogPostId",
                table: "likes",
                column: "BlogPostId",
                principalTable: "blogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_likes_comments_CommentId",
                table: "likes",
                column: "CommentId",
                principalTable: "comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
