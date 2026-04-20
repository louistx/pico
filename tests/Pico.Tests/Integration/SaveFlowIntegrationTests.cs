using System.IO;
using Pico.Infrastructure;
using Pico.Services;
using Pico.Tests.Support;
using Pico.ViewModels;

namespace Pico.Tests.Integration;

public sealed class SaveFlowIntegrationTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task ImportThenSave_WritesImageToDisk()
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), "pico-tests", Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolder);

        try
        {
            var fileNameService = new FileNameService(new TestClock(new DateTimeOffset(2026, 4, 20, 15, 32, 10, TimeSpan.Zero)));
            var fileRevealService = new SpyFileRevealService();
            var nativeNotificationService = new SpyNativeNotificationService();
            var viewModel = new MainWindowViewModel(
                new InMemorySettingsStore(tempFolder),
                new ImageSaveService(fileNameService),
                fileNameService,
                fileRevealService,
                nativeNotificationService);
            var importService = new ClipboardImportService(new ClipboardTextParser());
            var clipboard = new FakeClipboardSource(text: TestImageData.PngDataUri);

            viewModel.Initialize();
            var result = await importService.ImportAsync(clipboard);
            viewModel.ApplyClipboardImport(result);
            await viewModel.SaveAsync();

            var savedFile = Path.Combine(tempFolder, "img-2026-04-20-153210.png");
            Assert.True(File.Exists(savedFile));
            Assert.Equal($"Salvo em: {savedFile}", viewModel.StatusMessage);
            Assert.True(viewModel.IsSaveNotificationVisible);
            Assert.Equal(savedFile, viewModel.LastSavedFilePath);
            Assert.Equal(savedFile, nativeNotificationService.LastPath);
        }
        finally
        {
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }
    }
}
