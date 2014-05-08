using System.Windows;

namespace MultiAlign.Data
{
    public class ApplicationStatusMediator : DependencyObject
    {
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof (string), typeof (ApplicationStatusMediator));

        static ApplicationStatusMediator()
        {
            Mediator = new ApplicationStatusMediator();
        }

        public static ApplicationStatusMediator Mediator { get; set; }

        public string Status
        {
            get { return (string) GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static void SetStatus(string message)
        {
            Mediator.Status = message;
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
    }
}