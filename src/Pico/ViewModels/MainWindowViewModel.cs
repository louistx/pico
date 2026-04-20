using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Pico.Abstractions;
using Pico.Services;

namespace Pico.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly IAppSettingsStore _settingsStore;
    private readonly IImageSaveService _imageSaveService;
    private readonly IFileNameService _fileNameService;
    private readonly IFileRevealService _fileRevealService;
    private readonly INativeNotificationService _nativeNotificationService;
    private ClipboardImagePayload? _currentPayload;
    private string _detectedType = "Nenhum";
    private string _suggestedFormat = "png";
    private string _statusMessage = "Cole algo para comecar.";
    private string? _defaultFolderPath;
    private string _fileName;
    private string? _lastSavedFilePath;
    private bool _isSaveNotificationVisible;

    public MainWindowViewModel(
        IAppSettingsStore settingsStore,
        IImageSaveService imageSaveService,
        IFileNameService fileNameService,
        IFileRevealService fileRevealService,
        INativeNotificationService nativeNotificationService)
    {
        _settingsStore = settingsStore;
        _imageSaveService = imageSaveService;
        _fileNameService = fileNameService;
        _fileRevealService = fileRevealService;
        _nativeNotificationService = nativeNotificationService;
        _fileName = _fileNameService.Generate("png");
    }

    public string DetectedType
    {
        get => _detectedType;
        private set => SetProperty(ref _detectedType, value);
    }

    public string SuggestedFormat
    {
        get => _suggestedFormat;
        private set => SetProperty(ref _suggestedFormat, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string? DefaultFolderPath
    {
        get => _defaultFolderPath;
        private set
        {
            if (SetProperty(ref _defaultFolderPath, value))
            {
                RaisePropertyChanged(nameof(CanSave));
            }
        }
    }

    public string FileName
    {
        get => _fileName;
        set => SetProperty(ref _fileName, value);
    }

    public bool IsSaveNotificationVisible
    {
        get => _isSaveNotificationVisible;
        private set => SetProperty(ref _isSaveNotificationVisible, value);
    }

    public string SaveNotificationTitle => "Arquivo salvo";

    public string SaveNotificationDescription =>
        string.IsNullOrWhiteSpace(_lastSavedFilePath)
            ? string.Empty
            : Path.GetFileName(_lastSavedFilePath);

    public string? LastSavedFilePath
    {
        get => _lastSavedFilePath;
        private set
        {
            if (SetProperty(ref _lastSavedFilePath, value))
            {
                RaisePropertyChanged(nameof(SaveNotificationDescription));
                RaisePropertyChanged(nameof(CanRevealSavedFile));
            }
        }
    }

    public bool CanRevealSavedFile => !string.IsNullOrWhiteSpace(LastSavedFilePath);

    public bool CanSave => _currentPayload is not null && !string.IsNullOrWhiteSpace(DefaultFolderPath);

    public void Initialize()
    {
        var settings = _settingsStore.Load();
        DefaultFolderPath = settings.DefaultFolderPath;
        FileName = _fileNameService.Generate(SuggestedFormat);
    }

    public void SetDefaultFolder(string folderPath)
    {
        DefaultFolderPath = folderPath;
        _settingsStore.Save(new AppSettings(folderPath));
        StatusMessage = $"Pasta padrao: {folderPath}";
    }

    public void ApplyClipboardImport(ClipboardImportResult result)
    {
        if (!result.HasPayload)
        {
            StatusMessage = result.StatusMessage;
            return;
        }

        var payload = result.Payload!;
        _currentPayload = payload;
        DetectedType = payload.SourceType;
        SuggestedFormat = payload.FileExtension;
        FileName = _fileNameService.Generate(payload.FileExtension);
        StatusMessage = result.StatusMessage;
        RaisePropertyChanged(nameof(CanSave));
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (_currentPayload is null)
        {
            StatusMessage = "Nada para salvar ainda.";
            return;
        }

        var folderPath = DefaultFolderPath?.Trim();
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            StatusMessage = "Escolha pasta padrao antes de salvar.";
            return;
        }

        var fullPath = await _imageSaveService.SaveAsync(_currentPayload, folderPath, FileName, cancellationToken);
        StatusMessage = $"Salvo em: {fullPath}";
        LastSavedFilePath = fullPath;
        IsSaveNotificationVisible = true;
        await _nativeNotificationService.NotifyFileSavedAsync(fullPath, cancellationToken);
        FileName = _fileNameService.Generate(_currentPayload.FileExtension);
    }

    public void DismissSaveNotification()
    {
        IsSaveNotificationVisible = false;
    }

    public async Task RevealLastSavedFileAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(LastSavedFilePath))
        {
            StatusMessage = "Nenhum arquivo salvo para abrir.";
            return;
        }

        try
        {
            await _fileRevealService.RevealAsync(LastSavedFilePath, cancellationToken);
            StatusMessage = $"Abrindo pasta: {LastSavedFilePath}";
            IsSaveNotificationVisible = false;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Falha ao abrir pasta: {ex.Message}";
        }
    }

    public void Dispose()
    {
    }
}
