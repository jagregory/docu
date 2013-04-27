using System;
using System.Collections.Generic;
using System.Linq;
using Docu.Documentation.Comments;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    public static class CommentAssertionExtensions
    {
        public static void ShouldMatchStructure<T>(this T comment, Action<CommentBuilder> builderAction)
            where T : Comment, new()
        {
            var builder = new CommentBuilder();

            builderAction(builder);

            builder.VerifyExpectations(comment);
        }

        public class CommentBuilder
        {
            readonly List<Action<Comment>> assertions = new List<Action<Comment>>();

            public void InlineText(string text)
            {
                var currentCount = assertions.Count;

                assertions.Add(comment =>
                {
                    var child = comment.Children.ElementAtOrDefault(currentCount);

                    child.ShouldNotBeNull();
                    child.ShouldBeOfType<InlineText>();
                    ((InlineText)child).Text.ShouldEqual(text);
                });
            }

            public void VerifyExpectations(Comment comment)
            {
                foreach (var assertion in assertions)
                    assertion(comment);
            }
        }
    }
}