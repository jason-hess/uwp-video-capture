using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private static int _voiceToUse;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await SayWithTheVoice(
                $"Hello {txtName.Text}! I am MJ, your personal AI.  You're looking quite dashing today!  How can I help?",
                "James");
        }

        private async Task SayWithTheVoice(string text, string speaker)
        {
            var mediaElement = medElement;
            using (var synth = new SpeechSynthesizer())
            {
                var voices = SpeechSynthesizer.AllVoices.ToArray();

                var theVoice = voices.First(voice => voice.DisplayName.Contains(speaker));
                synth.Voice = theVoice;

                var stream = await synth.SynthesizeTextToStreamAsync(text);
                mediaElement.SetSource(stream, stream.ContentType);

                var tsc = new TaskCompletionSource<bool>();
                mediaElement.MediaEnded += (o, e) => { tsc.TrySetResult(true); };
                mediaElement.Play();
                await tsc.Task;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await SayWithTheVoice(
                $"Hello {txtName.Text}, I am Sam, The Tip of the Sword, and the better looking AI.  You're looking fine today.  How can I help?",
                "Mark");


            // Create an instance of SpeechRecognizer.
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();

            // Compile the dictation grammar by default.
            await speechRecognizer.CompileConstraintsAsync();

            // Start recognition.
            Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeAsync();

            // Do something with the recognition result.
            //var messageDialog = new Windows.UI.Popups.MessageDialog(speechRecognitionResult.Text, "Text spoken");
            //await messageDialog.ShowAsync();

            if (speechRecognitionResult.Text.Contains("coffee"))
            {
                await SayWithTheVoice("I'm sorry, I don't make coffee", "Mark");
            }
            else if (speechRecognitionResult.Text.Contains("chocolate"))
            {
                await SayWithTheVoice("Coming right up!", "Mark");
            }
            else
            {
                await SayWithTheVoice("I'm confused", "Mark");
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await SayWithTheVoice(
                $"{txtName.Text}",
                "Mark");

        }

        private async void MainPage_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!await AudioCapturePermissions.RequestMicrophonePermission())
            {
                return;
            }

        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
        }
    }
}