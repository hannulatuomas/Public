using System.Windows;
using System.Windows.Controls;

namespace IntegerToRomanConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Convert_Click(object sender, RoutedEventArgs e)
        {
            Converter converter = new Converter();
            string text = Textbox_Input.Text;

            if (int.TryParse(text, out int value))
            {
                string roman = converter.ConvertToRoman(value);
                this.Textbox_Output.Text = roman;
            }
        }

        private void Textbox_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = Textbox_Input.Text;

            if (!int.TryParse(text, out int value))
            {
                Textbox_Input.Text = "1";
                MessageBox.Show("Only integers allowed!");
            }
            else if (value < 1 || value > 3999)
            {
                Textbox_Input.Text = "1";
                MessageBox.Show("Value should be: 1 <= num <= 3999 !");
            }
        }
    }
}
