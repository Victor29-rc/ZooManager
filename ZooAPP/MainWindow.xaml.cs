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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Diagnostics.Eventing.Reader;

namespace ZooAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["ZooAPP.Properties.Settings.ZooDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            ShowZoos();
            ShowAnimals();
        }

        private void ShowZoos()
        {
            try 
            {
                string query = "SELECT * FROM Zoo";
                SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnection);

                using (adapter)
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    listZoos.DisplayMemberPath = "name";
                    listZoos.SelectedValuePath = "Id";

                    listZoos.ItemsSource = dt.DefaultView;
                }
            } 
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowAnimals()
        {
            try
            {
                string query = "SELECT * FROM ANIMAL";
                SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnection);

                using(adapter)
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    listAnimals.ItemsSource = dt.DefaultView;

                    listAnimals.DisplayMemberPath = "Name";
                    listAnimals.SelectedValuePath = "Id";
                }
            } 
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowAssociatedAnimals(int zooId)
        {
            try
            {
                string query = "SELECT b.Id, a.Name FROM Animal AS a " +
                               "INNER JOIN ZooAnimal AS b ON a.Id = b.AnimalId " +
                               "WHERE b.ZooId = @ZooId";

                SqlCommand command = new SqlCommand(query, sqlConnection);
                           command.Parameters.AddWithValue("@ZooId", zooId);

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                using (adapter)
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    listAssociatedAnimals.ItemsSource = dt.DefaultView;

                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    listAssociatedAnimals.SelectedValuePath = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ListZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listZoos.SelectedValue != null)
            {
                int zooId = int.Parse(listZoos.SelectedValue.ToString());
                ShowAssociatedAnimals(zooId);
            }
        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            if(listZoos.SelectedValue != null)
            {
                try
                {
                    string query = "DELETE FROM Zoo WHERE Id = @ZooId";
                    string zooId = listZoos.SelectedValue.ToString();

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@ZooId", zooId);

                    sqlConnection.Open();
                    cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    ShowZoos();
                }
            }
            else
            {
                MessageBox.Show("You should select a ZOO first", "Attention", MessageBoxButton.OK);
            }
        }

        private void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            string zooName = textEntry.Text;

            if (!String.IsNullOrEmpty(zooName))
            {
                try
                {
                    string query = "INSERT INTO Zoo (name) Values (@Name)";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@Name", zooName);

                    sqlConnection.Open();
                    cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    ShowZoos();
                    textEntry.Clear();
                }
            }
            else
            {
                MessageBox.Show("You should enter the Zoo's name first", "Attention", MessageBoxButton.OK);
            }
        }

        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            string animalName = textEntry.Text;

            if (!String.IsNullOrEmpty(animalName))
            {
                try
                {
                    string query = "INSERT INTO Animal (Name) Values (@Name)";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@Name", animalName);

                    sqlConnection.Open();
                    cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    textEntry.Clear();
                    ShowAnimals();             
                }
            }
            else
            {
                MessageBox.Show("You should enter the Animal's name first", "Attention", MessageBoxButton.OK);
            }
        }

        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            string zooName = textEntry.Text;

            if (!String.IsNullOrEmpty(zooName) && listZoos.SelectedValue != null)
            {
                try
                {
                    string zooId = listZoos.SelectedValue.ToString();
                    string query = "UPDATE Zoo set name = @Name WHERE Id = @ID";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@Name", zooName);
                    cmd.Parameters.AddWithValue("@ID", zooId);

                    sqlConnection.Open();
                    cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    textEntry.Clear();
                    ShowZoos();
                }
            }
            else
            {
                MessageBox.Show("You should select the Zoo to update and enter the new name", "Attention", MessageBoxButton.OK);
            }
        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            string animalName = textEntry.Text;

            if (!String.IsNullOrEmpty(animalName) && listAnimals.SelectedValue != null)
            {
                try
                {
                    string animalId = listAnimals.SelectedValue.ToString();
                    string query = "UPDATE Animal set Name = @Name WHERE Id = @ID";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@Name", animalName);
                    cmd.Parameters.AddWithValue("@ID", animalId);

                    sqlConnection.Open();
                    cmd.ExecuteScalar();

                    if (listZoos.SelectedValue != null)
                    {
                        ShowAssociatedAnimals(int.Parse(listZoos.SelectedValue.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    textEntry.Clear();
                    ShowAnimals();
                }
            }
            else
            {
                MessageBox.Show("You should select the Animal to update and enter the new name", "Attention", MessageBoxButton.OK);
            }
        }

        private void AddAnimalToZoo_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                if (listZoos.SelectedValue != null && listAnimals.SelectedValue != null)
                {
                    string zooId = listZoos.SelectedValue.ToString();
                    string animalId = listAnimals.SelectedValue.ToString();

                    string query = "INSERT INTO ZooAnimal (ZooId, AnimalId) VALUES (@ZooId, @AnimalId)";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);

                    cmd.Parameters.AddWithValue("@ZooId", zooId);
                    cmd.Parameters.AddWithValue("@AnimalId", animalId);

                    sqlConnection.Open();
                    cmd.ExecuteScalar();

                    ShowAssociatedAnimals(int.Parse(zooId));
                }
                else
                {
                    MessageBox.Show("You should select the Zoo and the Animal first", "Attention", MessageBoxButton.OK);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void RemoveAnimal_Click(Object sender, RoutedEventArgs e)
        {
            if(listAssociatedAnimals.SelectedItem != null) 
            {
                try
                {
                    string zooAnimalId = listAssociatedAnimals.SelectedValue.ToString();
                    string query = "DELETE FROM ZooAnimal WHERE Id = @ZooAnimalId";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@ZooAnimalId", zooAnimalId);

                    sqlConnection.Open();
                    cmd.ExecuteScalar();

                    ShowAssociatedAnimals(int.Parse(listZoos.SelectedValue.ToString()));
                } 
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                }

            }
            else
            {
                MessageBox.Show("You should select an animal in the zoo first", "Attention", MessageBoxButton.OK);
            }
        }

        private void DeleteAnimal_Click(Object sender, RoutedEventArgs e)
        {
            if (listAnimals.SelectedValue != null)
            {
                try
                {
                    string query = "DELETE FROM Animal WHERE Id = @AnimalId";
                    string animalId = listAnimals.SelectedValue.ToString();

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@AnimalId", animalId);

                    sqlConnection.Open();
                    cmd.ExecuteScalar();

                    ShowAnimals();

                    if (listZoos.SelectedValue != null)
                    {
                        ShowAssociatedAnimals(int.Parse(listZoos.SelectedValue.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("You should select an Animal first", "Attention", MessageBoxButton.OK);
            }
        }
    }
}
