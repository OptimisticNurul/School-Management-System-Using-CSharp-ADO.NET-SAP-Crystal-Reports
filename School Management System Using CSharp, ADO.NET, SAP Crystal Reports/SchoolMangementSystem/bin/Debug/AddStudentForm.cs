using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;


namespace SchoolMangementSystem
{
    public partial class AddStudentForm : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=DESKTOP-SNOOUCH\SQLEXPRESS;Initial Catalog=school;Integrated Security=True;");
        SqlTransaction transaction;

        public AddStudentForm()
        {
            InitializeComponent();
            displayStudentData();
        }

        public void displayStudentData()
        {
            AddStudentData adData = new AddStudentData();
            student_studentData.DataSource = adData.studentData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (student_id.Text == "" || student_name.Text == "" || student_gender.Text == ""
                || student_address.Text == "" || student_grade.Text == "" || student_section.Text == ""
                || student_status.Text == "" || student_image.Image == null || imagePath == null)
            {
                MessageBox.Show("Please fill all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();
                        transaction = connect.BeginTransaction();

                        string checkStudentID = "SELECT COUNT(*) FROM students WHERE student_id = @studentID";

                        using (SqlCommand checkSID = new SqlCommand(checkStudentID, connect, transaction))
                        {
                            checkSID.Parameters.AddWithValue("@studentID", student_id.Text.Trim());
                            int count = (int)checkSID.ExecuteScalar();

                            if (count >= 1)
                            {
                                MessageBox.Show("Student ID: " + student_id.Text.Trim() + " is already exist"
                                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                transaction.Rollback();
                                return;
                            }
                        }

                        DateTime today = DateTime.Today;
                        string insertData = "INSERT INTO students (student_id, student_name" +
                            ", student_gender, student_address, student_grade, student_section" +
                            ", student_image, student_status, date_insert) " +
                            "VALUES(@studentID, @studentName, @studentGender, @studentAddress" +
                            ", @studentGrade, @studentSection, @studentImage, @studentStatus" +
                            ", @dateInsert)";

                        string path = Path.Combine(@"..\..\Student_Directory\", student_id.Text.Trim() + ".jpg");
                        string directoryPath = Path.GetDirectoryName(path);

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        File.Copy(imagePath, path, true);

                        using (SqlCommand cmd = new SqlCommand(insertData, connect, transaction))
                        {
                            cmd.Parameters.AddWithValue("@studentID", student_id.Text.Trim());
                            cmd.Parameters.AddWithValue("@studentName", student_name.Text.Trim());
                            cmd.Parameters.AddWithValue("@studentGender", student_gender.Text.Trim());
                            cmd.Parameters.AddWithValue("@studentAddress", student_address.Text.Trim());
                            cmd.Parameters.AddWithValue("@studentGrade", student_grade.Text.Trim());
                            cmd.Parameters.AddWithValue("@studentSection", student_section.Text.Trim());
                            cmd.Parameters.AddWithValue("@studentImage", path);
                            cmd.Parameters.AddWithValue("@studentStatus", student_status.Text.Trim());
                            cmd.Parameters.AddWithValue("@dateInsert", today.ToString());

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        displayStudentData();

                        MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        clearFields();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error connecting Database: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        public void clearFields()
        {
            student_id.Text = "";
            student_name.Text = "";
            student_gender.SelectedIndex = -1;
            student_address.Text = "";
            student_grade.SelectedIndex = -1;
            student_section.SelectedIndex = -1;
            student_status.SelectedIndex = -1;
            student_image.Image = null;
            imagePath = "";
        }

        public string imagePath;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image files (*.jpg; *.png)|*.jpg;*.png";

            if (open.ShowDialog() == DialogResult.OK)
            {
                imagePath = open.FileName;

                student_image.ImageLocation = imagePath;
            }
        }

        private void student_updateBtn_Click(object sender, EventArgs e)
        {
            if (student_id.Text == "" || student_name.Text == "" || student_gender.Text == ""
                || student_address.Text == "" || student_grade.Text == "" || student_section.Text == ""
                || student_status.Text == "" || student_image.Image == null || imagePath == null)
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();
                        transaction = connect.BeginTransaction();

                        DialogResult check = MessageBox.Show("Are you sure you want to Update Student ID: "
                            + student_id.Text.Trim() + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (check == DialogResult.Yes)
                        {
                            DateTime today = DateTime.Today;

                            String updateData = "UPDATE students SET student_name = @studentName, " +
                                "student_gender = @studentGender, student_address = @studentAddress, " +
                                "student_grade = @studentGrade, student_section = @studentSection, " +
                                "student_status = @studentStatus, date_update = @dateUpdate " +
                                "WHERE student_id = @studentID";

                            using (SqlCommand cmd = new SqlCommand(updateData, connect, transaction))
                            {
                                cmd.Parameters.AddWithValue("@studentName", student_name.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentGender", student_gender.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentAddress", student_address.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentGrade", student_grade.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentSection", student_section.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentStatus", student_status.Text.Trim());
                                cmd.Parameters.AddWithValue("@dateUpdate", today.ToString());
                                cmd.Parameters.AddWithValue("@studentID", student_id.Text.Trim());

                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            displayStudentData();

                            MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            clearFields();
                        }
                        else
                        {
                            MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            clearFields();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error connecting Database: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        private void student_studentData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = student_studentData.Rows[e.RowIndex];
                student_id.Text = row.Cells[1].Value.ToString();
                student_name.Text = row.Cells[2].Value.ToString();
                student_gender.Text = row.Cells[3].Value.ToString();
                student_address.Text = row.Cells[4].Value.ToString();
                student_grade.Text = row.Cells[5].Value.ToString();
                student_section.Text = row.Cells[6].Value.ToString();

                imagePath = row.Cells[5].Value.ToString();

                string imageData = row.Cells[7].Value.ToString();

                if (imageData != null && imageData.Length > 0)
                {
                    student_image.Image = Image.FromFile(imageData);
                }
                else
                {
                    student_image.Image = null;
                }

                student_status.Text = row.Cells[8].Value.ToString();

            }
        }

        private void student_deleteBtn_Click(object sender, EventArgs e)
        {
            if (student_id.Text == "")
            {
                MessageBox.Show("Please select item first", "Error Message"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    DialogResult check = MessageBox.Show("Are you sure you want to Delete Student ID: "
                        + student_id.Text + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (check == DialogResult.Yes)
                    {

                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;

                            string deleteData = "UPDATE students SET date_delete = @dateDelete " +
                                "WHERE student_id = @studentID";

                            using (SqlCommand cmd = new SqlCommand(deleteData, connect))
                            {
                                cmd.Parameters.AddWithValue("@dateDelete", today.ToString());
                                cmd.Parameters.AddWithValue("@studentID", student_id.Text.Trim());

                                cmd.ExecuteNonQuery();

                                displayStudentData();

                                MessageBox.Show("Deleted successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error connecting Database: " + ex, "Error Message"
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancelled.", "Information Message"
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }

        private void student_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }
    }
}
