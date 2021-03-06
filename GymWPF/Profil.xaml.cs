﻿using System;
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
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Animation;

namespace GymWPF
{
    /// <summary>
    /// Interaction logic for Profil.xaml
    /// </summary>
    public partial class Profil : Page
    {
        //Declaration -------------------------//
        SqlConnection cn = new SqlConnection("Data Source=.;Initial Catalog=NSS_Salle_Application;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter("", "Data Source=.;Initial Catalog=NSS_Salle_Application;Integrated Security=True");
        DataSet ds = new DataSet();
        SqlDataReader dr;
        //------------------------------------//

        string ConnectedSalle, ConnectedSport, iduser, nom, prenom ,ToolTip1, ToolTip2 , ToolTip3, ToolTip4, ToolTip5;
        bool SlideDown_Up = false;
        bool PostUp = false;


        TimeSpan ts;


        int cpt1, cpt2, cpt3, cpt4; 
        string soon, end, mcha;


        public class notifications
        {
            public string nom { get; set; }
            public string state { get; set; }
        }

        List<notifications> notif = new List<notifications>();
        MainApp mv;
        public Profil(MainApp mv, string ConnectedSalle, string ConnectedSport, string iduser, string nom, string prenom)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.ConnectedSalle = ConnectedSalle;
            this.ConnectedSport = ConnectedSport;
            this.iduser = iduser;
            this.mv = mv;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            if (PostUp == true)
            {
                if (SlideDown_Up == true)
                {
                    SlideDown(BoderNotif);
                    ((Storyboard)App.Current.Resources["ToolTipAnimationGoOut"]).Begin(nots);
                    ((Storyboard)App.Current.Resources["ToolTipAnimationGoIn"]).Begin(ListViewNotif);

                }
                if (SlideDown_Up == false)
                {
                    SlideUp(BoderNotif);
                    ((Storyboard)App.Current.Resources["ToolTipAnimationGoOut"]).Begin(ListViewNotif);
                    ((Storyboard)App.Current.Resources["ToolTipAnimationGoIn"]).Begin(nots);
                }
            }
            PostUp = true;


            da.SelectCommand.CommandText = "select u.Nom,u.Prenom,s.nom_Salle,t.nom_Type,u.Valide,s.IdSalle,t.IdType,u.IdUser from Utilisateur u join UtilisateurSportSalle us on u.IdUser = us.IdUser join Salle s on s.IdSalle = us.IdSalle join Type_Sport t on t.IdType = us.IdType where s.IdSalle = '" + ConnectedSalle + "' and t.IdType = '" + ConnectedSport + "' and u.IdUser = '" + iduser + "'";
            da.Fill(ds, "infos");

            da.SelectCommand.CommandText = "select * from Clients c join SportClients s on c.IdClient = s.IdClient where IdSalle = '"+ConnectedSalle+"' and IdType = '"+ConnectedSport+ "' and Active = '" + true + "'";
            da.Fill(ds, "Clients");

            da.SelectCommand.CommandText = "select * from Clients c join SportClients s on c.IdClient = s.IdClient and Active = '" + true + "'";
            da.Fill(ds, "Clientsprin");

            da.SelectCommand.CommandText = "select p.IdPayment,convert(varchar, p.date_Payment, 103),p.IdClient,p.IdSalle,p.IdType,p.Prix,c.IdClient,c.nom,c.prenom,c.Tel,c.img,c.Active,c.LastPay from Payments p join Clients c on p.IdClient = c.IdClient  where IdSalle = '" + ConnectedSalle + "' and IdType = '" + ConnectedSport + "' and Active = '" + true + "'";
            da.Fill(ds, "PriceTest");

            if (ds.Tables["infos"].Rows.Count != 0 )
            {

                UserName.Text =  ds.Tables["infos"].Rows[0][1].ToString().ToUpper();
                SalleName.Text = ds.Tables["infos"].Rows[0][2].ToString();
                SportName.Text = ds.Tables["infos"].Rows[0][3].ToString();
                if (Convert.ToBoolean(ds.Tables["infos"].Rows[0][4]) == true)
                {
                    cn.Open();
                    cmd.Connection = cn;
                    cmd.CommandText = "select prix from SportSalle where IdSalle = '" + ConnectedSalle + "' and IdType = '" + ConnectedSport + "'";
                    double price = double.Parse(cmd.ExecuteScalar().ToString());
                    cn.Close();


                    Acsess.Text = "Admin";
                    icon1.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                    icon2.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                    icon3.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                    icon4.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                    icon5.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                    ToolTip1 = "Vous avez l'access a Ajouter, Modifier et Supprimer Une Salle";
                    ToolTip2 = "Vous avez l'access a Ajouter, Modifier et Supprimer Un Sport. Tu peut affecter Un Sport a tout les Salles";
                    ToolTip3 = "Vous avez l'access pour ajouter un nouveau Utilisateur, determiner son Rolle. Supprimer et Modifier tout les Utilisateur";
                    ToolTip4 = "vous avez l'access a Ajouter ,Modifier et Supprimer un Clients a partir de votre Salle et Sport";
                    ToolTip5 = "Vous avez l'access de gestioner votre depenses a partir de votre Salle et Sport";

                    for (int i = 0; i < ds.Tables["Clients"].Rows.Count; i++)
                    {
                        if (ds.Tables["Clients"].Rows[i][6].ToString() != "")
                        {
                            ts = DateTime.Now - DateTime.Parse(ds.Tables["Clients"].Rows[i][6].ToString());
                            int count = int.Parse(ts.Days.ToString());
                            if (count >= 28 && count == 30)
                            {
                                cpt1++;
                                notif.Add(new notifications() { nom = ds.Tables["Clients"].Rows[i][1].ToString() + " " + ds.Tables["Clients"].Rows[i][2].ToString(), state = "L'abonnement De Cette Personne Se Termine Très Bientôt" });

                            }
                            if (count > 30 && count <= 40)
                            {
                                cpt2++;
                                notif.Add(new notifications() { nom = ds.Tables["Clients"].Rows[i][1].ToString() + " " + ds.Tables["Clients"].Rows[i][2].ToString(), state = "L'abonnement De Cette Personne Est Terminé" });

                            }
                            if (count > 40)
                            {
                                cpt3++;
                                notif.Add(new notifications() { nom = ds.Tables["Clients"].Rows[i][1].ToString() + " " + ds.Tables["Clients"].Rows[i][2].ToString(), state = "Vous Pouvez Désactiver Cette Personne, Car Son Abonnement Est Terminé Il y A Longtemps" });

                            }

                        }
                    }

                    for (int i = 0; i < ds.Tables["PriceTest"].Rows.Count; i++)
                    {
                        if(double.Parse(ds.Tables["PriceTest"].Rows[i][5].ToString()) < price)
                        {
                            cpt4++;
                            notif.Add(new notifications() { nom = ds.Tables["PriceTest"].Rows[i][7].ToString() + " " + ds.Tables["PriceTest"].Rows[i][8].ToString() , state = " À Un Paiement Inferieur Au Montant Convenu Dans La Date: " + " " + ds.Tables["PriceTest"].Rows[i][1].ToString() });

                        }
                    }
                    nots.Text = (cpt1 + cpt2 + cpt3 + cpt4).ToString();
                    ListViewNotif.ItemsSource = notif;

                    cn.Open();
                    cmd.Connection = cn;
                    cmd.CommandText = "select count(*) from Clients c join SportClients s on c.IdClient = s.IdClient where IdSalle = '" + ConnectedSalle + "' and IdType = '" + ConnectedSport + "' and Active = '"+true+"'";
                    int nbClients = int.Parse(cmd.ExecuteScalar().ToString());
                    nbClient.Text = nbClients.ToString();
                    cn.Close();
                }
                else 
                {
                    cn.Open();
                    cmd.Connection = cn;
                    cmd.CommandText = "select prix from SportSalle where IdSalle = '" + ConnectedSalle + "' and IdType = '" + ConnectedSport + "'";
                    double price = double.Parse(cmd.ExecuteScalar().ToString());
                    cn.Close();


                    Acsess.Text = "Editeur";
                    icon1.Foreground = new SolidColorBrush(Color.FromRgb(255, 52, 73));
                    icon2.Foreground = new SolidColorBrush(Color.FromRgb(255, 52, 73));
                    icon3.Foreground = new SolidColorBrush(Color.FromRgb(255, 52, 73));
                    icon4.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                    icon5.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                    ToolTip1 = "Pas d'access";
                    ToolTip2 = "Pas d'access";
                    ToolTip3 = "Pas d'access";
                    ToolTip4 = "vous avez l'access a Ajouter ,Modifier et Supprimer un Clients a partir de votre Salle et Sport";
                    ToolTip5 = "Vous avez l'access de gestioner votre depenses a partir de votre Salle et Sport";

                    for (int i = 0; i < ds.Tables["Clients"].Rows.Count; i++)
                    {
                        if (ds.Tables["Clients"].Rows[i][6].ToString() != "")
                        {
                            ts = DateTime.Now - DateTime.Parse(ds.Tables["Clients"].Rows[i][6].ToString());
                            int count = int.Parse(ts.Days.ToString());
                            if (count >= 28 && count == 30)
                            {
                                cpt1++;
                                notif.Add(new notifications() { nom = ds.Tables["Clients"].Rows[i][1].ToString() +" "+ ds.Tables["Clients"].Rows[i][2].ToString(), state = "L'abonnement De Cette Personne Se Termine Très Bientôt" });

                            }
                            if (count > 30 && count <= 40)
                            {
                                cpt2++;
                                notif.Add(new notifications() { nom = ds.Tables["Clients"].Rows[i][1].ToString() + " " + ds.Tables["Clients"].Rows[i][2].ToString(), state = "L'abonnement De Cette Personne Est Terminé" });


                            }
                            if (count > 40)
                            {
                                cpt3++;
                                notif.Add(new notifications() { nom = ds.Tables["Clients"].Rows[i][1].ToString() + " " + ds.Tables["Clients"].Rows[i][2].ToString(), state = "Vous Pouvez Désactiver Cette Personne, Car Son Abonnement Est Terminé Il Y A Longtemps" });


                            }
                           
                        }
                    }

                    for (int i = 0; i < ds.Tables["PriceTest"].Rows.Count; i++)
                    {
                        if (double.Parse(ds.Tables["PriceTest"].Rows[i][5].ToString()) < price)
                        {
                            cpt4++;
                            notif.Add(new notifications() { nom = ds.Tables["PriceTest"].Rows[i][7].ToString() + " " + ds.Tables["PriceTest"].Rows[i][8].ToString(), state = " À Un Paiement Inferieur Au Montant Convenu Dans La Date: " + " " +ds.Tables["PriceTest"].Rows[i][1].ToString() });

                        }
                    }
                    nots.Text = (cpt1 + cpt2 + cpt3 + cpt4).ToString();
                    ListViewNotif.ItemsSource = notif;
                }
                cn.Open();
                cmd.Connection = cn;
                cmd.CommandText = "select count(*) from Clients c join SportClients s on c.IdClient = s.IdClient where IdSalle = '" + ConnectedSalle + "' and IdType = '" + ConnectedSport + "' and Active = '" + true + "'";
                int nbClients2 = int.Parse(cmd.ExecuteScalar().ToString());
                nbClient.Text = nbClients2.ToString();
                cn.Close();

            }
            else
            {
                SalleName.Text = "Tout";
                SportName.Text = "Tout";
                da.SelectCommand.CommandText = "select * from Utilisateur where IdUser = '"+iduser+"'";
                da.Fill(ds, "users");
                Acsess.Text = "Admin Principale";
                UserName.Text = ds.Tables["users"].Rows[0][2].ToString().ToUpper();
                icon1.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                icon2.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                icon3.Foreground = new SolidColorBrush(Color.FromRgb(52, 255, 72));
                icon4.Foreground = new SolidColorBrush(Color.FromRgb(255, 174, 52));
                icon5.Foreground = new SolidColorBrush(Color.FromRgb(255, 174, 52));
                ToolTip1 = "Vous avez l'access a Ajouter, Modifier et Supprimer Une Salle";
                ToolTip2 = "Vous avez l'access a Ajouter, Modifier et Supprimer Un Sport. Tu peut affecter Un Sport a tout les Salles";
                ToolTip3 = "Vous avez l'access pour ajouter un nouveau Utilisateur, determiner son Rolle. Supprimer et Modifier tout les Utilisateur";
                ToolTip4 = "Vous avez l'access a Consulter la liste des Clients de tout les Salles et Sports, Activer, Desactiver ou Supprimer un Client";
                ToolTip5 = "Vous avez l'access a Consulter les depenses de tout les salles";

               

                for (int i = 0; i < ds.Tables["Clientsprin"].Rows.Count; i++)
                {
                    if (ds.Tables["Clientsprin"].Rows[i][6].ToString() != "")
                    {
                        ts = DateTime.Now - DateTime.Parse(ds.Tables["Clientsprin"].Rows[i][6].ToString());
                        int count = int.Parse(ts.Days.ToString());
                        if (count >= 28 && count == 30)
                        {
                            cpt1++;
                            notif.Add(new notifications() { nom = ds.Tables["Clientsprin"].Rows[i][1].ToString() + " " + ds.Tables["Clientsprin"].Rows[i][2].ToString(), state = "L'abonnement De Cette Personne Se Termine Très Bientôt" });

                        }
                        if (count > 30 && count <= 40)
                        {
                            cpt2++;
                            notif.Add(new notifications() { nom = ds.Tables["Clientsprin"].Rows[i][1].ToString() + " " + ds.Tables["Clientsprin"].Rows[i][2].ToString(), state = "L'abonnement De Cette Personne Est Terminé" });

                        }
                        if (count > 40)
                        {
                            cpt3++;
                            notif.Add(new notifications() { nom = ds.Tables["Clientsprin"].Rows[i][1].ToString() + " " + ds.Tables["Clientsprin"].Rows[i][2].ToString(), state = "Vous Pouvez Désactiver Cette Personne, Car Son Abonnement Est Terminé Il Y A Longtemps" });

                        }
                        nots.Text = (cpt1 + cpt2 + cpt3).ToString();
                        ListViewNotif.ItemsSource = notif;

                    }
                }
                cn.Open();
                cmd.Connection = cn;
                cmd.CommandText = "select count(*) from Clients c join SportClients s on c.IdClient = s.IdClient where  Active = '" + true + "'";
                int nbClients3 = int.Parse(cmd.ExecuteScalar().ToString());
                nbClient.Text = nbClients3.ToString();
                cn.Close();
            }

            

            

            da.SelectCommand.CommandText = "select * from Utilisateur";
            da.Fill(ds, "users");

            DataTable dataTable = ds.Tables["users"];

            foreach (DataRow row in dataTable.Rows)
            {

                if (row[0].ToString() == iduser)
                {
                    if (row[7].ToString() != "")
                    {
                        byte[] blob = (byte[])row[7];
                        MemoryStream stream = new MemoryStream();
                        stream.Write(blob, 0, blob.Length);
                        stream.Position = 0;

                        System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();

                        MemoryStream ms = new MemoryStream();
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        ms.Seek(0, SeekOrigin.Begin);
                        bi.StreamSource = ms;
                        bi.EndInit();
                        back.Source = bi;
                    }
                    else
                    {
                        back.Source = new BitmapImage(new Uri("/Resource/profil.jpg", UriKind.Relative));
                    }


                }
            }

        }
        

