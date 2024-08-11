using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagement
{
    public partial class Enroll : Form
    {
        public Enroll()
        {
            InitializeComponent();
        }
        DataSet GetEnroll(string search = "")
        {
            DataSet data = new DataSet();
            string query = "SELECT * FROM Enrollments";

            if (!string.IsNullOrEmpty(search))
            {
                query += " WHERE student_id = @Search OR class_id = @ClassSearch";
            }

            using (SqlConnection connection = new SqlConnection(Connection.connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Search", search);

                if (int.TryParse(search, out int classId))
                {
                    cmd.Parameters.AddWithValue("@ClassSearch", classId);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ClassSearch", DBNull.Value);
                }

                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(data);
            }
            return data;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            datagrvEnroll.DataSource = GetEnroll(search).Tables[0];
            datagrvEnroll.Refresh();
        }

        private void Enroll_Load(object sender, EventArgs e)
        {
            btnDeleteStu.Enabled = false;
            btnEditStu.Enabled = false;
            //Fill bảng vừa form
            datagrvEnroll.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            datagrvEnroll.DataSource = GetEnroll().Tables[0];
        }

        void refresh()
        {
            datagrvEnroll.DataSource = GetEnroll().Tables[0];
            datagrvEnroll.Refresh();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddStu_Click(object sender, EventArgs e)
        {
            try
            {
                string stuId = txtStuID.Text.Trim();
                int classId = int.Parse(txtClassID.Text.Trim());
                decimal grade = decimal.Parse(txtGrade.Text.Trim());

                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    string addQry = "INSERT INTO Enrollments VALUES (@StuID, @ClassID, @Grade)";
                    SqlCommand cmd = new SqlCommand(addQry, connection);
                    cmd.Parameters.AddWithValue("@StuId", stuId);
                    cmd.Parameters.AddWithValue("@ClassID", classId);
                    cmd.Parameters.AddWithValue("@Grade", grade);
                    cmd.ExecuteNonQuery();
                    refresh();
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numeric values for Student ID, Class ID, and Grade.");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Duplicate ID error
                {
                    MessageBox.Show("ID cannot be duplicated!");
                }
                else if (ex.Number == 547) // Foreign key error
                {
                    MessageBox.Show("ID does not exist!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void btnEditStu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    string addQry = "UPDATE Enrollments \n SET student_id = @StuId, class_id = @ClassID, grade = @Grade WHERE student_id = @StuId";
                    SqlCommand cmd = new SqlCommand(addQry, connection);
                    cmd.Parameters.AddWithValue("@StuID", txtStuID.Text.Trim());
                    cmd.Parameters.AddWithValue("@ClassID", int.Parse(txtClassID.Text.Trim()));
                    cmd.Parameters.AddWithValue("@Grade", decimal.Parse(txtGrade.Text.Trim()));
                    cmd.ExecuteNonQuery();
                    refresh();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnDeleteStu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.connectionString))
                {
                    connection.Open();
                    string delQry = "DELETE FROM Enrollments WHERE student_id = @StuId";
                    SqlCommand cmd = new SqlCommand(delQry, connection);
                    cmd.Parameters.AddWithValue("@StuId", txtStuID.Text.Trim());
                    cmd.ExecuteNonQuery();
                    refresh();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void datagrvEnroll_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow rowData = datagrvEnroll.Rows[e.RowIndex];

                txtStuID.Text = rowData.Cells["student_id"].Value.ToString();
                txtClassID.Text = rowData.Cells["class_id"].Value.ToString();
                txtGrade.Text = rowData.Cells["grade"].Value.ToString();
                btnEditStu.Enabled = true;
                btnDeleteStu.Enabled = true;
            }
        }

        private void btnStudentView_Click(object sender, EventArgs e)
        {
            Student student = new Student();
            student.StartPosition = FormStartPosition.Manual;
            student.Show();
        }

        private void btnClassView_Click(object sender, EventArgs e)
        {
            Class _class = new Class();
            _class.StartPosition = FormStartPosition.Manual;
            _class.Show();
        }

        private void txtStuID_TextChanged(object sender, EventArgs e)
        {

        }

        private void datagrvEnroll_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
