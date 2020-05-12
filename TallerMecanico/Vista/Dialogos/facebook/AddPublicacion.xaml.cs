using Facebook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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


namespace TallerMecanico.Vista.Dialogos.facebook
{
    /// <summary>
    /// Lógica de interacción para AddPublicacion.xaml
    /// </summary>
    public partial class AddPublicacion : Window
    {
        FacebookClient client;
        public AddPublicacion()
        {
            InitializeComponent();


                 client = new FacebookClient("EAAL6fuCJBIIBAM4xpauZCiID0q0Epk5Y08p6YrRKNp8aRestoqFEHrAUREVa6AcTySNyr1up2H1T1oHfIeJbucqfxsZA9kpEODlLP5Woq4yUAvdQVMzw1ygHYo2ZA87YvexG2sLiHZCGstHXZCWaDJLmdqgbdJoCljbjgh9EazpG4W7dGs4CDNfatNmHBIWyoDoBaBvuaTfXow53TqOn1QOZCSprioZBPjtuhiLXz3vPwZDZD");


         

               //client.Post("me/feed", new { message = "tutorial" });
               
            
            
        }
        private void publicar()
        {

            string token = "EAAL6fuCJBIIBAOy1Sz4G6ZCNmpAAdzvbLMHRwbIzwaFyZAaLFqc1y6ZAnbNE7dG5SgaQZBtw6YYouyBY6lDuKORbpdWwovfeLWDauZCJ60qaqZBRsFx2cftz7KcJrDQXdOAVbhgZAoaWI8fZByYJDanizT96fKJWre6IPoByMdESQcofneGsG8rlWE2jP5zjfFmJLZBW2y5ZABaDOQZCl5rm7kaBESgHIll74x57Ri8NoLPxwZDZD";

            string app_id = "838372793320578";
            string app_secret = "876e9e4ebd83fc4d325056344720c403";
            client = new FacebookClient();
           
           
        }

        private void WBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.ToString().StartsWith("http://www.Facebook.com/connect/login_sucess.html"))
            {
                MessageBox.Show(e.Uri.Fragment);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
