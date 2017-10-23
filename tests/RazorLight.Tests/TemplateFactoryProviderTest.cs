﻿using RazorLight.Compilation;
using RazorLight.Razor;
using Xunit;

namespace RazorLight.Tests
{
    public class TemplateFactoryProviderTest
    {
        private static EmbeddedRazorProject project = new EmbeddedRazorProject(typeof(TemplateFactoryProviderTest));

        private TemplateFactoryProvider GetProvider()
        {
            var sourceGenerator = new RazorSourceGenerator(new EngineFactory().RazorEngine, project);
            var metadataReferences = new DefaultMetadataReferenceManager();
            var compiler = new RoslynCompilationService(metadataReferences);

            var provider = new TemplateFactoryProvider(sourceGenerator, compiler, new RazorLightOptions());

            return provider;
        }

        [Fact]
        public void Throws_On_NullTemplateKey_ForTemplateKey()
        {
            var provider = GetProvider();

            Assert.ThrowsAsync<System.ArgumentNullException>(async () => await provider.CreateFactoryAsync(templateKey: null));
        }

        [Fact]
        public void Throws_On_NullTemplateKey_ForProjectItem()
        {
            var provider = GetProvider();

            Assert.ThrowsAsync<System.ArgumentNullException>(async () => await provider.CreateFactoryAsync(projectItem: null));
        }

        [Fact]
        public void Returns_Diagnostics_OnErrors()
        {
            var provider = GetProvider();

            TemplateGenerationException ex = null;

            try
            {
                provider.CreateFactoryAsync("Assets.Embedded.WrongRazorSyntax").GetAwaiter().GetResult();
            }
            catch (TemplateGenerationException exception)
            {
                ex = exception;
            }

            Assert.NotNull(ex);
            Assert.NotEmpty(ex.Diagnostics);
        }

        [Fact]
        public void Ensure_FactoryReturnsValidTemplateType()
        {
            var provider = GetProvider();
            string templateKey = "Assets.Embedded.Empty";

            TemplateFactoryResult result = provider.CreateFactoryAsync(templateKey).GetAwaiter().GetResult();
            var templatePage = result.TemplatePageFactory();

            Assert.NotNull(result.TemplateDescriptor);
            Assert.NotNull(result.TemplatePageFactory);
            Assert.NotNull(templatePage);

            Assert.IsAssignableFrom<TemplatePage>(templatePage);
        }
    }
}
