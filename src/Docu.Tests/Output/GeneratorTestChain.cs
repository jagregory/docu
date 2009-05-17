using System;
using System.Collections.Generic;
using Docu.Generation;
using NUnit.Framework;

namespace Docu.Tests.Generation
{
    internal class GeneratorTestChain
    {
        private readonly InMemoryHtmlGenerator generator;
        private string body;
        private ViewData viewData;

        public GeneratorTestChain(InMemoryHtmlGenerator generator)
        {
            this.generator = generator;
        }

        public GeneratorTestChain FromTemplate(string templateBody)
        {
            body = templateBody;
            return this;
        }

        public GeneratorTestChain WithData(ViewData data)
        {
            viewData = data;
            return this;
        }

        public void ShouldEqual(string expectedBody)
        {
            var content = generator.Convert("template", viewData);

            Assert.That(content, Is.EqualTo(expectedBody));
        }
    }
}