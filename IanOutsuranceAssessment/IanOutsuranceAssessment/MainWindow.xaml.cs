using Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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

namespace IanOutsuranceAssessment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Private Methods

        private void Question1()
        {

            try
            {

                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@FileName", txtFileName.Text);

                DataSet ds = SQLHelper.GetDataFromStoredProcedure("spQuestion1", parameters);

                if (ds.Tables.Count == 0)
                {
                    MessageBox.Show("There is no data available to be written to the resulting text file", "No Data", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }

                StringBuilder sb = new StringBuilder();

                foreach(DataRow row in ds.Tables[0].Rows)
                {
                    sb.AppendLine(String.Format("{0}\t{1}", row[0], row[1]));
                }

                string fileName = String.Format("{0}\\Question1 {1}.txt", FileIOHelper.ExtractPathFromFullFileName(txtFileName.Text), DateTime.Now.ToString("yyyy-MM-dd HHmmss"));

                string result = sb.ToString();

                File.WriteAllText(fileName, result, ASCIIEncoding.ASCII);

                if (MessageBox.Show(String.Format("A new text file has been created at {0}. Would you like to view it now?", FileIOHelper.ExtractPathFromFullFileName(txtFileName.Text)), "New File Created", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(fileName);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Question2()
        {
            try
            {

                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@FileName", txtFileName.Text);

                DataSet ds = SQLHelper.GetDataFromStoredProcedure("spQuestion2", parameters);

                if (ds.Tables.Count == 0)
                {
                    MessageBox.Show("There is no data available to be written to the resulting text file", "No Data", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }

                dgQuestion2.ItemsSource = ds.Tables[0].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

}
        #endregion Private Methods

        #region Event Handlers
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Comma Separated Values (*.csv)|*.csv";
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                txtFileName.Text = ofd.FileName;
                //chkIncludesColumnHeaders.IsChecked = true;
                //UpdateFieldValues();
            }
        }

        #endregion Event Handlers

        private void btnQuestion1_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtFileName.Text))
            {
                MessageBox.Show("Please specify the file name that needs to be parsed.", "No input file specified", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Question1();
        }

        private void btnQuestion2_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtFileName.Text))
            {
                MessageBox.Show("Please specify the file name that needs to be parsed.", "No input file specified", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Question2();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("OUTSurance - You ALWAYS get something out!", "Goodbye!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            Close();
        }

        
    }
}