        private void Icon1_MouseEnter(object sender, MouseEventArgs e)
        {
            animateBorderIn(BorderToolTip);
            ToolTipTextBlock.Text = ToolTip1;
        }

        string  imageName;

        private void NotifBtn_Click(object sender, RoutedEventArgs e)
        {
            SlideDown_Up = !SlideDown_Up;
            if (SlideDown_Up == true)
            {
                SlideDown(BoderNotif);
                ((Storyboard)App.Current.Resources["ToolTipAnimationGoOut"]).Begin(nots);
                ((Storyboard)App.Current.Resources["ToolTipAnimationGoIn"]).Begin(ListViewNotif);

            }
            if (SlideDown_Up == false)
            {
                SlideUp(BoderNotif);
                ((Storyboard)App.Current.Resources["ToolTipAnimationGoOut"]).Begin(ListViewNotif);
                ((Storyboard)App.Current.Resources["ToolTipAnimationGoIn"]).Begin(nots);
            }
        }

        private void AddBackImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fl = new OpenFileDialog();
                fl.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                if (fl.ShowDialog() == true)
                {
                    imageName = fl.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    back.SetValue(Image.SourceProperty, isc.ConvertFromString(imageName));
                    if (imageName != null)
                    {
                        FileStream fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);
                        byte[] imgByte = new byte[fs.Length];
                        fs.Read(imgByte, 0, Convert.ToInt32(fs.Length));
                        fs.Close();


                        cn.Open();
                        cmd.Connection = cn;
                        cmd.CommandText = "update Utilisateur set imaBack = @img where IdUser = '" + iduser + "'";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("img", imgByte);
                        cmd.ExecuteNonQuery();
                        cn.Close();

                    }
                    
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                MessageForm m = new MessageForm(msg);
                m.ShowDialog();
            }
        }

        private void Icon1_MouseLeave(object sender, MouseEventArgs e)
        {
            animateBorderOut(BorderToolTip);
        }

        private void Icon2_MouseEnter(object sender, MouseEventArgs e)
        {
            animateBorderIn(BorderToolTip);
            ToolTipTextBlock.Text = ToolTip2;
        }

        private void Icon2_MouseLeave(object sender, MouseEventArgs e)
        {
            animateBorderOut(BorderToolTip);
        }

        private void Icon3_MouseEnter(object sender, MouseEventArgs e)
        {
            animateBorderIn(BorderToolTip);
            ToolTipTextBlock.Text = ToolTip3;
        }

        private void Icon3_MouseLeave(object sender, MouseEventArgs e)
        {
            animateBorderOut(BorderToolTip);
        }

        private void Icon4_MouseEnter(object sender, MouseEventArgs e)
        {
            animateBorderIn(BorderToolTip);
            ToolTipTextBlock.Text = ToolTip4;
        }

        private void Icon4_MouseLeave(object sender, MouseEventArgs e)
        {
            animateBorderOut(BorderToolTip);
        }

        private void Icon5_MouseEnter(object sender, MouseEventArgs e)
        {
            animateBorderIn(BorderToolTip);
            ToolTipTextBlock.Text = ToolTip5;
        }

        private void Icon5_MouseLeave(object sender, MouseEventArgs e)
        {
            animateBorderOut(BorderToolTip);
        }

        
        
        public void animateBorderIn(Border c)
        {
            ((Storyboard)App.Current.Resources["ToolTipAnimationGoIn"]).Begin(c);
        }
        public void animateBorderOut(Border c)
        {
            ((Storyboard)App.Current.Resources["ToolTipAnimationGoOut"]).Begin(c);
        }
        public void SlideDown(Border c)
        {
            ((Storyboard)App.Current.Resources["SlideDown"]).Begin(c);
        }
        public void SlideUp(Border c)
        {
            ((Storyboard)App.Current.Resources["SlideUp"]).Begin(c);
        }


    }
}
