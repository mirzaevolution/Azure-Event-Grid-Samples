using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;

namespace CustomEventGridInvoker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _eventGridTopicPrimaryKey = "HTq2GnfDYAVV55n/ScKe5/eRgIKtR8L9w5REgX//cEI=";
        private readonly string _eventGridTopicHost = "https://azevgridtopic.southeastasia-1.eventgrid.azure.net/api/events";


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SubmitClickHandler(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FirstNameTextBox.Text.Trim()) ||
                string.IsNullOrEmpty(LastNameTextBox.Text.Trim()) ||
                string.IsNullOrEmpty(EmailTextBox.Text.Trim()))
            {
                MessageBox.Show("Please complete all fields", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string jsonData = JsonConvert.SerializeObject(
                        new Profile
                        {
                            FirstName = FirstNameTextBox.Text.Trim(),
                            LastName = LastNameTextBox.Text.Trim(),
                            Email = EmailTextBox.Text.Trim()

                        }
                    );
                if (await SendMessageV2(jsonData))
                {
                    MessageBox.Show("Data has been submitted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    FirstNameTextBox.Text = "";
                    LastNameTextBox.Text = "";
                    EmailTextBox.Text = "";
                }
                else
                {
                    MessageBox.Show("Failed to submit data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private async Task<bool> SendMessage(string message)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("aeg-sas-key", _eventGridTopicPrimaryKey);
                    string id = Guid.NewGuid().ToString();

                    EventGridEvent eventGridEvent = new EventGridEvent(
                        id: id,
                        subject: $"customevent/profile/{id}",
                        data: (object)message,
                        eventType: "Custom.Profile",
                        eventTime: DateTime.Now,
                        dataVersion: "1.0.0"
                    );
                    List<EventGridEvent> eventGridEvents = new List<EventGridEvent>()
                    {
                        eventGridEvent
                    };
                    var response = await httpClient.PostAsync(_eventGridTopicHost,
                        new StringContent(JsonConvert.SerializeObject(eventGridEvents), Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        return true;

                    }
                    return false;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        private async Task<bool> SendMessageV2(string message)
        {


            try
            {
                TopicCredentials topicCredentials = new TopicCredentials(_eventGridTopicPrimaryKey);
                EventGridClient eventGridClient = new EventGridClient(topicCredentials);
                string id = Guid.NewGuid().ToString();

                EventGridEvent eventGridEvent = new EventGridEvent(
                    id: id,
                    subject: $"customevent/profile/{id}",
                    data: (object)message,
                    eventType: "Custom.Profile",
                    eventTime: DateTime.Now,
                    dataVersion: "1.0.0"
                );
                List<EventGridEvent> eventGridEvents = new List<EventGridEvent>()
                    {
                        eventGridEvent
                    };
                await eventGridClient.PublishEventsAsync(new Uri(_eventGridTopicHost).Host, eventGridEvents);
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
