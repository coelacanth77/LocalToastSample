using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LocalToastSample.Model;
using System.Windows.Input;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace LocalToastSample.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                if (_welcomeTitle == value)
                {
                    return;
                }

                _welcomeTitle = value;
                RaisePropertyChanged(WelcomeTitlePropertyName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    WelcomeTitle = item.Title;
                });
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public ICommand startToast
        {
            get
            {
                return new RelayCommand<string>(async (p) =>
                {
                    // トースト用のテンプレートを選択
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                    // テンプレートの各要素を指定
                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode("ローカルトースト通知(ノーマル)"));

                    XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                    ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "https://si0.twimg.com/profile_images/2508237913/rctgk6cpgea29ejboclt.png");
                    ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "profile icon");

                    // 表示間隔をロングに(デフォルトはショート)
                    IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                    ((XmlElement)toastNode).SetAttribute("duration", "long");

                    // トースト表示時の音声をオフに
                    toastNode = toastXml.SelectSingleNode("/toast");
                    XmlElement audio = toastXml.CreateElement("audio");
                    audio.SetAttribute("silent", "true");

                    toastNode.AppendChild(audio);

                    // トーストをクリックした場合にアプリを起動する
                    // ローカル通知の場合はあまり関係ない
                    // 下記のようにパラメーターを渡すこともできる
                    ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"param1\":\"12345\",\"param2\":\"67890\"}");

                    // 表示
                    ToastNotification toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                });
            }
        }

        public ICommand startExtensionToast
        {
            get
            {
                return new RelayCommand<string>(async (p) =>
                {
                    var toastContent = NotificationsExtensions.ToastContent.ToastContentFactory.CreateToastImageAndText01();
                    toastContent.TextBodyWrap.Text = "ローカルトースト通知(Extention)";
                    
                    toastContent.Image.Src = "https://si0.twimg.com/profile_images/2508237913/rctgk6cpgea29ejboclt.png";
                    toastContent.Image.Alt = "profile icon";

                    toastContent.Duration = NotificationsExtensions.ToastContent.ToastDuration.Long;
                    toastContent.Audio.Content = NotificationsExtensions.ToastContent.ToastAudioContent.Silent;

                    toastContent.Launch = "{\"type\":\"toast\",\"param1\":\"12345\",\"param2\":\"67890\"}";

                    var toast = toastContent.CreateNotification();
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                });
            }
        }

    }
}