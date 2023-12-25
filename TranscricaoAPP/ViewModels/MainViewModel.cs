using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Media;

namespace TranscricaoAPP.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    string texto;

    private readonly ISpeechToText speechToText;

    public MainViewModel(ISpeechToText speechToText)
    {
        this.speechToText = speechToText;
    }

    [RelayCommand]
    public async Task Listen(CancellationToken cancellationToken)
    {
        Texto = "";
        try
        {
            var isGranted = await speechToText.RequestPermissions(cancellationToken);
            if (!isGranted)
            {
                await Toast.Make("Permission not granted").Show(CancellationToken.None);
                return;
            }

            var recognitionResult = await speechToText.ListenAsync(
                                                CultureInfo.GetCultureInfo("pt-br"),
                                                new Progress<string>(partialText =>
                                                {
                                                    Texto += partialText + " ";
                                                }), cancellationToken);

            if (recognitionResult.IsSuccessful)
            {
                Texto = recognitionResult.Text;
            }
            else
            {
                await Toast.Make(recognitionResult.Exception?.Message ?? "Unable to recognize speech").Show(CancellationToken.None);
            }
        }
        catch(Exception ex)
        {
            await Toast.Make(ex.Message).Show(CancellationToken.None);
        }
        
    }
}
