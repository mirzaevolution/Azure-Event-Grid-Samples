using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ServiceBusInvoker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _serviceBusConnectionString = "Endpoint=sb://mgrevo2.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=VJ7jCzLtCeNVnNFF97vAg//k6a9iCPzg4VAQo5DpGaI=";
        private readonly string _queueName = "message_queue";
        private QueueClient _queueClient;

        public MainWindow()
        {
            _queueClient = new QueueClient(_serviceBusConnectionString, _queueName);
            InitializeComponent();

        }

        private async void SubmitClickHandler(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Message cannot be null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {

                    MessageItem messageItem = new MessageItem
                    {
                        MessageContent = message
                    };
                    byte[] messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageItem));
                    Message messagePayload = new Message(messageBytes);
                    await _queueClient.SendAsync(messagePayload);
                    MessageBox.Show("Message sent successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    MessageTextBox.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
