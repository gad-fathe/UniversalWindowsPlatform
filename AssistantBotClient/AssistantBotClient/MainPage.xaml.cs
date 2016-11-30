using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Bot.Connector.DirectLine.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AssistantBotClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DirectLineClient _client;
        Conversation _conversation;
        ObservableCollection<Message> _messagesFromBot;
        public MainPage()
        {
            InitializeComponent();
            //Set binding context to update message list items:
            DataContext = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //Create collection for bot messages:
            _messagesFromBot = new ObservableCollection<Message>();
            //Initialize conversation with bot:
            await InitializeBotConversation();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            await sendMessageToBot();
        }

        //Handle button click when user wants to send message to bot:
        async Task sendMessageToBot()
        {
            //Message object with name of the user and text:
            Message userMessage = new Message
            {
                FromProperty = "Daniel",
                Text = NewMessageTextBox.Text
            };
            //Post message to your bot:
            if (_conversation != null)
                await _client.Conversations.PostMessageAsync(_conversation.ConversationId, userMessage);
        }

        async Task InitializeBotConversation()
        {
            //Initialize Direct Client with secret obtained in the Bot Portal:
            _client = new DirectLineClient("<<YOUR BOT SECRET>>");
            //Initialize new converstation:
            _conversation = await _client.Conversations.NewConversationAsync();
            //Wait for the responses from bot:
            await ReadBotMessagesAsync(_client, _conversation.ConversationId);
        }

        //This method is responsible for handling messages from bot:
        async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId)
        {
            //You can optionally set watermark - this is last message id seen by bot
            //It is for paging:
            string watermark = null;

            while (true)
            {
                //Get all messages returned by bot:
                var messages = await client.Conversations.GetMessagesAsync(conversationId, watermark);
                watermark = messages?.Watermark;

                //get messages from your bot - FromProperty should match your Bot Handle:
                var messagesFromBotText = from x in messages.Messages
                                          where x.FromProperty == "AssistantBot10"
                                          select x;

                //Iterate through all messages:
                foreach (Message message in messagesFromBotText)
                {
                    message.Text = "Daniel" + message.Text;
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                        {
                            //Add message to the list and update ListView source to display response on the UI:
                            if (!_messagesFromBot.Contains(message))
                                _messagesFromBot.Add(message);
                            MessagesList.ItemsSource = _messagesFromBot;
                        });
                }
                await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            }
        }

        private void NewMessageTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().CoreWindow.IsInputEnabled = false;
            }
        }
    }
}
