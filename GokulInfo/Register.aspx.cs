using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GokulInfo
{
    public partial class Register : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ub"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindgrid();
                ToggleInsertValidators(true);  
            }
        }

        protected void bindgrid()
        {
            string query = "SELECT * FROM gokulinfo";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        GridView1.DataSource = dataTable;
                        GridView1.DataBind();
                    }
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Page.IsValid) // Only insert if the page is valid
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("RegisterUser", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FirstName", TextBox1.Text);
                    command.Parameters.AddWithValue("@LastName", TextBox2.Text);
                    command.Parameters.AddWithValue("@PhoneNumber", TextBox3.Text);
                    command.Parameters.AddWithValue("@Email", TextBox4.Text);
                    command.Parameters.AddWithValue("@Address", TextBox5.Text);
                    int result = command.ExecuteNonQuery();
                    if (result != 0)
                    {
                        Response.Write("Customer registered successfully");
                    }
                    else
                    {
                        Response.Write("Customer not registered due to invalid details");
                    }
                    bindgrid();
                }
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            bindgrid();

            ToggleInsertValidators(false); 
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];
            int userID = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            TextBox t1 = (TextBox)row.FindControl("TextBox1");
            TextBox t2 = (TextBox)row.FindControl("TextBox2");
            TextBox t3 = (TextBox)row.FindControl("TextBox3");
            TextBox t4 = (TextBox)row.FindControl("TextBox4");
            TextBox t5 = (TextBox)row.FindControl("TextBox5");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("UPDATE gokulinfo SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber, Email = @Email, Address = @Address WHERE UserID = @UserID", connection);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@FirstName", t1.Text.Trim());
                command.Parameters.AddWithValue("@LastName", t2.Text.Trim());
                command.Parameters.AddWithValue("@PhoneNumber", t3.Text.Trim());
                command.Parameters.AddWithValue("@Email", t4.Text.Trim());
                command.Parameters.AddWithValue("@Address", t5.Text.Trim());

                connection.Open();
                command.ExecuteNonQuery();
                GridView1.EditIndex = -1;
                bindgrid();
            }

            ToggleInsertValidators(true);  
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            bindgrid();

            ToggleInsertValidators(true);  
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int userID = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM gokulinfo WHERE UserID = @UserID", connection);
                command.Parameters.AddWithValue("@UserID", userID);
                connection.Open();
                int result = command.ExecuteNonQuery();

                if (result != 0)
                {
                    Response.Write("<center><h4>Record Deleted</h4></center>");
                }
                else
                {
                    Response.Write("<center><h4>Record Not Deleted</h4></center>");
                }
                bindgrid();
            }
        }

        private void ToggleInsertValidators(bool enable)
        {
            RequiredFieldValidator1.Enabled = enable;
            RequiredFieldValidator2.Enabled = enable;
            RequiredFieldValidator3.Enabled = enable;
            RequiredFieldValidator4.Enabled = enable;
            RequiredFieldValidator5.Enabled = enable;
        }
    }
}
