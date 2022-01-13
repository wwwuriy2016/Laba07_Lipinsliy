using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using AEDDES.Model;
using System.Linq;
using System.Text.RegularExpressions;

namespace AEDDES
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TypeCrypt TypeCrypt;
        private CryptoKey Key;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            string input = TextBoxInputFile.Text;
            string output = TextBoxOutputFile.Text;

            if (IsValidPath(input) == false || IsValidPath(output) == false)
            {
                MessageBox.Show("Невірно вказаний шлях файлу!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DateTime timeStart = DateTime.Now;

            Encrypting encrypting = new Encrypting(Key);
            

            if (RadiobuttonEncrypt.IsChecked == true)
            {
                encrypting.Encrypt(input, output, TypeCrypt);
                LabelResult.Content = "Файл зашифровано";
                LabelResult.Foreground = Brushes.Green;
            }
            else
            {
                try
                {
                    encrypting.Decrypt(input, output, TypeCrypt);
                    LabelResult.Content = "Файл розшифровано";
                    LabelResult.Foreground = Brushes.Green;
                }
                catch (Exception)
                {
                    MessageBox.Show("Неможливо розшифрувати файл!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
                    LabelResult.Content = "Помилка!";
                    LabelResult.Foreground = Brushes.Red;
                }
            }

            LabelOutEntropy.Content = "Ентропія: " + Entropy.CalculateEntropy(output);
            LabelOutputSize.Content = "Розмір: " + new FileInfo(output).Length + " Байт";
            Label_time.Content = (DateTime.Now - timeStart).ToString(@"hh\:mm\:ss\:fff");
        }
        private void ButtonInputFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "";
            dlg.Filter = "(*.txt)|*.txt|(*.docx)|*.docx";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                TextBoxInputFile.Text = dlg.FileName;
                LabelInputEntropy.Content = "Ентропія: " + Entropy.CalculateEntropy(dlg.FileName);
                LabelInputSize.Content = "Розмір: " + new FileInfo(dlg.FileName).Length + " Байт";
            }
        }
        private void ButtonOutputFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "";
            dlg.Filter = "(*.txt)|*.txt|(*.docx)|*.docx";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                TextBoxOutputFile.Text = dlg.FileName;
            }
        }
        private void TextBoxInputFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox texBox = sender as TextBox;
            if (IsValidPath(texBox.Text) == false)
            {
                texBox.BorderBrush = Brushes.Red;
                LabelResult.Content = "Невірний шлях до файлу!";
                LabelResult.Foreground = Brushes.Red;
            }
            else
            {
                LabelResult.Content = "";
                LabelResult.Foreground = Brushes.Green;
                texBox.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0078D7");
            }
        }

        private bool IsValidPath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            bool isValidPath = filePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            bool isValidDirecroty = Directory.Exists(Path.GetDirectoryName(filePath));
            return isValidPath & isValidDirecroty;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Key = new CryptoKey(int.Parse(ComboBoxKeyLength.Text));
            ButtonGenerateKey_Click(null, null);
            ButtonGeneratIV_Click(null, null);
            //RadioButtonAdamar.IsChecked = true;
        }
        private void ButtonGenerateKey_Click(object sender, RoutedEventArgs e)
        {
            //key = new KeyPermutation().Key;
            Key.GenerateKey();
            //TextBoxKey.Text = string.Join("-", Regex.Split(BitConverter.ToString(Key.Key).Replace("-", string.Empty), @"(?<=\G.{4})"));
            string str = BitConverter.ToString(Key.Key).Replace("-", string.Empty);
            TextBoxKey.Text = string.Join("-", str.Select((c, i) => new { c, i }).GroupBy(x => x.i / 4).Select(g => String.Join("", g.Select(y => y.c))));
        }
        private void ButtonGeneratIV_Click(object sender, RoutedEventArgs e)
        {
            Key.GenerateIV();
            string str = BitConverter.ToString(Key.IV).Replace("-", string.Empty);
            TextBoxIV.Text = string.Join("-", str.Select((c, i) => new { c, i }).GroupBy(x => x.i / 4).Select(g => String.Join("", g.Select(y => y.c))));
        }
        private void Window_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LabelResult.Content = "";
        }

        private void ComboBoxKeyLength_DropDownClosed(object sender, EventArgs e)
        {
            Window_Loaded(null, null);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (Enum.TryParse(rb.Content?.ToString(), out TypeCrypt res))
            {
                TypeCrypt = res;
                if (TypeCrypt == TypeCrypt.AES)
                {
                    ComboBoxKeyLength.Items.Add(new ComboBoxItem() { Content = "256" });
                }
                else
                {
                    ComboBoxKeyLength.Items.RemoveAt(2);
                    ComboBoxKeyLength.SelectedIndex = 0;
                    Window_Loaded(null, null);
                }
            }
        }

        private void buttonAuthor_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("студент 404 групи" + "\r\n" + "Липинский Юрий");
        }
    }
}
