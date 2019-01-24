using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
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
            var speaker = "James";
            await SayWithTheVoice(
                $"Hello {txtName.Text}!",
                speaker);



            var whatWasSaid = await Listen();
            if (whatWasSaid.Contains("coffee"))
            {
                await SayWithTheVoice("I'm sorry, I don't make coffee", speaker);
            }
            else if (whatWasSaid.Contains("chocolate"))
            {
                await SayWithTheVoice("Coming right up!", speaker);
            }
            else if (whatWasSaid.Contains("joke"))
            {
                await SayWithTheVoice("Knock Kock", speaker);

                await Listen();

                await SayWithTheVoice("Tank", speaker);

                var response = await Listen();

                if (response.Contains("tank"))
                {
                    await SayWithTheVoice("You're welcome", speaker);
                }

            }
            else
            {
                await SayWithTheVoice("I'm confused", speaker);
            }
        }

        private static async Task<string> Listen()
        {
// Create an instance of SpeechRecognizer.
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();

            // Compile the dictation grammar by default.
            await speechRecognizer.CompileConstraintsAsync();

            // Start recognition.
            Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult =
                await speechRecognizer.RecognizeWithUIAsync();

            // Do something with the recognition result.
            //var messageDialog = new Windows.UI.Popups.MessageDialog(speechRecognitionResult.Text, "Text spoken");
            //await messageDialog.ShowAsync();

            var whatWasSaid = speechRecognitionResult.Text;
            return whatWasSaid;
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
                mediaElement.MediaEnded += (o, e) =>
                {
                    tsc.TrySetResult(true);
                };
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
            //CameraCaptureUI captureUI = new CameraCaptureUI();
            //captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            //captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

            //StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            //if (photo == null)
            //{
            //    // User cancelled photo capture
            //    return;
            //}

            StorageFolder destinationFolder =
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProfilePhotoFolder",
                    CreationCollisionOption.OpenIfExists);


            //await photo.CopyAsync(destinationFolder, "ProfilePhoto.jpg", NameCollisionOption.ReplaceExisting);
            //await photo.DeleteAsync();

            MakeRequest(destinationFolder.Path + @"\ProfilePhoto.jpg");

        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        const string uriBase =
            "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect";

        static async void MakeRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", "7f573507036e4912a71acc101526db5d");

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                                       "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                                       "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Console.WriteLine("\nResponse:\n");
                //Console.WriteLine(JsonPrettyPrint(contentString));
                Console.WriteLine("\nPress Enter to exit...");
            }
        }
    }
}