using Pico.Infrastructure;
using Pico.Services;
using Pico.Tests.Support;
using Pico.ViewModels;

namespace Pico.Tests.Unit;

public sealed class MainWindowViewModelTests
{
    [Fact]
    public void Initialize_LoadsFolder_AndGeneratesName()
    {
        var fileNameService = new FileNameService(new TestClock(new DateTimeOffset(2026, 4, 20, 15, 32, 10, TimeSpan.Zero)));
        var fileRevealService = new SpyFileRevealService();
        var nativeNotificationService = new SpyNativeNotificationService();
        var viewModel = new MainWindowViewModel(
            new InMemorySettingsStore(@"C:\temp\images"),
            new StubImageSaveService(),
            fileNameService,
            fileRevealService,
            nativeNotificationService);

        viewModel.Initialize();

        Assert.Equal(@"C:\temp\images", viewModel.DefaultFolderPath);
        Assert.Equal("img-2026-04-20-153210.png", viewModel.FileName);
    }

    [Fact]
    public void ApplyClipboardImport_WithPayload_UpdatesState()
    {
        var fileNameService = new FileNameService(new TestClock(new DateTimeOffset(2026, 4, 20, 15, 32, 10, TimeSpan.Zero)));
        var fileRevealService = new SpyFileRevealService();
        var nativeNotificationService = new SpyNativeNotificationService();
        var viewModel = new MainWindowViewModel(
            new InMemorySettingsStore(@"C:\temp\images"),
            new StubImageSaveService(),
            fileNameService,
            fileRevealService,
            nativeNotificationService);
        var parser = new ClipboardTextParser();
        var payload = parser.Parse(TestImageData.PngBase64)!;

        viewModel.Initialize();
        viewModel.ApplyClipboardImport(new ClipboardImportResult(payload, "ready"));

        Assert.Equal("raw base64", viewModel.DetectedType);
        Assert.Equal("png", viewModel.SuggestedFormat);
        Assert.Equal("ready", viewModel.StatusMessage);
        Assert.True(viewModel.CanSave);
    }

    [Fact]
    public async Task SaveAsync_ShowsNotificationForSavedFile()
    {
        var fileNameService = new FileNameService(new TestClock(new DateTimeOffset(2026, 4, 20, 15, 32, 10, TimeSpan.Zero)));
        var fileRevealService = new SpyFileRevealService();
        var nativeNotificationService = new SpyNativeNotificationService();
        var saveService = new StubImageSaveService
        {
            SavedPath = @"C:\temp\images\img-2026-04-20-153210.png"
        };
        var viewModel = new MainWindowViewModel(
            new InMemorySettingsStore(@"C:\temp\images"),
            saveService,
            fileNameService,
            fileRevealService,
            nativeNotificationService);
        var parser = new ClipboardTextParser();

        viewModel.Initialize();
        viewModel.ApplyClipboardImport(new ClipboardImportResult(parser.Parse(TestImageData.PngBase64)!, "ready"));

        await viewModel.SaveAsync();

        Assert.True(viewModel.IsSaveNotificationVisible);
        Assert.Equal(@"C:\temp\images\img-2026-04-20-153210.png", viewModel.LastSavedFilePath);
        Assert.Equal("img-2026-04-20-153210.png", viewModel.SaveNotificationDescription);
        Assert.Equal(1, nativeNotificationService.NotifyCallCount);
        Assert.Equal(@"C:\temp\images\img-2026-04-20-153210.png", nativeNotificationService.LastPath);
    }

    [Fact]
    public async Task RevealLastSavedFileAsync_UsesRevealService_AndHidesNotification()
    {
        var fileNameService = new FileNameService(new TestClock(new DateTimeOffset(2026, 4, 20, 15, 32, 10, TimeSpan.Zero)));
        var fileRevealService = new SpyFileRevealService();
        var nativeNotificationService = new SpyNativeNotificationService();
        var saveService = new StubImageSaveService
        {
            SavedPath = @"C:\temp\images\img-2026-04-20-153210.png"
        };
        var viewModel = new MainWindowViewModel(
            new InMemorySettingsStore(@"C:\temp\images"),
            saveService,
            fileNameService,
            fileRevealService,
            nativeNotificationService);
        var parser = new ClipboardTextParser();

        viewModel.Initialize();
        viewModel.ApplyClipboardImport(new ClipboardImportResult(parser.Parse(TestImageData.PngBase64)!, "ready"));
        await viewModel.SaveAsync();
        await viewModel.RevealLastSavedFileAsync();

        Assert.Equal(@"C:\temp\images\img-2026-04-20-153210.png", fileRevealService.LastPath);
        Assert.Equal(1, fileRevealService.CallCount);
        Assert.False(viewModel.IsSaveNotificationVisible);
    }
}
