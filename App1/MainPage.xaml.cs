using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Popups;
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
            await SayWithTheVoice("Hello, I am MJ, your personal AI.  You are looking fine today.  How can I help?", "James");
        }

        private static async Task SayWithTheVoice(string text, string speaker)
        {
            var mediaElement = new MediaElement();
            using (var synth = new SpeechSynthesizer())
            {
                var voices = SpeechSynthesizer.AllVoices.ToArray();

                var theVoice = voices.First(voice => voice.DisplayName.Contains(speaker));
                synth.Voice = theVoice;

                var stream = await synth.SynthesizeTextToStreamAsync(text);
                mediaElement.SetSource(stream, stream.ContentType);

                mediaElement.Play();
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await SayWithTheVoice("Hello, I am Sam, The Tip of the Sword, and the better looking AI.  You're looking fine today.  How can I help?", "Mark");

        }
    }
}
