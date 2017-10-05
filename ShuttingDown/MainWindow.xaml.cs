using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace ShuttingDown
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool apagar = true;
        bool reiniciar = false;
        bool lastMessage = false;

        bool cancel = false;
        int count = 0;


        public MainWindow()
        {
            InitializeComponent();
            
        }        

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            

            if (button1.Content.Equals("Cancelar"))
            { 
                button1.Content = "Comenzar";
                cancel = true;
                count = 0;

                comboBox1.IsEnabled = true;
                Apagar.IsEnabled = true;
                Reiniciar.IsEnabled = true;
            }
            else {
                cancel = false; 
                button1.Content = "Cancelar";
                var selectedValue = ((ComboBoxItem)comboBox1.SelectedItem).Content.ToString();
                count = Convert.ToInt32(selectedValue)*60;

                comboBox1.IsEnabled = false;
                Apagar.IsEnabled = false;
                Reiniciar.IsEnabled = false;
            }
            string accion = null;
            if (apagar || reiniciar)
            {
                accion = apagar ? "apagar" : "reiniciar";
            }
            
            //tbTime.Text = "00:00:01";
            Countdown(/*15,*/ TimeSpan.FromSeconds(1), cur => tbTime.Text = GenTimeSpanFromMillisec(Convert.ToInt32(cur.ToString())*1000)/*cur.ToString()*/);
            var test = accion;
        }

        public string GenTimeSpanFromMillisec(Double millisec)
        {
            // Create a TimeSpan object and TimeSpan string from 
            // a number of milliseconds.
            TimeSpan interval = TimeSpan.FromMilliseconds(millisec);
            string timeInterval = interval.ToString();

            // Pad the end of the TimeSpan string with spaces if it 
            // does not contain milliseconds.
            int pIndex = timeInterval.IndexOf(':');
            pIndex = timeInterval.IndexOf('.', pIndex);
            if (pIndex < 0) timeInterval += "        ";

            
            return timeInterval;
        } 

        void Countdown(/*int count,*/ TimeSpan interval, Action<int> ts)
        {
            var dt = new System.Windows.Threading.DispatcherTimer();
            dt.Interval = interval;
            dt.Tick += (_, a) =>
            {

                if (lastMessage) {
                    dt.Stop();
                    Thread.Sleep(2000);
                    System.Windows.Application.Current.Shutdown();
                }
                else {
                    //Console.WriteLine(count);
                    if (count-- == 0 || count < 0)
                    {
                        if (cancel == true) { Console.WriteLine("Se canceló"); dt.Stop(); }
                        else
                        {
                            button1.Content = apagar ? "El computador se apagará" : "El computador se reiniciará"; ;
                            if (apagar)
                            { /*Shutdown (/s)*/
                                System.Diagnostics.Process.Start("shutdown.exe", "-s");
                            }
                            else
                            { /*Shutdown and Restart (/r)*/
                                System.Diagnostics.Process.Start("shutdown.exe", "-r");
                            }
                            lastMessage = true;
                            
                        }
                    }
                    else
                    {
                        ts(count);
                    }
                }
            };
            ts(count);
            dt.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {
            apagar = true;
            reiniciar = false;
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            apagar = false;
            reiniciar = true;
        }
    }
}
