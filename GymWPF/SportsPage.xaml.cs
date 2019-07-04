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

namespace GymWPF
{
    /// <summary>
    /// Logique d'interaction pour SportsPage.xaml
    /// </summary>
    public partial class SportsPage : Page
    {
        //Declaration -------------------------//
        SqlConnection cn = new SqlConnection("Data Source=.;Initial Catalog=NSS_Salle_Application;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter("", "Data Source=.;Initial Catalog=NSS_Salle_Application;Integrated Security=True");
        DataSet ds = new DataSet();
        SqlDataReader dr;
        //------------------------------------//

        MainApp dade;
        public SportsPage(MainApp dade)
        {
            InitializeComponent();
            this.dade = dade;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            da.SelectCommand.CommandText = "select * from Salle";
            da.Fill(ds, "Salle");
            SallesComboBox.ItemsSource = ds.Tables["Salle"].DefaultView;
            SallesComboBox.DisplayMemberPath = "nom_Salle";
            SallesComboBox.SelectedValuePath = "IdSalle";

            da.SelectCommand.CommandText = "select t.IdType as IdType,s.nom_Salle as nom_Salle,t.nom_Type as nom_Type,ss.prix as prix from Salle s join SportSalle ss on s.IdSalle=ss.IdSalle join  Type_Sport t on ss.IdType=t.IdType";
            da.Fill(ds, "sport");
            ListViewSports.DataContext = ds.Tables["sport"].DefaultView;
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SportName.Text != "" && SallesComboBox.SelectedItem != null && SportPrix.Text != "")
                {
                    cn.Open();
                    cmd.Connection = cn;
                    cmd.CommandText = "insert into Type_Sport values ('" + SportName.Text + "')";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "select MAX(IdType) from Type_Sport";
                    int id = int.Parse(cmd.ExecuteScalar().ToString());

                    cmd.CommandText = "insert into SportSalle values ('" + SallesComboBox.SelectedValue + "','" + id + "','" + SportPrix.Text + "')";
                    cmd.ExecuteNonQuery();

                    cn.Close();
                    MessageBox.Show("ok");

                }
                else
                {
                    MessageBox.Show("errors");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void ListViewSports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewSports.SelectedIndex;
            DataRowView row = ListViewSports.Items.GetItemAt(index) as DataRowView;
            SallesComboBox.SelectedItem = row.Row[1].ToString();
            SportName.Text = row.Row[2].ToString();
            SportPrix.Text = row.Row[3].ToString();
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                    int index = ListViewSports.SelectedIndex;
                    DataRowView row = ListViewSports.Items.GetItemAt(index) as DataRowView;
                    int id = int.Parse(row.Row[0].ToString());

                    cn.Open();
                    cmd.Connection = cn;
                    cmd.CommandText = "update Type_Sport set nom_Type = '" + SportName.Text + "' where IdType ='"+id+"'";
                    cmd.ExecuteNonQuery();
                    
                    cmd.CommandText = "update  SportSalle set IdSalle ='" + SallesComboBox.SelectedValue + "', prix ='" + SportPrix.Text + "' where IdType ='" + id + "'";
                    cmd.ExecuteNonQuery();

                    cn.Close();
                    MessageBox.Show("ok");

                
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
